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
using TestDataFramework.ValueProvider;
using TestDataFramework.WritePrimitives;

namespace TestDataFramework.Factories
{
    public class PopulatorFactory : IDisposable
    {
        private DisposableContainer sqlClientPopulatorContainer;
        private DisposableContainer memoryPopulatorContainer;

        public IPopulator CreateSqlClientPopulator(string connectionStringWithDefaultCatalogue,
            bool mustBeInATransaction = true)
        {
            IWindsorContainer iocContainer = this.GetSqlClientPopulatorContainer(connectionStringWithDefaultCatalogue,
                mustBeInATransaction);

            var result = iocContainer.Resolve<IPopulator>();

            return result;
        }

        public IPopulator CreateMemoryPopulator()
        {
            IWindsorContainer iocContainer = this.GetMemoryPopulatorContainer();

            var result = iocContainer.Resolve<IPopulator>();

            return result;
        }

        private IWindsorContainer GetSqlClientPopulatorContainer(string connectionStringWithDefaultCatalogue, bool mustBeInATransaction)
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

                Component.For<IPropertyDataGenerator<ulong>>().ImplementedBy<SqlClientInitialCountGenerator>(),

                Component.For<IPersistence>().ImplementedBy<SqlClientPersistence>(),

                Component.For<IUniqueValueGenerator>().ImplementedBy<KeyTypeUniqueValueGenerator>(),

                Component.For<IWriterDictinary>().ImplementedBy<SqlWriterDictionary>(),

                Component.For<SqlWriterCommandTextGenerator>()

                );

            return this.sqlClientPopulatorContainer.Container;
        }

        private IWindsorContainer GetMemoryPopulatorContainer()
        {
            if (this.memoryPopulatorContainer != null && !this.memoryPopulatorContainer.IsDisposed)
            {
                return this.memoryPopulatorContainer.Container;
            }

            this.memoryPopulatorContainer = new DisposableContainer(PopulatorFactory.CommonContainer);

            this.memoryPopulatorContainer.Container.Register(

                Component.For<IPropertyDataGenerator<ulong>>().ImplementedBy<DefaultInitialValueGenerator>(),

                Component.For<IPersistence>().ImplementedBy<MemoryPersistence>(),

                Component.For<IUniqueValueGenerator>().ImplementedBy<MemoryUniqueValueGenerator>()

                );

            return this.memoryPopulatorContainer.Container;
        }

        private static IWindsorContainer CommonContainer
        {
            get
            {
                const string standardValueGenerator = "StandardValueGenerator";
                const string valueAccumulator = "ValueAccumulator";
                const string accumulatorValueGenerator = "AccumulatorValueGenerator";
                const string uniqueValueTypeGenerator = "UniqueValueTypeGenerator";
                const string getUniqueValueTypeGenerator = "GetUniqueValueTypeGenerator";
                const string standardTypeGenerator = "StandardTypeGenerator";
                const string getStandardTypeGenerator = "GetStandardTypeGenerator";
                const string standardValueProvider = "StandardValueProvider";

                var commonContainer = new WindsorContainer();

                commonContainer.Register(

                    #region Common Region

                    Component.For<IPopulator>().ImplementedBy<StandardPopulator>()
                        .DependsOn(ServiceOverride.ForKey<ITypeGenerator>().Eq(standardTypeGenerator)),

                    Component.For<ITypeGenerator>()
                        .ImplementedBy<StandardTypeGenerator>()
                        .DependsOn(ServiceOverride.ForKey<IValueGenerator>().Eq(standardValueGenerator))
                        .Named(standardTypeGenerator),

                    Component.For<IValueGenerator>()
                        .ImplementedBy<StandardValueGenerator>()
                        .DependsOn(ServiceOverride.ForKey<IValueProvider>().Eq(standardValueProvider))
                        .DependsOn(
                            ServiceOverride.ForKey<StandardValueGenerator.GetTypeGeneratorDelegate>()
                                .Eq(getStandardTypeGenerator))
                        .Named(standardValueGenerator),

                    Component.For<StandardValueGenerator.GetTypeGeneratorDelegate>()
                        .Instance(() => commonContainer.Resolve<ITypeGenerator>(standardTypeGenerator))
                        .Named(getStandardTypeGenerator),

                    Component.For<Random>().ImplementedBy<Random>(),

                    Component.For<DateTimeProvider>().Instance(() => Helper.Now),

                    Component.For<Func<IArrayRandomizer>>()
                        .Instance(() => commonContainer.Resolve<IArrayRandomizer>(standardValueGenerator)),

                    Component.For<IArrayRandomizer>()
                        .ImplementedBy<StandardArrayRandomizer>()
                        .DependsOn(
                            ServiceOverride.ForKey<IValueGenerator>().Eq(standardValueGenerator)),

                    Component.For<LetterEncoder>(),

                    Component.For<IPropertyValueAccumulator>()
                        .ImplementedBy<StandardPropertyValueAccumulator>(),

                    Component.For<IDeferredValueGenerator<ulong>>()
                        .ImplementedBy<StandardDeferredValueGenerator<ulong>>(),

                    Component.For<IValueProvider>().ImplementedBy<StandardRandomizer>()
                    .DependsOn(new { dateTimeMinValue = SqlDateTime.MinValue.Value.Ticks, dateTimeMaxValue = SqlDateTime.MaxValue.Value.Ticks })
                    .Named(standardValueProvider),

                    Component.For<IRandomSymbolStringGenerator>().ImplementedBy<RandomSymbolStringGenerator>(),

                    #endregion Common Region

                    #region Handled Type Generator

                    Component.For<IHandledTypeGenerator>()
                        .ImplementedBy<StandardHandledTypeGenerator>()
                        .DependsOn(
                            ServiceOverride.ForKey<IValueGenerator>().Eq(standardValueGenerator)),

                    Component.For<StandardHandledTypeGenerator.CreateAccumulatorValueGeneratorDelegate>()
                        .Instance(() => commonContainer.Resolve<IValueGenerator>(accumulatorValueGenerator)),

                    Component.For<IValueGenerator>()
                        .ImplementedBy<StandardValueGenerator>()
                        .DependsOn(ServiceOverride.ForKey<IValueProvider>().Eq(valueAccumulator))
                        .DependsOn(
                            ServiceOverride.ForKey<StandardValueGenerator.GetTypeGeneratorDelegate>()
                                .Eq(getUniqueValueTypeGenerator))
                        .Named(accumulatorValueGenerator)
                        .LifestyleTransient(),

                    Component.For<IValueProvider>().ImplementedBy<AccumulatorValueProvider>().Named(valueAccumulator)
                        .LifestyleTransient(),

                    Component.For<StandardValueGenerator.GetTypeGeneratorDelegate>()
                        .Instance(() => commonContainer.Resolve<ITypeGenerator>(uniqueValueTypeGenerator))
                        .Named(getUniqueValueTypeGenerator),

                    Component.For<ITypeGenerator>()
                        .ImplementedBy<UniqueValueTypeGenerator>().Named(uniqueValueTypeGenerator),

                    Component.For<UniqueValueTypeGenerator.GetAccumulatorValueGenerator>()
                        .Instance(typeGenerator => commonContainer.Resolve<IValueGenerator>(accumulatorValueGenerator))

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
