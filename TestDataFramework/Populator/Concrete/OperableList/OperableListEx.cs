using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator.Concrete.FieldExpression;
using TestDataFramework.Populator.Concrete.MakeableEnumerable;
using TestDataFramework.TypeGenerator.Interfaces;

namespace TestDataFramework.Populator.Concrete.OperableList
{
    public class OperableListEx<TListElement> : OperableList<TListElement>
    {
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

            return result;
        }

        public virtual ListParentOperableList<TListElement> Take(int count)
        {
            IEnumerable<RecordReference<TListElement>> input = this.InternalEnumerable.Take(count);

            ListParentOperableList<TListElement> result =
                this.CreateSubset(input);

            this.AddChild(result);
            return result;
        }

        public virtual ListParentOperableList<TListElement> Skip(int count)
        {
            IEnumerable<RecordReference<TListElement>> input = this.InternalEnumerable.Skip(count);

            ListParentOperableList<TListElement> result =
                this.CreateSubset(input);

            this.AddChild(result);
            return result;
        }

        private ListParentOperableList<TPropertyElement, TListElement> SetList<TPropertyElement>(int size)
        {
            List<RecordReference<TPropertyElement>> input = this.CreateRecordReferences<TPropertyElement>(size);

            ListParentOperableList<TPropertyElement, TListElement> result = this.CreateChild(input);

            this.AddChild(result);
            return result;
        }

        public virtual ListParentMakeableEnumerable<ListParentOperableList<TResultElement, TListElement>, TListElement>
            SelectListSet<TResultElement>(Expression<Func<TListElement, IEnumerable<TResultElement>>> selector,
                int listSize)
        {
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
