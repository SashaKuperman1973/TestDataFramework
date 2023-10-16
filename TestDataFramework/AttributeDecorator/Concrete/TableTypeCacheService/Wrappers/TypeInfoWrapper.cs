/*
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TestDataFramework.Helpers;

namespace TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers
{
    public class TypeInfoWrapper : MemberInfoProxy, IWrapper<TypeInfo>
    {
        public readonly Guid Id = Guid.NewGuid();

        public TypeInfoWrapper(TypeInfo typeInfo) : base(typeInfo)
        {
            this.Wrapped = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));
        }

        public TypeInfoWrapper(Type type) : base(type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            this.Wrapped = type.GetTypeInfo();
        }

        public TypeInfoWrapper(): base(null)
        {
        }

        public TypeInfo Wrapped { get; }

        public virtual AssemblyWrapper Assembly => this.Wrapped == null
            ? new AssemblyWrapper()
            : new AssemblyWrapper(this.Wrapped.Assembly);

        public virtual Type Type => this.Wrapped;

        public Type ReflectedType => this.Wrapped.ReflectedType;

        public object[] GetCustomAttributes(bool inherit)
        {
            return this.Wrapped.GetCustomAttributes(inherit);
        }

        public bool IsDefined(Type attributeType, bool inherit)
        {
            return this.Wrapped.IsDefined(attributeType, inherit);
        }

        public object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return this.Wrapped.GetCustomAttributes(attributeType, inherit);
        }

        public PropertyInfoProxy GetField(string fieldName)
        {
            var fieldInfoProxy = new PropertyInfoProxy(this.Wrapped.GetField(fieldName));
            return fieldInfoProxy;
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

        public PropertyInfoProxy[] GetProperties()
        {
            PropertyInfo[] properties = this.Wrapped.GetProperties(BindingFlags.Instance | BindingFlags.Public |
                BindingFlags.GetProperty | BindingFlags.SetProperty);

            FieldInfo[] fields = this.Wrapped.GetFields(BindingFlags.Instance | BindingFlags.Public);

            IEnumerable<PropertyInfoProxy> propertyProxies =
                properties.Select(property => new PropertyInfoProxy(property));

            IEnumerable<PropertyInfoProxy> fieldProxies =
                fields.Select(field => new PropertyInfoProxy(field));

            PropertyInfoProxy[] result = propertyProxies.Concat(fieldProxies).ToArray();

            return result;
        }

        public PropertyInfoProxy GetProperty(string propertyNamne)
        {
            PropertyInfoProxy propertyInfoProxy;
            PropertyInfo propertyInfo = this.Wrapped.GetProperty(propertyNamne);
            if (propertyInfo != null)
            {
                propertyInfoProxy = new PropertyInfoProxy(propertyInfo);
            }
            else
            {
                FieldInfo fieldInfo = this.Wrapped.GetField(propertyNamne);
                propertyInfoProxy = new PropertyInfoProxy(fieldInfo);
            }

            return propertyInfoProxy;
        }
    }
}