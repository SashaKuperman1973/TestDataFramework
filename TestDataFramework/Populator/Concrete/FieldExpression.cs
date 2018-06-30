using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TestDataFramework.DeepSetting;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.ListOperations.Concrete;

namespace TestDataFramework.Populator.Concrete
{
    public class FieldExpression<TListElement, TProperty>
    {
        private readonly Expression<Func<TListElement, TProperty>> expression;
        private readonly IObjectGraphService objectGraphService;

        public FieldExpression(Expression<Func<TListElement, TProperty>> expression,
            OperableList<TListElement> operableList, IObjectGraphService objectGraphService)
        {
            this.expression = expression;
            this.OperableList = operableList;
            this.objectGraphService = objectGraphService;
        }

        public virtual OperableList<TListElement> OperableList { get; }

        public virtual IEnumerable<TListElement> RecordObjects => this.OperableList.RecordObjects;

        public virtual IEnumerable<TListElement> Make()
        {
            return this.OperableList.Make();
        }

        public virtual IEnumerable<TListElement> BindAndMake()
        {
            return this.OperableList.BindAndMake();
        }

        public virtual FieldExpression<TListElement, TProperty> GuaranteePropertiesByFixedQuantity(
            IEnumerable<Func<TProperty>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual FieldExpression<TListElement, TProperty> GuaranteePropertiesByFixedQuantity(
            IEnumerable<Func<TProperty>> guaranteedValues, int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            List<PropertyInfo> setterObjectGraph = this.objectGraphService.GetObjectGraph(this.expression);

            guaranteedValues = guaranteedValues.ToList();

            if (fixedQuantity == 0)
                fixedQuantity = guaranteedValues.Count();

            this.OperableList.GuaranteedPropertySetters.Add(new GuaranteedValues
            {
                TotalFrequency = fixedQuantity,
                Values = guaranteedValues.Select(value => new ExplicitPropertySetter
                {
                    PropertyChain = setterObjectGraph,
                    Action = @object => setterObjectGraph.Last().SetValue(@object, value())
                }),
                ValueCountRequestOption = valueCountRequestOption
            });

            return this;
        }

        public virtual FieldExpression<TListElement, TProperty> GuaranteePropertiesByFixedQuantity(
            IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual FieldExpression<TListElement, TProperty> GuaranteePropertiesByFixedQuantity(IEnumerable<object> guaranteedValues,
            int fixedQuantity, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            List<PropertyInfo> setterObjectGraph = this.objectGraphService.GetObjectGraph(this.expression);

            guaranteedValues = guaranteedValues.ToList();

            if (fixedQuantity == 0)
                fixedQuantity = guaranteedValues.Count();

            this.OperableList.GuaranteedPropertySetters.Add(new GuaranteedValues
            {
                TotalFrequency = fixedQuantity,
                Values = guaranteedValues.Select(value => FieldExpressionHelper
                    .GetFuncOrValueBasedExlicitPropertySetter<TProperty>(value, setterObjectGraph)),
                ValueCountRequestOption = valueCountRequestOption
            });

            return this;
        }

        public virtual FieldExpression<TListElement, TProperty> GuaranteePropertiesByFixedQuantity(
            IEnumerable<TProperty> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual FieldExpression<TListElement, TProperty> GuaranteePropertiesByFixedQuantity(IEnumerable<TProperty> guaranteedValues,
            int fixedQuantity, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(
                guaranteedValues.Select<TProperty, Func<TProperty>>(value => () => value), fixedQuantity, valueCountRequestOption);
        }

        public virtual FieldExpression<TListElement, TProperty> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<Func<TProperty>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual FieldExpression<TListElement, TProperty> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<Func<TProperty>> guaranteedValues, int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            List<PropertyInfo> setterObjectGraph = this.objectGraphService.GetObjectGraph(this.expression);

            this.OperableList.GuaranteedPropertySetters.Add(new GuaranteedValues
            {
                FrequencyPercentage = frequencyPercentage,

                Values = guaranteedValues.Select(value => new ExplicitPropertySetter
                {
                    PropertyChain = setterObjectGraph,
                    Action = @object => setterObjectGraph.Last().SetValue(@object, value())
                }),
                ValueCountRequestOption = valueCountRequestOption
            });

            return this;
        }

        public virtual FieldExpression<TListElement, TProperty> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<TProperty> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual FieldExpression<TListElement, TProperty> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<TProperty> guaranteedValues, int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(
                guaranteedValues.Select<TProperty, Func<TProperty>>(value => () => value), frequencyPercentage,
                valueCountRequestOption);
        }

        public virtual FieldExpression<TListElement, TProperty> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<object> guaranteedValues, 
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual FieldExpression<TListElement, TProperty> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<object> guaranteedValues, int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            List<PropertyInfo> setterObjectGraph = this.objectGraphService.GetObjectGraph(this.expression);

            guaranteedValues = guaranteedValues.ToList();

            this.OperableList.GuaranteedPropertySetters.Add(new GuaranteedValues
            {
                FrequencyPercentage = frequencyPercentage,
                Values = guaranteedValues.Select(value => FieldExpressionHelper
                    .GetFuncOrValueBasedExlicitPropertySetter<TProperty>(value, setterObjectGraph)),
                ValueCountRequestOption = valueCountRequestOption
            });

            return this;
        }
    }
}