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
    public class ListParentOperableList<TListElement, TParentList, TRootListElement> : OperableListEx<TListElement>,
        IMakeableCollectionContainer<TRootListElement>
    {
        public ListParentOperableList(
            OperableListEx<TRootListElement> rootList,
            TParentList parentList,
            IEnumerable<RecordReference<TListElement>> input, ValueGuaranteePopulator valueGuaranteePopulator,
            BasePopulator populator, IObjectGraphService objectGraphService, IAttributeDecorator attributeDecorator,
            DeepCollectionSettingConverter deepCollectionSettingConverter, ITypeGenerator typeGenerator,
            bool isShallowCopy) : 
            base(input, valueGuaranteePopulator, populator, objectGraphService, attributeDecorator, 
                deepCollectionSettingConverter, typeGenerator, isShallowCopy)
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
                this.TypeGenerator,
                isShallowCopy: false
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
                this.TypeGenerator,
                isShallowCopy:true
            );

            return result;
        }

        public virtual OperableListEx<TRootListElement> RootList
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
            IEnumerable<RecordReference<TListElement>> input = this.InternalEnumerable.Take(count);

            ListParentOperableList<TListElement, TParentList, TRootListElement> result = this.CreateSubset(input);

            this.AddChild(result);
            return result;
        }

        public new ListParentOperableList<TListElement, TParentList, TRootListElement> Skip(int count)
        {
            IEnumerable<RecordReference<TListElement>> input = this.InternalEnumerable.Skip(count);

            ListParentOperableList<TListElement, TParentList, TRootListElement> result = this.CreateSubset(input);

            this.AddChild(result);
            return result;
        }

        private ListParentOperableList<TPropertyElement,
            ListParentOperableList<TListElement, TParentList, TRootListElement>, TRootListElement> SetList<TPropertyElement>(int size)
        {
            List<RecordReference<TPropertyElement>> list = this.CreateRecordReferences<TPropertyElement>(size);

            ListParentOperableList<TPropertyElement, ListParentOperableList<TListElement, TParentList, TRootListElement>,
                TRootListElement> result = this.CreateChild(list);

            this.AddChild(result);
            return result;
        }

        public new virtual ListParentOperableList<TListElement, TParentList, TRootListElement> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, TProperty value)
        {
            base.Set(fieldExpression, value);
            return this;
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

        public new virtual ListParentMakeableEnumerable<ListParentOperableList<TResultElement,
                ListParentOperableList<TListElement, TParentList, TRootListElement>, TRootListElement>, TRootListElement>

            SelectListSet<TResultElement>(Expression<Func<TListElement, IEnumerable<TResultElement>>> selector, int listSize)
        {
            var listCollection =
                new ListParentOperableList<TResultElement, ListParentOperableList<TListElement, TParentList, TRootListElement>,
                    TRootListElement>[this.Count];

            for (int i = 0; i < listCollection.Length; i++)
            {
                ListParentOperableList<TResultElement, ListParentOperableList<TListElement, TParentList, TRootListElement>,
                    TRootListElement> list
                    = this.SetList<TResultElement>(listSize);

                listCollection[i] = list;
                this[i].AddToExplicitPropertySetters(selector, list);
            }

            var result =
                new ListParentMakeableEnumerable<ListParentOperableList<TResultElement,
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
