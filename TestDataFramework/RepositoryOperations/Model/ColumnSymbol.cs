﻿/*
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

using System;

namespace TestDataFramework.RepositoryOperations.Model
{
    public class ColumnSymbol
    {
        public Type TableType { get; set; }
        public string ColumnName { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            string result = $"TableType: {this.TableType}, ColumnName: {this.ColumnName}, Value: {this.Value?.GetType()}";
            return result;
        }
    }
}