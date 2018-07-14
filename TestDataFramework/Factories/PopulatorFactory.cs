/*
    Copyright 2016, 2017, 2018 Alexander Kuperman

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
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using log4net;
using TestDataFramework.ArrayRandomizer;
using TestDataFramework.AttributeDecorator.Concrete;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeepSetting.Concrete;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.DeferredValueGenerator.Concrete;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.Helpers;
using TestDataFramework.Helpers.Concrete;
using TestDataFramework.Helpers.Interfaces;
using TestDataFramework.ListOperations;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Logger;
using TestDataFramework.Persistence.Concrete;
using TestDataFramework.Persistence.Interfaces;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Interfaces;
using TestDataFramework.PropertyValueAccumulator;
using TestDataFramework.TypeGenerator.Concrete;
using TestDataFramework.TypeGenerator.Interfaces;
using TestDataFramework.UniqueValueGenerator.Concrete;
using TestDataFramework.UniqueValueGenerator.Interfaces;
using TestDataFramework.ValueFormatter.Concrete;
using TestDataFramework.ValueFormatter.Interfaces;
using TestDataFramework.ValueGenerator.Concrete;
using TestDataFramework.ValueGenerator.Interfaces;
using TestDataFramework.ValueProvider.Concrete;
using TestDataFramework.ValueProvider.Interfaces;
using TestDataFramework.WritePrimitives.Concrete;
using TestDataFramework.WritePrimitives.Interfaces;

namespace TestDataFramework.Factories
{
    public static class StaticPopulatorFactory
    {
        public static IPopulator CreateSqlClientPopulator(string connectionStringWithDefaultCatalogue,
            bool mustBeInATransaction = true, string defaultSchema = "dbo",
            bool enforceKeyReferenceCheck = true, bool throwIfUnhandledPrimaryKeyType = true,
            DeepCollectionSettingConverter deepCollectionSettingConverter = null)
        {
            using (var factory = new PopulatorFactory())
            {
                return factory.CreateSqlClientPopulator(connectionStringWithDefaultCatalogue,
                    mustBeInATransaction, defaultSchema,
                    enforceKeyReferenceCheck, throwIfUnhandledPrimaryKeyType, deepCollectionSettingConverter);
            }
        }

        public static IPopulator CreateMemoryPopulator(bool throwIfUnhandledPrimaryKeyType = false,
            string defaultSchema = null,
            DeepCollectionSettingConverter deepCollectionSettingConverter = null)
        {
            using (var factory = new PopulatorFactory())
            {
                return factory.CreateMemoryPopulator(throwIfUnhandledPrimaryKeyType, defaultSchema,
                    deepCollectionSettingConverter);
            }
        }
    }

    public class PopulatorFactory : IDisposable
    {
        private const string GetStandardTypeGenerator = "GetStandardTypeGenerator";
        private const string StandardValueProvider = "StandardValueProvider";
        private const string StandardValueGenerator = "StandardValueGenerator";
        private const string ValueAccumulator = "ValueAccumulator";
        private const string AccumulatorValueGenerator = "AccumulatorValueGenerator";
        private const string GetUniqueValueTypeGenerator = "GetUniqueValueTypeGenerator";
        private const string StandardTypeGenerator = "StandardTypeGenerator";
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(PopulatorFactory));
        internal DisposableContainer MemoryPopulatorContainer;

        internal DisposableContainer SqlClientPopulatorContainer;

        public void Dispose()
        {
            this.SqlClientPopulatorContainer?.Dispose();
            this.MemoryPopulatorContainer?.Dispose();
        }

        public IPopulator CreateSqlClientPopulator(string connectionStringWithDefaultCatalogue,
            bool mustBeInATransaction = true, string defaultSchema = "dbo",
            bool enforceKeyReferenceCheck = true, bool throwIfUnhandledPrimaryKeyType = true,
            DeepCollectionSettingConverter deepCollectionSettingConverter = null)
        {
            PopulatorFactory.Logger.Debug(
                "Entering CreateSqlClientPopulator." +
                $" mustBeInATransaction: {mustBeInATransaction}," +
                $" defaultSchema: {defaultSchema}," +
                $" enforceKeyReferenceCheck: {enforceKeyReferenceCheck}," +
                $" throwIfUnhandledPrimaryKeyType: {throwIfUnhandledPrimaryKeyType}"
            );

            IWindsorContainer iocContainer = this.GetSqlClientPopulatorContainer(connectionStringWithDefaultCatalogue,
                mustBeInATransaction, new Schema {Value = defaultSchema}, enforceKeyReferenceCheck,
                throwIfUnhandledPrimaryKeyType, new AssemblyWrapper(Assembly.GetCallingAssembly()),
                deepCollectionSettingConverter);
            var result = iocContainer.Resolve<IPopulator>();

            PopulatorFactory.Logger.Debug("Exiting CreateSqlClientPopulator");
            return result;
        }

        public IPopulator CreateMemoryPopulator(bool throwIfUnhandledPrimaryKeyType = false,
            string defaultSchema = null, DeepCollectionSettingConverter deepCollectionSettingConverter = null)
        {
            PopulatorFactory.Logger.Debug(
                $"Entering CreateMemoryPopulator. throwIfUnhandledPrimaryKeyType: {throwIfUnhandledPrimaryKeyType}, defaultSchema: {defaultSchema}");

            IWindsorContainer iocContainer =
                this.GetMemoryPopulatorContainer(new AssemblyWrapper(Assembly.GetCallingAssembly()),
                    throwIfUnhandledPrimaryKeyType, new Schema {Value = defaultSchema}, deepCollectionSettingConverter);

            var result = iocContainer.Resolve<IPopulator>();

            PopulatorFactory.Logger.Debug("Exiting CreateMemoryPopulator");
            return result;
        }

        private static void InstanceBugWorkAround<TService>(IWindsorContainer container, string serviceKey,
            string delegateKey) where TService : class
        {
            var lazyStandardTypeGenerator =
                new Lazy<TService>(() => container.Resolve<TService>(serviceKey));

            container.Register(
                Component.For<Func<TService>>()
                    .Instance(() => lazyStandardTypeGenerator.Value)
                    .Named(delegateKey));

            TService q = lazyStandardTypeGenerator.Value;
        }

        private IWindsorContainer GetSqlClientPopulatorContainer(string connectionStringWithDefaultCatalogue,
            bool mustBeInATransaction, Schema defaultSchema,
            bool enforceKeyReferenceCheck, bool throwIfUnhandledPrimaryKeyType, AssemblyWrapper callingAssembly,
            DeepCollectionSettingConverter deepCollectionSettingConverter = null)
        {
            PopulatorFactory.Logger.Debug("Entering GetSqlClientPopulatorContainer");

            if (this.SqlClientPopulatorContainer != null && !this.SqlClientPopulatorContainer.IsDisposed)
            {
                PopulatorFactory.Logger.Debug("Returning existing DI container");
                return this.SqlClientPopulatorContainer.Container;
            }

            this.SqlClientPopulatorContainer =
                new DisposableContainer(PopulatorFactory.GetCommonContainer(callingAssembly, defaultSchema,
                    deepCollectionSettingConverter));

            this.SqlClientPopulatorContainer.Container.Register(
                Component.For<IWritePrimitives>().ImplementedBy<SqlClientWritePrimitives>()
                    .DependsOn(new
                    {
                        connectionStringWithDefaultCatalogue,
                        mustBeInATransaction,
                        configuration = ConfigurationManager.AppSettings
                    }),
                Component.For<DbProviderFactory>().UsingFactoryMethod(() => SqlClientFactory.Instance, true),
                Component.For<IValueFormatter>().ImplementedBy<SqlClientValueFormatter>(),
                Component.For<IPropertyDataGenerator<LargeInteger>>().ImplementedBy<SqlClientInitialCountGenerator>(),
                Component.For<IPersistence>().ImplementedBy<SqlClientPersistence>()
                    .DependsOn(Dependency.OnValue("enforceKeyReferenceCheck", enforceKeyReferenceCheck)),
                Component.For<IUniqueValueGenerator>().ImplementedBy<KeyTypeUniqueValueGenerator>()
                    .DependsOn(new {throwIfUnhandledType = throwIfUnhandledPrimaryKeyType}),
                Component.For<IWriterDictinary>().ImplementedBy<SqlWriterDictionary>(),
                Component.For<SqlWriterCommandTextGenerator>(),
                Component.For<IValueGenerator>()
                    .ImplementedBy<SqlClientValueGenerator>()
                    .DependsOn(ServiceOverride.ForKey<IValueProvider>().Eq(PopulatorFactory.ValueAccumulator))
                    .DependsOn(
                        ServiceOverride.ForKey<Func<ITypeGenerator>>()
                            .Eq(PopulatorFactory.GetUniqueValueTypeGenerator))
                    .Named(PopulatorFactory.AccumulatorValueGenerator),
                Component.For<IValueGenerator>()
                    .ImplementedBy<SqlClientValueGenerator>()
                    .DependsOn(ServiceOverride.ForKey<IValueProvider>().Eq(PopulatorFactory.StandardValueProvider))
                    .DependsOn(
                        ServiceOverride.ForKey<Func<ITypeGenerator>>()
                            .Eq(PopulatorFactory.GetStandardTypeGenerator))
                    .Named(PopulatorFactory.StandardValueGenerator),
                Component.For<SqlWriterCommandText>().ImplementedBy<SqlWriterCommandText>()
            );

            PopulatorFactory.InstanceBugWorkAround<ITypeGenerator>(this.SqlClientPopulatorContainer.Container,
                PopulatorFactory.StandardTypeGenerator, PopulatorFactory.GetStandardTypeGenerator);

            PopulatorFactory.Logger.Debug("Exiting GetSqlClientPopulatorContainer");
            return this.SqlClientPopulatorContainer.Container;
        }

        private IWindsorContainer GetMemoryPopulatorContainer(AssemblyWrapper callingAssembly,
            bool throwIfUnhandledPrimaryKeyType, Schema defaultSchema,
            DeepCollectionSettingConverter deepCollectionSettingConverter = null)
        {
            PopulatorFactory.Logger.Debug("Entering GetMemoryPopulatorContainer");

            if (this.MemoryPopulatorContainer != null && !this.MemoryPopulatorContainer.IsDisposed)
            {
                PopulatorFactory.Logger.Debug("Returning existing DI container");
                return this.MemoryPopulatorContainer.Container;
            }

            this.MemoryPopulatorContainer =
                new DisposableContainer(PopulatorFactory.GetCommonContainer(callingAssembly, defaultSchema,
                    deepCollectionSettingConverter));

            this.MemoryPopulatorContainer.Container.Register(
                Component.For<IPropertyDataGenerator<LargeInteger>>().ImplementedBy<DefaultInitialCountGenerator>(),
                Component.For<IPersistence>().ImplementedBy<MemoryPersistence>(),
                Component.For<IUniqueValueGenerator>().ImplementedBy<MemoryUniqueValueGenerator>()
                    .DependsOn(new {throwIfUnhandledType = throwIfUnhandledPrimaryKeyType}),
                Component.For<IValueGenerator>()
                    .ImplementedBy<MemoryValueGenerator>()
                    .DependsOn(ServiceOverride.ForKey<IValueProvider>().Eq(PopulatorFactory.ValueAccumulator))
                    .DependsOn(
                        ServiceOverride.ForKey<Func<ITypeGenerator>>()
                            .Eq(PopulatorFactory.GetUniqueValueTypeGenerator))
                    .Named(PopulatorFactory.AccumulatorValueGenerator),
                Component.For<IValueGenerator>()
                    .ImplementedBy<MemoryValueGenerator>()
                    .DependsOn(ServiceOverride.ForKey<IValueProvider>().Eq(PopulatorFactory.StandardValueProvider))
                    .DependsOn(
                        ServiceOverride.ForKey<Func<ITypeGenerator>>()
                            .Eq(PopulatorFactory.GetStandardTypeGenerator))
                    .Named(PopulatorFactory.StandardValueGenerator)
            );

            PopulatorFactory.InstanceBugWorkAround<ITypeGenerator>(this.MemoryPopulatorContainer.Container,
                PopulatorFactory.StandardTypeGenerator, PopulatorFactory.GetStandardTypeGenerator);

            PopulatorFactory.Logger.Debug("Exiting GetMemoryPopulatorContainer");

            return this.MemoryPopulatorContainer.Container;
        }

        private static IWindsorContainer GetCommonContainer(AssemblyWrapper callingAssembly, Schema defaultSchema,
            DeepCollectionSettingConverter deepCollectionSettingConverter)
        {
            if (deepCollectionSettingConverter == null)
                deepCollectionSettingConverter = new DeepCollectionSettingConverter();

            const string uniqueValueTypeGenerator = "UniqueValueTypeGenerator";
            const string standardHandledTypeGenerator = "StandardHandledTypeGenerator";
            const string accumulatorValueGenerator_StandardHandledTypeGenerator =
                "AccumulatorValueGenerator_StandardHandledTypeGenerator ";

            var commonContainer = new WindsorContainer();

            commonContainer.Register(

                #region Common Region

                Component.For<DeepCollectionSettingConverter>().Instance(deepCollectionSettingConverter),
                Component.For<IPopulator>().ImplementedBy<StandardPopulator>()
                    .DependsOn(ServiceOverride.ForKey<ITypeGenerator>().Eq(PopulatorFactory.StandardTypeGenerator))
                    .DependsOn(ServiceOverride.ForKey<IHandledTypeGenerator>().Eq(standardHandledTypeGenerator)),
                Component.For<ITypeGenerator>()
                    .ImplementedBy<StandardTypeGenerator>()
                    .DependsOn(ServiceOverride.ForKey<IValueGenerator>().Eq(PopulatorFactory.StandardValueGenerator))
                    .DependsOn(ServiceOverride.ForKey<IHandledTypeGenerator>().Eq(standardHandledTypeGenerator))
                    .Named(PopulatorFactory.StandardTypeGenerator),
                Component.For<Func<IArrayRandomizer>>()
                    .Instance(
                        () => commonContainer.Resolve<IArrayRandomizer>()),
                Component.For<Random>().ImplementedBy<Random>(),
                Component.For<DateTimeProvider>().Instance(() => Helper.Now),
                Component.For<IArrayRandomizer>()
                    .ImplementedBy<StandardArrayRandomizer>()
                    .DependsOn(
                        ServiceOverride.ForKey<IValueGenerator>().Eq(PopulatorFactory.StandardValueGenerator)),
                Component.For<LetterEncoder>(),
                Component.For<IPropertyValueAccumulator>()
                    .ImplementedBy<StandardPropertyValueAccumulator>(),
                Component.For<IDeferredValueGenerator<LargeInteger>>()
                    .ImplementedBy<StandardDeferredValueGenerator<LargeInteger>>(),
                Component.For<IValueProvider>().ImplementedBy<StandardRandomizer>()
                    .DependsOn(
                        new
                        {
                            dateTimeMinValue = SqlDateTime.MinValue.Value.Ticks,
                            dateTimeMaxValue = SqlDateTime.MaxValue.Value.Ticks
                        })
                    .Named(PopulatorFactory.StandardValueProvider),
                Component.For<IRandomSymbolStringGenerator>().ImplementedBy<RandomSymbolStringGenerator>(),
                Component.For<ValueGuaranteePopulator>().ImplementedBy<ValueGuaranteePopulator>(),
                Component.For<IObjectGraphService>().ImplementedBy<ObjectGraphService>(),
                Component.For<ITableTypeCacheService>().ImplementedBy<StandardTableTypeCacheService>(),
                Component.For<TableTypeLookup>().ImplementedBy<TableTypeLookup>(),
                Component.For<ITypeGeneratorService>().ImplementedBy<StandardTypeGeneratorService>(),

                #endregion Common Region

                #region Handled Type Generator

                Component.For<IHandledTypeGenerator>()
                    .ImplementedBy<StandardHandledTypeGenerator>()
                    .DependsOn(
                        ServiceOverride.ForKey<IValueGenerator>().Eq(PopulatorFactory.StandardValueGenerator))
                    .Named(standardHandledTypeGenerator),
                Component.For<IHandledTypeGenerator>()
                    .ImplementedBy<StandardHandledTypeGenerator>()
                    .DependsOn(
                        ServiceOverride.ForKey<IValueGenerator>().Eq(PopulatorFactory.AccumulatorValueGenerator))
                    .Named(accumulatorValueGenerator_StandardHandledTypeGenerator),
                Component.For<StandardHandledTypeGenerator.CreateAccumulatorValueGeneratorDelegate>()
                    .Instance(
                        () => commonContainer.Resolve<IValueGenerator>(PopulatorFactory.AccumulatorValueGenerator)),
                Component.For<IValueProvider>()
                    .ImplementedBy<AccumulatorValueProvider>()
                    .Named(PopulatorFactory.ValueAccumulator)
                    .LifestyleTransient(),
                Component.For<Func<ITypeGenerator>>()
                    .Instance(() => commonContainer.Resolve<ITypeGenerator>(uniqueValueTypeGenerator))
                    .Named(PopulatorFactory.GetUniqueValueTypeGenerator),
                Component.For<ITypeGenerator>()
                    .ImplementedBy<UniqueValueTypeGenerator>()
                    .DependsOn(
                        ServiceOverride.ForKey<IHandledTypeGenerator>()
                            .Eq(accumulatorValueGenerator_StandardHandledTypeGenerator))
                    .Named(uniqueValueTypeGenerator),
                Component.For<UniqueValueTypeGenerator.GetAccumulatorValueGenerator>()
                    .Instance(
                        typeGenerator =>
                            commonContainer.Resolve<IValueGenerator>(PopulatorFactory.AccumulatorValueGenerator)),
                Component.For<StandardTableTypeCache>().ImplementedBy<StandardTableTypeCache>(),
                Component.For<IAttributeDecorator>().ImplementedBy<StandardAttributeDecorator>(),
                Component.For<IAttributeDecoratorBase>().ImplementedBy<StandardAttributeDecoratorBase>(),
                Component.For<AssemblyWrapper>().Instance(callingAssembly),
                Component.For<Schema>().Instance(defaultSchema)

                #endregion Handled Type Generator

            );

            return commonContainer;
        }

        internal class DisposableContainer : IDisposable
        {
            public DisposableContainer(IWindsorContainer container)
            {
                this.Container = container;
            }

            public IWindsorContainer Container { get; }

            public bool IsDisposed { get; private set; }

            public void Dispose()
            {
                this.Container.Dispose();
                this.IsDisposed = true;
            }
        }
    }
}