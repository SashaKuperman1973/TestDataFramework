using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TestDataFramework.Populator.Interfaces;

namespace TestDataFramework.Populator.Concrete
{
    public class FieldExpression<TListElement, TProperty> : IRangeOperableList<TListElement>
    {
        private readonly Expression<Func<TListElement, TProperty>> expression;
        private readonly RangeOperableList<TListElement> rangeOperableList;

        public FieldExpression(Expression<Func<TListElement, TProperty>> expression,
            RangeOperableList<TListElement> rangeOperableList)
        {
            this.expression = expression;
            this.rangeOperableList = rangeOperableList;
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

        public virtual OperableList<TListElement> GuaranteeByFixedQuantity<TValue>(IEnumerable<TValue> guaranteedValues,
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

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal<TValue>(
            IEnumerable<TValue> guaranteedValues, int frequencyPercentage = 10)
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

        public virtual void Ignore<TPropertyType>(Expression<Func<TListElement, TPropertyType>> fieldExpression)
        {
            this.rangeOperableList.Ignore(fieldExpression);
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