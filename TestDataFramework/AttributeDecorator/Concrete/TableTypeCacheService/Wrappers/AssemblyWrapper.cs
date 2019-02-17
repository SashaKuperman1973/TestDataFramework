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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers
{
    public class AssemblyWrapper : IWrapper<Assembly>
    {
        private readonly Guid id = Guid.NewGuid();

        public AssemblyWrapper(Assembly assembly)
        {
            this.Wrapped = assembly ?? throw new ArgumentNullException(nameof(assembly));
        }

        public AssemblyWrapper()
        {
        }

        public virtual IEnumerable<TypeInfoWrapper> DefinedTypes => this.Wrapped.DefinedTypes
            .Select(typeInfo => new TypeInfoWrapper(typeInfo));

        public Assembly Wrapped { get; }

        public virtual AssemblyNameWrapper[] GetReferencedAssemblies()
        {
            return this.Wrapped.GetReferencedAssemblies()
                .Select(assembly => new AssemblyNameWrapper(assembly)).ToArray();
        }

        public virtual AssemblyNameWrapper GetName()
        {
            return new AssemblyNameWrapper(this.Wrapped.GetName());
        }

        public override bool Equals(object obj)
        {
            bool result = EqualityHelper.Equals<Assembly>(this, obj as AssemblyWrapper);
            return result;
        }

        public override int GetHashCode()
        {
            return this.Wrapped?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return this.Wrapped?.ToString() ?? $"Empty Assembly Wrapper. ID: {this.id}";
        }
    }
}