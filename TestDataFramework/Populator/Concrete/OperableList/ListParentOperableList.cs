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
    public class ListParentOperableList<TListElement, TParentList, TRootListElement> : OperableList<TListElement>,
        IMakeableCollectionContainer<TRootListElement>
    {
        public ListParentOperableList(
            OperableList<TRootListElement> rootList,
            TParentList parentList,
            IEnumerable<RecordReference<TListElement>> input, ValueGuaranteePopulator valueGuaranteePopulator,
            BasePopulator populator, IObjectGraphService objectGraphService, IAttributeDecorator attributeDecorator,
            DeepCollectionSettingConverter deepCollectionSettingConverter, ITypeGenerator typeGenerator) : 
            base(input, valueGuaranteePopulator, populator, objectGraphService, attributeDecorator, 
                deepCollectionSettingConverter, typeGenerator)
        {
            this.RootList = rootList;
            this.ParentList = parentList;
        }

        private ListParentOperableList<
                TChildListElement,
                ListParentOperableList<TListElement, TParentList, TRootListElement>,
                TRootListElement>

            CreateChild<TChildListElement>(
                IEnumerable<RecordReference<TChildListElement>> input
            )
        {
            var result = new ListParentOperableList<
                TChildListElement,
                ListParentOperableList<TListElement, TParentList, TRootListElement>,
                TRootListElement>(
                
                this.RootList,
                this,
                input,
                this.ValueGuaranteePopulator,
                this.Populator,
                this.ObjectGraphService,
                this.AttributeDecorator,
                this.DeepCollectionSettingConverter,
                this.TypeGenerator
            );

            return result;
        }

        private ListParentOperableList<TListElement, TParentList, TRootListElement> CreateSubset(
            IEnumerable<RecordReference<TListElement>> input)
        {
            var result = new ListParentOperableList<TListElement, TParentList, TRootListElement>
            (
                this.RootList,
                this.ParentList,
                input,
                this.ValueGuaranteePopulator,
                this.Populator,
                this.ObjectGraphService,
                this.AttributeDecorator,
                this.DeepCollectionSettingConverter,
                this.TypeGenerator
            );

            return result;
        }

        public virtual OperableList<TRootListElement> RootList
        {
            get;
        }

        public virtual TParentList ParentList { get; }

        public new virtual IEnumerable<TRootListElement> Make()
        {
            this.RootList.Populate();
            return this.RootList.RecordObjects;
        }

        public new virtual IEnumerable<TRootListElement> BindAndMake()
        {
            this.Populator.Bind();
            return this.RootList.RecordObjects;
        }

        public new ListParentOperableList<TListElement, TParentList, TRootListElement> Take(int count)
        {
            IEnumerable<RecordReference<TListElement>> input = this.InternalList.Take(count);

            ListParentOperableList<TListElement, TParentList, TRootListElement> result = this.CreateSubset(input);

            this.Children.Add(result);
            return result;
        }

        public new ListParentOperableList<TListElement, TParentList, TRootListElement> Skip(int count)
        {
            IEnumerable<RecordReference<TListElement>> input = this.InternalList.Skip(count);

            ListParentOperableList<TListElement, TParentList, TRootListElement> result = this.CreateSubset(input);

            this.Children.Add(result);
            return result;
        }

        public new virtual ListParentOperableList<TPropertyElement,
            ListParentOperableList<TListElement, TParentList, TRootListElement>, TRootListElement> SetList<TPropertyElement>(
            Expression<Func<TListElement, IEnumerable<TPropertyElement>>> listFieldExpression, int size)
        {
            List<RecordReference<TPropertyElement>> list = this.CreateRecordReferences<TPropertyElement>(size);

            ListParentOperableList<TPropertyElement, ListParentOperableList<TListElement, TParentList, TRootListElement>,
                TRootListElement> result = this.CreateChild(list);

            this.Children.Add(result);
            return result;
        }

        public new virtual ListParentOperableList<TListElement, TParentList, TRootListElement> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, TProperty value)
        {
            return this.Set(fieldExpression, () => value);
        }

        public new virtual ListParentOperableList<TListElement, TParentList, TRootListElement> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, Func<TProperty> valueFactory)
        {
            base.Set(fieldExpression, valueFactory);
            return this;
        }

        public new virtual ListParentFieldExpression<TListElement, TParentList, TRootListElement, TProperty> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> expression)
        {
            var fieldExpression =
                new ListParentFieldExpression<TListElement, TParentList, TRootListElement, TProperty>(expression, this,
                    this.ObjectGraphService);

            return fieldExpression;
        }

        public new virtual ListParentOperableList<TListElement, TParentList, TRootListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
            return this;
        }

        public new virtual ListParentOperableList<TListElement, TParentList, TRootListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<object> guaranteedValues,
            int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public new virtual ListParentOperableList<TListElement, TParentList, TRootListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<Func<TListElement>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
            return this;
        }

        public new virtual ListParentOperableList<TListElement, TParentList, TRootListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<Func<TListElement>> guaranteedValues,
            int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public new virtual ListParentOperableList<TListElement, TParentList, TRootListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<TListElement> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
            return this;
        }

        public new virtual ListParentOperableList<TListElement, TParentList, TRootListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<TListElement> guaranteedValues,
            int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public new virtual ListParentOperableList<TListElement, TParentList, TRootListElement> GuaranteeByFixedQuantity(
            IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
            return this;
        }

        public new virtual ListParentOperableList<TListElement, TParentList, TRootListElement> GuaranteeByFixedQuantity(
            IEnumerable<object> guaranteedValues,
            int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this;
        }

        public new virtual ListParentOperableList<TListElement, TParentList, TRootListElement> GuaranteeByFixedQuantity(
            IEnumerable<Func<TListElement>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
            return this;
        }

        public new virtual ListParentOperableList<TListElement, TParentList, TRootListElement> GuaranteeByFixedQuantity(
            IEnumerable<Func<TListElement>> guaranteedValues,
            int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this;
        }

        public new virtual ListParentOperableList<TListElement, TParentList, TRootListElement> GuaranteeByFixedQuantity(
            IEnumerable<TListElement> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
            return this;
        }

        public new virtual ListParentMakeableEnumerable<ListParentOperableList<TResult,
                ListParentOperableList<TListElement, TParentList, TRootListElement>, TRootListElement>, TRootListElement>

            Select<TResult>(Expression<Func<TListElement, IEnumerable<TResult>>> selector, int listSize,
                int listCollectionSize)
        {
            var listCollection =
                new ListParentOperableList<TResult, ListParentOperableList<TListElement, TParentList, TRootListElement>,
                    TRootListElement>[listCollectionSize];

            for (int i = 0; i < listCollection.Length; i++)
            {
                ListParentOperableList<TResult, ListParentOperableList<TListElement, TParentList, TRootListElement>,
                    TRootListElement> list
                    = this.SetList(selector, listSize);

                listCollection[i] = list;
            }

            var result =
                new ListParentMakeableEnumerable<ListParentOperableList<TResult,
                    ListParentOperableList<TListElement, TParentList, TRootListElement>, TRootListElement>, TRootListElement>(
                    listCollection, this.RootList);

            return result;
        }

        public new virtual ListParentOperableList<TListElement, TParentList, TRootListElement> Ignore<TPropertyType>(
            Expression<Func<TListElement, TPropertyType>> fieldExpression)
        {
            base.Ignore(fieldExpression);
            return this;
        }
    }
}
