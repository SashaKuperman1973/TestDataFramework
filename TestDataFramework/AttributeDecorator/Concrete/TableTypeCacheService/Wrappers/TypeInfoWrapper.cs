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
using System.Reflection;

namespace TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers
{
    public class TypeInfoWrapper : MemberInfo, IWrapper<TypeInfo>
    {
        public readonly Guid Id = Guid.NewGuid();

        public TypeInfoWrapper(TypeInfo typeInfo)
        {
            this.Wrapped = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));
        }

        public TypeInfoWrapper(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            this.Wrapped = type.GetTypeInfo();
        }

        public TypeInfoWrapper()
        {
        }

        public TypeInfo Wrapped { get; }

        public virtual AssemblyWrapper Assembly => this.Wrapped == null
            ? new AssemblyWrapper()
            : new AssemblyWrapper(this.Wrapped.Assembly);

        public virtual Type Type => this.Wrapped;

        public override MemberTypes MemberType => this.Wrapped.MemberType;

        public override Type DeclaringType => this.Wrapped.DeclaringType;

        public override Type ReflectedType => this.Wrapped.ReflectedType;

        public override string Name => this.Wrapped.Name;

        public override object[] GetCustomAttributes(bool inherit)
        {
            return this.Wrapped.GetCustomAttributes(inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return this.Wrapped.IsDefined(attributeType, inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return this.Wrapped.GetCustomAttributes(attributeType, inherit);
        }

        public override string ToString()
        {
            return this.Wrapped?.ToString() ?? $"Empty TypeInfo Wrapper. ID: {this.Id}";
        }

        public override bool Equals(object obj)
        {
            TypeInfoWrapper toCompare = null;

            var typeInfo = obj as TypeInfo;
            if (typeInfo != null)
            {
                toCompare = new TypeInfoWrapper(typeInfo);
            }

            bool result = EqualityHelper.Equals<TypeInfo>(this, toCompare ?? obj as TypeInfoWrapper);
            return result;
        }

        public override int GetHashCode()
        {
            int result = this.Wrapped == null ? 0 : this.Wrapped.GetHashCode();
            return result;
        }
    }
}