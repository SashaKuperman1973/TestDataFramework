﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using log4net;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Logger;
using TestDataFramework.Populator.Concrete.OperableList;

namespace TestDataFramework.Populator.Concrete.FieldExpression
{
    public class FieldExpressionsSet<TListElement>
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(FieldExpressionsSet<TListElement>));

        private readonly object fieldSetterIdentifier = new object();

        private readonly IObjectGraphService objectGraphService;

        public virtual OperableListEx<TListElement> OperableList { get; }

        public FieldExpressionsSet(OperableListEx<TListElement> operableList, IObjectGraphService objectGraphService)
        {
            this.OperableList = operableList;
            this.objectGraphService = objectGraphService;
        }

        public virtual IEnumerable<TListElement> RecordObjects => this.OperableList.RecordObjects;

        public virtual IEnumerable<TListElement> Make()
        {
            FieldExpressionsSet<TListElement>.Logger.Debug("Calling Make");
            return this.OperableList.Make();
        }

        public virtual IEnumerable<TListElement> BindAndMake()
        {
            FieldExpressionsSet<TListElement>.Logger.Debug("Calling BindAndMake");
            return this.OperableList.BindAndMake();
        }

        public virtual FieldExpressionsSet<TListElement> GuaranteePropertiesByFixedQuantity<TProperty>(
            Expression<Func<TListElement, TProperty>> expression,
            IEnumerable<object> guaranteedValues,
            int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall
        )
        {
            FieldExpressionsSet<TListElement>.Logger.Entering(nameof(this.GuaranteePropertiesByFixedQuantity));

            List<PropertyInfoProxy> setterObjectGraph = this.objectGraphService.GetObjectGraph(expression);

            guaranteedValues = guaranteedValues.ToList();

            FieldExpressionsSet<TListElement>.Logger.Debug(
                $"Guaranteed value count: {guaranteedValues.Count()}, FixedQuantity: {fixedQuantity}");

            if (fixedQuantity == 0)
                fixedQuantity = guaranteedValues.Count();

            this.OperableList.AddGuaranteedPropertySetter(new GuaranteedValues
            {
                TotalFrequency = fixedQuantity,

                Values = guaranteedValues.Select(value => FieldExpressionHelper
                    .GetFuncOrValueBasedExlicitPropertySetter<TProperty>(value, setterObjectGraph)),

                ValueCountRequestOption = valueCountRequestOption,

                FieldSetterIdentifier = this.fieldSetterIdentifier
            });

            FieldExpressionsSet<TListElement>.Logger.Exiting(nameof(this.GuaranteePropertiesByFixedQuantity));

            return this;
        }

        public virtual FieldExpressionsSet<TListElement> GuaranteePropertiesByFixedQuantity<TProperty>(
            Expression<Func<TListElement, TProperty>> expression,
            IEnumerable<Func<TProperty>> guaranteedValues, int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(expression, guaranteedValues.Cast<object>(), fixedQuantity,
                valueCountRequestOption);
        }

        public virtual FieldExpressionsSet<TListElement> GuaranteePropertiesByFixedQuantity<TProperty>(
            Expression<Func<TListElement, TProperty>> expression,
            IEnumerable<TProperty> guaranteedValues, int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(expression, guaranteedValues.Cast<object>(), fixedQuantity,
                valueCountRequestOption);
        }

        public virtual FieldExpressionsSet<TListElement> GuaranteePropertiesByFixedQuantity<TProperty>(
            Expression<Func<TListElement, TProperty>> expression,
            IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(expression, guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual FieldExpressionsSet<TListElement> GuaranteePropertiesByFixedQuantity<TProperty>(
            Expression<Func<TListElement, TProperty>> expression,
            IEnumerable<Func<TProperty>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(expression, guaranteedValues.Cast<object>(), valueCountRequestOption);
        }

        public virtual FieldExpressionsSet<TListElement> GuaranteePropertiesByFixedQuantity<TProperty>(
            Expression<Func<TListElement, TProperty>> expression,
            IEnumerable<TProperty> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(expression, guaranteedValues.Cast<object>(), valueCountRequestOption);
        }

        public virtual FieldExpressionsSet<TListElement> GuaranteePropertiesByPercentageOfTotal<TProperty>(
            Expression<Func<TListElement, TProperty>> expression,
            IEnumerable<object> guaranteedValues, 
            int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            FieldExpressionsSet<TListElement>.Logger.Entering(nameof(this.GuaranteePropertiesByPercentageOfTotal));

            List<PropertyInfoProxy> setterObjectGraph = this.objectGraphService.GetObjectGraph(expression);

            guaranteedValues = guaranteedValues.ToList();

            FieldExpressionsSet<TListElement>.Logger.Debug(
                $"Guaranteed value count: {guaranteedValues.Count()}, Frequency percentage: {frequencyPercentage}");

            this.OperableList.AddGuaranteedPropertySetter(new GuaranteedValues
            {
                FrequencyPercentage = frequencyPercentage,

                Values = guaranteedValues.Select(value => FieldExpressionHelper
                    .GetFuncOrValueBasedExlicitPropertySetter<TProperty>(value, setterObjectGraph)),

                ValueCountRequestOption = valueCountRequestOption,

                FieldSetterIdentifier = this.fieldSetterIdentifier
            });

            FieldExpressionsSet<TListElement>.Logger.Exiting(nameof(this.GuaranteePropertiesByPercentageOfTotal));
            return this;
        }

        public virtual FieldExpressionsSet<TListElement> GuaranteePropertiesByPercentageOfTotal<TProperty>(
            Expression<Func<TListElement, TProperty>> expression,
            IEnumerable<Func<TProperty>> guaranteedValues, int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(expression, guaranteedValues.Cast<object>(), frequencyPercentage,
                valueCountRequestOption);
        }

        public virtual FieldExpressionsSet<TListElement> GuaranteePropertiesByPercentageOfTotal<TProperty>(
            Expression<Func<TListElement, TProperty>> expression,
            IEnumerable<TProperty> guaranteedValues,
            int frequencyPercentage, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(expression, guaranteedValues.Cast<object>(), frequencyPercentage,
                valueCountRequestOption);
        }

        public virtual FieldExpressionsSet<TListElement> GuaranteePropertiesByPercentageOfTotal<TProperty>(
            Expression<Func<TListElement, TProperty>> expression,
            IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(expression, guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual FieldExpressionsSet<TListElement> GuaranteePropertiesByPercentageOfTotal<TProperty>(
            Expression<Func<TListElement, TProperty>> expression,
            IEnumerable<Func<TProperty>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(expression, guaranteedValues.Cast<object>(),
                valueCountRequestOption);
        }

        public virtual FieldExpressionsSet<TListElement> GuaranteePropertiesByPercentageOfTotal<TProperty>(
            Expression<Func<TListElement, TProperty>> expression,
            IEnumerable<TProperty> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(expression, guaranteedValues.Cast<object>(),
                valueCountRequestOption);
        }
    }
}
