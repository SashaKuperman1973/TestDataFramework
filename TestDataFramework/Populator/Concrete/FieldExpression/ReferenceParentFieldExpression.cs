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
using System.Linq.Expressions;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator.Concrete.OperableList;

namespace TestDataFramework.Populator.Concrete.FieldExpression
{
    public class ReferenceParentFieldExpression<TListElement, TProperty, TParentList, TRootListElement, TRootElement> 
        : FieldExpression<TListElement, TProperty>
    {
        public new virtual ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement> OperableList
            =>
                (ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement>) base
                    .OperableList;

        public ReferenceParentFieldExpression(Expression<Func<TListElement, TProperty>> expression,
            ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement> operableList,
            IObjectGraphService objectGraphService) :            
            base(expression, operableList, objectGraphService)
        {
        }

        public new virtual TRootElement Make()
        {
            return this.OperableList.Make();
        }

        public new virtual TRootElement BindAndMake()
        {
            return this.OperableList.BindAndMake();
        }

        public new virtual ReferenceParentFieldExpression<TListElement, TProperty, TParentList, TRootListElement, TRootElement> SetRange(
            IEnumerable<TProperty> range)
        {
            base.SetRange(range);
            return this;
        }

        public new virtual ReferenceParentFieldExpression<TListElement, TProperty, TParentList, TRootListElement, TRootElement> SetRange(
                Func<IEnumerable<TProperty>> rangeFactory)
        {
            base.SetRange(rangeFactory);
            return this;
        }

        public new virtual ReferenceParentFieldExpression<TListElement, TProperty, TParentList, TRootListElement, TRootElement>
            GuaranteePropertiesByFixedQuantity(
                IEnumerable<Func<TProperty>> guaranteedValues,
                ValueCountRequestOption valueCountRequestOption =
                    ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteePropertiesByFixedQuantity(guaranteedValues, valueCountRequestOption);
            return this;
        }

        public new virtual ReferenceParentFieldExpression<TListElement, TProperty, TParentList, TRootListElement, TRootElement>
            GuaranteePropertiesByFixedQuantity(
                IEnumerable<Func<TProperty>> guaranteedValues, int fixedQuantity,
                ValueCountRequestOption valueCountRequestOption =
                    ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteePropertiesByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this;
        }

        public new virtual ReferenceParentFieldExpression<TListElement, TProperty, TParentList, TRootListElement, TRootElement>
            GuaranteePropertiesByFixedQuantity(
                IEnumerable<object> guaranteedValues,
                ValueCountRequestOption valueCountRequestOption =
                    ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteePropertiesByFixedQuantity(guaranteedValues, valueCountRequestOption);
            return this;
        }

        public new virtual ReferenceParentFieldExpression<TListElement, TProperty, TParentList, TRootListElement, TRootElement>
            GuaranteePropertiesByFixedQuantity(IEnumerable<object> guaranteedValues,
                int fixedQuantity,
                ValueCountRequestOption valueCountRequestOption =
                    ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteePropertiesByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this;
        }

        public new virtual ReferenceParentFieldExpression<TListElement, TProperty, TParentList, TRootListElement, TRootElement>
            GuaranteePropertiesByFixedQuantity(
                IEnumerable<TProperty> guaranteedValues,
                ValueCountRequestOption valueCountRequestOption =
                    ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteePropertiesByFixedQuantity(guaranteedValues, valueCountRequestOption);
            return this;
        }

        public new virtual ReferenceParentFieldExpression<TListElement, TProperty, TParentList, TRootListElement, TRootElement>
            GuaranteePropertiesByFixedQuantity(IEnumerable<TProperty> guaranteedValues,
                int fixedQuantity,
                ValueCountRequestOption valueCountRequestOption =
                    ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteePropertiesByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this;
        }

        public new virtual ReferenceParentFieldExpression<TListElement, TProperty, TParentList, TRootListElement, TRootElement>
            GuaranteePropertiesByPercentageOfTotal(
                IEnumerable<Func<TProperty>> guaranteedValues,
                ValueCountRequestOption valueCountRequestOption =
                    ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteePropertiesByPercentageOfTotal(guaranteedValues, valueCountRequestOption);
            return this;
        }

        public new virtual ReferenceParentFieldExpression<TListElement, TProperty, TParentList, TRootListElement, TRootElement>
            GuaranteePropertiesByPercentageOfTotal(
                IEnumerable<Func<TProperty>> guaranteedValues, int frequencyPercentage,
                ValueCountRequestOption valueCountRequestOption =
                    ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteePropertiesByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public new virtual ReferenceParentFieldExpression<TListElement, TProperty, TParentList, TRootListElement, TRootElement>
            GuaranteePropertiesByPercentageOfTotal(
                IEnumerable<TProperty> guaranteedValues,
                ValueCountRequestOption valueCountRequestOption =
                    ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteePropertiesByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
            return this;
        }

        public new virtual ReferenceParentFieldExpression<TListElement, TProperty, TParentList, TRootListElement, TRootElement>
            GuaranteePropertiesByPercentageOfTotal(
                IEnumerable<TProperty> guaranteedValues, int frequencyPercentage,
                ValueCountRequestOption valueCountRequestOption =
                    ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteePropertiesByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public new virtual ReferenceParentFieldExpression<TListElement, TProperty, TParentList, TRootListElement, TRootElement>
            GuaranteePropertiesByPercentageOfTotal(
                IEnumerable<object> guaranteedValues,
                ValueCountRequestOption valueCountRequestOption =
                    ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteePropertiesByPercentageOfTotal(guaranteedValues, valueCountRequestOption);
            return this;
        }

        public new virtual ReferenceParentFieldExpression<TListElement, TProperty, TParentList, TRootListElement, TRootElement>
            GuaranteePropertiesByPercentageOfTotal(
                IEnumerable<object> guaranteedValues, int frequencyPercentage,
                ValueCountRequestOption valueCountRequestOption =
                    ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteePropertiesByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }
    }
}
