﻿/*
    Copyright 2016, 2017, 2018, 2019, 2023 Alexander Kuperman

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
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Logger;
using TestDataFramework.Persistence.Interfaces;
using TestDataFramework.Populator.Concrete.OperableList;
using TestDataFramework.Populator.Interfaces;
using TestDataFramework.TypeGenerator.Interfaces;
using TestDataFramework.ValueGenerator.Interfaces;

namespace TestDataFramework.Populator.Concrete
{
    public class StandardPopulator : BasePopulator, IPopulator
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(StandardPopulator));
        private readonly DeepCollectionSettingConverter deepCollectionSettingConverter;
        private readonly IHandledTypeGenerator handledTypeGenerator;
        private readonly IObjectGraphService objectGraphService;
        private readonly IPersistence persistence;

        private readonly ITypeGenerator typeGenerator;
        private readonly ValueGuaranteePopulator valueGuaranteePopulator;

        private readonly List<RecordReference> persisted = new List<RecordReference>();
        private bool isDeleted = false;

        public StandardPopulator(ITypeGenerator typeGenerator, IPersistence persistence,
            IAttributeDecorator attributeDecorator, IHandledTypeGenerator handledTypeGenerator,
            IValueGenerator valueGenerator, ValueGuaranteePopulator valueGuaranteePopulator,
            IObjectGraphService objectGraphService, DeepCollectionSettingConverter deepCollectionSettingConverter)
            : base(attributeDecorator)
        {
            StandardPopulator.Logger.Debug("Entering constructor");

            this.typeGenerator = typeGenerator;
            this.persistence = persistence;
            this.handledTypeGenerator = handledTypeGenerator;
            this.ValueGenerator = valueGenerator;
            this.valueGuaranteePopulator = valueGuaranteePopulator;
            this.objectGraphService = objectGraphService;
            this.deepCollectionSettingConverter = deepCollectionSettingConverter;

            StandardPopulator.Logger.Debug("Entering constructor");
        }

        public override IValueGenerator ValueGenerator { get; }

        public override void Clear()
        {
            this.Populatables.Clear();
        }

        public void DeleteAll()
        {
            if (this.isDeleted)
            {
                return;
            }

            var reversePersisted = new List<RecordReference>(this.persisted);
            reversePersisted.Reverse();

            this.persistence.DeleteAll(reversePersisted);

            this.isDeleted = true;
        }

        public override void Extend(Type type, HandledTypeValueGetter valueGetter)
        {
            if (this.handledTypeGenerator.HandledTypeValueGetterDictionary.ContainsKey(type))
            {
                this.handledTypeGenerator.HandledTypeValueGetterDictionary.Remove(type);
            }

            this.handledTypeGenerator.HandledTypeValueGetterDictionary.Add(type, (forType, dummyContextParameter) => valueGetter(forType));
        }

        public override OperableListEx<T> Add<T>(int copies, params RecordReference[] primaryRecordReferences)
        {
            StandardPopulator.Logger.Debug(
                $"Entering Add. T: {typeof(T)}, copies: {copies}, primaryRecordReference: {primaryRecordReferences?.GetType()}");

            var result = new OperableListEx<T>(
                Enumerable.Empty<RecordReference<T>>(),
                this.valueGuaranteePopulator,
                this,
                this.objectGraphService,
                this.AttributeDecorator,
                this.deepCollectionSettingConverter,
                this.typeGenerator,
                isShallowCopy: false
                );

            this.Populatables.Add(result);

            for (int i = 0; i < copies; i++)
                result.AddItem(this.Get<T>(primaryRecordReferences));

            StandardPopulator.Logger.Debug("Exiting Add");
            return result;
        }

        public override RecordReference<T> Add<T>(params RecordReference[] primaryRecordReferences)
        {
            StandardPopulator.Logger.Debug(
                $"Entering Add. T: {typeof(T)}, primaryRecordReference: {primaryRecordReferences?.GetType()}");

            RecordReference<T> recordReference = this.Get<T>(primaryRecordReferences);
            this.Populatables.Add(recordReference);

            StandardPopulator.Logger.Debug("Exiting Add<T>(primaryRecordReference, propertyExpressionDictionary)");

            StandardPopulator.Logger.Debug($"Exiting Add. record object: {recordReference.RecordObject?.GetType()}");
            return recordReference;
        }

        private RecordReference<T> Get<T>(params RecordReference[] primaryRecordReferences)
        {
            StandardPopulator.Logger.Debug(
                $"Entering Get. T: {typeof(T)}, primaryRecordReference: {primaryRecordReferences?.GetType()}");

            var recordReference = new RecordReference<T>(this.typeGenerator, this.AttributeDecorator, this,
                this.objectGraphService, this.valueGuaranteePopulator, this.deepCollectionSettingConverter);

            recordReference.AddPrimaryRecordReference(primaryRecordReferences);

            StandardPopulator.Logger.Debug("Exiting Get<T>(primaryRecordReference, propertyExpressionDictionary)");

            StandardPopulator.Logger.Debug($"Exiting Get. record object: {recordReference.RecordObject?.GetType()}");
            return recordReference;
        }

        public override void Bind()
        {
            StandardPopulator.Logger.Debug("Entering Bind()");

            this.Populatables.ForEach(populatable => populatable.Populate());
            this.Persist();

            StandardPopulator.Logger.Debug("Exiting Bind()");
        }

        internal override void Bind(RecordReference recordReference)
        {
            if (recordReference.IsPopulated)
            {
                StandardPopulator.Logger.Debug("RecordReference is already processed. Exiting.");
                return;
            }

            recordReference.Populate();

            this.persistence.Persist(new[] {recordReference});
            recordReference.IsPopulated = true;
        }

        private void Persist()
        {
            var recordReferences = new List<RecordReference>();
            this.Populatables.ForEach(populatable =>
            {
                if (!populatable.IsPersisted)
                {
                    populatable.AddToReferences(recordReferences);
                    populatable.SelectMany().ToList().ForEach(selfAndDescendants => selfAndDescendants.IsPersisted = true);
                }
            });
            this.persistence.Persist(recordReferences);
            this.persisted.AddRange(recordReferences);
        }
    }
}