using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.ValueGenerator;
using TestDataFramework.Helpers;
using TestDataFramework.ValueGenerator.Interface;

namespace TestDataFramework.TypeGenerator
{
    public class UniqueValueTypeGenerator : StandardTypeGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UniqueValueTypeGenerator));

        public delegate IValueGenerator GetAccumulatorValueGenerator(ITypeGenerator typeGenerator);

        private readonly IValueGenerator accumulatorValueGenerator;

        public UniqueValueTypeGenerator(GetAccumulatorValueGenerator getAccumulatorValueGenerator, IValueGenerator valueGenerator,
            IHandledTypeGenerator handledTypeGenerator) : base(valueGenerator, handledTypeGenerator)
        {
            this.accumulatorValueGenerator = getAccumulatorValueGenerator(this);
        }

        protected override void SetProperty(object objectToFill, PropertyInfo targetPropertyInfo)
        {
            UniqueValueTypeGenerator.Logger.Debug("Entering SetProperty. targetPropertyInfo: " +
                                                  targetPropertyInfo.GetExtendedMemberInfoString());

            if (!targetPropertyInfo.PropertyType.IsValueLikeType())
            {
                UniqueValueTypeGenerator.Logger.Debug("Property type is value like. Calling base.");

                base.SetProperty(objectToFill, targetPropertyInfo);
                return;
            }

            object targetPropertyValue = this.accumulatorValueGenerator.GetValue(targetPropertyInfo);
            UniqueValueTypeGenerator.Logger.Debug($"targetPropertyValue: {targetPropertyValue}");
            targetPropertyInfo.SetValue(objectToFill, targetPropertyValue);

            UniqueValueTypeGenerator.Logger.Debug("Exiting SetProperty");
        }
    }
}
