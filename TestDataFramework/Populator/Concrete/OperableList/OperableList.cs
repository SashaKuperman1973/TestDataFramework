/*
    Copyright 2016, 2017, 2018, 2019 Alexander Kuperman

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

using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Logger;
using TestDataFramework.Populator.Interfaces;
using TestDataFramework.TypeGenerator.Interfaces;

namespace TestDataFramework.Populator.Concrete.OperableList
{
    public class OperableList<TListElement> : 
        Populatable, 
        IList<RecordReference<TListElement>>, 
        IMakeableCollectionContainer<TListElement>
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(OperableList<TListElement>));

        private readonly IAttributeDecorator attributeDecorator;
        private readonly DeepCollectionSettingConverter deepCollectionSettingConverter;
        private readonly IObjectGraphService objectGraphService;
        private readonly ITypeGenerator typeGenerator;
        private readonly ValueGuaranteePopulator valueGuaranteePopulator;

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
            this.valueGuaranteePopulator = valueGuaranteePopulator;
            this.objectGraphService = objectGraphService;
            this.attributeDecorator = attributeDecorator;
            this.deepCollectionSettingConverter = deepCollectionSettingConverter;
            this.typeGenerator = typeGenerator;

            List<RecordReference<TListElement>> list = input?.ToList() ?? new List<RecordReference<TListElement>>();

            for (int i = 0; i < list.Count; i++)
                if (list[i] == null)
                    list[i] = this.CreateRecordReference<TListElement>();

            this.InternalEnumerable = list;
        }

        internal override void Populate()
        {
            OperableList<TListElement>.Logger.Entering(nameof(this.Populate));

            this.PopulateChildren();

            if (this.guaranteedPropertySetters.Any())
                this.valueGuaranteePopulator.Bind(this, this.guaranteedPropertySetters,
                    ExplicitPropertySetterContextService.Instance);

            if (this.privateGuaranteedValues.Any())
                this.valueGuaranteePopulator.Bind(this, this.privateGuaranteedValues, ValueSetContextService.Instance);

            if (this.IsPopulated || this.IsShallowCopy)
                return;

            this.InternalList.ForEach(recordReference => recordReference.Populate());
            this.IsPopulated = true;

            OperableList<TListElement>.Logger.Exiting(nameof(this.Populate));
        }

        internal override IEnumerable<Populatable> SelectMany()
        {
            IEnumerable<Populatable> baseSelectMany = base.SelectMany();
            IEnumerable<Populatable> manyResult = this.InternalEnumerable.SelectMany(item => item.SelectMany());
            IEnumerable<Populatable> result = baseSelectMany.Concat(manyResult);
            return result;
        }

        internal override void AddToReferences(IList<RecordReference> collection)
        {
            OperableList<TListElement>.Logger.Entering(nameof(this.AddToReferences));
            this.InternalList.ForEach(collection.Add);
            OperableList<TListElement>.Logger.Exiting(nameof(this.AddToReferences));
        }

        private RecordReference<TCustomListElement> CreateRecordReference<TCustomListElement>()
        {
            var result = new RecordReference<TCustomListElement>(this.typeGenerator, this.attributeDecorator,
                this.Populator, this.objectGraphService, this.valueGuaranteePopulator,
                this.deepCollectionSettingConverter);

            return result;
        }

        protected internal virtual void AddGuaranteedPropertySetter(GuaranteedValues values)
        {
            OperableList<TListElement>.Logger.Entering(nameof(this.AddGuaranteedPropertySetter));
            this.guaranteedPropertySetters.Add(values);
            OperableList<TListElement>.Logger.Exiting(nameof(this.AddGuaranteedPropertySetter));
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

        public virtual void SetRange<TPropertyValue>(Expression<Func<TListElement, TPropertyValue>> fieldExpression,
            IEnumerable<TPropertyValue> range)
        {
            OperableList<TListElement>.Logger.Entering(nameof(this.SetRange));

            List<TPropertyValue> rangeList = range.ToList();
            OperableList<TListElement>.Logger.Debug($"Value count: {rangeList.Count}. Selector: {fieldExpression}");

            this.InternalList.ForEach(l => l.SetRange(fieldExpression, rangeList));
        }

        public virtual void SetRange<TPropertyValue>(Expression<Func<TListElement, TPropertyValue>> fieldExpression,
            Func<IEnumerable<TPropertyValue>> rangeFactory)
        {
            OperableList<TListElement>.Logger.Entering(nameof(this.SetRange), "Taking a range factory.");

            this.InternalList.ForEach(l => l.SetRange(fieldExpression, rangeFactory));
        }

        public virtual OperableList<TListElement> Ignore<TPropertyType>(Expression<Func<TListElement, TPropertyType>> fieldExpression)
        {
            OperableList<TListElement>.Logger.Entering(nameof(this.Ignore), this.GetType().FullName);
            this.InternalList.ForEach(reference => reference.Ignore(fieldExpression));
            return this;
        }

        public virtual IEnumerable<TListElement> Make()
        {
            OperableList<TListElement>.Logger.Debug("Calling Make");
            this.Populate();
            return this.RecordObjects;
        }

        public virtual IEnumerable<TListElement> BindAndMake()
        {
            OperableList<TListElement>.Logger.Debug("Calling BindAndMake");
            this.Populator.Bind();
            return this.RecordObjects;
        }

        public virtual OperableList<TListElement> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, TProperty value)
        {
            OperableList<TListElement>.Logger.Calling(nameof(this.Set), $"{this.GetType().FullName} - Selector: {fieldExpression} - Value: {value}");
            this.InternalList.ForEach(reference => reference.Set(fieldExpression, value));
            return this;
        }

        public virtual OperableList<TListElement> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, Func<TProperty> valueFactory)
        {
            OperableList<TListElement>.Logger.Calling(nameof(this.Set), $"{this.GetType().FullName} - Selector: {fieldExpression} - With value factory.");
            this.InternalList.ForEach(reference => reference.Set(fieldExpression, valueFactory));
            return this;
        }

        public virtual OperableList<TListElement> GuaranteeByPercentageOfTotal(IEnumerable<object> guaranteedValues,
            int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            OperableList<TListElement>.Logger.Calling(nameof(this.GuaranteeByPercentageOfTotal), this.GetType().FullName);
            
            guaranteedValues = guaranteedValues.ToList();

            OperableList<TListElement>.Logger.Debug(
                $"Value count: {guaranteedValues.Count()} - Frequency percentage: {frequencyPercentage}");
            
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
            OperableList<TListElement>.Logger.Calling(nameof(this.GuaranteeByFixedQuantity), this.GetType().FullName);

            guaranteedValues = guaranteedValues.ToList();

            OperableList<TListElement>.Logger.Debug(
                $"Value count: {guaranteedValues.Count()} - Fixed total quantity: {fixedQuantity}");

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

        protected internal void AddItem(RecordReference<TListElement> item)
        {
            OperableList<TListElement>.Logger.Calling(nameof(this.AddItem), this.GetType().FullName);
            this.InternalList.Add(item);
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
            throw new NotSupportedException(Messages.OperableListIsReadOnly);
        }

        public int Count => this.InternalList.Count;

        public bool IsReadOnly => true;

        public void Clear()
        {
            throw new NotSupportedException(Messages.OperableListIsReadOnly);
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
            throw new NotSupportedException(Messages.OperableListIsReadOnly);
        }

        public int IndexOf(RecordReference<TListElement> item)
        {
            return this.InternalList.IndexOf(item);
        }

        public void Insert(int index, RecordReference<TListElement> item)
        {
            throw new NotSupportedException(Messages.OperableListIsReadOnly);
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException(Messages.OperableListIsReadOnly);
        }

        public RecordReference<TListElement> this[int index]
        {
            get => this.InternalList[index];
            set => throw new NotSupportedException(Messages.OperableListIsReadOnly);
        }

        #endregion IList<> members
    }
}