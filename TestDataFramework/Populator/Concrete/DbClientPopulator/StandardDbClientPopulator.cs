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

using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Persistence.Concrete;
using TestDataFramework.Populator.Interfaces;
using TestDataFramework.TypeGenerator.Interfaces;
using TestDataFramework.ValueGenerator.Interfaces;

namespace TestDataFramework.Populator.Concrete.DbClientPopulator
{
    public class StandardDbClientPopulator : StandardPopulator, IDbClientPopulator
    {
        private readonly SqlClientPersistence persistence;
        private DbClientTransactionOptions transactionOptions;

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

        public IDbClientTransaction BindInATransaction()
        {
            if (this.transactionOptions == null)
            {
                this.transactionOptions = new DbClientTransactionOptions();
            }

            var transaction = new DbClientTransaction(this.transactionOptions);

            this.persistence.UseTransaction(transaction);

            this.Bind();

            return transaction;
        }

        public void SetTransationOptions(DbClientTransactionOptions options)
        {
            this.transactionOptions = options;
        }
    }
}
