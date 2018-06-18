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

using log4net;
using TestDataFramework.Logger;
using TestDataFramework.RepositoryOperations.Model;

namespace TestDataFramework.ValueFormatter.Concrete
{
    public class SqlClientValueFormatter : DbValueFormatter
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(SqlClientValueFormatter));

        public override string Format(object value)
        {
            SqlClientValueFormatter.Logger.Debug("Entering Format.");

            var variable = value as Variable;

            if (variable != null)
            {
                SqlClientValueFormatter.Logger.Debug("Value is a Variable. Exiting Format.");
                return "@" + variable.Symbol;
            }

            SqlClientValueFormatter.Logger.Debug("Value is not a Variable. Calling base.Format.");
            string result = base.Format(value);

            SqlClientValueFormatter.Logger.Debug($"Exiting Format. Result: {result ?? "<Null>"}");
            return result ?? "null";
        }
    }
}