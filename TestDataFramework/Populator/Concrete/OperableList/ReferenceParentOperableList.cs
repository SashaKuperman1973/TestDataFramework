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
    public class ReferenceParentOperableList<TListElement, TParent> : OperableList<TListElement>, IMakeable<TParent>
    {
        private readonly RecordReference<TParent> parent;

        public ReferenceParentOperableList(RecordReference<TParent> parent, ValueGuaranteePopulator valueGuaranteePopulator, BasePopulator populator, ITypeGenerator typeGenerator, IAttributeDecorator attributeDecorator, IObjectGraphService objectGraphService, DeepCollectionSettingConverter deepCollectionSettingConverter, List<RecordReference<TListElement>> internalList = null) : base(valueGuaranteePopulator, populator, typeGenerator, attributeDecorator, objectGraphService, deepCollectionSettingConverter, internalList)
        {
            this.parent = parent;
        }

        public ReferenceParentOperableList(RecordReference<TParent> parent, int size, ValueGuaranteePopulator valueGuaranteePopulator, BasePopulator populator, ITypeGenerator typeGenerator, IAttributeDecorator attributeDecorator, IObjectGraphService objectGraphService, DeepCollectionSettingConverter deepCollectionSettingConverter) : base(size, valueGuaranteePopulator, populator, typeGenerator, attributeDecorator, objectGraphService, deepCollectionSettingConverter)
        {
            this.parent = parent;
        }

        public ReferenceParentOperableList(RecordReference<TParent> parent, IEnumerable<RecordReference<TListElement>> input, ValueGuaranteePopulator valueGuaranteePopulator, BasePopulator populator) : base(input, valueGuaranteePopulator, populator)
        {
            this.parent = parent;
        }

        public new virtual TParent Make()
        {
            TParent result = this.parent.Make();
            return result;
        }

        public new virtual TParent BindAndMake()
        {
            TParent result = this.parent.BindAndMake();
            return result;
        }

        public new virtual ReferenceParentOperableList<TListElement, TParent> Take(int count)
        {
            IEnumerable<RecordReference<TListElement>> input = base.Take(count);
            var result =
                new ReferenceParentOperableList<TListElement, TParent>(this.parent, input, this.ValueGuaranteePopulator,
                    this.Populator);
            return result;
        }

        public new virtual ReferenceParentOperableList<TListElement, TParent> Skip(int count)
        {
            IEnumerable<RecordReference<TListElement>> input = base.Skip(count);
            var result =
                new ReferenceParentOperableList<TListElement, TParent>(this.parent, input, this.ValueGuaranteePopulator,
                    this.Populator);
            return result;
        }

        public virtual ReferenceParentOperableList<TListElement, TParent> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, TProperty value)
        {
            return this.Set(fieldExpression, () => value);
        }

        public new virtual ReferenceParentOperableList<TListElement, TParent> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, Func<TProperty> valueFactory)
        {
            base.Set(fieldExpression, valueFactory);
            return this;
        }

        public virtual ReferenceParentFieldExpression<TListElement, TProperty, TParent> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> expression)
        {
            var fieldExpression =
                new ReferenceParentFieldExpression<TListElement, TProperty, TParent>(expression, this, this.ObjectGraphService);
            return fieldExpression;
        }

        public virtual ReferenceParentOperableList<TListElement, TParent> GuaranteeByPercentageOfTotal(IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual ReferenceParentOperableList<TListElement, TParent> GuaranteeByPercentageOfTotal(IEnumerable<object> guaranteedValues,
            int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.AddGuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public virtual ReferenceParentOperableList<TListElement, TParent> GuaranteeByPercentageOfTotal(IEnumerable<Func<TListElement>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual ReferenceParentOperableList<TListElement, TParent> GuaranteeByPercentageOfTotal(IEnumerable<Func<TListElement>> guaranteedValues,
            int frequencyPercentage, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.AddGuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public virtual ReferenceParentOperableList<TListElement, TParent> GuaranteeByPercentageOfTotal(IEnumerable<TListElement> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual ReferenceParentOperableList<TListElement, TParent> GuaranteeByPercentageOfTotal(IEnumerable<TListElement> guaranteedValues,
            int frequencyPercentage, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.AddGuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public virtual ReferenceParentOperableList<TListElement, TParent> GuaranteeByFixedQuantity(IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual ReferenceParentOperableList<TListElement, TParent> GuaranteeByFixedQuantity(IEnumerable<object> guaranteedValues,
            int fixedQuantity, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.AddGuaranteeByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this;
        }

        public virtual ReferenceParentOperableList<TListElement, TParent> GuaranteeByFixedQuantity(IEnumerable<Func<TListElement>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual ReferenceParentOperableList<TListElement, TParent> GuaranteeByFixedQuantity(IEnumerable<Func<TListElement>> guaranteedValues,
            int fixedQuantity, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            base.AddGuaranteeByFixedQuantity(guaranteedValues, fixedQuantity, valueCountRequestOption);
            return this;
        }

        public virtual ReferenceParentOperableList<TListElement, TParent> GuaranteeByFixedQuantity(IEnumerable<TListElement> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            this.AddGuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
            return this;
        }

        public virtual ReferenceParentMakeableEnumerable<ReferenceParentOperableList<TResult, TParent>, TParent> Select<TResult>(
            Expression<Func<TListElement, IEnumerable<TResult>>> selector, int size)
        {
            IEnumerable<ReferenceParentOperableList<TResult, TParent>> operableListCollection =
                this.InternalList.Select(recordReference => recordReference.SetReferenceParentList(selector, size, this.parent));

            var result =
                new ReferenceParentMakeableEnumerable<ReferenceParentOperableList<TResult, TParent>, TParent>(
                    operableListCollection, this.parent);

            return result;
        }

        public new virtual OperableList<TListElement> Ignore<TPropertyType>(Expression<Func<TListElement, TPropertyType>> fieldExpression)
        {
            base.IgnoreBase(fieldExpression);
            return this;
        }
    }
}
