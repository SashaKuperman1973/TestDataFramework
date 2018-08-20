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
            DeepCollectionSettingConverter deepCollectionSettingConverter, ITypeGenerator typeGenerator,
            bool isShallowCopy) : 
            base(input, valueGuaranteePopulator, populator, objectGraphService, attributeDecorator,
                deepCollectionSettingConverter, typeGenerator, isShallowCopy)
        {
            this.Root = root;

            this.attributeDecorator = attributeDecorator;
            this.deepCollectionSettingConverter = deepCollectionSettingConverter;
            this.objectGraphService = objectGraphService;
            this.typeGenerator = typeGenerator;
            this.valueGuaranteePopulator = valueGuaranteePopulator;
        }

        private readonly IAttributeDecorator attributeDecorator;
        private readonly DeepCollectionSettingConverter deepCollectionSettingConverter;
        private readonly IObjectGraphService objectGraphService;
        private readonly ITypeGenerator typeGenerator;
        private readonly ValueGuaranteePopulator valueGuaranteePopulator;

        private ReferenceParentOperableList<TChildListElement, TListElement, TRootElement>
            CreateChild<TChildListElement>(
                IEnumerable<RecordReference<TChildListElement>> input
            )
        {
            var result =
                new ReferenceParentOperableList<TChildListElement, TListElement, TRootElement>
                (
                    this,
                    this.Root,
                    this,
                    input,
                    this.valueGuaranteePopulator,
                    this.Populator,
                    this.objectGraphService,
                    this.attributeDecorator,
                    this.deepCollectionSettingConverter,
                    this.typeGenerator,
                    isShallowCopy: false
                );

            return result;
        }

        private ReferenceParentOperableList<TListElement, TRootElement>
            CreateSubset(IEnumerable<RecordReference<TListElement>> input)
        {
            var subset =
                new ReferenceParentOperableList<TListElement, TRootElement>(
                    this,
                    this.Root,
                    this,
                    input,
                    this.valueGuaranteePopulator,
                    this.Populator,
                    this.objectGraphService,
                    this.attributeDecorator,
                    this.deepCollectionSettingConverter,
                    this.typeGenerator,
                    isShallowCopy: true
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

        public new virtual ReferenceParentOperableList<TListElement, TRootElement>
            Take(int count)
        {
            IEnumerable<RecordReference<TListElement>> input = this.InternalEnumerable.Take(count);

            ReferenceParentOperableList<TListElement, TRootElement> result = this.CreateSubset(input);

            this.AddChild(result);
            return result;
        }

        public new virtual ReferenceParentOperableList<TListElement, TRootElement>
            Skip(int count)
        {
            IEnumerable<RecordReference<TListElement>> input = this.InternalEnumerable.Skip(count);

            ReferenceParentOperableList<TListElement, TRootElement> result = this.CreateSubset(input);

            this.AddChild(result);
            return result;
        }

        private ReferenceParentOperableList<TPropertyElement, TListElement, TRootElement> SetList<TPropertyElement>(int size)
        {
            List<RecordReference<TPropertyElement>> list = this.CreateRecordReferences<TPropertyElement>(size);

            ReferenceParentOperableList<TPropertyElement, TListElement, TRootElement> result = this.CreateChild(list);

            this.AddChild(result);
            return result;
        }

        public new virtual RootReferenceParentOperableList<TListElement, TRootElement> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, TProperty value)
        {
            base.Set(fieldExpression, value);
            return this;
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
                    this.objectGraphService);

            return fieldExpression;
        }

        public new virtual RootReferenceParentOperableList<TListElement, TRootElement> GuaranteeByPercentageOfTotal(
            IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, valueCountRequestOption);
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
            base.GuaranteeByPercentageOfTotal(guaranteedValues, valueCountRequestOption);
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
            base.GuaranteeByPercentageOfTotal(guaranteedValues, valueCountRequestOption);
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
            base.GuaranteeByFixedQuantity(guaranteedValues, valueCountRequestOption);
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
            base.GuaranteeByFixedQuantity(guaranteedValues, valueCountRequestOption);
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
            base.GuaranteeByFixedQuantity(guaranteedValues, valueCountRequestOption);
            return this;
        }

        public new virtual ShortReferenceParentMakeableEnumerable<TResultElement, TListElement, TRootElement>
            SelectListSet<TResultElement>(Expression<Func<TListElement, IEnumerable<TResultElement>>> selector, int listSize)
        {
            var listCollection =
                new ReferenceParentOperableList<TResultElement, TListElement, TRootElement>[this.Count];

            for (int i = 0; i < listCollection.Length; i++)
            {
                ReferenceParentOperableList<TResultElement, TListElement, TRootElement> list =
                    this.SetList<TResultElement>(listSize);

                listCollection[i] = list;
                this[i].AddToExplicitPropertySetters(selector, list);
            }

            var result =
                new ShortReferenceParentMakeableEnumerable<TResultElement, TListElement, TRootElement>(listCollection,
                    this.Root, this);

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
