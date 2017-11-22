﻿/*
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
using System.Collections.Generic;
using System.Linq;
using log4net;
using TestDataFramework.Logger;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.ListOperations;
using TestDataFramework.Persistence.Interfaces;
using TestDataFramework.TypeGenerator.Interfaces;
using TestDataFramework.ValueGenerator.Interfaces;

namespace TestDataFramework.Populator.Concrete
{
    public class StandardPopulator : BasePopulator
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof (StandardPopulator));

        private readonly ITypeGenerator typeGenerator;
        private readonly IPersistence persistence;
        private readonly IHandledTypeGenerator handledTypeGenerator;
        private readonly ValueGuaranteePopulator valueGuaranteePopulator;
        private readonly IObjectGraphService objectGraphService;

        public override IValueGenerator ValueGenerator { get; }
        private readonly List<RecordReference> recordReferences = new List<RecordReference>();
        private readonly List<OperableList> setOfLists = new List<OperableList>();

        public StandardPopulator(ITypeGenerator typeGenerator, IPersistence persistence,
            IAttributeDecorator attributeDecorator, IHandledTypeGenerator handledTypeGenerator, 
            IValueGenerator valueGenerator, ValueGuaranteePopulator valueGuaranteePopulator,
            IObjectGraphService objectGraphService)
            : base(attributeDecorator)
        {
            StandardPopulator.Logger.Debug("Entering constructor");

            this.typeGenerator = typeGenerator;
            this.persistence = persistence;
            this.handledTypeGenerator = handledTypeGenerator;
            this.ValueGenerator = valueGenerator;
            this.valueGuaranteePopulator = valueGuaranteePopulator;
            this.objectGraphService = objectGraphService;

            StandardPopulator.Logger.Debug("Entering constructor");
        }

        #region Public Methods

        public override void Clear()
        {
            this.recordReferences.Clear();
            this.setOfLists.Clear();
        }

        public override void Extend(Type type, HandledTypeValueGetter valueGetter)
        {
            this.handledTypeGenerator.HandledTypeValueGetterDictionary.Add(type, valueGetter);
        }

        public override OperableList<T> Add<T>(int copies, params RecordReference[] primaryRecordReferences)
        {
            StandardPopulator.Logger.Debug($"Entering Add. T: {typeof(T)}, copies: {copies}, primaryRecordReference: {primaryRecordReferences}");

            var result = new OperableList<T>(this.valueGuaranteePopulator, this);
            this.setOfLists.Add(result);

            for (int i = 0; i < copies; i++)
            {
                result.Add(this.Add<T>(primaryRecordReferences));
            }

            StandardPopulator.Logger.Debug("Exiting Add");
            return result;
        }

        public override RecordReference<T> Add<T>(params RecordReference[] primaryRecordReferences)
        {
            StandardPopulator.Logger.Debug($"Entering Add. T: {typeof(T)}, primaryRecordReference: {primaryRecordReferences}");

            var recordReference = new RecordReference<T>(this.typeGenerator, this.AttributeDecorator, this, this.objectGraphService);

            this.recordReferences.Add(recordReference);
            recordReference.AddPrimaryRecordReference(primaryRecordReferences);

            StandardPopulator.Logger.Debug("Exiting Add<T>(primaryRecordReference, propertyExpressionDictionary)");

            StandardPopulator.Logger.Debug($"Exiting Add. record object: {recordReference.RecordObject}");
            return recordReference;
        }

        public override void Bind()
        {
            StandardPopulator.Logger.Debug("Entering Bind()");

            List<OperableList> unprocessedLists = this.setOfLists.Where(list => !list.IsProcessed).ToList();

            foreach (OperableList list in unprocessedLists)
            {
                list.Bind();
                list.IsProcessed = true;
            }

            IEnumerable<RecordReference> unprocessedReferences =
                this.recordReferences.Where(reference => !reference.IsProcessed).ToList();

            foreach (RecordReference recordReference in unprocessedReferences)
            {
                if (recordReference.PreBoundObject != null)
                {
                    recordReference.RecordObject = recordReference.PreBoundObject();
                    continue;
                }

                recordReference.Populate();
            }

            this.persistence.Persist(unprocessedReferences);

            this.recordReferences.ForEach(reference => reference.IsProcessed = true);

            StandardPopulator.Logger.Debug("Exiting Bind()");
        }

        protected internal override void Bind(RecordReference recordReference)
        {
            if (recordReference.IsProcessed)
            {
                return;
            }

            StandardPopulator.Logger.Debug("Entering Bind(RecordReference)");

            if (recordReference.PreBoundObject != null)
            {
                recordReference.RecordObject = recordReference.PreBoundObject();
            }
            else
            {
                recordReference.Populate();
            }

            this.persistence.Persist(new[] {recordReference});
            recordReference.IsProcessed = true;

            StandardPopulator.Logger.Debug("Exiting Bind(RecordReference)");
        }

        protected internal override void Bind<T>(OperableList<T> operableList)
        {
            if (!operableList.IsProcessed)
            {
                operableList.Bind();
                operableList.IsProcessed = true;
            }

            operableList.ToList().ForEach(this.Bind);
        }

        #endregion Public Methods
    }
}