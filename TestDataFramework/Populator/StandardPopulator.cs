using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TestDataFramework.Persistence;
using TestDataFramework.Randomizer;
using TestDataFramework.ValueGenerator;
using log4net;

namespace TestDataFramework.Populator
{
    public class StandardPopulator : IPopulator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (StandardPopulator));

        private readonly IValueGenerator valueGenerator;
        private IPersistence persistence;

        private readonly List<Type> typesToGenerate = new List<Type>();
        private readonly List<object> recordObjects = new List<object>();

        public StandardPopulator(IValueGenerator valueGenerator, IPersistence persistence)
        {
            this.valueGenerator = valueGenerator;
            this.persistence = persistence;
        }

        #region Public Methods

        public void Populate()
        {
            StandardPopulator.Logger.Debug("Entering Populate");

            this.typesToGenerate.ForEach(this.GenerateRecord);

            this.persistence.Persist(this.recordObjects);

            StandardPopulator.Logger.Debug("Exiting Populate");
        }

        public void Add<T>() where T : new()
        {
            StandardPopulator.Logger.Debug("Entering Add<T>");

            StandardPopulator.Logger.Debug("Adding type " + typeof (T));
            this.typesToGenerate.Add(typeof(T));

            StandardPopulator.Logger.Debug("Exiting Add<T>");
        }

        #endregion Public Methods

        #region Private Methods

        private void GenerateRecord(Type typeToGenerate)
        {
            StandardPopulator.Logger.Debug("Entering GenerateRecord");

            StandardPopulator.Logger.Debug("Generaing type " + typeToGenerate);

            PropertyInfo[] propertyInfoCollection = typeToGenerate.GetProperties();

            object recordObject = typeToGenerate.GetConstructor(Type.EmptyTypes).Invoke(null);

            propertyInfoCollection.ToList().ForEach(propertyInfo => this.SetProperty(propertyInfo, recordObject));

            this.recordObjects.Add(recordObject);

            StandardPopulator.Logger.Debug("Exiting GenerateRecord");
        }

        private void SetProperty(PropertyInfo propertyInfo, object recordObject)
        {
            StandardPopulator.Logger.Debug("Entering SetProperty");

            StandardPopulator.Logger.Debug("Setting property " + propertyInfo.Name);

            object value = this.valueGenerator.GetValue(propertyInfo.PropertyType);

            StandardPopulator.Logger.Debug("Setting property value: " + value);
            propertyInfo.SetValue(recordObject, value);

            StandardPopulator.Logger.Debug("Exiting SetProperty");
        }

        #endregion Private Methods
    }
}