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
using TestDataFramework.TypeGenerator.Interfaces;

namespace TestDataFramework.Populator.Concrete.OperableList
{
    public class OperableListEx<TListElement> : OperableList<TListElement>
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(OperableListEx<TListElement>));

        public OperableListEx(IEnumerable<RecordReference<TListElement>> input,
            ValueGuaranteePopulator valueGuaranteePopulator, BasePopulator populator,
            IObjectGraphService objectGraphService, IAttributeDecorator attributeDecorator,
            DeepCollectionSettingConverter deepCollectionSettingConverter, ITypeGenerator typeGenerator, 
            bool isShallowCopy) : base(input,
            valueGuaranteePopulator, populator, objectGraphService, attributeDecorator, deepCollectionSettingConverter,
            typeGenerator, isShallowCopy)
        {
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

        private ListParentOperableList<
                TChildListElement,
                TListElement>
            CreateChild<TChildListElement>(
                IEnumerable<RecordReference<TChildListElement>> input
            )
        {
            var result = new ListParentOperableList<
                TChildListElement,
                TListElement>(

                this,
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

        private ListParentOperableList<TListElement> CreateSubset(
            IEnumerable<RecordReference<TListElement>> input)
        {
            var result = new ListParentOperableList<TListElement>
            (
                this,
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

            this.AddChild(result);
            return result;
        }

        public virtual ListParentOperableList<TListElement> Take(int count)
        {
            OperableListEx<TListElement>.Logger.Calling(nameof(this.Take), $"Count: {count}");

            IEnumerable<RecordReference<TListElement>> input = this.InternalEnumerable.Take(count);

            ListParentOperableList<TListElement> result = this.CreateSubset(input);
            return result;
        }

        public virtual ListParentOperableList<TListElement> Skip(int count)
        {
            OperableListEx<TListElement>.Logger.Calling(nameof(this.Skip), $"Count: {count}");

            IEnumerable<RecordReference<TListElement>> input = this.InternalEnumerable.Skip(count);

            ListParentOperableList<TListElement> result = this.CreateSubset(input);
            return result;
        }

        private ListParentOperableList<TPropertyElement, TListElement> SetList<TPropertyElement>(int size)
        {
            OperableListEx<TListElement>.Logger.Calling(nameof(this.SetList), $"Size: {size}");

            List<RecordReference<TPropertyElement>> input = this.CreateRecordReferences<TPropertyElement>(size);

            ListParentOperableList<TPropertyElement, TListElement> result = this.CreateChild(input);
            return result;
        }

        public virtual ListParentMakeableEnumerable<ListParentOperableList<TResultElement, TListElement>, TListElement>
            SelectListSet<TResultElement>(Expression<Func<TListElement, IEnumerable<TResultElement>>> selector,
                int listSize)
        {
            OperableListEx<TListElement>.Logger.Entering(nameof(this.SelectListSet), $"Selector: {selector} - List size: {listSize}");

            var listCollection =
                new ListParentOperableList<TResultElement, TListElement>
                    [this.Count];

            for (int i = 0; i < listCollection.Length; i++)
            {
                ListParentOperableList<TResultElement, TListElement> list = this.SetList<TResultElement>(listSize);

                listCollection[i] = list;
                this[i].AddToExplicitPropertySetters(selector, list);
            }

            var result =
                new ListParentMakeableEnumerable<ListParentOperableList<TResultElement, TListElement>, TListElement>(
                    listCollection, this, this);

            OperableListEx<TListElement>.Logger.Exiting(nameof(this.SelectListSet));
            return result;
        }

        public virtual FieldExpressionsSet<TListElement> SetMultipleProperties()
        {
            var result = new FieldExpressionsSet<TListElement>(this, this.objectGraphService);
            return result;
        }

        public new virtual OperableListEx<TListElement> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, TProperty value)
        {
            base.Set(fieldExpression, value);
            return this;
        }

        public new virtual OperableListEx<TListElement> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, Func<TProperty> valueFactory)
        {
            base.Set(fieldExpression, valueFactory);
            return this;
        }

        public virtual FieldExpression<TListElement, TProperty> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> expression)
        {
            OperableListEx<TListElement>.Logger.Calling(nameof(this.Set), $"Returning a FieldExpression - Selector: {expression}");

            var fieldExpression =
                new FieldExpression<TListElement, TProperty>(expression, this,
                    this.objectGraphService);

            return fieldExpression;
        }

        public new virtual OperableListEx<TListElement> Ignore<TPropertyType>(
            Expression<Func<TListElement, TPropertyType>> fieldExpression)
        {
            base.Ignore(fieldExpression);
            return this;
        }

        public new virtual OperableListEx<TListElement> GuaranteeByPercentageOfTotal(IEnumerable<object> guaranteedValues,
            int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public new virtual OperableListEx<TListElement> GuaranteeByFixedQuantity(IEnumerable<object> guaranteedValues,
            int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this;
        }

        public new virtual OperableListEx<TListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, valueCountRequestOption);
            return this;
        }

        public new virtual OperableListEx<TListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<Func<TListElement>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, valueCountRequestOption);
            return this;
        }

        public new virtual OperableListEx<TListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<Func<TListElement>> guaranteedValues,
            int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public new virtual OperableListEx<TListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<TListElement> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, valueCountRequestOption);
            return this;
        }

        public new virtual OperableListEx<TListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<TListElement> guaranteedValues,
            int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public new virtual OperableListEx<TListElement> GuaranteeByFixedQuantity(
            IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, valueCountRequestOption);
            return this;
        }

        public new virtual OperableListEx<TListElement> GuaranteeByFixedQuantity(
            IEnumerable<Func<TListElement>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, valueCountRequestOption);
            return this;
        }

        public new virtual OperableListEx<TListElement> GuaranteeByFixedQuantity(
            IEnumerable<Func<TListElement>> guaranteedValues,
            int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this;
        }

        public new virtual OperableListEx<TListElement> GuaranteeByFixedQuantity(
            IEnumerable<TListElement> guaranteedValues,
            int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this;
        }

        public new virtual OperableListEx<TListElement> GuaranteeByFixedQuantity(
            IEnumerable<TListElement> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.GuaranteeByFixedQuantity(guaranteedValues, valueCountRequestOption);
            return this;
        }
    }
}
