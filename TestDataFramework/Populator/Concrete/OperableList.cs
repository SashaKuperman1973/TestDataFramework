using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using TestDataFramework.ListOperations;

namespace TestDataFramework.Populator.Concrete
{
    public abstract class OperableList
    {
        public abstract void Bind();
    }

    public class GuaranteedValues<T>
    {
        public IEnumerable<object> Values { get; set; }
        public int? FrequencyPercentage { get; set; }
        public int? TotalFrequency { get; set; }
    }

    public class OperableList<T> : OperableList, IList<RecordReference<T>>
    {
        private readonly List<GuaranteedValues<T>> guaranteedValues = new List<GuaranteedValues<T>>();

        private readonly List<RecordReference<T>> internalList;
        private readonly ValueGuaranteePopulator valueGuaranteePopulator;

        public OperableList(ValueGuaranteePopulator valueGuaranteePopulator)
        {
            this.internalList = new List<RecordReference<T>>();
            this.valueGuaranteePopulator = valueGuaranteePopulator;
        }

        public OperableList(IEnumerable<RecordReference<T>> input, ValueGuaranteePopulator valueGuaranteePopulator)
        {            
            this.internalList = new List<RecordReference<T>>(input);
            this.valueGuaranteePopulator = valueGuaranteePopulator;
        }

        public virtual OperableList<T> GuaranteeByPercentageOfTotal(IEnumerable<object> guaranteedValues, int frequencyPercentage = 10)
        {
            this.guaranteedValues.Add(new GuaranteedValues<T>
            {
                FrequencyPercentage = frequencyPercentage,
                Values = guaranteedValues,
            });

            return this;
        }

        public virtual OperableList<T> GuaranteeByPercentageOfTotal(IEnumerable<Func<T>> guaranteedValues, int frequencyPercentage = 10)
        {
            this.guaranteedValues.Add(new GuaranteedValues<T>
            {
                FrequencyPercentage = frequencyPercentage,
                Values = guaranteedValues,
            });

            return this;
        }

        public virtual OperableList<T> GuaranteeByPercentageOfTotal(IEnumerable<T> guaranteedValues, int frequencyPercentage = 10)
        {
            this.guaranteedValues.Add(new GuaranteedValues<T>
            {
                FrequencyPercentage = frequencyPercentage,
                Values = guaranteedValues.Cast<object>(),
            });

            return this;
        }

        public virtual OperableList<T> GuaranteeByFixedQuantity(IEnumerable<object> guaranteedValues, int fixedQuantity = 0)
        {
            if (fixedQuantity == 0)
            {
                fixedQuantity = guaranteedValues.Count();
            }

            this.guaranteedValues.Add(new GuaranteedValues<T>
            {
                TotalFrequency = fixedQuantity,
                Values = guaranteedValues,
            });

            return this;
        }

        public virtual OperableList<T> GuaranteeByFixedQuantity(IEnumerable<Func<T>> guaranteedValues, int fixedQuantity = 0)
        {
            if (fixedQuantity == 0)
            {
                fixedQuantity = guaranteedValues.Count();
            }

            this.guaranteedValues.Add(new GuaranteedValues<T>
            {
                TotalFrequency = fixedQuantity,
                Values = guaranteedValues,
            });

            return this;
        }

        public virtual OperableList<T> GuaranteeByFixedQuantity(IEnumerable<T> guaranteedValues, int fixedQuantity = 0)
        {
            if (fixedQuantity == 0)
            {
                fixedQuantity = guaranteedValues.Count();
            }

            this.guaranteedValues.Add(new GuaranteedValues<T>
            {
                TotalFrequency = fixedQuantity,
                Values = guaranteedValues.Cast<object>(),
            });

            return this;
        }

        public override void Bind()
        {
            if (!this.guaranteedValues.Any())
            {
                return;
            }

            this.valueGuaranteePopulator.Bind(this, this.guaranteedValues);
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

        public void Clear()
        {
            this.internalList.Clear();
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

        public int Count => this.internalList.Count;
        public bool IsReadOnly => ((IList) this.internalList).IsReadOnly;
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
