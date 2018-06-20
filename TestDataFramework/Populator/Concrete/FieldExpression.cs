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
            int fixedQuantity = 0)
        {
            return this.rangeOperableList.GuaranteeByFixedQuantity(guaranteedValues, fixedQuantity);
        }

        public virtual OperableList<TListElement> GuaranteeByFixedQuantity(
            IEnumerable<Func<TListElement>> guaranteedValues, int fixedQuantity = 0)
        {
            return this.rangeOperableList.GuaranteeByFixedQuantity(guaranteedValues, fixedQuantity);
        }

        public virtual OperableList<TListElement> GuaranteeByFixedQuantity(IEnumerable<TListElement> guaranteedValues,
            int fixedQuantity = 0)
        {
            return this.rangeOperableList.GuaranteeByFixedQuantity(guaranteedValues, fixedQuantity);
        }

        public virtual OperableList<TListElement> GuaranteePropertiesByFixedQuantity(
            IEnumerable<Func<TProperty>> guaranteedValues, int fixedQuantity = 0)
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
                })
            });

            return this.rangeOperableList;
        }

        public virtual OperableList<TListElement> GuaranteePropertiesByFixedQuantity(IEnumerable<object> guaranteedValues,
            int fixedQuantity = 0)
        {
            List<PropertyInfo> setterObjectGraph = this.objectGraphService.GetObjectGraph(this.expression);

            guaranteedValues = guaranteedValues.ToList();

            if (fixedQuantity == 0)
                fixedQuantity = guaranteedValues.Count();

            this.rangeOperableList.GuaranteedPropertySetters.Add(new GuaranteedValues
            {
                TotalFrequency = fixedQuantity,
                Values = guaranteedValues.Select(value => FieldExpression<TListElement, TProperty>
                    .GetFuncOrValueBasedExlicitPropertySetter(value, setterObjectGraph))
            });

            return this.rangeOperableList;
        }

        public virtual OperableList<TListElement> GuaranteePropertiesByFixedQuantity(IEnumerable<TProperty> guaranteedValues,
            int fixedQuantity = 0)
        {
            return this.GuaranteePropertiesByFixedQuantity(
                guaranteedValues.Select<TProperty, Func<TProperty>>(value => () => value), fixedQuantity);
        }

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<object> guaranteedValues, int frequencyPercentage = 10)
        {
            return this.rangeOperableList.GuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage);
        }

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<Func<TListElement>> guaranteedValues, int frequencyPercentage = 10)
        {
            return this.rangeOperableList.GuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage);
        }

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<TListElement> guaranteedValues, int frequencyPercentage = 10)
        {
            return this.rangeOperableList.GuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage);
        }

        public virtual OperableList<TListElement> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<Func<TProperty>> guaranteedValues, int frequencyPercentage = 10)
        {
            List<PropertyInfo> setterObjectGraph = this.objectGraphService.GetObjectGraph(this.expression);

            this.rangeOperableList.GuaranteedPropertySetters.Add(new GuaranteedValues
            {
                FrequencyPercentage = frequencyPercentage,

                Values = guaranteedValues.Select(value => new ExplicitPropertySetter
                {
                    PropertyChain = setterObjectGraph,
                    Action = @object => setterObjectGraph.Last().SetValue(@object, value())
                })
            });

            return this.rangeOperableList;
        }

        public virtual OperableList<TListElement> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<TProperty> guaranteedValues, int frequencyPercentage = 10)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(
                guaranteedValues.Select<TProperty, Func<TProperty>>(value => () => value), frequencyPercentage);
        }

        public virtual OperableList<TListElement> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<object> guaranteedValues, int frequencyPercentage = 10)
        {
            List<PropertyInfo> setterObjectGraph = this.objectGraphService.GetObjectGraph(this.expression);

            guaranteedValues = guaranteedValues.ToList();

            this.rangeOperableList.GuaranteedPropertySetters.Add(new GuaranteedValues
            {
                FrequencyPercentage = frequencyPercentage,
                Values = guaranteedValues.Select(value => FieldExpression<TListElement, TProperty>
                    .GetFuncOrValueBasedExlicitPropertySetter(value, setterObjectGraph))
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

        private static ExplicitPropertySetter GetFuncOrValueBasedExlicitPropertySetter(object value, List<PropertyInfo> setterObjectGraph)
        {
            if (value == null)
                return new ExplicitPropertySetter
                {
                    PropertyChain = setterObjectGraph,
                    Action = @object => setterObjectGraph.Last().SetValue(@object, null)
                };

            var valueFunc = value as Func<TProperty>;
            if (valueFunc == null && !(value is TProperty))
            {
                throw new ValueGuaranteeException(string.Format(Messages.GuaranteedTypeNotOfListType,
                    typeof(TProperty), value.GetType()));
            }

            if (valueFunc != null)
            {
                return new ExplicitPropertySetter
                {
                    PropertyChain = setterObjectGraph,
                    Action = @object => setterObjectGraph.Last().SetValue(@object, valueFunc())
                };
            }

            return new ExplicitPropertySetter
            {
                PropertyChain = setterObjectGraph,
                Action = @object => setterObjectGraph.Last().SetValue(@object, value)
            };
        }
    }
}