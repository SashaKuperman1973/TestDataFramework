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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using log4net;
using TestDataFramework.Logger;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.TypeGenerator.Interfaces;

namespace TestDataFramework.Populator.Concrete
{
    public class RecordReference<T> : RecordReference
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof (RecordReference<T>));

        protected internal readonly ConcurrentDictionary<PropertyInfo, Action<T>> ExplicitProperySetters =
            new ConcurrentDictionary<PropertyInfo, Action<T>>(
                RecordReference<T>.ExplicitPropertySetterEqualityComparerObject);

        private static readonly ExplicitPropertySetterEqualityComparer ExplicitPropertySetterEqualityComparerObject =
            new ExplicitPropertySetterEqualityComparer();

        private class ExplicitPropertySetterEqualityComparer : IEqualityComparer<PropertyInfo>
        {
            public bool Equals(PropertyInfo x, PropertyInfo y)
            {
                bool result = x.Name.Equals(y.Name, StringComparison.Ordinal);
                return result;
            }

            public int GetHashCode(PropertyInfo obj)
            {
                int result = obj.Name.GetHashCode();
                return result;
            }
        }

        public RecordReference(ITypeGenerator typeGenerator, IAttributeDecorator attributeDecorator) : base(typeGenerator, attributeDecorator)
        {
            RecordReference<T>.Logger.Debug($"Entering constructor. T: {typeof(T)}");

            this.RecordType = typeof (T);

            RecordReference<T>.Logger.Debug("Exiting constructor");
        }

        public new virtual T RecordObject => (T) (base.RecordObject ?? default(T));

        protected internal override bool IsExplicitlySet(PropertyInfo propertyInfo)
        {
            RecordReference<T>.Logger.Debug($"Entering IsExplicitlySet. propertyInfo: {propertyInfo}");

            bool result = this.ExplicitProperySetters.ContainsKey(propertyInfo);

            RecordReference<T>.Logger.Debug("Exiting IsExplicitlySet");
            return result;
        }

        protected internal override void Populate()
        {
            RecordReference<T>.Logger.Debug("Entering Populate");

            base.RecordObject = this.TypeGenerator.GetObject<T>(this.ExplicitProperySetters);

            RecordReference<T>.Logger.Debug("Exiting Populate");
        }

        public virtual RecordReference<T> Set<TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression,
            TPropertyType value)
        {
            RecordReference<T>.Logger.Debug(
                $"Entering Set(fieldExpression, value). TPropertyType: {typeof(TPropertyType)}, fieldExpression: {fieldExpression}, value: {value}");

            RecordReference<T> result = this.Set(fieldExpression, () => value);

            RecordReference<T>.Logger.Debug("Exiting Set(fieldExpression, value)");
            return result;
        }

        public virtual RecordReference<T> Set<TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression, Func<TPropertyType> valueFactory)
        {
            RecordReference<T>.Logger.Debug(
                $"Entering Set(fieldExpression, valueFactory). TPropertyType: {typeof(TPropertyType)}, fieldExpression: {fieldExpression}, valueFactory: {valueFactory}");

            var propertyInfo = Helper.ValidateFieldExpression(fieldExpression) as PropertyInfo;

            if (propertyInfo == null)
            {
                throw new SetExpressionException(Messages.MustBePropertyAccess);
            }

            void Setter(T @object) => propertyInfo.SetValue(@object, valueFactory());

            this.ExplicitProperySetters.AddOrUpdate(propertyInfo, Setter, (pi, lambda) =>
            {
                RecordReference<T>.Logger.Debug("Updatng explicitProperySetters dictionary");
                return Setter;
            });

            RecordReference<T>.Logger.Debug("Exiting Set(fieldExpression, valueFactory)");
            return this;
        }

        public virtual RecordReference<T> SetRange<TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression,
            IEnumerable<TPropertyType> range)
        {
            RecordReference<T>.Logger.Debug(
                $"Entering SetRange(fieldExpression, range). TPropertyType: {typeof(TPropertyType)}, fieldExpression: {fieldExpression}, valueFactory: {range}");

            RecordReference<T> result = this.SetRange(fieldExpression, () => range);

            RecordReference<T>.Logger.Debug("Exiting Set(fieldExpression, value)");
            return result;
        }

        public virtual RecordReference<T> SetRange<TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression,
            Func<IEnumerable<TPropertyType>> rangeFactory)
        {
            RecordReference<T>.Logger.Debug(
                $"Entering SetRange(fieldExpression, rangeFactory). TPropertyType: {typeof(TPropertyType)}, fieldExpression: {fieldExpression}, valueFactory: {rangeFactory}");

            var propertyInfo = Helper.ValidateFieldExpression(fieldExpression) as PropertyInfo;

            if (propertyInfo == null)
            {
                throw new SetExpressionException(Messages.MustBePropertyAccess);
            }

            void Setter(T @object) => propertyInfo.SetValue(@object, RecordReference<T>.ChooseElementInRange(rangeFactory()));

            this.ExplicitProperySetters.AddOrUpdate(propertyInfo, Setter, (pi, lambda) =>
            {
                RecordReference<T>.Logger.Debug("Updatng explicitProperySetters dictionary");
                return Setter;
            });

            RecordReference<T>.Logger.Debug("Exiting SetRange(fieldExpression, rangeFactory)");
            return this;
        }

        private static TPropertyType ChooseElementInRange<TPropertyType>(IEnumerable<TPropertyType> elements)
        {
            elements = elements.ToList();
            int index = new Random().Next(elements.Count());
            TPropertyType result = elements.ElementAt(index);
            return result;
        }
    }
}
