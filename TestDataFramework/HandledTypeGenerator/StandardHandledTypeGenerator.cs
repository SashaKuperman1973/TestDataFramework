/*
    Copyright 2016, 2017, 2018, 2019, 2023 Alexander Kuperman

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
using System.Reflection;
using log4net;
using TestDataFramework.Helpers;
using TestDataFramework.Logger;
using TestDataFramework.TypeGenerator.Concrete;
using TestDataFramework.ValueGenerator.Interfaces;

namespace TestDataFramework.HandledTypeGenerator
{
    public delegate object HandledTypeValueGetterWithContext(Type forType, TypeGeneratorContext typeGeneratorContext);
    public delegate object HandledTypeValueGetter(Type forType);

    public class StandardHandledTypeGenerator : IHandledTypeGenerator
    {
        public StandardHandledTypeGenerator(IValueGenerator valueGenerator,
            CreateAccumulatorValueGeneratorDelegate getAccumulatorValueGenerator,
            int collectionElementCount = 5)
        {
            StandardHandledTypeGenerator.Logger.Debug("Entering constructor");

            this.valueGenerator = valueGenerator;
            this.getAccumulatorValueGenerator = getAccumulatorValueGenerator;
            this.collectionElementCount = collectionElementCount;

            this.HandledTypeValueGetterDictionary = new Dictionary<Type, HandledTypeValueGetterWithContext>
            {
                {typeof(KeyValuePair<,>), this.GetKeyValuePair},
                {typeof(IDictionary<,>), this.GetDictionary},
                {typeof(Dictionary<,>), this.GetDictionary},
                {typeof(IEnumerable<>), this.GetList},
                {typeof(ICollection<>), this.GetList},
                {typeof(List<>), this.GetList},
                {typeof(IList<>), this.GetList},
                {typeof(Tuple<>), this.GetTuple},
                {typeof(Tuple<,>), this.GetTuple},
                {typeof(Tuple<,,>), this.GetTuple}
            };

            StandardHandledTypeGenerator.Logger.Debug("Exiting constructor");
        }

        public virtual object GetObject(Type forType, TypeGeneratorContext typeGeneratorContext)
        {
            StandardHandledTypeGenerator.Logger.Debug("Entering GetObject. forType: " + forType);

            HandledTypeValueGetterWithContext getter;

            if (!this.HandledTypeValueGetterDictionary.TryGetValue(forType, out getter)
                && !(forType.IsGenericType &&
                     this.HandledTypeValueGetterDictionary.TryGetValue(forType.GetGenericTypeDefinition(), out getter))
            )
            {
                StandardHandledTypeGenerator.Logger.Debug($"No handler found for type {forType}. Returning null.");
                return null;
            }

            object result = getter(forType, typeGeneratorContext);

            StandardHandledTypeGenerator.Logger.Debug($"Exiting GetObject. result: {result?.GetType()}");
            return result;
        }

        protected virtual object GetGenericCollection(Type[] genericArgumentTypes, Type concreteOpenType,
            Func<Type[], object[]> genericCollectionValueGenerator, int? collectionElementCountOverride = null)
        {
            StandardHandledTypeGenerator.Logger.Debug(
                $"Entering GetGenericCollection. concreteOpenType: {concreteOpenType}");

            Type targetType = concreteOpenType.GetGenericTypeDefinition().MakeGenericType(genericArgumentTypes);

            ConstructorInfo constructor = targetType.GetConstructor(Type.EmptyTypes);
            object collection = constructor.Invoke(null);

            MethodInfo add = targetType.GetMethod("Add");

            int actualCollectionElementCount = collectionElementCountOverride ?? this.collectionElementCount;

            for (int i = 0; i < actualCollectionElementCount; i++)
            {
                object[] parameters = genericCollectionValueGenerator(genericArgumentTypes);
                if (parameters == null)
                    return collection;
                add.Invoke(collection, parameters);
            }

            StandardHandledTypeGenerator.Logger.Debug($"Exiting GetGenericCollection. Result collection: {collection?.GetType()}");
            return collection;
        }

        #region Fields

        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(StandardHandledTypeGenerator));

        public delegate IValueGenerator CreateAccumulatorValueGeneratorDelegate();

        public IDictionary<Type, HandledTypeValueGetterWithContext> HandledTypeValueGetterDictionary { get; }

        private readonly IValueGenerator valueGenerator;
        private readonly CreateAccumulatorValueGeneratorDelegate getAccumulatorValueGenerator;
        private readonly int collectionElementCount;

        #endregion Fields

        #region Type getters

        private object GetKeyValuePair(Type forType, TypeGeneratorContext typeGeneratorContext)
        {
            StandardHandledTypeGenerator.Logger.Debug($"Entering GetKeyValuePair. forType: {forType}");

            Type[] genericArgumentTypes = forType.GetGenericArguments();
            ConstructorInfo constructor = forType.GetConstructor(genericArgumentTypes);

            object key = this.valueGenerator.GetValue(null, genericArgumentTypes[0], typeGeneratorContext);
            object value = this.valueGenerator.GetValue(null, genericArgumentTypes[1], typeGeneratorContext);

            object result = constructor.Invoke(new[] {key, value});

            StandardHandledTypeGenerator.Logger.Debug($"Exiting GetKeyValuePair. result: {result?.GetType()}");
            return result;
        }

        private object GetDictionary(Type forType, TypeGeneratorContext typeGeneratorContext)
        {
            StandardHandledTypeGenerator.Logger.Debug($"Entering GetDictionary. forType: {forType}");

            Type[] genericArgumentTypes = forType.GetGenericArguments();
            Array keyEnumValues = null;
            int keyIndex = 0;

            if (typeof(Enum).IsAssignableFrom(genericArgumentTypes[0]))
            {
                Type enumType = genericArgumentTypes[0];
                keyEnumValues = enumType.GetEnumValues();
            }

            Func<Type[], object[]> genericCollectionValueGenerator = typeArray =>
            {
                StandardHandledTypeGenerator.Logger.Debug($"Type array values: {typeArray[0]}, {typeArray[1]}");

                object key;

                if (typeof(Enum).IsAssignableFrom(typeArray[0]))
                    if (keyIndex < keyEnumValues.Length)
                        key = keyEnumValues.GetValue(keyIndex++);
                    else
                        return null;
                else
                    key = this.GenerateCollectionKey(typeArray, typeGeneratorContext);

                object value = this.valueGenerator.GetValue(null, typeArray[1], typeGeneratorContext);

                StandardHandledTypeGenerator.Logger.Debug($"genericCollectionValueGenerator result: {key?.GetType()}, {value?.GetType()}");
                return new[] {key, value};
            };

            object result = this.GetGenericCollection(genericArgumentTypes, typeof(Dictionary<,>),
                genericCollectionValueGenerator);

            StandardHandledTypeGenerator.Logger.Debug($"Exiting GetDictionary. result: {result?.GetType()}");
            return result;
        }

        private object GenerateCollectionKey(Type[] typeArray, TypeGeneratorContext typeGeneratorContext)
        {
            object key;

            if (typeArray[0].IsValueLikeType() && !typeof(Enum).IsAssignableFrom(typeArray[0]))
            {
                IValueGenerator accumulatorValueGenerator = this.getAccumulatorValueGenerator();
                key = accumulatorValueGenerator.GetValue(null, typeArray[0], typeGeneratorContext);
            }
            else
            {
                key = this.valueGenerator.GetValue(null, typeArray[0], typeGeneratorContext);
            }

            return key;
        }

        private object GetList(Type forType, TypeGeneratorContext typeGeneratorContext)
        {
            StandardHandledTypeGenerator.Logger.Debug($"Entering GetList. forType: {forType}");

            Func<Type[], object[]> genericCollectionValueGenerator = typeArray =>
            {
                object valueGeneratorResult = this.valueGenerator.GetValue(null, typeArray[0], typeGeneratorContext);

                StandardHandledTypeGenerator.Logger.Debug(
                    $"genericCollectionValueGenerator valueGeneratorResult: {valueGeneratorResult}");
                return new[] {valueGeneratorResult};
            };

            object result = this.GetGenericCollection(forType.GetGenericArguments(), typeof(List<>),
                genericCollectionValueGenerator);

            StandardHandledTypeGenerator.Logger.Debug($"Exiting GetList. result: {result?.GetType()}");
            return result;
        }

        private object GetTuple(Type forType, TypeGeneratorContext typeGeneratorContext)
        {
            StandardHandledTypeGenerator.Logger.Debug($"Entering GetTuple. forType: {forType}");

            Type[] genericArgumentTypes = forType.GetGenericArguments();
            ConstructorInfo constructor = forType.GetConstructor(genericArgumentTypes);

            var argumentValues = new object[genericArgumentTypes.Length];

            for (int i = 0; i < argumentValues.Length; i++)
                argumentValues[i] = this.valueGenerator.GetValue(null, genericArgumentTypes[i], typeGeneratorContext);

            object result = constructor.Invoke(argumentValues);

            StandardHandledTypeGenerator.Logger.Debug($"Exiting GetTuple. result: {result?.GetType()}");
            return result;
        }

        #endregion Type getters
    }
}