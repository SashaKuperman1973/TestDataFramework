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
        public ReferenceParentOperableList<TListElement, TParent> OperableList { get; }

        public ReferenceParentFieldExpression(Expression<Func<TListElement, TProperty>> expression,
            ReferenceParentOperableList<TListElement, TParent> operableList, IObjectGraphService objectGraphService) :
            base(expression, operableList, objectGraphService)
        {
            this.OperableList = operableList;
        }

        public virtual TParent Make()
        {
            return this.OperableList.Make();
        }

        public virtual TParent BindAndMake()
        {
            return this.OperableList.BindAndMake();
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
