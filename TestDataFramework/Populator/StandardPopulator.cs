using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Castle.Windsor.Diagnostics.Inspectors;
using log4net;
using TestDataFramework.Exceptions;
using TestDataFramework.Persistence;
using TestDataFramework.TypeGenerator;
using TestDataFramework.ValueGenerator;

namespace TestDataFramework.Populator
{
    public class SetExpression<T>
    {
        public Expression<Func<T, object>> FieldExpression { get; set; }
        public object Value { get; set; }
    }

    public class StandardPopulator : IPopulator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (StandardPopulator));

        private readonly ITypeGenerator typeGenerator;
        private readonly IPersistence persistence;

        private readonly List<RecordReference> recordReferences = new List<RecordReference>();

        public StandardPopulator(ITypeGenerator typeGenerator, IPersistence persistence)
        {
            StandardPopulator.Logger.Debug("Entering constructor");

            this.typeGenerator = typeGenerator;
            this.persistence = persistence;

            StandardPopulator.Logger.Debug("Entering constructor");
        }

        #region Public Methods

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

            var recordReference = new RecordReference<T>(this.typeGenerator);

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