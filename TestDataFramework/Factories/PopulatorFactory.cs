using System;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using TestDataFramework.ArrayRandomizer;
using TestDataFramework.DeferredValueGenerator.Concrete;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.Helpers;
using TestDataFramework.Helpers.Concrete;
using TestDataFramework.Helpers.Interfaces;
using TestDataFramework.Persistence;
using TestDataFramework.Populator;
using TestDataFramework.PropertyValueAccumulator;
using TestDataFramework.TypeGenerator;
using TestDataFramework.UniqueValueGenerator;
using TestDataFramework.UniqueValueGenerator.Concrete;
using TestDataFramework.UniqueValueGenerator.Interface;
using TestDataFramework.ValueFormatter;
using TestDataFramework.ValueGenerator;
using TestDataFramework.ValueGenerator.Concrete;
using TestDataFramework.ValueGenerator.Interface;
using TestDataFramework.ValueProvider;
using TestDataFramework.WritePrimitives;

namespace TestDataFramework.Factories
{
    public class PopulatorFactory : IDisposable
    {
        private DisposableContainer sqlClientPopulatorContainer;
        private DisposableContainer memoryPopulatorContainer;

        private const string GetStandardTypeGenerator = "GetStandardTypeGenerator";
        private const string StandardValueProvider = "StandardValueProvider";
        private const string StandardValueGenerator = "StandardValueGenerator";
        private const string ValueAccumulator = "ValueAccumulator";
        private const string AccumulatorValueGenerator = "AccumulatorValueGenerator";
        private const string GetUniqueValueTypeGenerator = "GetUniqueValueTypeGenerator";

        public IPopulator CreateSqlClientPopulator(string connectionStringWithDefaultCatalogue,
            bool mustBeInATransaction = true, bool throwIfUnhandledPrimaryKeyType = true)
        {
            IWindsorContainer iocContainer = this.GetSqlClientPopulatorContainer(connectionStringWithDefaultCatalogue,
                mustBeInATransaction, throwIfUnhandledPrimaryKeyType);

            var result = iocContainer.Resolve<IPopulator>();

            return result;
        }

        public IPopulator CreateMemoryPopulator(bool throwIfUnhandledPrimaryKeyType = false)
        {
            IWindsorContainer iocContainer = this.GetMemoryPopulatorContainer(throwIfUnhandledPrimaryKeyType);

            var result = iocContainer.Resolve<IPopulator>();

            return result;
        }

        private IWindsorContainer GetSqlClientPopulatorContainer(string connectionStringWithDefaultCatalogue, bool mustBeInATransaction, bool throwIfUnhandledPrimaryKeyType)
        {
            if (this.sqlClientPopulatorContainer != null && !this.sqlClientPopulatorContainer.IsDisposed)
            {
                return this.sqlClientPopulatorContainer.Container;
            }

            this.sqlClientPopulatorContainer = new DisposableContainer(PopulatorFactory.CommonContainer);

            this.sqlClientPopulatorContainer.Container.Register(

                Component.For<IWritePrimitives>().ImplementedBy<SqlClientWritePrimitives>()
                    .DependsOn(new { connectionStringWithDefaultCatalogue, mustBeInATransaction, configuration = ConfigurationManager.AppSettings }),

                Component.For<DbProviderFactory>().UsingFactoryMethod(() => SqlClientFactory.Instance, true),

                Component.For<IValueFormatter>().ImplementedBy<SqlClientValueFormatter>(),

                Component.For<IPropertyDataGenerator<LargeInteger>>().ImplementedBy<SqlClientInitialCountGenerator>(),

                Component.For<IPersistence>().ImplementedBy<SqlClientPersistence>(),

                Component.For<IUniqueValueGenerator>().ImplementedBy<KeyTypeUniqueValueGenerator>().DependsOn(new { throwIfUnhandledType = throwIfUnhandledPrimaryKeyType }),

                Component.For<IWriterDictinary>().ImplementedBy<SqlWriterDictionary>(),

                Component.For<SqlWriterCommandTextGenerator>(),

                Component.For<IValueGenerator>()
                    .ImplementedBy<SqlClientValueGenerator>()
                    .DependsOn(ServiceOverride.ForKey<IValueProvider>().Eq(PopulatorFactory.ValueAccumulator))
                    .DependsOn(
                        ServiceOverride.ForKey<BaseValueGenerator.GetTypeGeneratorDelegate>()
                            .Eq(PopulatorFactory.GetUniqueValueTypeGenerator))
                    .Named(PopulatorFactory.AccumulatorValueGenerator)
                    .LifestyleTransient(),

                Component.For<IValueGenerator>()
                    .ImplementedBy<SqlClientValueGenerator>()
                    .DependsOn(ServiceOverride.ForKey<IValueProvider>().Eq(PopulatorFactory.StandardValueProvider))
                    .DependsOn(
                        ServiceOverride.ForKey<BaseValueGenerator.GetTypeGeneratorDelegate>()
                            .Eq(PopulatorFactory.GetStandardTypeGenerator))
                    .Named(PopulatorFactory.StandardValueGenerator)
                );

            return this.sqlClientPopulatorContainer.Container;
        }

        private IWindsorContainer GetMemoryPopulatorContainer(bool throwIfUnhandledPrimaryKeyType)
        {
            if (this.memoryPopulatorContainer != null && !this.memoryPopulatorContainer.IsDisposed)
            {
                return this.memoryPopulatorContainer.Container;
            }

            this.memoryPopulatorContainer = new DisposableContainer(PopulatorFactory.CommonContainer);

            this.memoryPopulatorContainer.Container.Register(

                Component.For<IPropertyDataGenerator<LargeInteger>>().ImplementedBy<DefaultInitialCountGenerator>(),

                Component.For<IPersistence>().ImplementedBy<MemoryPersistence>(),

                Component.For<IUniqueValueGenerator>().ImplementedBy<MemoryUniqueValueGenerator>().DependsOn(new { throwIfUnhandledType = throwIfUnhandledPrimaryKeyType }),

                Component.For<IValueGenerator>()
                    .ImplementedBy<MemoryValueGenerator>()
                    .DependsOn(ServiceOverride.ForKey<IValueProvider>().Eq(PopulatorFactory.ValueAccumulator))
                    .DependsOn(
                        ServiceOverride.ForKey<BaseValueGenerator.GetTypeGeneratorDelegate>()
                            .Eq(PopulatorFactory.GetUniqueValueTypeGenerator))
                    .Named(PopulatorFactory.AccumulatorValueGenerator)
                    .LifestyleTransient(),

                Component.For<IValueGenerator>()
                    .ImplementedBy<MemoryValueGenerator>()
                    .DependsOn(ServiceOverride.ForKey<IValueProvider>().Eq(PopulatorFactory.StandardValueProvider))
                    .DependsOn(
                        ServiceOverride.ForKey<BaseValueGenerator.GetTypeGeneratorDelegate>()
                            .Eq(PopulatorFactory.GetStandardTypeGenerator))
                    .Named(PopulatorFactory.StandardValueGenerator)

                );

            return this.memoryPopulatorContainer.Container;
        }

        private static IWindsorContainer CommonContainer
        {
            get
            {
                const string uniqueValueTypeGenerator = "UniqueValueTypeGenerator";
                const string standardTypeGenerator = "StandardTypeGenerator";
                const string standardHandledTypeGenerator = "StandardHandledTypeGenerator";
                const string accumulatorValueGenerator_StandardHandledTypeGenerator = "AccumulatorValueGenerator_StandardHandledTypeGenerator ";                

                var commonContainer = new WindsorContainer();

                commonContainer.Register(

                    #region Common Region

                    Component.For<IPopulator>().ImplementedBy<StandardPopulator>()
                        .DependsOn(ServiceOverride.ForKey<ITypeGenerator>().Eq(standardTypeGenerator)),

                    Component.For<ITypeGenerator>()
                        .ImplementedBy<StandardTypeGenerator>()
                        .DependsOn(ServiceOverride.ForKey<IValueGenerator>().Eq(PopulatorFactory.StandardValueGenerator))
                        .DependsOn(ServiceOverride.ForKey<IHandledTypeGenerator>().Eq(standardHandledTypeGenerator))
                        .Named(standardTypeGenerator),

                    Component.For<BaseValueGenerator.GetTypeGeneratorDelegate>()
                        .Instance(() => commonContainer.Resolve<ITypeGenerator>(standardTypeGenerator))
                        .Named(PopulatorFactory.GetStandardTypeGenerator),

                    Component.For<Random>().ImplementedBy<Random>(),

                    Component.For<DateTimeProvider>().Instance(() => Helper.Now),

                    Component.For<Func<IArrayRandomizer>>()
                        .Instance(() => commonContainer.Resolve<IArrayRandomizer>(StandardValueGenerator)),

                    Component.For<IArrayRandomizer>()
                        .ImplementedBy<StandardArrayRandomizer>()
                        .DependsOn(
                            ServiceOverride.ForKey<IValueGenerator>().Eq(StandardValueGenerator)),

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

                    Component.For<BaseValueGenerator.GetTypeGeneratorDelegate>()
                        .Instance(() => commonContainer.Resolve<ITypeGenerator>(uniqueValueTypeGenerator))
                        .Named(PopulatorFactory.GetUniqueValueTypeGenerator),

                    Component.For<ITypeGenerator>()
                        .ImplementedBy<UniqueValueTypeGenerator>()
                        .DependsOn(ServiceOverride.ForKey<IHandledTypeGenerator>().Eq(accumulatorValueGenerator_StandardHandledTypeGenerator))
                        .Named(uniqueValueTypeGenerator),

                    Component.For<UniqueValueTypeGenerator.GetAccumulatorValueGenerator>()
                        .Instance(
                            typeGenerator =>
                                commonContainer.Resolve<IValueGenerator>(PopulatorFactory.AccumulatorValueGenerator))

                    #endregion Handled Type Generator

                    );

                return commonContainer;
            }
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
