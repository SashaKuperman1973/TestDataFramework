using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Persistence.Concrete;
using TestDataFramework.Persistence.Interfaces;
using TestDataFramework.Populator.Interfaces;
using TestDataFramework.TypeGenerator.Interfaces;
using TestDataFramework.ValueGenerator.Interfaces;
using TestDataFramework.WritePrimitives;

namespace TestDataFramework.Populator.Concrete.DbClientPopulator
{
    public class StandardDbClientPopulator : StandardPopulator, IDbClientPopulator
    {
        private readonly SqlClientPersistence persistence;

        public StandardDbClientPopulator(ITypeGenerator typeGenerator, SqlClientPersistence persistence,
            IAttributeDecorator attributeDecorator, IHandledTypeGenerator handledTypeGenerator,
            IValueGenerator valueGenerator, ValueGuaranteePopulator valueGuaranteePopulator,
            IObjectGraphService objectGraphService, DeepCollectionSettingConverter deepCollectionSettingConverter
            ) :
            base(typeGenerator, persistence, attributeDecorator, handledTypeGenerator, valueGenerator,
                valueGuaranteePopulator, objectGraphService, deepCollectionSettingConverter)
        {
            this.persistence = persistence;
        }

        public virtual DbClientTransaction BindInATransaction(DbClientTransactionOptions options = null)
        {
            if (options == null)
            {
                options = new DbClientTransactionOptions();
            }

            var transaction = new DbClientTransaction(options);

            this.persistence.UseTransaction(transaction);

            this.Bind();

            return transaction;
        }
    }
}
