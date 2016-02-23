using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using log4net;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.RepositoryOperations.Model;

namespace TestDataFramework.AttributeDecorator
{
    public class StandardAttributeDecorator : IAttributeDecorator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StandardAttributeDecorator));

        private readonly ConcurrentDictionary<MemberInfo, List<Attribute>> memberAttributeDicitonary =
            new ConcurrentDictionary<MemberInfo, List<Attribute>>();

        public void DecorateMember<T, TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression, Attribute attribute)
        {
            MemberInfo memberInfo = Helper.ValidateFieldExpression(fieldExpression);

            this.memberAttributeDicitonary.AddOrUpdate(memberInfo, new List<Attribute> { attribute },
                (mi, list) =>
                {
                    list.Add(attribute);
                    return list;
                });
        }

        public void DecorateType(Type type, Attribute attribute)
        {
            this.memberAttributeDicitonary.AddOrUpdate(type, new List<Attribute> {attribute}, (t, list) =>
            {
                list.Add(attribute);
                return list;
            });
        }

        public T GetSingleAttribute<T>(MemberInfo memberInfo) where T : Attribute
        {
            StandardAttributeDecorator.Logger.Debug(
                $"Entering GetSingleAttribute. T: {typeof(T)} memberInfo: {memberInfo.GetExtendedMemberInfoString()}");

            T[] result = this.GetCustomAttributes<T>(memberInfo).ToArray();

            if (result.Length <= 1)
            {
                T firstOrDefaultResult = result.FirstOrDefault();

                StandardAttributeDecorator.Logger.Debug($"Member attributes count <= 1. firstOrDefaultResult: {firstOrDefaultResult}");
                return firstOrDefaultResult;
            }

            string message =
                memberInfo.MemberType == MemberTypes.Property
                    ? Messages.AmbigousPropertyAttributeMatch
                    : memberInfo.MemberType == (MemberTypes.TypeInfo | MemberTypes.NestedType)
                        ? Messages.AmbigousTypeAttributeMatch
                        : Messages.AmbigousAttributeMatch;

            throw new AmbiguousMatchException(string.Format(message, typeof(T), memberInfo.Name, memberInfo.DeclaringType));
        }

        public IEnumerable<T> GetUniqueAttributes<T>(Type type) where T : Attribute
        {
            StandardAttributeDecorator.Logger.Debug($"Entering GetUniqueAttributes. T: {typeof(T)} type: {type}");

            IEnumerable<T> result = type.GetPropertiesHelper()
                .Select(this.GetSingleAttribute<T>).Where(a => a != null);

            StandardAttributeDecorator.Logger.Debug($"Exiting GetUniqueAttributes. result: {result}");
            return result;
        }

        public PropertyAttribute<T> GetPropertyAttribute<T>(PropertyInfo propertyInfo) where T : Attribute
        {
            StandardAttributeDecorator.Logger.Debug($"Entering GetPropertyAttribute. T: {typeof(T)} propertyInfo: {propertyInfo.GetExtendedMemberInfoString()}");

            var result = new PropertyAttribute<T>
            {
                PropertyInfo = propertyInfo,
                Attribute = this.GetSingleAttribute<T>(propertyInfo)
            };

            StandardAttributeDecorator.Logger.Debug($"Exiting GetPropertyAttribute. result: {result}");
            return result;
        }

        public IEnumerable<PropertyAttribute<T>> GetPropertyAttributes<T>(Type type) where T : Attribute
        {
            StandardAttributeDecorator.Logger.Debug($"Entering GetPropertyAttributes. T: {typeof(T)}, type: {type}");

            IEnumerable<PropertyAttribute<T>> result =
                type.GetPropertiesHelper().Select(this.GetPropertyAttribute<T>).Where(pa => pa.Attribute != null);

            StandardAttributeDecorator.Logger.Debug($"Exiting GetPropertyAttributes. result: {result}");

            return result;
        }

        public IEnumerable<RepositoryOperations.Model.PropertyAttributes> GetPropertyAttributes(Type type)
        {
            StandardAttributeDecorator.Logger.Debug($"Entering GetPropertyAttributes. type: {type}");

            IEnumerable<RepositoryOperations.Model.PropertyAttributes> result =
                type.GetPropertiesHelper()
                    .Select(
                        pi =>
                            new RepositoryOperations.Model.PropertyAttributes
                            {
                                Attributes = this.GetCustomAttributes(pi).ToArray(),
                                PropertyInfo = pi
                            });

            StandardAttributeDecorator.Logger.Debug($"Exiting GetPropertyAttributes. result: {result}");
            return result;
        }

        public IEnumerable<T> GetCustomAttributes<T>(MemberInfo memberInfo) where T : Attribute
        {
            List<Attribute> programmaticAttributeList;

            List<T> result = this.memberAttributeDicitonary.TryGetValue(memberInfo, out programmaticAttributeList)

                ? programmaticAttributeList.Where(a => a.GetType() == typeof(T)).Cast<T>().ToList()
                : new List<T>();

            result.AddRange(memberInfo.GetCustomAttributes<T>());
            return result;
        }

        public T GetCustomAttribute<T>(MemberInfo memberInfo) where T : Attribute
        {
            T result = this.GetCustomAttributes<T>(memberInfo).FirstOrDefault();
            return result;
        }

        public IEnumerable<Attribute> GetCustomAttributes(MemberInfo memberInfo)
        {
            List<Attribute> result;

            if (!this.memberAttributeDicitonary.TryGetValue(memberInfo, out result))
            {
                result = new List<Attribute>();
            }

            result.AddRange(memberInfo.GetCustomAttributes());
            return result;
        }

        private readonly ConcurrentDictionary<Assembly, ConcurrentDictionary<string, Type>> stringTypeDictionary =
            new ConcurrentDictionary<Assembly, ConcurrentDictionary<string, Type>>();

        public virtual Type GetTableType(ForeignKeyAttribute foreignAttribute, Type foreignType)
        {
            if (foreignAttribute.PrimaryTableType != null)
            {
                return foreignAttribute.PrimaryTableType;
            }

            Assembly assembly = foreignType.Assembly;

            ConcurrentDictionary<string, Type> typeDictionary = this.stringTypeDictionary.AddOrUpdate(assembly,
                new ConcurrentDictionary<string, Type>(), (a, d) => d);

            Type cachedType;
            if (typeDictionary.TryGetValue(foreignAttribute.PrimaryTableName, out cachedType))
            {
                return cachedType;
            }

            List<AssemblyName> assemblyNameList = assembly.GetReferencedAssemblies().ToList();
            assemblyNameList.Add(assembly.GetName());

            AppDomain domain = AppDomain.CreateDomain("TestDataFramework_" + Guid.NewGuid(), null, AppDomain.CurrentDomain.SetupInformation);

            foreach (AssemblyName assemblyName in assemblyNameList)
            {
                Assembly loadedAssembly = domain.Load(assemblyName);

                foreach (TypeInfo definedType in loadedAssembly.DefinedTypes)
                {
                    var tableAttribute = this.GetSingleAttribute<TableAttribute>(definedType);

                    if (tableAttribute != null)
                    {
                        typeDictionary.TryAdd(tableAttribute.Name, definedType);
                    }
                }
            }

            AppDomain.Unload(domain);

            if (typeDictionary.TryGetValue(foreignAttribute.PrimaryTableName, out cachedType))
            {
                return cachedType;
            }

            throw new AttributeDecoratorException(Messages.CannotResolveForeignTableString, foreignAttribute,
                foreignType);
        }
    }
}
