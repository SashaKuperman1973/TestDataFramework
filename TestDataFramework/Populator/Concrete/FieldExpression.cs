using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TestDataFramework.DeepSetting;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator.Interfaces;

namespace TestDataFramework.Populator.Concrete
{
    public class FieldExpression<TListElement, TProperty> : IRangeOperableList<TListElement>
    {
        private readonly Expression<Func<TListElement, TProperty>> expression;
        private readonly RangeOperableList<TListElement> rangeOperableList;
        private readonly IObjectGraphService objectGraphService;

        public FieldExpression(Expression<Func<TListElement, TProperty>> expression,
            RangeOperableList<TListElement> rangeOperableList, IObjectGraphService objectGraphService)
        {
            this.expression = expression;
            this.rangeOperableList = rangeOperableList;
            this.objectGraphService = objectGraphService;
        }

        public virtual IEnumerable<TListElement> RecordObjects => this.rangeOperableList.RecordObjects;

        public virtual IEnumerable<TListElement> Make()
        {
            return this.rangeOperableList.Make();
        }

        public virtual IEnumerable<TListElement> BindAndMake()
        {
            return this.rangeOperableList.BindAndMake();
        }

        public virtual OperableList<TListElement> GuaranteeByFixedQuantity(IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteeByFixedQuantity(IEnumerable<object> guaranteedValues,
            int fixedQuantity, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.rangeOperableList.GuaranteeByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
        }

        public OperableList<TListElement> GuaranteeByFixedQuantity(IEnumerable<Func<TListElement>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteeByFixedQuantity(
            IEnumerable<Func<TListElement>> guaranteedValues, int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.rangeOperableList.GuaranteeByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
        }

        public OperableList<TListElement> GuaranteeByFixedQuantity(IEnumerable<TListElement> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteeByFixedQuantity(IEnumerable<TListElement> guaranteedValues,
            int fixedQuantity, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.rangeOperableList.GuaranteeByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteePropertiesByFixedQuantity(
            IEnumerable<Func<TProperty>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteePropertiesByFixedQuantity(
            IEnumerable<Func<TProperty>> guaranteedValues, int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            List<PropertyInfo> setterObjectGraph = this.objectGraphService.GetObjectGraph(this.expression);

            guaranteedValues = guaranteedValues.ToList();

            if (fixedQuantity == 0)
                fixedQuantity = guaranteedValues.Count();

            this.rangeOperableList.GuaranteedPropertySetters.Add(new GuaranteedValues
            {
                TotalFrequency = fixedQuantity,
                Values = guaranteedValues.Select(value => new ExplicitPropertySetter
                {
                    PropertyChain = setterObjectGraph,
                    Action = @object => setterObjectGraph.Last().SetValue(@object, value())
                }),
                ValueCountRequestOption = valueCountRequestOption
            });

            return this.rangeOperableList;
        }

        public virtual OperableList<TListElement> GuaranteePropertiesByFixedQuantity(
            IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteePropertiesByFixedQuantity(IEnumerable<object> guaranteedValues,
            int fixedQuantity, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            List<PropertyInfo> setterObjectGraph = this.objectGraphService.GetObjectGraph(this.expression);

            guaranteedValues = guaranteedValues.ToList();

            if (fixedQuantity == 0)
                fixedQuantity = guaranteedValues.Count();

            this.rangeOperableList.GuaranteedPropertySetters.Add(new GuaranteedValues
            {
                TotalFrequency = fixedQuantity,
                Values = guaranteedValues.Select(value => FieldExpressionHelper
                    .GetFuncOrValueBasedExlicitPropertySetter<TProperty>(value, setterObjectGraph)),
                ValueCountRequestOption = valueCountRequestOption
            });

            return this.rangeOperableList;
        }

        public virtual OperableList<TListElement> GuaranteePropertiesByFixedQuantity(
            IEnumerable<TProperty> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteePropertiesByFixedQuantity(IEnumerable<TProperty> guaranteedValues,
            int fixedQuantity, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(
                guaranteedValues.Select<TProperty, Func<TProperty>>(value => () => value), fixedQuantity, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<object> guaranteedValues, 
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<object> guaranteedValues, int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.rangeOperableList.GuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<Func<TListElement>> guaranteedValues, 
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<Func<TListElement>> guaranteedValues, int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.rangeOperableList.GuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<TListElement> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<TListElement> guaranteedValues, int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.rangeOperableList.GuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<Func<TProperty>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<Func<TProperty>> guaranteedValues, int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            List<PropertyInfo> setterObjectGraph = this.objectGraphService.GetObjectGraph(this.expression);

            this.rangeOperableList.GuaranteedPropertySetters.Add(new GuaranteedValues
            {
                FrequencyPercentage = frequencyPercentage,

                Values = guaranteedValues.Select(value => new ExplicitPropertySetter
                {
                    PropertyChain = setterObjectGraph,
                    Action = @object => setterObjectGraph.Last().SetValue(@object, value())
                }),
                ValueCountRequestOption = valueCountRequestOption
            });

            return this.rangeOperableList;
        }

        public virtual OperableList<TListElement> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<TProperty> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<TProperty> guaranteedValues, int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(
                guaranteedValues.Select<TProperty, Func<TProperty>>(value => () => value), frequencyPercentage,
                valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<object> guaranteedValues, 
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<object> guaranteedValues, int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            List<PropertyInfo> setterObjectGraph = this.objectGraphService.GetObjectGraph(this.expression);

            guaranteedValues = guaranteedValues.ToList();

            this.rangeOperableList.GuaranteedPropertySetters.Add(new GuaranteedValues
            {
                FrequencyPercentage = frequencyPercentage,
                Values = guaranteedValues.Select(value => FieldExpressionHelper
                    .GetFuncOrValueBasedExlicitPropertySetter<TProperty>(value, setterObjectGraph)),
                ValueCountRequestOption = valueCountRequestOption
            });

            return this.rangeOperableList;
        }

        public virtual OperableList<TListElement> Ignore<TPropertyType>(Expression<Func<TListElement, TPropertyType>> fieldExpression)
        {
            return this.rangeOperableList.Ignore(fieldExpression);
        }

        public RangeOperableList<TListElement> Set<TValueProperty>(
            Expression<Func<TListElement, TValueProperty>> fieldExpression, TValueProperty value, params Range[] ranges)
        {
            return this.rangeOperableList.Set(fieldExpression, value, ranges);
        }

        public RangeOperableList<TListElement> Set<TValueProperty>(
            Expression<Func<TListElement, TValueProperty>> fieldExpression, Func<TValueProperty> valueFactory,
            params Range[] ranges)
        {
            return this.rangeOperableList.Set(fieldExpression, valueFactory, ranges);
        }

        public FieldExpression<TListElement, TValueProperty> Set<TValueProperty>(
            Expression<Func<TListElement, TValueProperty>> expression)
        {
            return this.rangeOperableList.Set(expression);
        }

        public virtual FieldExpression<TListElement, TProperty> Value(TProperty value, params Range[] ranges)
        {
            this.rangeOperableList.Set(this.expression, value, ranges);
            return this;
        }

        public virtual FieldExpression<TListElement, TProperty> Value(Func<TProperty> valueFactory,
            params Range[] ranges)
        {
            this.rangeOperableList.Set(this.expression, valueFactory, ranges);
            return this;
        }
    }
}