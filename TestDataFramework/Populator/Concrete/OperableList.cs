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
using TestDataFramework.Exceptions;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.ListOperations.Interfaces;
using TestDataFramework.TypeGenerator.Interfaces;

namespace TestDataFramework.Populator.Concrete
{
    public class OperableList<TListElement> : Populatable, IList<RecordReference<TListElement>>
    {
        internal readonly List<GuaranteedValues> GuaranteedValues = new List<GuaranteedValues>();
        internal readonly List<GuaranteedValues> GuaranteedPropertySetters = new List<GuaranteedValues>();

        protected internal List<RecordReference<TListElement>> InternalList;

        protected readonly BasePopulator Populator;
        protected readonly ValueGuaranteePopulator ValueGuaranteePopulator;
        protected readonly IAttributeDecorator AttributeDecorator;
        protected readonly DeepCollectionSettingConverter DeepCollectionSettingConverter;
        protected readonly IObjectGraphService ObjectGraphService;
        protected readonly ITypeGenerator TypeGenerator;

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
            this.ValueGuaranteePopulator = valueGuaranteePopulator;
            this.Populator = populator;
            this.TypeGenerator = typeGenerator;
            this.AttributeDecorator = attributeDecorator;
            this.ObjectGraphService = objectGraphService;
            this.DeepCollectionSettingConverter = deepCollectionSettingConverter;

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

        public OperableList(IEnumerable<RecordReference<TListElement>> internalList,
            ValueGuaranteePopulator valueGuaranteePopulator, BasePopulator populator,
            ITypeGenerator typeGenerator, IAttributeDecorator attributeDecorator,
            IObjectGraphService objectGraphService,
            DeepCollectionSettingConverter deepCollectionSettingConverter)
            : this(valueGuaranteePopulator, populator, typeGenerator, attributeDecorator, objectGraphService,
                deepCollectionSettingConverter, internalList.ToList())
        {
        }

        public OperableList(IEnumerable<RecordReference<TListElement>> input, ValueGuaranteePopulator valueGuaranteePopulator,
            BasePopulator populator)
        {
            this.InternalList = new List<RecordReference<TListElement>>(input);
            this.ValueGuaranteePopulator = valueGuaranteePopulator;
            this.Populator = populator;
        }

        public virtual IEnumerable<TListElement> RecordObjects
        {
            get
            {
                IEnumerable<TListElement> result = this.InternalList.Select(reference => reference.RecordObject);
                return result;
            }
        }

        public virtual OperableList<TListElement> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, TProperty value, params Range[] ranges)
        {
            OperableList<TListElement> result = this.Set(fieldExpression, () => value, ranges);
            return result;
        }

        public virtual OperableList<TListElement> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, Func<TProperty> valueFactory,
            params Range[] ranges)
        {
            if (!ranges.Any())
                throw new ArgumentException(Messages.NoRangeOperableListPositionsPassedIn, nameof(ranges));

            int[] positions = this.GetPositions(ranges);
            this.ValidatePositionBoundaries(positions, nameof(ranges));
            this.SetInternalList(positions, fieldExpression, valueFactory);

            return this;
        }

        public virtual FieldExpression<TListElement, TProperty> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> expression)
        {
            var fieldExpression =
                new FieldExpression<TListElement, TProperty>(expression, this, this.ObjectGraphService);
            return fieldExpression;
        }

        public virtual CollectionOfRangeOperableLists<TNewListElement> SetList<TNewListElement>(
            Expression<Func<TListElement, IEnumerable<TNewListElement>>> listFieldExpression, int newSize, params Range[] rangesOfCallingList)
        {
            int[] positions = this.GetPositions(rangesOfCallingList);
            this.ValidatePositionBoundaries(positions, nameof(rangesOfCallingList));

            var lists = new CollectionOfRangeOperableLists<TNewListElement>();

            foreach (int i in positions)
            {
                OperableList<TNewListElement> list = this.InternalList[i].SetList(listFieldExpression, newSize);
                lists.Add(list);
            }

            return lists;
        }

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal(IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal(IEnumerable<object> guaranteedValues,
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

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal(IEnumerable<Func<TListElement>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal(IEnumerable<Func<TListElement>> guaranteedValues,
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

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal(IEnumerable<TListElement> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal(IEnumerable<TListElement> guaranteedValues,
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

        public virtual OperableList<TListElement> GuaranteeByFixedQuantity(IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteeByFixedQuantity(IEnumerable<object> guaranteedValues,
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

        public virtual OperableList<TListElement> GuaranteeByFixedQuantity(IEnumerable<Func<TListElement>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteeByFixedQuantity(IEnumerable<Func<TListElement>> guaranteedValues,
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

        public virtual OperableList<TListElement> GuaranteeByFixedQuantity(IEnumerable<TListElement> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteeByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual OperableList<TListElement> GuaranteeByFixedQuantity(IEnumerable<TListElement> guaranteedValues, int fixedQuantity,
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

        public virtual OperableList<TListElement> Ignore<TPropertyType>(Expression<Func<TListElement, TPropertyType>> fieldExpression, params Range[] ranges)
        {
            int[] positions = this.GetPositions(ranges);
            this.ValidatePositionBoundaries(positions, nameof(ranges));

            foreach (int position in positions)
            {
                this.InternalList[position].Ignore(fieldExpression);
            }

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

        protected int[] GetPositions(IEnumerable<Range> ranges)
        {
            ranges = ranges.ToList();

            int[] positions;

            if (!ranges.Any())
            {
                positions = new int[this.InternalList.Count];
                for (int i = 0; i < positions.Length; i++)
                {
                    positions[i] = i;
                }
            }
            else
            {
                positions = ranges.SelectMany(r =>
                {
                    var range = new int[r.EndPosition + 1 - r.StartPosition];

                    int i = 0;
                    for (int j = r.StartPosition; j <= r.EndPosition; j++)
                        range[i++] = j;

                    return range;
                }).ToArray();
            }

            return positions;
        }

        private void SetInternalList<TProperty>(IEnumerable<int> positions,
            Expression<Func<TListElement, TProperty>> fieldExpression, Func<TProperty> valueFactory)
        {
            foreach (int position in positions)
            {
                this.InternalList[position].Set(fieldExpression, valueFactory);
            }
        }

        private RecordReference<TListElement> CreateRecordReference()
        {
            var result = new RecordReference<TListElement>(this.TypeGenerator, this.AttributeDecorator,
                this.Populator, this.ObjectGraphService, this.ValueGuaranteePopulator,
                this.DeepCollectionSettingConverter);

            return result;
        }

        protected void ValidatePositionBoundaries(IEnumerable<int> positions, string rangesParameterName)
        {
            IOrderedEnumerable<int> orderedPositions = positions.OrderBy(i => i);
            int highestPosition = orderedPositions.Last();
            int lowestPosition = orderedPositions.First();

            if (highestPosition >= this.InternalList.Count || lowestPosition < 0)
                throw new ArgumentOutOfRangeException(rangesParameterName);
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