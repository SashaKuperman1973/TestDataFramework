/*
    Copyright 2016, 2017, 2018, 2019, 2023 Alexander Kuperman

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
using log4net;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Logger;
using TestDataFramework.Populator.Concrete.FieldExpression;
using TestDataFramework.Populator.Concrete.MakeableEnumerable;
using TestDataFramework.Populator.Interfaces;
using TestDataFramework.TypeGenerator.Interfaces;

namespace TestDataFramework.Populator.Concrete.OperableList
{
    public class ReferenceParentOperableList<TListElement, TRootElement> :
        ReferenceParentOperableList<TListElement, RootReferenceParentOperableList<TListElement, TRootElement>,
            TListElement, TRootElement>
    {
        public ReferenceParentOperableList(RootReferenceParentOperableList<TListElement, TRootElement> rootList,
            RecordReference<TRootElement> root, RootReferenceParentOperableList<TListElement, TRootElement> parentList,
            IEnumerable<RecordReference<TListElement>> input, ValueGuaranteePopulator valueGuaranteePopulator,
            BasePopulator populator, IObjectGraphService objectGraphService, IAttributeDecorator attributeDecorator,
            DeepCollectionSettingConverter deepCollectionSettingConverter, ITypeGenerator typeGenerator,
            bool isShallowCopy) : base(rootList, root, parentList, input, valueGuaranteePopulator, populator,
            objectGraphService, attributeDecorator, deepCollectionSettingConverter, typeGenerator, isShallowCopy)
        {
        }
    }

    public class ReferenceParentOperableList<TListElement, TParentListElement, TRootElement> :
        ReferenceParentOperableList<TListElement, RootReferenceParentOperableList<TParentListElement, TRootElement>, TParentListElement, TRootElement>
    {
        public ReferenceParentOperableList(RootReferenceParentOperableList<TParentListElement, TRootElement> rootList,
            RecordReference<TRootElement> root,
            RootReferenceParentOperableList<TParentListElement, TRootElement> parentList,
            IEnumerable<RecordReference<TListElement>> input, ValueGuaranteePopulator valueGuaranteePopulator,
            BasePopulator populator, IObjectGraphService objectGraphService, IAttributeDecorator attributeDecorator,
            DeepCollectionSettingConverter deepCollectionSettingConverter, ITypeGenerator typeGenerator,
            bool isShallowCopy) : base(rootList, root, parentList, input, valueGuaranteePopulator, populator,
            objectGraphService, attributeDecorator, deepCollectionSettingConverter, typeGenerator, isShallowCopy)
        {
        }
    }

    public class ReferenceParentOperableList<TResultElement, TListElement, TParentList, TRootListElement, TRootElement> :
        ReferenceParentOperableList<
            TResultElement,
            ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement>,
            TRootListElement,
            TRootElement>
    {
        public ReferenceParentOperableList(RootReferenceParentOperableList<TRootListElement, TRootElement> rootList,
            RecordReference<TRootElement> root,
            ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement> parentList,
            IEnumerable<RecordReference<TResultElement>> input, ValueGuaranteePopulator valueGuaranteePopulator,
            BasePopulator populator, IObjectGraphService objectGraphService, IAttributeDecorator attributeDecorator,
            DeepCollectionSettingConverter deepCollectionSettingConverter, ITypeGenerator typeGenerator,
            bool isShallowCopy) : base(rootList, root, parentList, input, valueGuaranteePopulator, populator,
            objectGraphService, attributeDecorator, deepCollectionSettingConverter, typeGenerator, isShallowCopy)
        {
        }
    }

    public class ShortReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement> :
        ReferenceParentOperableList<TListElement, ReferenceParentOperableList<TListElement, TParentList,
            TRootListElement, TRootElement>, TRootListElement, TRootElement>
    {
        public ShortReferenceParentOperableList(
            RootReferenceParentOperableList<TRootListElement, TRootElement> rootList,
            RecordReference<TRootElement> root,
            ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRootElement> parentList,
            IEnumerable<RecordReference<TListElement>> input, ValueGuaranteePopulator valueGuaranteePopulator,
            BasePopulator populator, IObjectGraphService objectGraphService, IAttributeDecorator attributeDecorator,
            DeepCollectionSettingConverter deepCollectionSettingConverter, ITypeGenerator typeGenerator,
            bool isShallowCopy) : base(rootList, root, parentList, input, valueGuaranteePopulator, populator,
            objectGraphService, attributeDecorator, deepCollectionSettingConverter, typeGenerator, isShallowCopy)
        {
        }
    }

    public class OperationReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot> :
        ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>
    {
        private static readonly ILog Logger =
            StandardLogManager.GetLogger(
                typeof(OperationReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>));

        private readonly IAttributeDecorator attributeDecorator;
        private readonly DeepCollectionSettingConverter deepCollectionSettingConverter;
        private readonly IObjectGraphService objectGraphService;
        private readonly ITypeGenerator typeGenerator;
        private readonly ValueGuaranteePopulator valueGuaranteePopulator;

        private readonly ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>
            preOperationList;

        public OperationReferenceParentOperableList(
            RootReferenceParentOperableList<TRootListElement, TRoot> rootList,
            RecordReference<TRoot> root, TParentList parentList,
            ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot> preOperationList,
            IEnumerable<RecordReference<TListElement>> input, ValueGuaranteePopulator valueGuaranteePopulator,
            BasePopulator populator, IObjectGraphService objectGraphService, IAttributeDecorator attributeDecorator,
            DeepCollectionSettingConverter deepCollectionSettingConverter, ITypeGenerator typeGenerator,
            bool isShallowCopy) : base(rootList, root, parentList, input, valueGuaranteePopulator, populator,
            objectGraphService, attributeDecorator, deepCollectionSettingConverter, typeGenerator, isShallowCopy)
        {
            this.attributeDecorator = attributeDecorator;
            this.deepCollectionSettingConverter = deepCollectionSettingConverter;
            this.objectGraphService = objectGraphService;
            this.typeGenerator = typeGenerator;
            this.valueGuaranteePopulator = valueGuaranteePopulator;

            this.preOperationList = preOperationList;
        }

        private ShortReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>
            CreateSubsetForTakeOrSkip(
                IEnumerable<RecordReference<TListElement>> input)
        {
            var result = new ShortReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>(
                this.RootList,
                this.Root,
                this.preOperationList,
                input,
                this.valueGuaranteePopulator,
                this.Populator,
                this.objectGraphService,
                this.attributeDecorator,
                this.deepCollectionSettingConverter,
                this.typeGenerator,
                isShallowCopy: true
                );

            this.AddChild(result);
            return result;
        }

        public new virtual ShortReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>
            Take(int count)
        {
            Logger.Calling(nameof(this.Take), $"Count: {count}");

            IEnumerable<RecordReference<TListElement>> input = this.InternalEnumerable.Take(count);

            ShortReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot> result =
                this.CreateSubsetForTakeOrSkip(input);

            return result;
        }

        public new virtual ShortReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>
            Skip(int count)
        {
            Logger.Calling(nameof(this.Skip), $"Count: {count}");

            IEnumerable<RecordReference<TListElement>> input = this.InternalEnumerable.Skip(count);

            ShortReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot> result =
                this.CreateSubsetForTakeOrSkip(input);

            return result;
        }
    }

    public class ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot> :
        OperableListEx<TListElement>,
        IMakeable<TRoot>
    {
        private static readonly ILog Logger =
            StandardLogManager.GetLogger(
                typeof(ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>));

        public ReferenceParentOperableList(            
            RootReferenceParentOperableList<TRootListElement, TRoot> rootList,
            RecordReference<TRoot> root,
            TParentList parentList,
            IEnumerable<RecordReference<TListElement>> input, ValueGuaranteePopulator valueGuaranteePopulator,
            BasePopulator populator, IObjectGraphService objectGraphService, IAttributeDecorator attributeDecorator,
            DeepCollectionSettingConverter deepCollectionSettingConverter, ITypeGenerator typeGenerator,
            bool isShallowCopy) :
            base(input, valueGuaranteePopulator, populator, objectGraphService, attributeDecorator,
                deepCollectionSettingConverter, typeGenerator, isShallowCopy)
        {
            this.Root = root;
            this.RootList = rootList;
            this.ParentList = parentList;

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

        private ReferenceParentOperableList<TChildListElement, TListElement, TParentList, TRootListElement, TRoot>
            CreateChild<TChildListElement>(
                IEnumerable<RecordReference<TChildListElement>> input
            )
        {
            var result =
                new ReferenceParentOperableList<TChildListElement, TListElement, TParentList, TRootListElement, TRoot>
                (
                    this.RootList,
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

            this.AddChild(result);
            return result;
        }

        private ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>
            CreateSubset(
                IEnumerable<RecordReference<TListElement>> input
            )
        {
            var subset =
                new ReferenceParentOperableList<
                    TListElement,
                    TParentList,
                    TRootListElement,
                    TRoot>(
                    this.RootList,
                    this.Root,
                    this.ParentList,
                    input,
                    this.valueGuaranteePopulator,
                    this.Populator,
                    this.objectGraphService,
                    this.attributeDecorator,
                    this.deepCollectionSettingConverter,
                    this.typeGenerator,
                    isShallowCopy: true
                );

            this.AddChild(subset);
            return subset;
        }

        private OperationReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot> CreateCopyWithThisParent()
        {
            var result = new OperationReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>(
                this.RootList,
                this.Root,
                this.ParentList,
                this,
                this.InternalEnumerable,
                this.valueGuaranteePopulator,
                this.Populator,
                this.objectGraphService,
                this.attributeDecorator,
                this.deepCollectionSettingConverter,
                this.typeGenerator,
                isShallowCopy: true
            );

            this.AddChild(result);
            return result;
        }

        public virtual RootReferenceParentOperableList<TRootListElement, TRoot> RootList { get; }

        public virtual TParentList ParentList { get; }

        public virtual RecordReference<TRoot> Root { get; }

        public new virtual TRoot Make()
        {
            ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>.Logger.Calling(nameof(this.Make));

            this.Root.Populate();
            return this.Root.RecordObject;
        }

        public new virtual TRoot BindAndMake()
        {
            this.Populator.Bind();
            return this.Root.RecordObject;
        }

        public new virtual ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>
            Take(int count)
        {
            ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>.Logger.Calling(
                nameof(this.Take), $"Count: {count}");

            IEnumerable<RecordReference<TListElement>> input = this.InternalEnumerable.Take(count);

            ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot> result =
                this.CreateSubset(input);
            return result;
        }

        public new virtual ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>
            Skip(int count)
        {
            ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>.Logger.Calling(
                nameof(this.Skip), $"Count: {count}");

            IEnumerable<RecordReference<TListElement>> input = this.InternalEnumerable.Skip(count);

            ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot> result =
                this.CreateSubset(input);
            return result;
        }

        private ReferenceParentOperableList<
            TPropertyElement, 
            TListElement, 
            TParentList, 
            TRootListElement, 
            TRoot> SetList<TPropertyElement>(int size)
        {
            List<RecordReference<TPropertyElement>> list = this.CreateRecordReferences<TPropertyElement>(size);

            ReferenceParentOperableList<TPropertyElement, TListElement, TParentList, TRootListElement, TRoot>
                result = this.CreateChild(list);
            return result;
        }

        public new virtual OperationReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>
            Set<TProperty>(
                Expression<Func<TListElement, TProperty>> fieldExpression, TProperty value)
        {
            base.Set(fieldExpression, value);
            return this.CreateCopyWithThisParent();
        }

        public new virtual OperationReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>
            Set<TProperty>(
                Expression<Func<TListElement, TProperty>> fieldExpression, Func<TProperty> valueFactory)
        {
            base.Set(fieldExpression, valueFactory);
            return this.CreateCopyWithThisParent();
        }

        public new virtual ReferenceParentFieldExpression<TListElement, TProperty, TParentList, TRootListElement,
            TRoot> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> expression)
        {
            ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>.Logger.Calling(
                nameof(this.Set), $"Returning a FieldExpression - Selector: {expression}");

            var fieldExpression =
                new ReferenceParentFieldExpression<TListElement, TProperty, TParentList, TRootListElement, TRoot>(
                    expression, this,
                    this.objectGraphService);

            return fieldExpression;
        }

        public new virtual OperationReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>
            GuaranteeByPercentageOfTotal(
                IEnumerable<object> guaranteedValues,
                ValueCountRequestOption valueCountRequestOption =
                    ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, valueCountRequestOption);
            return this.CreateCopyWithThisParent();
        }

        public new virtual OperationReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>
            GuaranteeByPercentageOfTotal(
                IEnumerable<object> guaranteedValues,
                int frequencyPercentage,
                ValueCountRequestOption valueCountRequestOption =
                    ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this.CreateCopyWithThisParent();
        }

        public new virtual OperationReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>
            GuaranteeByPercentageOfTotal(
                IEnumerable<Func<TListElement>> guaranteedValues,
                ValueCountRequestOption valueCountRequestOption =
                    ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, valueCountRequestOption);
            return this.CreateCopyWithThisParent();
        }

        public new virtual OperationReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>
            GuaranteeByPercentageOfTotal(
                IEnumerable<Func<TListElement>> guaranteedValues,
                int frequencyPercentage,
                ValueCountRequestOption valueCountRequestOption =
                    ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this.CreateCopyWithThisParent();
        }

        public new virtual OperationReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>
            GuaranteeByPercentageOfTotal(
                IEnumerable<TListElement> guaranteedValues,
                ValueCountRequestOption valueCountRequestOption =
                    ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, valueCountRequestOption);
            return this.CreateCopyWithThisParent();
        }

        public new virtual OperationReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>
            GuaranteeByPercentageOfTotal(
                IEnumerable<TListElement> guaranteedValues,
                int frequencyPercentage,
                ValueCountRequestOption valueCountRequestOption =
                    ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this.CreateCopyWithThisParent();
        }

        public new virtual OperationReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>
            GuaranteeByFixedQuantity(
                IEnumerable<object> guaranteedValues,
                ValueCountRequestOption valueCountRequestOption =
                    ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, valueCountRequestOption);
            return this.CreateCopyWithThisParent();
        }

        public new virtual OperationReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>
            GuaranteeByFixedQuantity(
                IEnumerable<object> guaranteedValues,
                int fixedQuantity,
                ValueCountRequestOption valueCountRequestOption =
                    ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this.CreateCopyWithThisParent();
        }

        public new virtual OperationReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>
            GuaranteeByFixedQuantity(
                IEnumerable<Func<TListElement>> guaranteedValues,
                ValueCountRequestOption valueCountRequestOption =
                    ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, valueCountRequestOption);
            return this.CreateCopyWithThisParent();
        }

        public new virtual OperationReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>
            GuaranteeByFixedQuantity(
                IEnumerable<Func<TListElement>> guaranteedValues,
                int fixedQuantity,
                ValueCountRequestOption valueCountRequestOption =
                    ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this.CreateCopyWithThisParent();
        }

        public new virtual OperationReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>
            GuaranteeByFixedQuantity(
                IEnumerable<TListElement> guaranteedValues,
                ValueCountRequestOption valueCountRequestOption =
                    ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, valueCountRequestOption);
            return this.CreateCopyWithThisParent();
        }

        public new virtual OperationReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>
            GuaranteeByFixedQuantity(
                IEnumerable<TListElement> guaranteedValues,
                int fixedQuantity,
                ValueCountRequestOption valueCountRequestOption =
                    ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this.CreateCopyWithThisParent();
        }

        public new virtual ReferenceParentMakeableEnumerable<TResultElement, TListElement, TParentList, TRootListElement, TRoot>
            SelectListSet<TResultElement>(Expression<Func<TListElement, IEnumerable<TResultElement>>> selector, int listSize)
        {
            ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>.Logger.Entering(
                nameof(this.SelectListSet), $"Selector: {selector} - List size: {listSize}");

            var listCollection =
                new ReferenceParentOperableList<TResultElement, TListElement, TParentList, TRootListElement, TRoot>[this.Count];

            for (int i = 0; i < listCollection.Length; i++)
            {
                ReferenceParentOperableList<TResultElement, TListElement, TParentList, TRootListElement, TRoot> list = 
                    this.SetList<TResultElement>(listSize);

                listCollection[i] = list;
                this[i].AddToExplicitPropertySetters(selector, list);
            }

            var result =
                new ReferenceParentMakeableEnumerable<TResultElement, TListElement, TParentList, TRootListElement, TRoot>(
                    listCollection, this.Root, this, this.RootList);

            ReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>.Logger.Exiting(nameof(this
                .SelectListSet));

            return result;
        }

        public new virtual OperationReferenceParentOperableList<TListElement, TParentList, TRootListElement, TRoot>
            Ignore<TPropertyType>(
                Expression<Func<TListElement, TPropertyType>> fieldExpression)
        {
            base.Ignore(fieldExpression);
            return this.CreateCopyWithThisParent();
        }
    }
}
