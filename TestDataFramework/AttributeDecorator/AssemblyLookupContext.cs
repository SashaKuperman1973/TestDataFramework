/*
    Copyright 2016, 2017, 2018, 2019 Alexander Kuperman

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

using System.Collections.Concurrent;
using System.Collections.Generic;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;

namespace TestDataFramework.AttributeDecorator
{
    public class AssemblyLookupContext
    {
        public AssemblyLookupContext()
        {
            this.TypeDictionaryEqualityComparer = new TypeDictionaryEqualityComparer();
            this.CollisionDictionary =
                new ConcurrentDictionary<Table, List<TypeInfoWrapper>>(this.TypeDictionaryEqualityComparer);
            this.TypeDictionary = new ConcurrentDictionary<Table, TypeInfoWrapper>(this.TypeDictionaryEqualityComparer);
        }

        public ConcurrentDictionary<Table, TypeInfoWrapper> TypeDictionary { get; }
        public ConcurrentDictionary<Table, List<TypeInfoWrapper>> CollisionDictionary { get; }
        public TypeDictionaryEqualityComparer TypeDictionaryEqualityComparer { get; }
    }
}