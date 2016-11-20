/*
    Copyright 2016 Alexander Kuperman

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
using TestDataFramework.AttributeDecorator;
using TestDataFramework.DeferredValueGenerator.Concrete;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.Helpers;
using TestDataFramework.Helpers.Concrete;
using TestDataFramework.Helpers.Interfaces;
using TestDataFramework.ListOperations;
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
using TestDataFramework.ValueGenerator;
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
            bool enforceKeyReferenceCheck = true, bool throwIfUnhandledPrimaryKeyType = true)
        {
            using (var factory = new PopulatorFactory())
            {
                return factory.CreateSqlClientPopulator(connectionStringWithDefaultCatalogue,
                    mustBeInATransaction, defaultSchema,
                    enforceKeyReferenceCheck, throwIfUnhandledPrimaryKeyType);
            }
        }

        public static IPopulator CreateMemoryPopulator(bool throwIfUnhandledPrimaryKeyType = false, string defaultSchema = null)
        {
            using (var factory = new PopulatorFactory())
            {
                return factory.CreateMemoryPopulator(throwIfUnhandledPrimaryKeyType, defaultSchema);
            }
        }
    }

    public class PopulatorFactory : IDisposable
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(PopulatorFactory));

        private DisposableContainer sqlClientPopulatorContainer;
        private DisposableContainer memoryPopulatorContainer;

        private const string GetStandardTypeGenerator = "GetStandardTypeGenerator";
        private const string StandardValueProvider = "StandardValueProvider";
        private const string StandardValueGenerator = "StandardValueGenerator";
        private const string ValueAccumulator = "ValueAccumulator";
        private const string AccumulatorValueGenerator = "AccumulatorValueGenerator";
        private const string GetUniqueValueTypeGenerator = "GetUniqueValueTypeGenerator";
        private const string StandardTypeGenerator = "StandardTypeGenerator";

        public IPopulator CreateSqlClientPopulator(string connectionStringWithDefaultCatalogue, 
            bool mustBeInATransaction = true, string defaultSchema = "dbo",
            bool enforceKeyReferenceCheck = true, bool throwIfUnhandledPrimaryKeyType = true)
        {
            PopulatorFactory.Logger.Debug(
                "Entering CreateSqlClientPopulator."+
                $" mustBeInATransaction: {mustBeInATransaction}," +
                $" defaultSchema: {defaultSchema}," +
                $" enforceKeyReferenceCheck: {enforceKeyReferenceCheck}," +
                $" throwIfUnhandledPrimaryKeyType: {throwIfUnhandledPrimaryKeyType}"
                );

            IWindsorContainer iocContainer = this.GetSqlClientPopulatorContainer(connectionStringWithDefaultCatalogue,
                mustBeInATransaction, defaultSchema, enforceKeyReferenceCheck, throwIfUnhandledPrimaryKeyType, Assembly.GetCallingAssembly());

            var result = iocContainer.Resolve<IPopulator>();

            PopulatorFactory.Logger.Debug("Exiting CreateSqlClientPopulator");
            return result;
        }

        public IPopulator CreateMemoryPopulator(bool throwIfUnhandledPrimaryKeyType = false, string defaultSchema = null)
        {
            PopulatorFactory.Logger.Debug($"Entering CreateMemoryPopulator. throwIfUnhandledPrimaryKeyType: {throwIfUnhandledPrimaryKeyType}, defaultSchema: {defaultSchema}");

            IWindsorContainer iocContainer = this.GetMemoryPopulatorContainer(Assembly.GetCallingAssembly(), throwIfUnhandledPrimaryKeyType, defaultSchema);

            var result = iocContainer.Resolve<IPopulator>();

            PopulatorFactory.Logger.Debug("Exiting CreateMemoryPopulator");
            return result;
        }

        private static void InstanceBugWorkAround<TService>(IWindsorContainer container, string serviceKey, string delegateKey) where TService : class
        {
            var lazyStandardTypeGenerator =
                new Lazy<TService>(() => container.Resolve<TService>(serviceKey));

            container.Register(
                Component.For<Func<TService>>().Instance(() => lazyStandardTypeGenerator.Value).Named(delegateKey));

            var q = lazyStandardTypeGenerator.Value;
        }

        private IWindsorContainer GetSqlClientPopulatorContainer(string connectionStringWithDefaultCatalogue, bool mustBeInATransaction, string defaultSchema,
            bool enforceKeyReferenceCheck, bool throwIfUnhandledPrimaryKeyType, Assembly callingAssembly)
        {
            PopulatorFactory.Logger.Debug("Entering GetSqlClientPopulatorContainer");

            if (this.sqlClientPopulatorContainer != null && !this.sqlClientPopulatorContainer.IsDisposed)
            {
                PopulatorFactory.Logger.Debug("Returning existing DI container");
                return this.sqlClientPopulatorContainer.Container;
            }

            this.sqlClientPopulatorContainer = new DisposableContainer(PopulatorFactory.GetCommonContainer(callingAssembly, defaultSchema));

            this.sqlClientPopulatorContainer.Container.Register(

                Component.For<IWritePrimitives>().ImplementedBy<SqlClientWritePrimitives>()
                    .DependsOn(new { connectionStringWithDefaultCatalogue, mustBeInATransaction, configuration = ConfigurationManager.AppSettings }),

                Component.For<DbProviderFactory>().UsingFactoryMethod(() => SqlClientFactory.Instance, true),

                Component.For<IValueFormatter>().ImplementedBy<SqlClientValueFormatter>(),

                Component.For<IPropertyDataGenerator<LargeInteger>>().ImplementedBy<SqlClientInitialCountGenerator>(),

                Component.For<IPersistence>().ImplementedBy<SqlClientPersistence>().DependsOn(Dependency.OnValue("enforceKeyReferenceCheck", enforceKeyReferenceCheck)),

                Component.For<IUniqueValueGenerator>().ImplementedBy<KeyTypeUniqueValueGenerator>().DependsOn(new { throwIfUnhandledType = throwIfUnhandledPrimaryKeyType }),

                Component.For<IWriterDictinary>().ImplementedBy<SqlWriterDictionary>(),

                Component.For<SqlWriterCommandTextGenerator>(),

                Component.For<IValueGenerator>()
                    .ImplementedBy<SqlClientValueGenerator>()
                    .DependsOn(ServiceOverride.ForKey<IValueProvider>().Eq(PopulatorFactory.ValueAccumulator))
                    .DependsOn(
                        ServiceOverride.ForKey<Func<ITypeGenerator>>()
                            .Eq(PopulatorFactory.GetUniqueValueTypeGenerator))
                    .Named(PopulatorFactory.AccumulatorValueGenerator)
                    .LifestyleTransient(),

                Component.For<IValueGenerator>()
                    .ImplementedBy<SqlClientValueGenerator>()
                    .DependsOn(ServiceOverride.ForKey<IValueProvider>().Eq(PopulatorFactory.StandardValueProvider))
                    .DependsOn(
                        ServiceOverride.ForKey<Func<ITypeGenerator>>()
                            .Eq(PopulatorFactory.GetStandardTypeGenerator))
                    .Named(PopulatorFactory.StandardValueGenerator),

                Component.For<SqlWriterCommandText>().ImplementedBy<SqlWriterCommandText>(),

                Component.For<Func<TableTypeCache, IAttributeDecorator>>()
                    .Instance(tableTypeCache => new StandardAttributeDecorator(x => tableTypeCache, callingAssembly, defaultSchema))

                );

            PopulatorFactory.InstanceBugWorkAround<ITypeGenerator>(this.sqlClientPopulatorContainer.Container,
                PopulatorFactory.StandardTypeGenerator, PopulatorFactory.GetStandardTypeGenerator);

            PopulatorFactory.Logger.Debug("Exiting GetSqlClientPopulatorContainer");
            return this.sqlClientPopulatorContainer.Container;
        }

        private IWindsorContainer GetMemoryPopulatorContainer(Assembly callingAssembly, bool throwIfUnhandledPrimaryKeyType, string defaultSchema)
        {
            PopulatorFactory.Logger.Debug("Entering GetMemoryPopulatorContainer");

            if (this.memoryPopulatorContainer != null && !this.memoryPopulatorContainer.IsDisposed)
            {
                PopulatorFactory.Logger.Debug("Returning existing DI container");
                return this.memoryPopulatorContainer.Container;
            }

            this.memoryPopulatorContainer = new DisposableContainer(PopulatorFactory.GetCommonContainer(callingAssembly, defaultSchema));

            this.memoryPopulatorContainer.Container.Register(

                Component.For<IPropertyDataGenerator<LargeInteger>>().ImplementedBy<DefaultInitialCountGenerator>(),

                Component.For<IPersistence>().ImplementedBy<MemoryPersistence>(),

                Component.For<IUniqueValueGenerator>().ImplementedBy<MemoryUniqueValueGenerator>().DependsOn(new { throwIfUnhandledType = throwIfUnhandledPrimaryKeyType }),

                Component.For<IValueGenerator>()
                    .ImplementedBy<MemoryValueGenerator>()
                    .DependsOn(ServiceOverride.ForKey<IValueProvider>().Eq(PopulatorFactory.ValueAccumulator))
                    .DependsOn(
                        ServiceOverride.ForKey<Func<ITypeGenerator>>()
                            .Eq(PopulatorFactory.GetUniqueValueTypeGenerator))
                    .Named(PopulatorFactory.AccumulatorValueGenerator)
                    .LifestyleTransient(),

                Component.For<IValueGenerator>()
                    .ImplementedBy<MemoryValueGenerator>()
                    .DependsOn(ServiceOverride.ForKey<IValueProvider>().Eq(PopulatorFactory.StandardValueProvider))
                    .DependsOn(
                        ServiceOverride.ForKey<Func<ITypeGenerator>>()
                            .Eq(PopulatorFactory.GetStandardTypeGenerator))
                    .Named(PopulatorFactory.StandardValueGenerator),

                Component.For<Func<TableTypeCache, IAttributeDecorator>>()
                    .Instance(tableTypeCache => new StandardAttributeDecorator(x => tableTypeCache, callingAssembly, null))

                );

            PopulatorFactory.InstanceBugWorkAround<ITypeGenerator>(this.memoryPopulatorContainer.Container,
                PopulatorFactory.StandardTypeGenerator, PopulatorFactory.GetStandardTypeGenerator);

            PopulatorFactory.Logger.Debug("Exiting GetMemoryPopulatorContainer");

            return this.memoryPopulatorContainer.Container;
        }

        private static IWindsorContainer GetCommonContainer(Assembly callingAssembly, string defaultSchema)
        {
            const string uniqueValueTypeGenerator = "UniqueValueTypeGenerator";
            const string standardHandledTypeGenerator = "StandardHandledTypeGenerator";
            const string accumulatorValueGenerator_StandardHandledTypeGenerator = "AccumulatorValueGenerator_StandardHandledTypeGenerator ";                

            var commonContainer = new WindsorContainer();

            commonContainer.Register(

            #region Common Region

            #region Delegates

                Component.For<Func<ITableTypeCacheService, TableTypeCache>>()
                    .Instance(attributeDecorator => new TableTypeCache(x => attributeDecorator)),

            #endregion Delegates

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

                Component.For<TableTypeCache>().ImplementedBy<TableTypeCache>(),

                Component.For<ValueGuaranteePopulator>().ImplementedBy<ValueGuaranteePopulator>(),


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
                            commonContainer.Resolve<IValueGenerator>(PopulatorFactory.AccumulatorValueGenerator))

                #endregion Handled Type Generator

                );

            if (defaultSchema != null)
            {
                commonContainer.Register(
                    Component.For<IAttributeDecorator>().ImplementedBy<StandardAttributeDecorator>()
                        .DependsOn(Dependency.OnValue("defaultSchema", defaultSchema))
                        .DependsOn(Dependency.OnValue("callingAssembly", callingAssembly))
                        );
            }
            else
            {
                commonContainer.Register(
                    Component.For<IAttributeDecorator>().ImplementedBy<StandardAttributeDecorator>()
                        .DependsOn(Dependency.OnValue("callingAssembly", callingAssembly))
                        );
            }

            return commonContainer;
        }

        private class DisposableContainer : IDisposable
        {
            public IWindsorContainer Container { get; }

            public bool IsDisposed { get; private set; }

            public DisposableContainer(IWindsorContainer container)
            {
                this.Container = container;
            }

            public void Dispose()
            {
                this.Container.Dispose();
                this.IsDisposed = true;
            }
        }

        public void Dispose()
        {
            this.sqlClientPopulatorContainer?.Dispose();
            this.memoryPopulatorContainer?.Dispose();
        }
    }
}
