﻿/*
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
using TestDataFramework.Populator.Interfaces;
using TestDataFramework.TypeGenerator.Interfaces;

namespace TestDataFramework.Populator
{
    public class OperableList<TListElement> : 
        Populatable, 
        IList<RecordReference<TListElement>>, 
        IMakeableCollectionContainer<TListElement>
    {
        protected readonly IAttributeDecorator AttributeDecorator;
        protected readonly DeepCollectionSettingConverter DeepCollectionSettingConverter;
        protected readonly IObjectGraphService ObjectGraphService;
        protected readonly ITypeGenerator TypeGenerator;
        protected readonly ValueGuaranteePopulator ValueGuaranteePopulator;

        private readonly IValueGauranteePopulatorContextService explicitPropertySetterContextService =
            new ExplicitPropertySetterContextService();        

        protected readonly BasePopulator Populator;

        private readonly List<GuaranteedValues> guaranteedPropertySetters = new List<GuaranteedValues>();
        private readonly List<GuaranteedValues> privateGuaranteedValues = new List<GuaranteedValues>();

        protected bool IsShallowCopy { get; } = false;

        protected IEnumerable<RecordReference<TListElement>> InternalEnumerable { get; }

        private List<RecordReference<TListElement>> InternalList
        {
            get
            {
                var list = (List<RecordReference<TListElement>>) this.InternalEnumerable;
                return list;
            }
        }

        public OperableList(IEnumerable<RecordReference<TListElement>> input,
            ValueGuaranteePopulator valueGuaranteePopulator, BasePopulator populator,
            IObjectGraphService objectGraphService, IAttributeDecorator attributeDecorator,
            DeepCollectionSettingConverter deepCollectionSettingConverter,
            ITypeGenerator typeGenerator,
            bool isShalowCopy)
        {
            this.IsShallowCopy = isShalowCopy;
            this.Populator = populator;
            this.ValueGuaranteePopulator = valueGuaranteePopulator;
            this.ObjectGraphService = objectGraphService;
            this.AttributeDecorator = attributeDecorator;
            this.DeepCollectionSettingConverter = deepCollectionSettingConverter;
            this.TypeGenerator = typeGenerator;

            List<RecordReference<TListElement>> list = input?.ToList() ?? new List<RecordReference<TListElement>>();

            for (int i = 0; i < list.Count; i++)
                if (list[i] == null)
                    list[i] = this.CreateRecordReference<TListElement>();

            this.InternalEnumerable = list;
        }

        internal override void Populate()
        {
            this.PopulateChildren();

            if (this.guaranteedPropertySetters.Any())
                this.ValueGuaranteePopulator.Bind(this, this.guaranteedPropertySetters,
                    this.explicitPropertySetterContextService);

            if (this.privateGuaranteedValues.Any())
                this.ValueGuaranteePopulator.Bind(this, this.privateGuaranteedValues, new ValueSetContextService());

            if (this.IsPopulated || this.IsShallowCopy)
                return;

            this.InternalList.ForEach(recordReference => recordReference.Populate());
            this.IsPopulated = true;
        }

        internal override void AddToReferences(IList<RecordReference> collection)
        {
            this.InternalList.ForEach(collection.Add);
        }

        private RecordReference<TCustomListElement> CreateRecordReference<TCustomListElement>()
        {
            var result = new RecordReference<TCustomListElement>(this.TypeGenerator, this.AttributeDecorator,
                this.Populator, this.ObjectGraphService, this.ValueGuaranteePopulator,
                this.DeepCollectionSettingConverter);

            return result;
        }

        protected internal virtual void AddGuaranteedPropertySetter(GuaranteedValues values)
        {
            this.guaranteedPropertySetters.Add(values);
        }

        protected List<RecordReference<TCustomListElement>> CreateRecordReferences<TCustomListElement>(int count)
        {
            var list = new List<RecordReference<TCustomListElement>>(count);
            for (int i = 0; i < count; i++)
                list.Add(this.CreateRecordReference<TCustomListElement>());

            return list;
        }

        public virtual IEnumerable<TListElement> RecordObjects
        {
            get
            {
                IEnumerable<TListElement> result = this.InternalList.Select(reference => reference.RecordObject);
                return result;
            }
        }

        public virtual void AddRange<TPropertyValue>(Expression<Func<TListElement, TPropertyValue>> fieldExpression,
            IEnumerable<TPropertyValue> range)
        {
            this.AddRange(fieldExpression, () => range);
        }

        public virtual void AddRange<TPropertyValue>(Expression<Func<TListElement, TPropertyValue>> fieldExpression,
            Func<IEnumerable<TPropertyValue>> rangeFactory)
        {
            this.InternalList.ForEach(l => l.SetRange(fieldExpression, rangeFactory));
        }

        public virtual OperableList<TListElement> Ignore<TPropertyType>(Expression<Func<TListElement, TPropertyType>> fieldExpression)
        {
            this.InternalList.ForEach(reference => reference.Ignore(fieldExpression));
            return this;
        }

        public virtual IEnumerable<TListElement> Make()
        {
            this.Populate();
            return this.RecordObjects;
        }

        public virtual IEnumerable<TListElement> BindAndMake()
        {
            this.Populator.Bind();
            return this.RecordObjects;
        }

        public virtual OperableList<TListElement> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, TProperty value)
        {
            return this.Set(fieldExpression, () => value);
        }

        public virtual OperableList<TListElement> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, Func<TProperty> valueFactory)
        {
            this.InternalList.ForEach(reference => reference.Set(fieldExpression, valueFactory));
            return this;
        }

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal(IEnumerable<object> guaranteedValues,
            int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            this.privateGuaranteedValues.Add(new GuaranteedValues
            {
                FrequencyPercentage = frequencyPercentage,
                Values = guaranteedValues,
                ValueCountRequestOption = valueCountRequestOption
            });

            return this;
        }

        public virtual OperableList<TListElement> GuaranteeByFixedQuantity(IEnumerable<object> guaranteedValues,
            int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            guaranteedValues = guaranteedValues.ToList();

            if (fixedQuantity == 0)
                fixedQuantity = guaranteedValues.Count();

            this.privateGuaranteedValues.Add(new GuaranteedValues
            {
                TotalFrequency = fixedQuantity,
                Values = guaranteedValues,
                ValueCountRequestOption = valueCountRequestOption
            });

            return this;
        }

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<Func<TListElement>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByPercentageOfTotal(guaranteedValues.Cast<object>(), 10, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<Func<TListElement>> guaranteedValues,
            int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            this.GuaranteeByPercentageOfTotal(guaranteedValues.Cast<object>(), frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<TListElement> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByPercentageOfTotal(guaranteedValues.Cast<object>(), 10, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal(
            IEnumerable<TListElement> guaranteedValues,
            int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            this.GuaranteeByPercentageOfTotal(guaranteedValues.Cast<object>(), frequencyPercentage, valueCountRequestOption);
            return this;
        }

        public virtual OperableList<TListElement> GuaranteeByFixedQuantity(
            IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteeByFixedQuantity(
            IEnumerable<Func<TListElement>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByFixedQuantity(guaranteedValues.Cast<object>(), 0, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteeByFixedQuantity(
            IEnumerable<Func<TListElement>> guaranteedValues,
            int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            this.GuaranteeByFixedQuantity(guaranteedValues.Cast<object>(), fixedQuantity, valueCountRequestOption);
            return this;
        }

        public virtual OperableList<TListElement> GuaranteeByFixedQuantity(
            IEnumerable<TListElement> guaranteedValues,
            int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            this.GuaranteeByFixedQuantity(guaranteedValues.Cast<object>(), fixedQuantity, valueCountRequestOption);
            return this;
        }

        public virtual OperableList<TListElement> GuaranteeByFixedQuantity(
            IEnumerable<TListElement> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            this.GuaranteeByFixedQuantity(guaranteedValues.Cast<object>(), 0, valueCountRequestOption);
            return this;
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

        public bool IsReadOnly => ((IList) this.InternalList).IsReadOnly;

        public void Clear()
        {
            this.InternalList.Clear();
            this.privateGuaranteedValues.Clear();
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