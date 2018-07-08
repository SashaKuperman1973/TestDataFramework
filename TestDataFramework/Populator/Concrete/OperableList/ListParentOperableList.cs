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
    public class ListParentOperableList<TListElement> : OperableList<TListElement>, IMakeableCollectionContainer<TListElement>
    {
        public ListParentOperableList(ValueGuaranteePopulator valueGuaranteePopulator, BasePopulator populator,
            ITypeGenerator typeGenerator, IAttributeDecorator attributeDecorator,
            IObjectGraphService objectGraphService, DeepCollectionSettingConverter deepCollectionSettingConverter,
            List<RecordReference<TListElement>> internalList = null) : base(valueGuaranteePopulator, populator,
            typeGenerator, attributeDecorator, objectGraphService, deepCollectionSettingConverter, internalList)
        {
        }

        public ListParentOperableList(int size, ValueGuaranteePopulator valueGuaranteePopulator,
            BasePopulator populator, ITypeGenerator typeGenerator, IAttributeDecorator attributeDecorator,
            IObjectGraphService objectGraphService, DeepCollectionSettingConverter deepCollectionSettingConverter) :
            base(size, valueGuaranteePopulator, populator, typeGenerator, attributeDecorator, objectGraphService,
                deepCollectionSettingConverter)
        {
        }

        public ListParentOperableList(IEnumerable<RecordReference<TListElement>> input,
            ValueGuaranteePopulator valueGuaranteePopulator, BasePopulator populator) : base(input,
            valueGuaranteePopulator, populator)
        {
        }

        public new ListParentOperableList<TListElement> Take(int count)
        {
            IEnumerable<RecordReference<TListElement>> input = this.InternalList.Take(count);
            var result =
                new ListParentOperableList<TListElement>(input, this.ValueGuaranteePopulator,
                    this.Populator);
            return result;
        }

        public new ListParentOperableList<TListElement> Skip(int count)
        {
            IEnumerable<RecordReference<TListElement>> input = this.InternalList.Skip(count);
            var result =
                new ListParentOperableList<TListElement>(input, this.ValueGuaranteePopulator,
                    this.Populator);
            return result;
        }

        public virtual ListParentOperableList<TListElement> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, TProperty value)
        {
            return this.Set(fieldExpression, () => value);
        }

        public new virtual ListParentOperableList<TListElement> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, Func<TProperty> valueFactory)
        {
            base.Set(fieldExpression, valueFactory);
            return this;
        }

        public virtual ListParentFieldExpression<TListElement, TProperty> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> expression)
        {
            var fieldExpression =
                new ListParentFieldExpression<TListElement, TProperty>(expression, this, this.ObjectGraphService);
            return fieldExpression;
        }

        public virtual ListParentOperableList<TListElement> GuaranteeByPercentageOfTotal(IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual ListParentOperableList<TListElement> GuaranteeByPercentageOfTotal(IEnumerable<object> guaranteedValues,
            int frequencyPercentage, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.AddGuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public virtual ListParentOperableList<TListElement> GuaranteeByPercentageOfTotal(IEnumerable<Func<TListElement>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual ListParentOperableList<TListElement> GuaranteeByPercentageOfTotal(IEnumerable<Func<TListElement>> guaranteedValues,
            int frequencyPercentage, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.AddGuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public virtual ListParentOperableList<TListElement> GuaranteeByPercentageOfTotal(IEnumerable<TListElement> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual ListParentOperableList<TListElement> GuaranteeByPercentageOfTotal(IEnumerable<TListElement> guaranteedValues,
            int frequencyPercentage, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.AddGuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public virtual ListParentOperableList<TListElement> GuaranteeByFixedQuantity(IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual ListParentOperableList<TListElement> GuaranteeByFixedQuantity(IEnumerable<object> guaranteedValues,
            int fixedQuantity, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.AddGuaranteeByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this;
        }

        public virtual ListParentOperableList<TListElement> GuaranteeByFixedQuantity(IEnumerable<Func<TListElement>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual ListParentOperableList<TListElement> GuaranteeByFixedQuantity(IEnumerable<Func<TListElement>> guaranteedValues,
            int fixedQuantity, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.AddGuaranteeByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this;
        }

        public virtual ListParentOperableList<TListElement> GuaranteeByFixedQuantity(IEnumerable<TListElement> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            this.AddGuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
            return this;
        }

        public virtual ListParentMakeableEnumerable<ListParentOperableList<TResult>, TListElement> Select<TResult>(
            Expression<Func<TListElement, IEnumerable<TResult>>> selector, int size)
        {
            IEnumerable<ListParentOperableList<TResult>> operableListCollection =
                this.InternalList.Select(recordReference => 
                    recordReference.SetListParentList(selector, size));

            var result =
                new ListParentMakeableEnumerable<ListParentOperableList<TResult>, TListElement>(operableListCollection,
                    this);

            return result;
        }

        public new virtual OperableList<TListElement> Ignore<TPropertyType>(Expression<Func<TListElement, TPropertyType>> fieldExpression)
        {
            base.IgnoreBase(fieldExpression);
            return this;
        }
    }
}
