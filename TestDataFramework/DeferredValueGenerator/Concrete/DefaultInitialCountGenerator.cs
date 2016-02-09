using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Helpers;

namespace TestDataFramework.DeferredValueGenerator.Concrete
{
    public class DefaultInitialCountGenerator : IPropertyDataGenerator<LargeInteger>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DefaultInitialCountGenerator));

        public void FillData(IDictionary<PropertyInfo, Data<LargeInteger>> propertyDataDictionary)
        {
            DefaultInitialCountGenerator.Logger.Debug("Entering FillData");

            DefaultInitialCountGenerator.Logger.Debug($"Initial count: {Helper.DefaultInitalCount}");

            propertyDataDictionary.ToList().ForEach(kvp =>
            {
                DefaultInitialCountGenerator.Logger.Debug($"Setting for property: {kvp.Key.GetExtendedMemberInfoString()}");
                kvp.Value.Item = Helper.DefaultInitalCount;
            });

            DefaultInitialCountGenerator.Logger.Debug("Exiting FillData");
        }
    }
}
