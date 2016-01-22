using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.Exceptions;
using TestDataFramework.Persistence;
using TestDataFramework.TypeGenerator;
using TestDataFramework.ValueGenerator;

namespace TestDataFramework.Populator
{
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
            StandardPopulator.Logger.Debug("Entering Add<T>(int)");

            StandardPopulator.Logger.DebugFormat("Adding {0} types of {1} ", copies, typeof(T));

            var result = new List<RecordReference<T>>(copies);

            for (int i = 0; i < copies; i++)
            {
                result.Add(this.Add<T>(primaryRecordReference));
            }

            StandardPopulator.Logger.Debug("Exiting Add<T>(int)");

            return result;
        }

        public virtual RecordReference<T> Add<T>(RecordReference primaryRecordReference = null) where T : new()
        {
            StandardPopulator.Logger.Debug("Entering Add<T>");

            StandardPopulator.Logger.Debug("Adding type " + typeof (T));

            this.typeGenerator.ResetRecursionGuard();
            object recordObject = this.typeGenerator.GetObject(typeof (T));

            var recordReference = new RecordReference<T>((T)recordObject);

            this.recordReferences.Add(recordReference);

            if (primaryRecordReference != null)
            {
                recordReference.AddPrimaryRecordReference(primaryRecordReference);
            }

            StandardPopulator.Logger.Debug("Exiting Add<T>");

            return recordReference;
        }

        public virtual void Bind()
        {
            StandardPopulator.Logger.Debug("Entering Populate");

            this.persistence.Persist(this.recordReferences);
            this.recordReferences.Clear();

            StandardPopulator.Logger.Debug("Exiting Populate");
        }

        #endregion Public Methods
    }
}