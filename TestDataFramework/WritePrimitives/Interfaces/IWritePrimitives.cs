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
using System.Collections.Generic;
using TestDataFramework.RepositoryOperations.Model;

namespace TestDataFramework.WritePrimitives.Interfaces
{
    public interface IWritePrimitives
    {
        void Insert(string catalogueName, string schema, string tableName, IEnumerable<Column> columns);
        object SelectIdentity(string columnName);
        object[] Execute();
        void AddSqlCommand(string command);
        object WriteGuid(string columnName);
    }
}
