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

using System;
using System.Collections.Generic;
using System.Linq;
using TestDataFramework.Populator.Concrete;

namespace CommonIntegrationTests
{
    public static class Helpers
    {
        public static void Dump<T>(IEnumerable<RecordReference<T>> recordReference)
        {
            recordReference.ToList().ForEach(r => Helpers.Dump(r.RecordObject));
        }

        public static void Dump(object target)
        {
            Console.WriteLine();
            Console.WriteLine(target.GetType().Name);
            target.GetType().GetProperties().ToList().ForEach(p =>
            {
                try
                {
                    Console.WriteLine(p.Name + " = " + p.GetValue(target));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(p.Name + ": " + Helpers.GetMessage(ex));
                }
            });

            Console.WriteLine();
        }

        private static string GetMessage(Exception ex)
        {
            if (ex.InnerException != null)
            {
                return Helpers.GetMessage(ex.InnerException);
            }

            return ex.Message;
        }
    }
}
