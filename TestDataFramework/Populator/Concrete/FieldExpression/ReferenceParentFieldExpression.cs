/*
    Copyright 2016, 2017, 2018 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TestDataFramework is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TestDataFramework.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator.Concrete.OperableList;

namespace TestDataFramework.Populator.Concrete.FieldExpression
{
    public class ReferenceParentFieldExpression<TListElement, TProperty, TParent> : FieldExpression<TListElement, TProperty>
    {
        private new ReferenceParentOperableList<TListElement, TParent> ThisOperableList =>
            (ReferenceParentOperableList<TListElement, TParent>) base.ThisOperableList;

        public new ReferenceParentOperableList<TListElement, TParent> OperableList => (ReferenceParentOperableList<TListElement, TParent>)base
            .OperableList;

        public ReferenceParentFieldExpression(Expression<Func<TListElement, TProperty>> expression,
            ReferenceParentOperableList<TListElement, TParent> operableList, IObjectGraphService objectGraphService) :
            base(expression, operableList, objectGraphService)
        {
        }

        public virtual TParent Make()
        {
            return this.OperableList.Make();
        }

        public virtual TParent BindAndMake()
        {
            return this.OperableList.BindAndMake();
        }

        public ReferenceParentFieldExpression<TListElement, TProperty, TParent> Take(int count)
        {
            ReferenceParentOperableList<TListElement, TParent> newOperableList = this.ThisOperableList.Take(count);
            var result =
                new ReferenceParentFieldExpression<TListElement, TProperty, TParent>(this.Expression, newOperableList,
                        this.ObjectGraphService)
                    { Parent = this };
            return result;
        }

        public ReferenceParentFieldExpression<TListElement, TProperty, TParent> Skip(int count)
        {
            ReferenceParentOperableList<TListElement, TParent> newOperableList = this.ThisOperableList.Skip(count);
            var result =
                new ReferenceParentFieldExpression<TListElement, TProperty, TParent>(this.Expression, newOperableList,
                        this.ObjectGraphService)
                    { Parent = this };
            return result;
        }

        public virtual ReferenceParentFieldExpression<TListElement, TProperty, TParent> SetRange<TPropertyValue>(
            Expression<Func<TListElement, TPropertyValue>> fieldExpression,
            IEnumerable<TPropertyValue> range)
        {
            base.SetRange(fieldExpression, () => range);
            return this;
        }

        public new virtual ReferenceParentFieldExpression<TListElement, TProperty, TParent> SetRange<TPropertyValue>(
            Expression<Func<TListElement, TPropertyValue>> fieldExpression,
            Func<IEnumerable<TPropertyValue>> rangeFactory)
        {
            base.SetRange(fieldExpression, rangeFactory);
            return this;
        }

        public virtual ReferenceParentFieldExpression<TListElement, TProperty, TParent> GuaranteePropertiesByFixedQuantity(
            IEnumerable<Func<TProperty>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public new virtual ReferenceParentFieldExpression<TListElement, TProperty, TParent> GuaranteePropertiesByFixedQuantity(
            IEnumerable<Func<TProperty>> guaranteedValues, int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteePropertiesByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this;
        }

        public virtual ReferenceParentFieldExpression<TListElement, TProperty, TParent> GuaranteePropertiesByFixedQuantity(
            IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public new virtual ReferenceParentFieldExpression<TListElement, TProperty, TParent> GuaranteePropertiesByFixedQuantity(IEnumerable<object> guaranteedValues,
            int fixedQuantity, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteePropertiesByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this;
        }

        public virtual ReferenceParentFieldExpression<TListElement, TProperty, TParent> GuaranteePropertiesByFixedQuantity(
            IEnumerable<TProperty> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual ReferenceParentFieldExpression<TListElement, TProperty, TParent> GuaranteePropertiesByFixedQuantity(IEnumerable<TProperty> guaranteedValues,
            int fixedQuantity, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(
                guaranteedValues.Select<TProperty, Func<TProperty>>(value => () => value), fixedQuantity, valueCountRequestOption);
        }

        public virtual ReferenceParentFieldExpression<TListElement, TProperty, TParent> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<Func<TProperty>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public new virtual ReferenceParentFieldExpression<TListElement, TProperty, TParent> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<Func<TProperty>> guaranteedValues, int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteePropertiesByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public virtual ReferenceParentFieldExpression<TListElement, TProperty, TParent> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<TProperty> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual ReferenceParentFieldExpression<TListElement, TProperty, TParent> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<TProperty> guaranteedValues, int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(
                guaranteedValues.Select<TProperty, Func<TProperty>>(value => () => value), frequencyPercentage,
                valueCountRequestOption);
        }

        public virtual ReferenceParentFieldExpression<TListElement, TProperty, TParent> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public new virtual ReferenceParentFieldExpression<TListElement, TProperty, TParent> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<object> guaranteedValues, int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteePropertiesByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }
    }
}
