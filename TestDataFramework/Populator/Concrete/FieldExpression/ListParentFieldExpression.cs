using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator.Concrete.OperableList;

namespace TestDataFramework.Populator.Concrete.FieldExpression
{
    public class ListParentFieldExpression<TListElement, TProperty> : FieldExpression<TListElement, TProperty>
    {
        public ListParentOperableList<TListElement> OperableList { get; }

        public ListParentFieldExpression(Expression<Func<TListElement, TProperty>> expression, ListParentOperableList<TListElement> operableList, IObjectGraphService objectGraphService) : base(expression, operableList, objectGraphService)
        {
            this.OperableList = operableList;
        }

        public virtual IEnumerable<TListElement> Make()
        {
            return this.OperableList.Make();
        }

        public virtual IEnumerable<TListElement> BindAndMake()
        {
            return this.OperableList.BindAndMake();
        }

        public virtual ListParentFieldExpression<TListElement, TProperty> GuaranteePropertiesByFixedQuantity(
            IEnumerable<Func<TProperty>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual ListParentFieldExpression<TListElement, TProperty> GuaranteePropertiesByFixedQuantity(
            IEnumerable<Func<TProperty>> guaranteedValues, int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteePropertiesByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this;
        }

        public virtual ListParentFieldExpression<TListElement, TProperty> GuaranteePropertiesByFixedQuantity(
            IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual ListParentFieldExpression<TListElement, TProperty> GuaranteePropertiesByFixedQuantity(IEnumerable<object> guaranteedValues,
            int fixedQuantity, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteePropertiesByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this;
        }

        public virtual ListParentFieldExpression<TListElement, TProperty> GuaranteePropertiesByFixedQuantity(
            IEnumerable<TProperty> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual ListParentFieldExpression<TListElement, TProperty> GuaranteePropertiesByFixedQuantity(IEnumerable<TProperty> guaranteedValues,
            int fixedQuantity, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(
                guaranteedValues.Select<TProperty, Func<TProperty>>(value => () => value), fixedQuantity, valueCountRequestOption);
        }

        public virtual ListParentFieldExpression<TListElement, TProperty> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<Func<TProperty>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual ListParentFieldExpression<TListElement, TProperty> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<Func<TProperty>> guaranteedValues, int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteePropertiesByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public virtual ListParentFieldExpression<TListElement, TProperty> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<TProperty> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual ListParentFieldExpression<TListElement, TProperty> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<TProperty> guaranteedValues, int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(
                guaranteedValues.Select<TProperty, Func<TProperty>>(value => () => value), frequencyPercentage,
                valueCountRequestOption);
        }

        public virtual ListParentFieldExpression<TListElement, TProperty> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual ListParentFieldExpression<TListElement, TProperty> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<object> guaranteedValues, int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteePropertiesByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }
    }
}
