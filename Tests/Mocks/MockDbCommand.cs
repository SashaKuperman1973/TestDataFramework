/*
    Copyright 2016, 2017 Alexander Kuperman

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
using System.Data;
using System.Data.Common;

namespace Tests.Mocks
{
	[System.ComponentModel.DesignerCategory("Code")]
    public class MockDbCommand : DbCommand
    {
        private readonly DbCommand command;
        private readonly DbDataReader reader;

        public MockDbCommand(DbCommand command, DbDataReader reader)
        {
            this.command = command;
            this.reader = reader;
        }

        public override void Prepare()
        {
            throw new NotImplementedException();
        }

        public override string CommandText
        {
            get { return this.command.CommandText; }

            set { this.command.CommandText = value; }
        }

        public override CommandType CommandType
        {
            get { return this.command.CommandType; }

            set { this.command.CommandType = value; }
        }

        public override int CommandTimeout { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }
        protected override DbParameterCollection DbParameterCollection { get; }
        protected override DbTransaction DbTransaction { get; set; }
        public override bool DesignTimeVisible { get; set; }

        protected override DbConnection DbConnection
        {
            get { return this.Connection; }

            set { this.Connection = value; }
        }

        public new virtual DbConnection Connection { get; set; }

        public override void Cancel()
        {
            throw new NotImplementedException();
        }

        protected override DbParameter CreateDbParameter()
        {
            throw new NotImplementedException();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            return this.ExecuteReader();
        }

        public new virtual DbDataReader ExecuteReader()
        {
            return this.reader;
        }

        public override int ExecuteNonQuery()
        {
            throw new NotImplementedException();
        }

        public override object ExecuteScalar()
        {
            throw new NotImplementedException();
        }
    }
}
