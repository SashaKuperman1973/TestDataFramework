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
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator.Concrete.FieldExpression;
using TestDataFramework.Populator.Concrete.MakeableEnumerable;
using TestDataFramework.Populator.Interfaces;
using TestDataFramework.TypeGenerator.Interfaces;

namespace TestDataFramework.Populator.Concrete.OperableList
{
    public class ReferenceParentOperableList<TListElement, TParent> : OperableList<TListElement>, IMakeable<TParent>
    {
        private readonly RecordReference<TParent> parentReference;

        public ReferenceParentOperableList(RecordReference<TParent> parentReference,
            ValueGuaranteePopulator valueGuaranteePopulator, BasePopulator populator, ITypeGenerator typeGenerator,
            IAttributeDecorator attributeDecorator, IObjectGraphService objectGraphService,
            DeepCollectionSettingConverter deepCollectionSettingConverter,
            List<RecordReference<TListElement>> internalList = null) : base(valueGuaranteePopulator, populator,
            typeGenerator, attributeDecorator, objectGraphService, deepCollectionSettingConverter, internalList)
        {
            this.parentReference = parentReference;
        }

        public ReferenceParentOperableList(RecordReference<TParent> parentReference, int size,
            ValueGuaranteePopulator valueGuaranteePopulator, BasePopulator populator, ITypeGenerator typeGenerator,
            IAttributeDecorator attributeDecorator, IObjectGraphService objectGraphService,
            DeepCollectionSettingConverter deepCollectionSettingConverter) : base(size, valueGuaranteePopulator,
            populator, typeGenerator, attributeDecorator, objectGraphService, deepCollectionSettingConverter)
        {
            this.parentReference = parentReference;
        }

        public ReferenceParentOperableList(ReferenceParentOperableList<TListElement, TParent> rootList,
            ReferenceParentOperableList<TListElement, TParent> rootParallelList,
            RecordReference<TParent> parentReference,
            IEnumerable<RecordReference<TListElement>> input, ValueGuaranteePopulator valueGuaranteePopulator,
            BasePopulator populator) : base(rootList, rootParallelList, input, valueGuaranteePopulator, populator)
        {
            this.parentReference = parentReference;
        }

        public new virtual ReferenceParentOperableList<TListElement, TParent> RootParallelList => (
            ReferenceParentOperableList<TListElement, TParent>)base
            .RootParallelList;

        public new virtual ReferenceParentOperableList<TListElement, TParent> RootList => (
            ReferenceParentOperableList<TListElement, TParent>)base
            .RootList;

        public new virtual TParent BindAndMake()
        {
            base.BindAndMake();

            return this.parentReference.RecordObject;
        }

        public new virtual TParent Make()
        {
            base.Make();

            TParent result = this.parentReference.Make();
            return result;
        }

        public new virtual ReferenceParentOperableList<TListElement, TParent> Take(int count)
        {
            IEnumerable<RecordReference<TListElement>> input = base.Take(count);
            var result =
                new ReferenceParentOperableList<TListElement, TParent>(this.RootList, this.RootParallelList,
                    this.parentReference, input, this.ValueGuaranteePopulator,
                    this.Populator);
            return result;
        }

        public new virtual ReferenceParentOperableList<TListElement, TParent> Skip(int count)
        {
            IEnumerable<RecordReference<TListElement>> input = base.Skip(count);
            var result =
                new ReferenceParentOperableList<TListElement, TParent>(this.RootList, this.RootParallelList,
                    this.parentReference, input, this.ValueGuaranteePopulator,
                    this.Populator);
            return result;
        }

        public virtual ReferenceParentOperableList<TListElement, TParent> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, TProperty value)
        {
            return this.Set(fieldExpression, () => value);
        }

        public new virtual ReferenceParentOperableList<TListElement, TParent> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, Func<TProperty> valueFactory)
        {
            base.Set(fieldExpression, valueFactory);
            return this;
        }

        public virtual ReferenceParentFieldExpression<TListElement, TProperty, TParent> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> expression)
        {
            var fieldExpression =
                new ReferenceParentFieldExpression<TListElement, TProperty, TParent>(expression, this, this.ObjectGraphService);
            return fieldExpression;
        }

        public virtual ReferenceParentOperableList<TListElement, TParent> GuaranteeByPercentageOfTotal(IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual ReferenceParentOperableList<TListElement, TParent> GuaranteeByPercentageOfTotal(IEnumerable<object> guaranteedValues,
            int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.AddGuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public virtual ReferenceParentOperableList<TListElement, TParent> GuaranteeByPercentageOfTotal(IEnumerable<Func<TListElement>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual ReferenceParentOperableList<TListElement, TParent> GuaranteeByPercentageOfTotal(IEnumerable<Func<TListElement>> guaranteedValues,
            int frequencyPercentage, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.AddGuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public virtual ReferenceParentOperableList<TListElement, TParent> GuaranteeByPercentageOfTotal(IEnumerable<TListElement> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual ReferenceParentOperableList<TListElement, TParent> GuaranteeByPercentageOfTotal(IEnumerable<TListElement> guaranteedValues,
            int frequencyPercentage, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.AddGuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public virtual ReferenceParentOperableList<TListElement, TParent> GuaranteeByFixedQuantity(IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual ReferenceParentOperableList<TListElement, TParent> GuaranteeByFixedQuantity(IEnumerable<object> guaranteedValues,
            int fixedQuantity, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.AddGuaranteeByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this;
        }

        public virtual ReferenceParentOperableList<TListElement, TParent> GuaranteeByFixedQuantity(IEnumerable<Func<TListElement>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual ReferenceParentOperableList<TListElement, TParent> GuaranteeByFixedQuantity(IEnumerable<Func<TListElement>> guaranteedValues,
            int fixedQuantity, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.AddGuaranteeByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this;
        }

        public virtual ReferenceParentOperableList<TListElement, TParent> GuaranteeByFixedQuantity(IEnumerable<TListElement> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            this.AddGuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
            return this;
        }

        public virtual ReferenceParentMakeableEnumerable<ReferenceParentOperableList<TResult, TParent>, TParent> Select<TResult>(
            Expression<Func<TListElement, IEnumerable<TResult>>> selector, int size)
        {
            IEnumerable<ReferenceParentOperableList<TResult, TParent>> operableListCollection =
                this.InternalList.Select(recordReference => recordReference.SetReferenceParentList(selector, size, this.parentReference));

            var result =
                new ReferenceParentMakeableEnumerable<ReferenceParentOperableList<TResult, TParent>, TParent>(
                    operableListCollection, this.parentReference);

            return result;
        }

        public new virtual OperableList<TListElement> Ignore<TPropertyType>(Expression<Func<TListElement, TPropertyType>> fieldExpression)
        {
            base.IgnoreBase(fieldExpression);
            return this;
        }

        public new virtual ReferenceParentOperableList<TListElement, TParent> SetRange<TPropertyValue>(
            Expression<Func<TListElement, TPropertyValue>> fieldExpression,
            Func<IEnumerable<TPropertyValue>> rangeFactory)
        {
            base.AddRange(fieldExpression, rangeFactory);
            return this;
        }

        public new virtual ReferenceParentOperableList<TListElement, TParent> SetRange<TPropertyValue>(
            Expression<Func<TListElement, TPropertyValue>> fieldExpression,
            IEnumerable<TPropertyValue> range)
        {
            base.AddRange(fieldExpression, () => range);
            return this;

        }
    }
}
