using System;
using System.Data.Common;
using System.Data.SqlClient;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using TestDataFramework.ArrayRandomizer;
using TestDataFramework.Helpers;
using TestDataFramework.Helpers.Concrete;
using TestDataFramework.Helpers.Interfaces;
using TestDataFramework.Persistence;
using TestDataFramework.Populator;
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
            this.sqlServerPopulatorContainer?.Dispose();
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

                    Component.For<IRandomizer>().ImplementedBy<StandardRandomizer>(),

                    Component.For<DateTimeProvider>().Instance(() => Helper.Now),

                    Component.For<Func<IValueGenerator, IArrayRandomizer>>()
                        .Instance(valueGenerator => commonContainer.Resolve<IArrayRandomizer>(new {valueGenerator})),

                    Component.For<IArrayRandomizer>().ImplementedBy<StandardArrayRandomizer>(),

                    Component.For<IUniqueValueGenerator>().ImplementedBy<StandardUniqueValueGenerator>(),

                    Component.For<StringGenerator>()

                    );

                return commonContainer;
            }
        }

        private DisposableContainer sqlServerPopulatorContainer;
        private IWindsorContainer GetSqlServerPopulatorContainer(string connectionStringWithDefaultCatalogue, bool mustBeInATransaction)
        {
            if (this.sqlServerPopulatorContainer != null && !this.sqlServerPopulatorContainer.IsDisposed)
            {
                return this.sqlServerPopulatorContainer.Container;
            }

            this.sqlServerPopulatorContainer = new DisposableContainer(PopulatorFactory.CommonContainer);

            this.sqlServerPopulatorContainer.Container.Register(
                Component.For<IPersistence>().ImplementedBy<StandardPersistence>(),

                Component.For<IWritePrimitives>().ImplementedBy<DbProviderWritePrimitives>()
                .DependsOn((k, d) =>
                {
                    d["connectionStringWithDefaultCatalogue"] = connectionStringWithDefaultCatalogue;
                    d["mustBeInATransaction"] = mustBeInATransaction;
                }),

                Component.For<DbProviderFactory>().UsingFactoryMethod(() => SqlClientFactory.Instance, true),

                Component.For<IValueFormatter>().ImplementedBy<InsertStatementValueFormatter>(),

                Component.For<IRandomSymbolStringGenerator>().ImplementedBy<RandomSymbolStringGenerator>()
                );

            return this.sqlServerPopulatorContainer.Container;
        }

        public IPopulator CreateSqlClientPopulator(string connectionStringWithDefaultCatalogue,
            bool mustBeInATransaction = true)
        {
            IWindsorContainer iocContainer = this.GetSqlServerPopulatorContainer(connectionStringWithDefaultCatalogue,
                mustBeInATransaction);

            var result = iocContainer.Resolve<IPopulator>();

            return result;
        }
    }
}
