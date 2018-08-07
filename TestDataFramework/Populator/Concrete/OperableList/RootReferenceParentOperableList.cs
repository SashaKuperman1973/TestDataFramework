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
    public class RootReferenceParentOperableList<TListElement, TRootElement> : OperableListEx<TListElement>,
        IMakeable<TRootElement>
    {
        public RootReferenceParentOperableList(
            RecordReference<TRootElement> root,
            IEnumerable<RecordReference<TListElement>> input, ValueGuaranteePopulator valueGuaranteePopulator,
            BasePopulator populator, IObjectGraphService objectGraphService, IAttributeDecorator attributeDecorator,
            DeepCollectionSettingConverter deepCollectionSettingConverter, ITypeGenerator typeGenerator) : 
            base(input, valueGuaranteePopulator, populator, objectGraphService, attributeDecorator,
                deepCollectionSettingConverter, typeGenerator)
        {
            this.Root = root;
        }

        private ReferenceParentOperableList<
                TChildListElement,
                RootReferenceParentOperableList<TListElement, TRootElement>,
                TListElement, 
                TRootElement>

            CreateChild<TChildListElement>(
                IEnumerable<RecordReference<TChildListElement>> input
            )
        {
            var result =
                new ReferenceParentOperableList<
                    TChildListElement,
                    RootReferenceParentOperableList<TListElement, TRootElement>,
                    TListElement, 
                    TRootElement>
                (
                    this,
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

        private ReferenceParentOperableList<
                TListElement,
                RootReferenceParentOperableList<TListElement, TRootElement>,
                TListElement,
                TRootElement>

            CreateSubset(RootReferenceParentOperableList<TListElement, TRootElement> rootList,
                IEnumerable<RecordReference<TListElement>> input
            )
        {
            var subset =
                new ReferenceParentOperableList<
                    TListElement,
                    RootReferenceParentOperableList<TListElement, TRootElement>,
                    TListElement,
                    TRootElement>(
                    rootList,
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

            return subset;
        }

        public virtual RecordReference<TRootElement> Root { get; }

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

        public new virtual ReferenceParentOperableList<
                TListElement,
                RootReferenceParentOperableList<TListElement, TRootElement>,
                TListElement,
                TRootElement>

            Take(int count)
        {
            IEnumerable<RecordReference<TListElement>> input = this.InternalList.Take(count);

            ReferenceParentOperableList<TListElement, RootReferenceParentOperableList<TListElement, TRootElement>,
                TListElement, TRootElement> result = this.CreateSubset(this, input);

            this.AddChild(result);
            return result;
        }

        public new virtual ReferenceParentOperableList<
                TListElement,
                RootReferenceParentOperableList<TListElement, TRootElement>,
                TListElement,
                TRootElement>

            Skip(int count)
        {
            IEnumerable<RecordReference<TListElement>> input = this.InternalList.Skip(count);

            ReferenceParentOperableList<TListElement, RootReferenceParentOperableList<TListElement, TRootElement>,
                TListElement, TRootElement> result = this.CreateSubset(this, input);

            this.AddChild(result);
            return result;
        }

        private ReferenceParentOperableList<TPropertyElement,
            RootReferenceParentOperableList<TListElement, TRootElement>,
            TListElement, TRootElement> SetList<TPropertyElement>(int size)
        {
            List<RecordReference<TPropertyElement>> list = this.CreateRecordReferences<TPropertyElement>(size);

            ReferenceParentOperableList<TPropertyElement, RootReferenceParentOperableList<TListElement, TRootElement>,
                TListElement, TRootElement> result = this.CreateChild(list);

            this.AddChild(result);
            return result;
        }

        public new virtual RootReferenceParentOperableList<TListElement, TRootElement> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, TProperty value)
        {
            return this.Set(fieldExpression, () => value);
        }

        public new virtual RootReferenceParentOperableList<TListElement, TRootElement> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, Func<TProperty> valueFactory)
        {
            base.Set(fieldExpression, valueFactory);
            return this;
        }

        public new virtual RootReferenceParentFieldExpression<TListElement, TProperty, TRootElement> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> expression)
        {
            var fieldExpression =
                new RootReferenceParentFieldExpression<TListElement, TProperty, TRootElement>(expression, this,
                    this.ObjectGraphService);

            return fieldExpression;
        }

        public new virtual RootReferenceParentOperableList<TListElement, TRootElement> GuaranteeByPercentageOfTotal(
            IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
            return this;
        }

        public new virtual RootReferenceParentOperableList<TListElement, TRootElement> GuaranteeByPercentageOfTotal(
            IEnumerable<object> guaranteedValues,
            int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public new virtual RootReferenceParentOperableList<TListElement, TRootElement> GuaranteeByPercentageOfTotal(
            IEnumerable<Func<TListElement>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
            return this;
        }

        public new virtual RootReferenceParentOperableList<TListElement, TRootElement> GuaranteeByPercentageOfTotal(
            IEnumerable<Func<TListElement>> guaranteedValues,
            int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public new virtual RootReferenceParentOperableList<TListElement, TRootElement> GuaranteeByPercentageOfTotal(
            IEnumerable<TListElement> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
            return this;
        }

        public new virtual RootReferenceParentOperableList<TListElement, TRootElement> GuaranteeByPercentageOfTotal(
            IEnumerable<TListElement> guaranteedValues,
            int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public new virtual RootReferenceParentOperableList<TListElement, TRootElement> GuaranteeByFixedQuantity(
            IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
            return this;
        }

        public new virtual RootReferenceParentOperableList<TListElement, TRootElement> GuaranteeByFixedQuantity(
            IEnumerable<object> guaranteedValues,
            int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this;
        }

        public new virtual RootReferenceParentOperableList<TListElement, TRootElement> GuaranteeByFixedQuantity(
            IEnumerable<Func<TListElement>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
            return this;
        }

        public new virtual RootReferenceParentOperableList<TListElement, TRootElement> GuaranteeByFixedQuantity(
            IEnumerable<Func<TListElement>> guaranteedValues,
            int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this;
        }

        public new virtual RootReferenceParentOperableList<TListElement, TRootElement> GuaranteeByFixedQuantity(
            IEnumerable<TListElement> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
            return this;
        }

        public new virtual ReferenceParentMakeableEnumerable<ReferenceParentOperableList<TResultElement,
                RootReferenceParentOperableList<TListElement, TRootElement>, TListElement, TRootElement>, TRootElement>

            SelectListSet<TResultElement>(Expression<Func<TListElement, IEnumerable<TResultElement>>> selector, int listSize)
        {
            var listCollection =
                new ReferenceParentOperableList<TResultElement, RootReferenceParentOperableList<TListElement, TRootElement>,
                    TListElement, TRootElement>[this.Count];

            for (int i = 0; i < listCollection.Length; i++)
            {
                ReferenceParentOperableList<TResultElement, RootReferenceParentOperableList<TListElement, TRootElement>,
                    TListElement, TRootElement> list
                    = this.SetList<TResultElement>(listSize);

                listCollection[i] = list;
                this.InternalList[i].AddToExplicitPropertySetters(selector, list);
            }

            var result =
                new ReferenceParentMakeableEnumerable<ReferenceParentOperableList<TResultElement,
                    RootReferenceParentOperableList<TListElement, TRootElement>,
                    TListElement, TRootElement>, TRootElement>(
                    listCollection, this.Root);

            return result;
        }

        public new virtual RootReferenceParentOperableList<TListElement, TRootElement> Ignore<TPropertyType>(
            Expression<Func<TListElement, TPropertyType>> fieldExpression)
        {
            base.Ignore(fieldExpression);
            return this;
        }
    }
}
