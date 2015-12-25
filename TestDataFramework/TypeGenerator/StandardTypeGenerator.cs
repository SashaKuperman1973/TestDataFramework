using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TestDataFramework.Exceptions;
using TestDataFramework.ValueGenerator;

namespace TestDataFramework.TypeGenerator
{
    public class StandardTypeGenerator : ITypeGenerator
    {
        private readonly IValueGenerator valueGenerator;

        private readonly List<Type> complexTypeProcessingRecursionGuard = new List<Type>();

        public StandardTypeGenerator(Func<ITypeGenerator, IValueGenerator> valueGeneratorFactory)
        {
            this.valueGenerator = valueGeneratorFactory(this);
        }

        public object GetObject(Type forType)
        {
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

            this.complexTypeProcessingRecursionGuard.Add(forType);

            object objectToFill = defaultConstructor.Invoke(null);

            PropertyInfo[] targetProperties = forType.GetProperties();

            foreach (PropertyInfo targetPropertyInfo in targetProperties)
            {
                object targetPropertyValue = this.valueGenerator.GetValue(targetPropertyInfo);
                targetPropertyInfo.SetValue(objectToFill, targetPropertyValue);
            }

            return objectToFill;
        }
    }
}
