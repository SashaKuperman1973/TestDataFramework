/*
    Copyright 2016, 2017 Alexander Kuperman

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
    public abstract class OperableList<TListElement> : Populatable, IList<RecordReference<TListElement>>
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

        protected OperableList(ValueGuaranteePopulator valueGuaranteePopulator, BasePopulator populator,
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

        protected OperableList(int size, ValueGuaranteePopulator valueGuaranteePopulator, BasePopulator populator,
            ITypeGenerator typeGenerator, IAttributeDecorator attributeDecorator,
            IObjectGraphService objectGraphService,
            DeepCollectionSettingConverter deepCollectionSettingConverter)
            : this(valueGuaranteePopulator, populator, typeGenerator, attributeDecorator, objectGraphService,
                deepCollectionSettingConverter, new RecordReference<TListElement>[size].ToList())
        {
        }

        protected OperableList(IEnumerable<RecordReference<TListElement>> input, ValueGuaranteePopulator valueGuaranteePopulator,
            BasePopulator populator)
        {
            this.InternalList = new List<RecordReference<TListElement>>(input);
            this.Populator = populator;
            this.ValueGuaranteePopulator = valueGuaranteePopulator;
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

        protected void GuaranteeByPercentageOfTotal(IEnumerable<object> guaranteedValues,
            int frequencyPercentage, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            this.GuaranteedValues.Add(new GuaranteedValues
            {
                FrequencyPercentage = frequencyPercentage,
                Values = guaranteedValues,
                ValueCountRequestOption = valueCountRequestOption
            });
        }

        protected void GuaranteeByPercentageOfTotal(IEnumerable<Func<TListElement>> guaranteedValues,
            int frequencyPercentage, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            this.GuaranteedValues.Add(new GuaranteedValues
            {
                FrequencyPercentage = frequencyPercentage,
                Values = guaranteedValues,
                ValueCountRequestOption = valueCountRequestOption
            });
        }

        protected void GuaranteeByPercentageOfTotal(IEnumerable<TListElement> guaranteedValues,
            int frequencyPercentage, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            this.GuaranteedValues.Add(new GuaranteedValues
            {
                FrequencyPercentage = frequencyPercentage,
                Values = guaranteedValues.Cast<object>(),
                ValueCountRequestOption = valueCountRequestOption
            });
        }

        protected void GuaranteeByFixedQuantity(IEnumerable<object> guaranteedValues,
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

        protected void GuaranteeByFixedQuantity(IEnumerable<Func<TListElement>> guaranteedValues,
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

        protected void GuaranteeByFixedQuantity(IEnumerable<TListElement> guaranteedValues, int fixedQuantity,
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

        public virtual OperableList<TListElement> Ignore<TPropertyType>(Expression<Func<TListElement, TPropertyType>> fieldExpression)
        {
            this.InternalList.ForEach(reference => reference.Ignore(fieldExpression));

            return this;
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