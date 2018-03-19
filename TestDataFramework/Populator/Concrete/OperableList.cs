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
using TestDataFramework.ListOperations;

namespace TestDataFramework.Populator.Concrete
{
    public abstract class OperableList : Populatable
    {
        public bool IsPopulated { get; protected set; }
    }

    public class GuaranteedValues
    {
        public IEnumerable<object> Values { get; set; }
        public int? FrequencyPercentage { get; set; }
        public int? TotalFrequency { get; set; }
    }

    public class OperableList<T> : OperableList, IList<RecordReference<T>>
    {
        private readonly List<GuaranteedValues> guaranteedValues = new List<GuaranteedValues>();

        private readonly List<RecordReference<T>> internalList;
        private readonly ValueGuaranteePopulator valueGuaranteePopulator;

        protected BasePopulator Populator;

        public OperableList(ValueGuaranteePopulator valueGuaranteePopulator, BasePopulator populator)
        {
            this.internalList = new List<RecordReference<T>>();
            this.valueGuaranteePopulator = valueGuaranteePopulator;
            this.Populator = populator;
        }

        public OperableList(IEnumerable<RecordReference<T>> input, ValueGuaranteePopulator valueGuaranteePopulator, BasePopulator populator)
        {            
            this.internalList = new List<RecordReference<T>>(input);
            this.valueGuaranteePopulator = valueGuaranteePopulator;
            this.Populator = populator;
        }

        public virtual OperableList<T> GuaranteeByPercentageOfTotal<TValue>(IEnumerable<TValue> guaranteedValues, int frequencyPercentage = 10)
        {
            this.guaranteedValues.Add(new GuaranteedValues
            {
                FrequencyPercentage = frequencyPercentage,
                Values = guaranteedValues.Cast<object>(),
            });

            return this;
        }

        public virtual OperableList<T> GuaranteeByPercentageOfTotal(IEnumerable<Func<T>> guaranteedValues, int frequencyPercentage = 10)
        {
            this.guaranteedValues.Add(new GuaranteedValues
            {
                FrequencyPercentage = frequencyPercentage,
                Values = guaranteedValues,
            });

            return this;
        }

        public virtual OperableList<T> GuaranteeByPercentageOfTotal(IEnumerable<T> guaranteedValues, int frequencyPercentage = 10)
        {
            this.guaranteedValues.Add(new GuaranteedValues
            {
                FrequencyPercentage = frequencyPercentage,
                Values = guaranteedValues.Cast<object>(),
            });

            return this;
        }

        public virtual OperableList<T> GuaranteeByFixedQuantity<TValue>(IEnumerable<TValue> guaranteedValues, int fixedQuantity = 0)
        {
            guaranteedValues = guaranteedValues.ToList();

            if (fixedQuantity == 0)
            {
                fixedQuantity = guaranteedValues.Count();
            }

            this.guaranteedValues.Add(new GuaranteedValues
            {
                TotalFrequency = fixedQuantity,
                Values = guaranteedValues.Cast<object>(),
            });

            return this;
        }

        public virtual OperableList<T> GuaranteeByFixedQuantity(IEnumerable<Func<T>> guaranteedValues, int fixedQuantity = 0)
        {
            guaranteedValues = guaranteedValues.ToList();

            if (fixedQuantity == 0)
            {
                fixedQuantity = guaranteedValues.Count();
            }

            this.guaranteedValues.Add(new GuaranteedValues
            {
                TotalFrequency = fixedQuantity,
                Values = guaranteedValues,
            });

            return this;
        }

        public virtual OperableList<T> GuaranteeByFixedQuantity(IEnumerable<T> guaranteedValues, int fixedQuantity = 0)
        {
            guaranteedValues = guaranteedValues.ToList();

            if (fixedQuantity == 0)
            {
                fixedQuantity = guaranteedValues.Count();
            }

            this.guaranteedValues.Add(new GuaranteedValues
            {
                TotalFrequency = fixedQuantity,
                Values = guaranteedValues.Cast<object>(),
            });

            return this;
        }

        public virtual OperableList<T> Ignore<TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression)
        {
            this.internalList.ForEach(reference => reference.Ignore(fieldExpression));
            return this;
        }

        protected internal override void Populate()
        {
            if (this.IsPopulated)
            {
                return;
            }

            if (!this.guaranteedValues.Any())
            {
                this.internalList.ForEach(recordReference => recordReference.Populate());
                return;
            }

            this.valueGuaranteePopulator.Bind(this, this.guaranteedValues);
            this.internalList.ForEach(recordReference => recordReference.Populate());
            this.IsPopulated = true;
        }

        protected internal override void AddToReferences(IList<RecordReference> collection)
        {
            this.internalList.ForEach(collection.Add);
        }

        public IEnumerable<T> BindAndMake()
        {
            this.Populator.Bind();
            IEnumerable<T> result = this.RecordObjects;
            return result;
        }

        public IEnumerable<T> Make()
        {
            this.Populator.Bind(this);
            IEnumerable<T> result = this.RecordObjects;
            return result;
        }

        public IEnumerable<T> RecordObjects
        {
            get
            {
                IEnumerable<T> result = this.internalList.Select(reference => reference.RecordObject);
                return result;
            }
        }

        #region IList<> members

        public IEnumerator<RecordReference<T>> GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Add(RecordReference<T> item)
        {
            this.internalList.Add(item);

        }

        public int Count => this.internalList.Count;

        public bool IsReadOnly => ((IList)this.internalList).IsReadOnly;

        public void Clear()
        {
            this.internalList.Clear();
            this.guaranteedValues.Clear();
        }

        public bool Contains(RecordReference<T> item)
        {
            return this.internalList.Contains(item);
        }

        public void CopyTo(RecordReference<T>[] array, int arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }

        public bool Remove(RecordReference<T> item)
        {
            return this.internalList.Remove(item);
        }

        public int IndexOf(RecordReference<T> item)
        {
            return this.internalList.IndexOf(item);
        }

        public void Insert(int index, RecordReference<T> item)
        {
            this.internalList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this.internalList.RemoveAt(index);
        }

        public RecordReference<T> this[int index]
        {
            get { return this.internalList[index]; }
            set { this.internalList[index] = value; }
        }

        #endregion IList<> members
    }
}
