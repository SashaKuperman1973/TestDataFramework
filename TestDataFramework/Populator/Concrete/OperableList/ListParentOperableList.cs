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
    public class ListParentOperableList<TListElement> : 
        ListParentOperableList<TListElement, OperableListEx<TListElement>, TListElement>
    {
        public ListParentOperableList(OperableListEx<TListElement> rootList, OperableListEx<TListElement> parentList,
            IEnumerable<RecordReference<TListElement>> input, ValueGuaranteePopulator valueGuaranteePopulator,
            BasePopulator populator, IObjectGraphService objectGraphService, IAttributeDecorator attributeDecorator,
            DeepCollectionSettingConverter deepCollectionSettingConverter, ITypeGenerator typeGenerator,
            bool isShallowCopy) : base(rootList, parentList, input, valueGuaranteePopulator, populator,
            objectGraphService, attributeDecorator, deepCollectionSettingConverter, typeGenerator, isShallowCopy)
        {
        }
    }

    public class ListParentOperableList<TListElement, TParentListElement> :
        ListParentOperableList<TListElement, OperableListEx<TParentListElement>, TParentListElement>
    {
        public ListParentOperableList(OperableListEx<TParentListElement> rootList,
            OperableListEx<TParentListElement> parentList, IEnumerable<RecordReference<TListElement>> input,
            ValueGuaranteePopulator valueGuaranteePopulator, BasePopulator populator,
            IObjectGraphService objectGraphService, IAttributeDecorator attributeDecorator,
            DeepCollectionSettingConverter deepCollectionSettingConverter, ITypeGenerator typeGenerator,
            bool isShallowCopy) : base(rootList, parentList, input, valueGuaranteePopulator, populator,
            objectGraphService, attributeDecorator, deepCollectionSettingConverter, typeGenerator, isShallowCopy)
        {
        }
    }

    public class ListParentOperableList<TResultElement, TListElement, TParentList, TRootListElement> :
        ListParentOperableList<TResultElement, ListParentOperableList<TListElement, TParentList, TRootListElement>,
            TRootListElement>
    {
        public ListParentOperableList(OperableListEx<TRootListElement> rootList,
            ListParentOperableList<TListElement, TParentList, TRootListElement> parentList,
            IEnumerable<RecordReference<TResultElement>> input, ValueGuaranteePopulator valueGuaranteePopulator,
            BasePopulator populator, IObjectGraphService objectGraphService, IAttributeDecorator attributeDecorator,
            DeepCollectionSettingConverter deepCollectionSettingConverter, ITypeGenerator typeGenerator,
            bool isShallowCopy) : base(rootList, parentList, input, valueGuaranteePopulator, populator,
            objectGraphService, attributeDecorator, deepCollectionSettingConverter, typeGenerator, isShallowCopy)
        {
        }
    }

    public class ShortListParentOperableList<TListElement, TParentList, TRootListElement> :
        ListParentOperableList<
            TListElement,
            ListParentOperableList<TListElement, TParentList, TRootListElement>,
            TRootListElement>
    {
        public ShortListParentOperableList(OperableListEx<TRootListElement> rootList,
            ListParentOperableList<TListElement, TParentList, TRootListElement> parentList,
            IEnumerable<RecordReference<TListElement>> input, ValueGuaranteePopulator valueGuaranteePopulator,
            BasePopulator populator, IObjectGraphService objectGraphService, IAttributeDecorator attributeDecorator,
            DeepCollectionSettingConverter deepCollectionSettingConverter, ITypeGenerator typeGenerator,
            bool isShallowCopy) : base(rootList, parentList, input, valueGuaranteePopulator, populator,
            objectGraphService, attributeDecorator, deepCollectionSettingConverter, typeGenerator, isShallowCopy)
        {
        }
    }

    public class OperationListParentOperableList<TListElement, TParentList, TRootListElement> :
        ListParentOperableList<TListElement, TParentList, TRootListElement>
    {
        private static readonly ILog Logger =
            StandardLogManager.GetLogger(
                typeof(OperationListParentOperableList<TListElement, TParentList, TRootListElement>));

        private readonly ListParentOperableList<TListElement, TParentList, TRootListElement> preOperationList;

        private readonly IAttributeDecorator attributeDecorator;
        private readonly DeepCollectionSettingConverter deepCollectionSettingConverter;
        private readonly IObjectGraphService objectGraphService;
        private readonly ITypeGenerator typeGenerator;
        private readonly ValueGuaranteePopulator valueGuaranteePopulator;

        public OperationListParentOperableList(OperableListEx<TRootListElement> rootList, TParentList parentList,
            ListParentOperableList<TListElement, TParentList, TRootListElement> preOperationList,
            IEnumerable<RecordReference<TListElement>> input, ValueGuaranteePopulator valueGuaranteePopulator,
            BasePopulator populator, IObjectGraphService objectGraphService, IAttributeDecorator attributeDecorator,
            DeepCollectionSettingConverter deepCollectionSettingConverter, ITypeGenerator typeGenerator,
            bool isShallowCopy) : base(rootList, parentList, input, valueGuaranteePopulator, populator,
            objectGraphService, attributeDecorator, deepCollectionSettingConverter, typeGenerator, isShallowCopy)
        {
            this.preOperationList = preOperationList;

            this.attributeDecorator = attributeDecorator;
            this.deepCollectionSettingConverter = deepCollectionSettingConverter;
            this.objectGraphService = objectGraphService;
            this.typeGenerator = typeGenerator;
            this.valueGuaranteePopulator = valueGuaranteePopulator;
        }

        private ShortListParentOperableList<TListElement, TParentList, TRootListElement> CreateSubsetForTakeOrSkip(
            IEnumerable<RecordReference<TListElement>> input)
        {
            var result = new ShortListParentOperableList<TListElement, TParentList, TRootListElement>
            (
                this.RootList,
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

        public new virtual ShortListParentOperableList<TListElement, TParentList, TRootListElement> Take(int count)
        {
            OperationListParentOperableList<TListElement, TParentList, TRootListElement>.Logger.Entering(nameof(this.Take), "Count: " + count);
            IEnumerable<RecordReference<TListElement>> input = this.InternalEnumerable.Take(count);

            ShortListParentOperableList<TListElement, TParentList, TRootListElement> result = this.CreateSubsetForTakeOrSkip(input);
            OperationListParentOperableList<TListElement, TParentList, TRootListElement>.Logger.Exiting(nameof(this.Take));
            return result;
        }

        public new virtual ShortListParentOperableList<TListElement, TParentList, TRootListElement> Skip(int count)
        {
            OperationListParentOperableList<TListElement, TParentList, TRootListElement>.Logger.Entering(nameof(this.Skip), "Count: " + count);
            IEnumerable<RecordReference<TListElement>> input = this.InternalEnumerable.Skip(count);

            ShortListParentOperableList<TListElement, TParentList, TRootListElement> result = this.CreateSubsetForTakeOrSkip(input);
            OperationListParentOperableList<TListElement, TParentList, TRootListElement>.Logger.Exiting(nameof(this.Skip));
            return result;
        }
    }

    public class ListParentOperableList<TListElement, TParentList, TRootListElement> : OperableListEx<TListElement>,
        IMakeableCollectionContainer<TRootListElement>
    {
        private static readonly ILog Logger =
            StandardLogManager.GetLogger(typeof(ListParentOperableList<TListElement, TParentList, TRootListElement>));

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

        private ListParentOperableList<TChildListElement, TListElement, TParentList, TRootListElement>
            CreateChild<TChildListElement>(
                IEnumerable<RecordReference<TChildListElement>> input
            )
        {
            var result = new ListParentOperableList<TChildListElement, TListElement, TParentList, TRootListElement>
            (
                this.RootList,
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

        private ListParentOperableList<TListElement, TParentList, TRootListElement> CreateSubset(
            IEnumerable<RecordReference<TListElement>> input)
        {
            var result = new ListParentOperableList<TListElement, TParentList, TRootListElement>
            (
                this.RootList,
                this.ParentList,
                input,
                this.valueGuaranteePopulator,
                this.Populator,
                this.objectGraphService,
                this.attributeDecorator,
                this.deepCollectionSettingConverter,
                this.typeGenerator,
                isShallowCopy:true
            );

            this.AddChild(result);
            return result;
        }

        private OperationListParentOperableList<TListElement, TParentList, TRootListElement> CreateCopyWithThisParent()
        {
            var result = new OperationListParentOperableList<TListElement, TParentList, TRootListElement>(
                this.RootList,
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

        public virtual OperableListEx<TRootListElement> RootList { get; }

        public virtual TParentList ParentList { get; }

        public new virtual IEnumerable<TRootListElement> Make()
        {
            ListParentOperableList<TListElement, TParentList, TRootListElement>.Logger.Debug("Calling Make");
            this.RootList.Populate();
            return this.RootList.RecordObjects;
        }

        public new virtual IEnumerable<TRootListElement> BindAndMake()
        {
            ListParentOperableList<TListElement, TParentList, TRootListElement>.Logger.Debug("Calling BindAndMake");
            this.Populator.Bind();
            return this.RootList.RecordObjects;
        }

        public new virtual ListParentOperableList<TListElement, TParentList, TRootListElement> Take(int count)
        {
            ListParentOperableList<TListElement, TParentList, TRootListElement>.Logger.Debug("Calling Take. Count: " + count);
            IEnumerable<RecordReference<TListElement>> input = this.InternalEnumerable.Take(count);

            ListParentOperableList<TListElement, TParentList, TRootListElement> result = this.CreateSubset(input);
            return result;
        }

        public new virtual ListParentOperableList<TListElement, TParentList, TRootListElement> Skip(int count)
        {
            ListParentOperableList<TListElement, TParentList, TRootListElement>.Logger.Debug("Calling Skip. Count: " + count);
            IEnumerable<RecordReference<TListElement>> input = this.InternalEnumerable.Skip(count);

            ListParentOperableList<TListElement, TParentList, TRootListElement> result = this.CreateSubset(input);
            return result;
        }

        private ListParentOperableList<TPropertyElement, TListElement, TParentList, TRootListElement> SetList<TPropertyElement>(int size)
        {
            List<RecordReference<TPropertyElement>> list = this.CreateRecordReferences<TPropertyElement>(size);

            ListParentOperableList<TPropertyElement, TListElement, TParentList, TRootListElement> result = this.CreateChild(list);
            return result;
        }

        public new virtual OperationListParentOperableList<TListElement, TParentList, TRootListElement> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, TProperty value)
        {
            ListParentOperableList<TListElement, TParentList, TRootListElement>.Logger.Debug("Calling Set Value. Selector: " + fieldExpression);
            base.Set(fieldExpression, value);
            return this.CreateCopyWithThisParent();
        }

        public new virtual OperationListParentOperableList<TListElement, TParentList, TRootListElement> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, Func<TProperty> valueFactory)
        {
            ListParentOperableList<TListElement, TParentList, TRootListElement>.Logger.Debug("Calling Set Func. Selector: " + fieldExpression);
            base.Set(fieldExpression, valueFactory);
            return this.CreateCopyWithThisParent();
        }

        public new virtual ListParentFieldExpression<TListElement, TParentList, TRootListElement, TProperty> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> expression)
        {
            ListParentOperableList<TListElement, TParentList, TRootListElement>.Logger.Debug(
                "Calling Set returning a FieldExpression. Selector: " + expression);
            var fieldExpression =
                new ListParentFieldExpression<TListElement, TParentList, TRootListElement, TProperty>(expression, this,
                    this.objectGraphService);

            return fieldExpression;
        }

        public new virtual OperationListParentOperableList<TListElement, TParentList, TRootListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, valueCountRequestOption);
            return this.CreateCopyWithThisParent();
        }

        public new virtual OperationListParentOperableList<TListElement, TParentList, TRootListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<object> guaranteedValues,
            int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this.CreateCopyWithThisParent();
        }

        public new virtual OperationListParentOperableList<TListElement, TParentList, TRootListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<Func<TListElement>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, valueCountRequestOption);
            return this.CreateCopyWithThisParent();
        }

        public new virtual OperationListParentOperableList<TListElement, TParentList, TRootListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<Func<TListElement>> guaranteedValues,
            int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this.CreateCopyWithThisParent();
        }

        public new virtual OperationListParentOperableList<TListElement, TParentList, TRootListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<TListElement> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, valueCountRequestOption);
            return this.CreateCopyWithThisParent();
        }

        public new virtual OperationListParentOperableList<TListElement, TParentList, TRootListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<TListElement> guaranteedValues,
            int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this.CreateCopyWithThisParent();
        }

        public new virtual OperationListParentOperableList<TListElement, TParentList, TRootListElement> GuaranteeByFixedQuantity(
            IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, valueCountRequestOption);
            return this.CreateCopyWithThisParent();
        }

        public new virtual OperationListParentOperableList<TListElement, TParentList, TRootListElement> GuaranteeByFixedQuantity(
            IEnumerable<object> guaranteedValues,
            int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this.CreateCopyWithThisParent();
        }

        public new virtual OperationListParentOperableList<TListElement, TParentList, TRootListElement> GuaranteeByFixedQuantity(
            IEnumerable<Func<TListElement>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, valueCountRequestOption);
            return this.CreateCopyWithThisParent();
        }

        public new virtual OperationListParentOperableList<TListElement, TParentList, TRootListElement> GuaranteeByFixedQuantity(
            IEnumerable<Func<TListElement>> guaranteedValues,
            int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this.CreateCopyWithThisParent();
        }

        public new virtual OperationListParentOperableList<TListElement, TParentList, TRootListElement> GuaranteeByFixedQuantity(
            IEnumerable<TListElement> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, valueCountRequestOption);
            return this.CreateCopyWithThisParent();
        }

        public new virtual OperationListParentOperableList<TListElement, TParentList, TRootListElement> GuaranteeByFixedQuantity(
            IEnumerable<TListElement> guaranteedValues,
            int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this.CreateCopyWithThisParent();
        }

        public new virtual ListParentMakeableEnumerable<TResultElement, TListElement, TParentList, TRootListElement>
            SelectListSet<TResultElement>(Expression<Func<TListElement, IEnumerable<TResultElement>>> selector, int listSize)
        {
            ListParentOperableList<TListElement, TParentList, TRootListElement>.Logger.Entering(
                nameof(this.SelectListSet), $"Selector: {selector}. List size: {listSize}");

            var listCollection =
                new ListParentOperableList<TResultElement, TListElement, TParentList, TRootListElement>[this.Count];

            for (int i = 0; i < listCollection.Length; i++)
            {
                ListParentOperableList<TResultElement, TListElement, TParentList, TRootListElement> list
                    = this.SetList<TResultElement>(listSize);

                listCollection[i] = list;
                this[i].AddToExplicitPropertySetters(selector, list);
            }

            var result =
                new ListParentMakeableEnumerable<TResultElement, TListElement, TParentList, TRootListElement>(
                    listCollection, this.RootList, this);

            ListParentOperableList<TListElement, TParentList, TRootListElement>.Logger.Exiting(nameof(this.SelectListSet));
            return result;
        }

        public new virtual OperationListParentOperableList<TListElement, TParentList, TRootListElement> Ignore<TPropertyType>(
            Expression<Func<TListElement, TPropertyType>> fieldExpression)
        {
            base.Ignore(fieldExpression);
            return this.CreateCopyWithThisParent();
        }
    }
}
