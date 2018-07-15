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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.ListOperations.Interfaces;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.TypeGenerator.Interfaces;

namespace TestDataFramework.Populator
{
    public class OperableList<TListElement> : Populatable, IList<RecordReference<TListElement>>
    {
        protected readonly BasePopulator Populator;
        protected readonly IAttributeDecorator AttributeDecorator;
        protected readonly DeepCollectionSettingConverter DeepCollectionSettingConverter;
        protected readonly IObjectGraphService ObjectGraphService;
        protected readonly ITypeGenerator TypeGenerator;
        protected readonly ValueGuaranteePopulator ValueGuaranteePopulator;
        protected internal readonly List<GuaranteedValues> GuaranteedValues = new List<GuaranteedValues>();
        protected internal List<RecordReference<TListElement>> InternalList;
        internal readonly List<GuaranteedValues> GuaranteedPropertySetters = new List<GuaranteedValues>();

        private readonly IValueGauranteePopulatorContextService valueSetContextService = new ValueSetContextService();

        private readonly IValueGauranteePopulatorContextService explicitPropertySetterContextService =
            new ExplicitPropertySetterContextService();

        public OperableList(ValueGuaranteePopulator valueGuaranteePopulator, BasePopulator populator,
            ITypeGenerator typeGenerator, IAttributeDecorator attributeDecorator,
            IObjectGraphService objectGraphService,
            DeepCollectionSettingConverter deepCollectionSettingConverter, 
            List<RecordReference<TListElement>> internalList = null)
        {
            this.InternalList = internalList ?? new List<RecordReference<TListElement>>();
            this.Populator = populator;
            this.TypeGenerator = typeGenerator;
            this.AttributeDecorator = attributeDecorator;
            this.ObjectGraphService = objectGraphService;
            this.DeepCollectionSettingConverter = deepCollectionSettingConverter;
            this.ValueGuaranteePopulator = valueGuaranteePopulator;

            for (int i = 0; i < this.InternalList.Count; i++)
                if (this.InternalList[i] == null)
                    this.InternalList[i] = this.CreateRecordReference();
        }

        public OperableList(int size, ValueGuaranteePopulator valueGuaranteePopulator, BasePopulator populator,
            ITypeGenerator typeGenerator, IAttributeDecorator attributeDecorator,
            IObjectGraphService objectGraphService,
            DeepCollectionSettingConverter deepCollectionSettingConverter)
            : this(valueGuaranteePopulator, populator, typeGenerator, attributeDecorator, objectGraphService,
                deepCollectionSettingConverter, new RecordReference<TListElement>[size].ToList())
        {
        }

        public OperableList(IEnumerable<RecordReference<TListElement>> input, ValueGuaranteePopulator valueGuaranteePopulator,
            BasePopulator populator)
        {
            this.InternalList = new List<RecordReference<TListElement>>(input);
            this.Populator = populator;
            this.ValueGuaranteePopulator = valueGuaranteePopulator;
        }

        public OperableList(ValueGuaranteePopulator valueGuaranteePopulator, BasePopulator populator) : 
            this(Enumerable.Empty<RecordReference<TListElement>>(), valueGuaranteePopulator, populator)
        {
        }

        protected internal override void Populate()
        {
            if (this.IsPopulated)
                return;

            if (this.GuaranteedPropertySetters.Any())
                this.ValueGuaranteePopulator.Bind(this, this.GuaranteedPropertySetters,
                    this.explicitPropertySetterContextService);

            if (this.GuaranteedValues.Any())
                this.ValueGuaranteePopulator.Bind(this, this.GuaranteedValues, this.valueSetContextService);

            this.InternalList.ForEach(recordReference => recordReference.Populate());
            this.IsPopulated = true;
        }

        internal override void AddToReferences(IList<RecordReference> collection)
        {
            this.InternalList.ForEach(collection.Add);
        }

        public virtual IEnumerable<TListElement> RecordObjects
        {
            get
            {
                IEnumerable<TListElement> result = this.InternalList.Select(reference => reference.RecordObject);
                return result;
            }
        }

        protected void Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, Func<TProperty> valueFactory)
        {
            this.InternalList.ForEach(reference => reference.Set(fieldExpression, valueFactory));
        }

        protected void AddGuaranteeByPercentageOfTotal(IEnumerable<object> guaranteedValues,
            int frequencyPercentage, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            this.GuaranteedValues.Add(new GuaranteedValues
            {
                FrequencyPercentage = frequencyPercentage,
                Values = guaranteedValues,
                ValueCountRequestOption = valueCountRequestOption
            });
        }

        protected void AddGuaranteeByPercentageOfTotal(IEnumerable<Func<TListElement>> guaranteedValues,
            int frequencyPercentage, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            this.GuaranteedValues.Add(new GuaranteedValues
            {
                FrequencyPercentage = frequencyPercentage,
                Values = guaranteedValues,
                ValueCountRequestOption = valueCountRequestOption
            });
        }

        protected void AddGuaranteeByPercentageOfTotal(IEnumerable<TListElement> guaranteedValues,
            int frequencyPercentage, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            this.GuaranteedValues.Add(new GuaranteedValues
            {
                FrequencyPercentage = frequencyPercentage,
                Values = guaranteedValues.Cast<object>(),
                ValueCountRequestOption = valueCountRequestOption
            });
        }

        protected void AddGuaranteeByFixedQuantity(IEnumerable<object> guaranteedValues,
            int fixedQuantity, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            guaranteedValues = guaranteedValues.ToList();

            if (fixedQuantity == 0)
                fixedQuantity = guaranteedValues.Count();

            this.GuaranteedValues.Add(new GuaranteedValues
            {
                TotalFrequency = fixedQuantity,
                Values = guaranteedValues,
                ValueCountRequestOption = valueCountRequestOption
            });
        }

        protected void AddGuaranteeByFixedQuantity(IEnumerable<Func<TListElement>> guaranteedValues,
            int fixedQuantity, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            guaranteedValues = guaranteedValues.ToList();

            if (fixedQuantity == 0)
                fixedQuantity = guaranteedValues.Count();

            this.GuaranteedValues.Add(new GuaranteedValues
            {
                TotalFrequency = fixedQuantity,
                Values = guaranteedValues,
                ValueCountRequestOption = valueCountRequestOption
            });
        }

        protected void AddGuaranteeByFixedQuantity(IEnumerable<TListElement> guaranteedValues, int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            guaranteedValues = guaranteedValues.ToList();

            if (fixedQuantity == 0)
                fixedQuantity = guaranteedValues.Count();

            this.GuaranteedValues.Add(new GuaranteedValues
            {
                TotalFrequency = fixedQuantity,
                Values = guaranteedValues.Cast<object>(),
                ValueCountRequestOption = valueCountRequestOption
            });
        }

        protected void AddRange<TPropertyValue>(Expression<Func<TListElement, TPropertyValue>> fieldExpression,
            Func<IEnumerable<TPropertyValue>> rangeFactory)
        {
            this.InternalList.ForEach(l => l.SetRange(fieldExpression, rangeFactory));
        }

        public void IgnoreBase<TPropertyType>(Expression<Func<TListElement, TPropertyType>> fieldExpression)
        {
            this.InternalList.ForEach(reference => reference.Ignore(fieldExpression));
        }

        private RecordReference<TListElement> CreateRecordReference()
        {
            var result = new RecordReference<TListElement>(this.TypeGenerator, this.AttributeDecorator,
                this.Populator, this.ObjectGraphService, this.ValueGuaranteePopulator,
                this.DeepCollectionSettingConverter);

            return result;
        }

        protected IEnumerable<RecordReference<TListElement>> Take(int count)
        {
            IEnumerable<RecordReference<TListElement>> result = this.InternalList.Take(count);
            return result;
        }

        protected IEnumerable<RecordReference<TListElement>> Skip(int count)
        {
            IEnumerable<RecordReference<TListElement>> result = this.InternalList.Skip(count);
            return result;
        }

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal(IEnumerable<object> guaranteedValues, int frequencyPercentage = 10)
        {
            this.AddGuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage);
            return this;
        }

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal(IEnumerable<Func<TListElement>> guaranteedValues, int frequencyPercentage = 10)
        {
            this.AddGuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage);
            return this;
        }

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal(IEnumerable<TListElement> guaranteedValues, int frequencyPercentage = 10)
        {
            this.AddGuaranteeByPercentageOfTotal(guaranteedValues, frequencyPercentage);
            return this;
        }

        public virtual OperableList<TListElement> GuaranteeByFixedQuantity(IEnumerable<object> guaranteedValues, int fixedQuantity = 0)
        {
            this.AddGuaranteeByFixedQuantity(guaranteedValues, fixedQuantity);
            return this;
        }

        public virtual OperableList<TListElement> GuaranteeByFixedQuantity(IEnumerable<Func<TListElement>> guaranteedValues, int fixedQuantity = 0)
        {
            this.AddGuaranteeByFixedQuantity(guaranteedValues, fixedQuantity);
            return this;
        }

        public virtual OperableList<TListElement> GuaranteeByFixedQuantity(IEnumerable<TListElement> guaranteedValues, int fixedQuantity = 0)
        {
            this.AddGuaranteeByFixedQuantity(guaranteedValues, fixedQuantity);
            return this;
        }

        public virtual OperableList<TListElement> Ignore<TPropertyType>(Expression<Func<TListElement, TPropertyType>> fieldExpression)
        {
            this.IgnoreBase(fieldExpression);
            return this;
        }

        public virtual IEnumerable<TListElement> BindAndMake()
        {
            this.Populator.Bind();
            IEnumerable<TListElement> result = this.RecordObjects;
            return result;
        }

        public virtual IEnumerable<TListElement> Make()
        {
            this.Populator.Bind(this);
            IEnumerable<TListElement> result = this.RecordObjects;
            return result;
        }

        public virtual OperableList<TListElement> SetRange<TPropertyValue>(
            Expression<Func<TListElement, TPropertyValue>> fieldExpression,
            Func<IEnumerable<TPropertyValue>> rangeFactory)
        {
            this.AddRange(fieldExpression, rangeFactory);
            return this;
        }

        public virtual OperableList<TListElement> SetRange<TPropertyValue>(
            Expression<Func<TListElement, TPropertyValue>> fieldExpression,
            IEnumerable<TPropertyValue> range)
        {
            return this.SetRange(fieldExpression, () => range);
        }

        #region IList<> members

        public IEnumerator<RecordReference<TListElement>> GetEnumerator()
        {
            return this.InternalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Add(RecordReference<TListElement> item)
        {
            this.InternalList.Add(item);
        }

        public int Count => this.InternalList.Count;

        public bool IsReadOnly => ((IList)this.InternalList).IsReadOnly;

        public void Clear()
        {
            this.InternalList.Clear();
            this.GuaranteedValues.Clear();
        }

        public bool Contains(RecordReference<TListElement> item)
        {
            return this.InternalList.Contains(item);
        }

        public void CopyTo(RecordReference<TListElement>[] array, int arrayIndex)
        {
            this.InternalList.CopyTo(array, arrayIndex);
        }

        public bool Remove(RecordReference<TListElement> item)
        {
            return this.InternalList.Remove(item);
        }

        public int IndexOf(RecordReference<TListElement> item)
        {
            return this.InternalList.IndexOf(item);
        }

        public void Insert(int index, RecordReference<TListElement> item)
        {
            this.InternalList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this.InternalList.RemoveAt(index);
        }

        public RecordReference<TListElement> this[int index]
        {
            get => this.InternalList[index];
            set => this.InternalList[index] = value;
        }

        #endregion IList<> members
    }
}