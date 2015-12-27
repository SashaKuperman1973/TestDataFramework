using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using TestDataFramework.Exceptions;
using TestDataFramework.ValueGenerator;

namespace TestDataFramework.TypeGenerator
{
    public class StandardTypeGenerator : ITypeGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StandardTypeGenerator));

        private readonly IValueGenerator valueGenerator;

        private readonly List<Type> complexTypeProcessingRecursionGuard = new List<Type>();

        public StandardTypeGenerator(Func<ITypeGenerator, IValueGenerator> valueGeneratorFactory)
        {
            this.valueGenerator = valueGeneratorFactory(this);
        }

        public virtual object GetObject(Type forType)
        {
            StandardTypeGenerator.Logger.Debug("Entering GetObject");

            if (this.complexTypeProcessingRecursionGuard.Contains(forType))
            {
                throw new TypeRecursionException(forType, this.complexTypeProcessingRecursionGuard);
            }

            this.complexTypeProcessingRecursionGuard.Add(forType);

            ConstructorInfo defaultConstructor = forType.GetConstructor(Type.EmptyTypes);

            if (defaultConstructor == null)
            {
                throw new NoDefaultConstructorException(forType);
            }

            object objectToFill = defaultConstructor.Invoke(null);

            this.FillObject(objectToFill);

            StandardTypeGenerator.Logger.Debug("Exiting GetObject");
            return objectToFill;
        }

        protected virtual void FillObject(object objectToFill)
        {
            StandardTypeGenerator.Logger.Debug("Entering FillObject");

            PropertyInfo[] targetProperties = objectToFill.GetType().GetProperties();

            foreach (PropertyInfo targetPropertyInfo in targetProperties)
            {
                object targetPropertyValue = this.valueGenerator.GetValue(targetPropertyInfo);
                targetPropertyInfo.SetValue(objectToFill, targetPropertyValue);
            }

            StandardTypeGenerator.Logger.Debug("Exiting FillObject");
        }
    }
}
