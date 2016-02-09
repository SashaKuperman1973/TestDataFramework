﻿/*
    Copyright 2016 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
*/
using TestDataFramework.RepositoryOperations.Model;

namespace TestDataFramework.ValueFormatter.Concrete
{
    public class SqlClientValueFormatter : DbValueFormatter
    {
        public override string Format(object value)
        {
            var variable = value as Variable;

            if (variable != null)
            {
                return "@" + variable.Symbol;
            }

            string result = base.Format(value);

            return result ?? "null";
        }
    }
}