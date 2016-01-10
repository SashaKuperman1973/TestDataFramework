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
using TestDataFramework.Helpers;
using TestDataFramework.Helpers.Concrete;
using TestDataFramework.Helpers.Interfaces;
using TestDataFramework.Persistence;
using TestDataFramework.Populator;
using TestDataFramework.PropertyValueAccumulator;
using TestDataFramework.Randomizer;
using TestDataFramework.TypeGenerator;
using TestDataFramework.UniqueValueGenerator;
using TestDataFramework.ValueFormatter;
using TestDataFramework.ValueGenerator;
using TestDataFramework.WritePrimitives;

namespace TestDataFramework.Factories
{
    public class PopulatorFactory : IDisposable
    {
        private DisposableContainer SqlClientPopulatorContainer;
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
            if (this.SqlClientPopulatorContainer != null && !this.SqlClientPopulatorContainer.IsDisposed)
            {
                return this.SqlClientPopulatorContainer.Container;
            }

            this.SqlClientPopulatorContainer = new DisposableContainer(PopulatorFactory.CommonContainer);

            this.SqlClientPopulatorContainer.Container.Register(

                Component.For<IWritePrimitives>().ImplementedBy<DbProviderWritePrimitives>()
                    .DependsOn((k, d) =>
                    {
                        d["connectionStringWithDefaultCatalogue"] = connectionStringWithDefaultCatalogue;
                        d["mustBeInATransaction"] = mustBeInATransaction;
                        d["configuration"] = ConfigurationManager.AppSettings;
                    }),

                Component.For<DbProviderFactory>().UsingFactoryMethod(() => SqlClientFactory.Instance, true),

                Component.For<IValueFormatter>().ImplementedBy<InsertStatementValueFormatter>(),

                Component.For<IPropertyDataGenerator<ulong>>().ImplementedBy<DbProviderDeferredValueGenerator<ulong>>()
                    .DependsOn((k, d) =>
                    {
                        d["connectionString"] = connectionStringWithDefaultCatalogue;
                    }),

                Component.For<IHandlerDictionary<ulong>>().ImplementedBy<HandlerDictionary<ulong>>(),

                Component.For<IDeferredValueGeneratorHandler<ulong>>().ImplementedBy<SqlClientInitialCountGenerator>(),

                Component.For<IPersistence>().ImplementedBy<SqlClientPersistence>()

                );

            return this.SqlClientPopulatorContainer.Container;
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

                Component.For<IPersistence>().ImplementedBy<MemoryPersistence>()

                );

            return this.memoryPopulatorContainer.Container;
        }

        private static IWindsorContainer CommonContainer
        {
            get
            {
                var commonContainer = new WindsorContainer();

                commonContainer.Register(
                    Component.For<IPopulator>().ImplementedBy<StandardPopulator>(),

                    Component.For<ITypeGenerator>()
                        .ImplementedBy<StandardTypeGenerator>(),

                    Component.For<Func<ITypeGenerator, IValueGenerator>>()
                        .Instance(typeGenerator => commonContainer.Resolve<IValueGenerator>(new {typeGenerator})),

                    Component.For<Random>(),

                    Component.For<IValueGenerator>()
                        .ImplementedBy<StandardValueGenerator>(),

                    Component.For<DateTimeProvider>().Instance(() => Helper.Now),

                    Component.For<Func<IValueGenerator, IArrayRandomizer>>()
                        .Instance(valueGenerator => commonContainer.Resolve<IArrayRandomizer>(new {valueGenerator})),

                    Component.For<IArrayRandomizer>().ImplementedBy<StandardArrayRandomizer>(),

                    Component.For<IUniqueValueGenerator>().ImplementedBy<StandardUniqueValueGenerator>(),

                    Component.For<StringGenerator>(),

                    Component.For<IPropertyValueAccumulator>().ImplementedBy<StandardPropertyValueAccumulator>(),

                    Component.For<IDeferredValueGenerator<ulong>>()
                        .ImplementedBy<StandardDeferredValueGenerator<ulong>>(),

                    Component.For<IRandomizer>().ImplementedBy<StandardRandomizer>().DependsOn((k, d) =>
                    {
                        d["dateTimeMinValue"] = SqlDateTime.MinValue.Value.Ticks;
                        d["dateTimeMaxValue"] = SqlDateTime.MaxValue.Value.Ticks;
                    }),

                    Component.For<IRandomSymbolStringGenerator>().ImplementedBy<RandomSymbolStringGenerator>()

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
            this.SqlClientPopulatorContainer?.Dispose();
            this.memoryPopulatorContainer?.Dispose();
        }
    }
}
