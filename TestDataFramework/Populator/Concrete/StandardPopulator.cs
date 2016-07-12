/*
    Copyright 2016 Alexander Kuperman

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
using log4net;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.Persistence.Interfaces;
using TestDataFramework.Populator.Interfaces;
using TestDataFramework.TypeGenerator.Interfaces;
using TestDataFramework.ValueGenerator.Interfaces;

namespace TestDataFramework.Populator.Concrete
{
    public class StandardPopulator : BasePopulator, IPopulator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (StandardPopulator));

        private readonly ITypeGenerator typeGenerator;
        private readonly IPersistence persistence;
        private readonly IHandledTypeGenerator handledTypeGenerator;

        public IValueGenerator ValueGenerator { get; }

        private readonly List<RecordReference> recordReferences = new List<RecordReference>();

        public StandardPopulator(ITypeGenerator typeGenerator, IPersistence persistence,
            IAttributeDecorator attributeDecorator, IHandledTypeGenerator handledTypeGenerator, IValueGenerator valueGenerator)
            : base(attributeDecorator)
        {
            StandardPopulator.Logger.Debug("Entering constructor");

            this.typeGenerator = typeGenerator;
            this.persistence = persistence;
            this.handledTypeGenerator = handledTypeGenerator;
            this.ValueGenerator = valueGenerator;

            StandardPopulator.Logger.Debug("Entering constructor");
        }

        #region Public Methods

        public virtual void Extend(Type type, HandledTypeValueGetter valueGetter)
        {
            this.handledTypeGenerator.HandledTypeValueGetterDictionary.Add(type, valueGetter);
        }

        public virtual IList<RecordReference<T>> Add<T>(int copies, RecordReference primaryRecordReference = null) where T : new()
        {
            StandardPopulator.Logger.Debug($"Entering Add. T: {typeof(T)}, copies: {copies}, primaryRecordReference: {primaryRecordReference}");

            var result = new List<RecordReference<T>>();

            for (int i = 0; i < copies; i++)
            {
                result.Add(this.Add<T>(primaryRecordReference));
            }

            StandardPopulator.Logger.Debug("Exiting Add");
            return result;
        }

        public virtual RecordReference<T> Add<T>(RecordReference primaryRecordReference = null) where T : new()
        {
            StandardPopulator.Logger.Debug($"Entering Add. T: {typeof(T)}, primaryRecordReference: {primaryRecordReference}");

            var recordReference = new RecordReference<T>(this.typeGenerator, this.AttributeDecorator);

            this.recordReferences.Add(recordReference);

            if (primaryRecordReference != null)
            {
                recordReference.AddPrimaryRecordReference(primaryRecordReference);
            }

            StandardPopulator.Logger.Debug("Exiting Add<T>(primaryRecordReference, propertyExpressionDictionary)");

            StandardPopulator.Logger.Debug($"Exiting Add. record object: {recordReference.RecordObject}");
            return recordReference;
        }

        public virtual void Bind()
        {
            StandardPopulator.Logger.Debug("Entering Populate");

            foreach (RecordReference recordReference in this.recordReferences)
            {
                recordReference.Populate();
            }

            this.persistence.Persist(this.recordReferences);
            this.recordReferences.Clear();

            StandardPopulator.Logger.Debug("Exiting Populate");
        }

        #endregion Public Methods
    }
}