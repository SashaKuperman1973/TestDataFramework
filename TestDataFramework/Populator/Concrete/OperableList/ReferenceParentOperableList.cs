﻿/*
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
    public class ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement> : OperableList<TListElement>,
        IMakeable<TRootElement>
    {
        public ReferenceParentOperableList(
            RootReferenceParentOperableList<TRootListElement, TRootElement> rootList,
            RecordReference<TRootElement> root,
            TParentList parentList,
            IEnumerable<RecordReference<TListElement>> input, ValueGuaranteePopulator valueGuaranteePopulator,
            BasePopulator populator, IObjectGraphService objectGraphService, IAttributeDecorator attributeDecorator,
            DeepCollectionSettingConverter deepCollectionSettingConverter, ITypeGenerator typeGenerator) : 
            base(input, valueGuaranteePopulator, populator, objectGraphService, attributeDecorator,
                deepCollectionSettingConverter, typeGenerator)
        {
            this.Root = root;
            this.RootList = rootList;
            this.ParentList = parentList;
        }

        private ReferenceParentOperableList<
                TChildListElement,
                ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement>,
                TRootListElement,
                TRootElement>

            CreateChild<TChildListElement>(
                IEnumerable<RecordReference<TChildListElement>> input
            )
        {
            var result =
                new ReferenceParentOperableList<
                    TChildListElement,
                    ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement>,
                    TRootListElement,
                    TRootElement>
                (
                    this.RootList,
                    this.Root,
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

        private ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement>

            CreateSubset(
                IEnumerable<RecordReference<TListElement>> input
            )
        {
            var subset =
                new ReferenceParentOperableList<
                    TListElement,
                    TParentList,
                    TRootListElement,
                    TRootElement>(
                    this.RootList,
                    this.Root,
                    this.ParentList,
                    input,
                    this.ValueGuaranteePopulator,
                    this.Populator,
                    this.ObjectGraphService,
                    this.AttributeDecorator,
                    this.DeepCollectionSettingConverter,
                    this.TypeGenerator
                );

            return subset;
        }

        public virtual RootReferenceParentOperableList<TRootListElement, TRootElement> RootList
        {
            get;
        }

        public virtual TParentList ParentList { get; }

        public RecordReference<TRootElement> Root { get; set; }

        public new virtual TRootElement Make()
        {
            this.Root.Populate();
            return this.Root.RecordObject;
        }

        public new virtual TRootElement BindAndMake()
        {
            this.Populator.Bind();
            return this.Root.RecordObject;
        }

        public new ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement> Take(int count)
        {
            IEnumerable<RecordReference<TListElement>> input = this.InternalList.Take(count);

            ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement> result = this.CreateSubset(input);

            this.Children.Add(result);
            return result;
        }

        public new ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement> Skip(int count)
        {
            IEnumerable<RecordReference<TListElement>> input = this.InternalList.Skip(count);

            ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement> result = this.CreateSubset(input);

            this.Children.Add(result);
            return result;
        }

        public new virtual ReferenceParentOperableList<TPropertyElement,
            ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement>, TRootListElement, TRootElement> SetList<TPropertyElement>(
            Expression<Func<TListElement, IEnumerable<TPropertyElement>>> listFieldExpression, int size)
        {
            List<RecordReference<TPropertyElement>> list = this.CreateRecordReferences<TPropertyElement>(size);

            ReferenceParentOperableList<TPropertyElement, ReferenceParentOperableList<TListElement, TParentList,
                    TRootListElement, TRootElement>,
                TRootListElement, TRootElement> result = this.CreateChild(list);

            this.Children.Add(result);
            return result;
        }

        public new virtual ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, TProperty value)
        {
            return this.Set(fieldExpression, () => value);
        }

        public new virtual ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, Func<TProperty> valueFactory)
        {
            base.Set(fieldExpression, valueFactory);
            return this;
        }

        public new virtual ReferenceParentFieldExpression<TListElement, TProperty, TParentList, TRootListElement, TRootElement> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> expression)
        {
            var fieldExpression =
                new ReferenceParentFieldExpression<TListElement, TProperty, TParentList, TRootListElement, TRootElement>(expression, this,
                    this.ObjectGraphService);

            return fieldExpression;
        }

        public new virtual ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement> GuaranteeByPercentageOfTotal(
            IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public new virtual ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement> GuaranteeByPercentageOfTotal(
            IEnumerable<object> guaranteedValues,
            int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public new virtual ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement> GuaranteeByPercentageOfTotal(
            IEnumerable<Func<TListElement>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
            return this;
        }

        public new virtual ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement> GuaranteeByPercentageOfTotal(
            IEnumerable<Func<TListElement>> guaranteedValues,
            int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public new virtual ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement> GuaranteeByPercentageOfTotal(
            IEnumerable<TListElement> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
            return this;
        }

        public new virtual ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement> GuaranteeByPercentageOfTotal(
            IEnumerable<TListElement> guaranteedValues,
            int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public new virtual ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement> GuaranteeByFixedQuantity(
            IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
            return this;
        }

        public new virtual ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement> GuaranteeByFixedQuantity(
            IEnumerable<object> guaranteedValues,
            int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this;
        }

        public virtual ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement> GuaranteeByFixedQuantity(
            IEnumerable<Func<TListElement>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
            return this;
        }

        public virtual ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement> GuaranteeByFixedQuantity(
            IEnumerable<Func<TListElement>> guaranteedValues,
            int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this;
        }

        public virtual ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement> GuaranteeByFixedQuantity(
            IEnumerable<TListElement> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
            return this;
        }

        public virtual ReferenceParentMakeableEnumerable<ReferenceParentOperableList<TResult,
                ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement>, TRootListElement
                , TRootElement>, TRootElement>
            Select<TResult>(Expression<Func<TListElement, IEnumerable<TResult>>> selector, int listSize,
                int listCollectionSize)
        {
            var listCollection =
                new ReferenceParentOperableList<TResult, ReferenceParentOperableList<TListElement, TParentList,
                        TRootListElement, TRootElement>,
                    TRootListElement, TRootElement>[listCollectionSize];

            for (int i = 0; i < listCollection.Length; i++)
            {
                ReferenceParentOperableList<TResult, ReferenceParentOperableList<TListElement, TParentList,
                        TRootListElement, TRootElement>,
                    TRootListElement, TRootElement> list
                    = this.SetList(selector, listSize);

                listCollection[i] = list;
            }

            var result =
                new ReferenceParentMakeableEnumerable<ReferenceParentOperableList<TResult,
                    ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement>,
                    TRootListElement, TRootElement>, TRootElement>(
                    listCollection, this.Root);

            return result;
        }

        public new virtual ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement> Ignore<TPropertyType>(
            Expression<Func<TListElement, TPropertyType>> fieldExpression)
        {
            base.Ignore(fieldExpression);
            return this;
        }
    }
}
