/*
    Copyright 2016, 2017, 2018 Alexander Kuperman

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

namespace TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers
{
    internal static class EqualityHelper
    {
        public static bool Equals<TWrapper, TWrapped>(TWrapper wrapper, object obj)
            where TWrapper : class, IWrapper<TWrapped>
        {
            var toCompare = obj as TWrapper;

            if (toCompare == null)
                return false;

            if (wrapper.Wrapped == null && toCompare.Wrapped != null
                || wrapper.Wrapped != null && toCompare.Wrapped == null)
                return false;

            if (wrapper.Wrapped == null && toCompare.Wrapped == null)
                return wrapper == toCompare;

            return wrapper.Wrapped.Equals(toCompare.Wrapped);
        }
    }
}