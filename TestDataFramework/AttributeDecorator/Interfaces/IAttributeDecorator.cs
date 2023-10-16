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
using System.Linq.Expressions;
using System.Reflection;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using TestDataFramework.Helpers;
using TestDataFramework.RepositoryOperations.Model;
using PropertyAttributes = TestDataFramework.RepositoryOperations.Model.PropertyAttributes;

namespace TestDataFramework.AttributeDecorator.Interfaces
{
    public interface IAttributeDecorator
    {
        T GetSingleAttribute<T>(MemberInfoProxy memberInfo) where T : Attribute;

        IEnumerable<T> GetUniqueAttributes<T>(Type type) where T : Attribute;

        PropertyAttribute<T> GetPropertyAttribute<T>(PropertyInfoProxy propertyInfo) where T : Attribute;

        IEnumerable<PropertyAttribute<T>> GetPropertyAttributes<T>(Type type) where T : Attribute;

        IEnumerable<PropertyAttributes> GetPropertyAttributes(Type type);

        IEnumerable<T> GetCustomAttributes<T>(MemberInfoProxy memberInfo) where T : Attribute;

        T GetCustomAttribute<T>(MemberInfoProxy memberInfo) where T : Attribute;

        IEnumerable<Attribute> GetCustomAttributes(MemberInfoProxy memberInfo);

        void DecorateMember<T, TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression, Attribute attribute);

        void DecorateType(Type type, Attribute attribute);

        Type GetPrimaryTableType(ForeignKeyAttribute foreignAttribute, TypeInfoWrapper foreignType);
    }
}