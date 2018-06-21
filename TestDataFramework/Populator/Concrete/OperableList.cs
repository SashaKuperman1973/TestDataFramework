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
using TestDataFramework.DeepSetting;
using TestDataFramework.ListOperations.Concrete;

namespace TestDataFramework.Populator.Concrete
{
    public class OperableList<T> : Populatable, IList<RecordReference<T>>
    {
        internal readonly List<GuaranteedValues> GuaranteedValues = new List<GuaranteedValues>();
        internal readonly List<GuaranteedValues> GuaranteedPropertySetters = new List<GuaranteedValues>();

        protected readonly BasePopulator Populator;
        protected readonly ValueGuaranteePopulator ValueGuaranteePopulator;
        protected internal List<RecordReference<T>> InternalList;

        private readonly ExplicitPropertySetterContextService explicitPropertySetterContextService =
            new ExplicitPropertySetterContextService();

        private readonly ValueSetContextService valueSetContextService = new ValueSetContextService();

        public OperableList(ValueGuaranteePopulator valueGuaranteePopulator, BasePopulator populator)
        {
            this.InternalList = new List<RecordReference<T>>();
            this.ValueGuaranteePopulator = valueGuaranteePopulator;
            this.Populator = populator;
        }

        public OperableList(IEnumerable<RecordReference<T>> input, ValueGuaranteePopulator valueGuaranteePopulator,
            BasePopulator populator)
        {
            this.InternalList = new List<RecordReference<T>>(input);
            this.ValueGuaranteePopulator = valueGuaranteePopulator;
            this.Populator = populator;
        }

        public virtual IEnumerable<T> RecordObjects
        {
            get
            {
                IEnumerable<T> result = this.InternalList.Select(reference => reference.RecordObject);
                return result;
            }
        }

        public virtual OperableList<T> GuaranteeByPercentageOfTotal(IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual OperableList<T> GuaranteeByPercentageOfTotal(IEnumerable<object> guaranteedValues,
            int frequencyPercentage, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            this.GuaranteedValues.Add(new GuaranteedValues
            {
                FrequencyPercentage = frequencyPercentage,
                Values = guaranteedValues,
                ValueCountRequestOption = valueCountRequestOption
            });

            return this;
        }

        public virtual OperableList<T> GuaranteeByPercentageOfTotal(IEnumerable<Func<T>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual OperableList<T> GuaranteeByPercentageOfTotal(IEnumerable<Func<T>> guaranteedValues,
            int frequencyPercentage, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            this.GuaranteedValues.Add(new GuaranteedValues
            {
                FrequencyPercentage = frequencyPercentage,
                Values = guaranteedValues,
                ValueCountRequestOption = valueCountRequestOption
            });

            return this;
        }

        public virtual OperableList<T> GuaranteeByPercentageOfTotal(IEnumerable<T> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual OperableList<T> GuaranteeByPercentageOfTotal(IEnumerable<T> guaranteedValues,
            int frequencyPercentage, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            this.GuaranteedValues.Add(new GuaranteedValues
            {
                FrequencyPercentage = frequencyPercentage,
                Values = guaranteedValues.Cast<object>(),
                ValueCountRequestOption = valueCountRequestOption
            });

            return this;
        }

        public virtual OperableList<T> GuaranteeByFixedQuantity(IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual OperableList<T> GuaranteeByFixedQuantity(IEnumerable<object> guaranteedValues,
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

            return this;
        }

        public virtual OperableList<T> GuaranteeByFixedQuantity(IEnumerable<Func<T>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual OperableList<T> GuaranteeByFixedQuantity(IEnumerable<Func<T>> guaranteedValues,
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

            return this;
        }

        public virtual OperableList<T> GuaranteeByFixedQuantity(IEnumerable<T> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual OperableList<T> GuaranteeByFixedQuantity(IEnumerable<T> guaranteedValues, int fixedQuantity,
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

            return this;
        }

        public virtual OperableList<T> Ignore<TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression)
        {
            this.InternalList.ForEach(reference => reference.Ignore(fieldExpression));
            return this;
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

        public virtual IEnumerable<T> BindAndMake()
        {
            this.Populator.Bind();
            IEnumerable<T> result = this.RecordObjects;
            return result;
        }

        public virtual IEnumerable<T> Make()
        {
            this.Populator.Bind(this);
            IEnumerable<T> result = this.RecordObjects;
            return result;
        }

        #region IList<> members

        public IEnumerator<RecordReference<T>> GetEnumerator()
        {
            return this.InternalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Add(RecordReference<T> item)
        {
            this.InternalList.Add(item);
        }

        public int Count => this.InternalList.Count;

        public bool IsReadOnly => ((IList) this.InternalList).IsReadOnly;

        public void Clear()
        {
            this.InternalList.Clear();
            this.GuaranteedValues.Clear();
        }

        public bool Contains(RecordReference<T> item)
        {
            return this.InternalList.Contains(item);
        }

        public void CopyTo(RecordReference<T>[] array, int arrayIndex)
        {
            this.InternalList.CopyTo(array, arrayIndex);
        }

        public bool Remove(RecordReference<T> item)
        {
            return this.InternalList.Remove(item);
        }

        public int IndexOf(RecordReference<T> item)
        {
            return this.InternalList.IndexOf(item);
        }

        public void Insert(int index, RecordReference<T> item)
        {
            this.InternalList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this.InternalList.RemoveAt(index);
        }

        public RecordReference<T> this[int index]
        {
            get => this.InternalList[index];
            set => this.InternalList[index] = value;
        }

        #endregion IList<> members
    }
}