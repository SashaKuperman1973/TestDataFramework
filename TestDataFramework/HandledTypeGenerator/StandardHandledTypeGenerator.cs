using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using TestDataFramework.Helpers;
using TestDataFramework.ValueGenerator.Interfaces;

namespace TestDataFramework.HandledTypeGenerator
{
    public class StandardHandledTypeGenerator : IHandledTypeGenerator
    {
        #region Fields

        private delegate object HandledTypeValueGetter(Type forType);

        private static readonly ILog Logger = LogManager.GetLogger(typeof(StandardHandledTypeGenerator));

        public delegate IValueGenerator CreateAccumulatorValueGeneratorDelegate();

        private readonly Dictionary<Type, HandledTypeValueGetter> handledTypeValueGetterDictionary;
        private readonly IValueGenerator valueGenerator;
        private readonly CreateAccumulatorValueGeneratorDelegate getAccumulatorValueGenerator;
        private readonly Random random;
        private readonly int maxCollectionElementCount;

        #endregion Fields

        public StandardHandledTypeGenerator(IValueGenerator valueGenerator,
            CreateAccumulatorValueGeneratorDelegate getAccumulatorValueGenerator, Random random,
            int maxCollectionElementCount = 5)
        {
            StandardHandledTypeGenerator.Logger.Debug("Entering constructor");

            this.valueGenerator = valueGenerator;
            this.random = random;
            this.getAccumulatorValueGenerator = getAccumulatorValueGenerator;
            this.maxCollectionElementCount = maxCollectionElementCount;

            this.handledTypeValueGetterDictionary = new Dictionary<Type, HandledTypeValueGetter>
            {
                {typeof (KeyValuePair<,>), this.GetKeyValuePair},
                {typeof (IDictionary<,>), this.GetDictionary},
                {typeof (Dictionary<,>), this.GetDictionary},
                {typeof (IEnumerable<>), this.GetList},
                {typeof (List<>), this.GetList},
                {typeof (IList<>), this.GetList},
            };

            StandardHandledTypeGenerator.Logger.Debug("Exiting constructor");
        }

        public virtual object GetObject(Type forType)
        {
            StandardHandledTypeGenerator.Logger.Debug("Entering GetObject. forType: " + forType);

            Type typeToCheck = forType.IsGenericType ? forType.GetGenericTypeDefinition() : forType;

            HandledTypeValueGetter getter;

            if (!this.handledTypeValueGetterDictionary.TryGetValue(typeToCheck, out getter))
            {
                StandardHandledTypeGenerator.Logger.Debug($"No handler found for type {forType}. Returning null.");
                return null;
            }

            object result = getter(forType);

            StandardHandledTypeGenerator.Logger.Debug($"Exiting GetObject. result: {result}");
            return result;
        }

        protected virtual object GetGenericCollection(Type forType, Type concreteOpenType, Func<Type[], object[]> genericCollectionValueGenerator)
        {
            StandardHandledTypeGenerator.Logger.Debug($"Entering GetGenericCollection. forType: {forType}, concreteOpenType: {concreteOpenType}");

            Type[] genericArgumentTypes = forType.GetGenericArguments();
            Type targetType = concreteOpenType.GetGenericTypeDefinition().MakeGenericType(genericArgumentTypes);

            ConstructorInfo constructor = targetType.GetConstructor(Type.EmptyTypes);
            object collection = constructor.Invoke(null);

            int elementCount = this.random.Next(this.maxCollectionElementCount) + 1;

            MethodInfo add = targetType.GetMethod("Add");

            for (int i = 0; i < elementCount; i++)
            {
                object[] parameters = genericCollectionValueGenerator(genericArgumentTypes);
                add.Invoke(collection, parameters);
            }

            StandardHandledTypeGenerator.Logger.Debug($"Exiting GetGenericCollection. Result collection: {collection}");
            return collection;
        }

        #region Type getters

        private object GetKeyValuePair(Type forType)
        {
            StandardHandledTypeGenerator.Logger.Debug($"Entering GetKeyValuePair. forType: {forType}");

            Type[] genericArgumentTypes = forType.GetGenericArguments();
            ConstructorInfo constructor = forType.GetConstructor(genericArgumentTypes);

            object key = this.valueGenerator.GetValue(null, genericArgumentTypes[0]);
            object value = this.valueGenerator.GetValue(null, genericArgumentTypes[1]);

            object result = constructor.Invoke(new[] {key, value});

            StandardHandledTypeGenerator.Logger.Debug($"Exiting GetKeyValuePair. result: {result}");
            return result;
        }

        private object GetDictionary(Type forType)
        {
            StandardHandledTypeGenerator.Logger.Debug($"Entering GetDictionary. forType: {forType}");

            Func<Type[], object[]> genericCollectionValueGenerator = typeArray =>
            {
                StandardHandledTypeGenerator.Logger.Debug($"Type array values: {typeArray[0]}, {typeArray[1]}");

                object key;

                if (typeArray[0].IsValueLikeType())
                {
                    IValueGenerator accumulatorValueGenerator = this.getAccumulatorValueGenerator();
                    key = accumulatorValueGenerator.GetValue(null, typeArray[0]);
                }
                else
                {
                    key = this.valueGenerator.GetValue(null, typeArray[0]);
                }

                object value = this.valueGenerator.GetValue(null, typeArray[1]);

                StandardHandledTypeGenerator.Logger.Debug($"genericCollectionValueGenerator result: {key}, {value}");
                return new[] {key, value};
            };

            object result = this.GetGenericCollection(forType, typeof(Dictionary<,>), genericCollectionValueGenerator);

            StandardHandledTypeGenerator.Logger.Debug($"Exiting GetDictionary. result: {result}");
            return result;
        }

        private object GetList(Type forType)
        {
            StandardHandledTypeGenerator.Logger.Debug($"Entering GetDictionary. forType: {forType}");

            Func<Type[], object[]> genericCollectionValueGenerator = typeArray =>
            {
                object valueGeneratorResult = this.valueGenerator.GetValue(null, typeArray[0]);

                StandardHandledTypeGenerator.Logger.Debug($"genericCollectionValueGenerator valueGeneratorResult: {valueGeneratorResult}");
                return new[] {valueGeneratorResult};
            };

            object result = this.GetGenericCollection(forType, typeof (List<>), genericCollectionValueGenerator);

            StandardHandledTypeGenerator.Logger.Debug($"Exiting GetDictionary. result: {result}");
            return result;
        }

        #endregion Type getters
    }
}
