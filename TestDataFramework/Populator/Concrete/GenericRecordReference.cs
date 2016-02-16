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
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using log4net;
using TestDataFramework.Exceptions;
using TestDataFramework.TypeGenerator.Interfaces;

namespace TestDataFramework.Populator.Concrete
{
    public class RecordReference<T> : RecordReference
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (RecordReference<T>));

        private readonly ConcurrentDictionary<PropertyInfo, Action<T>> explicitProperySetters =
            new ConcurrentDictionary<PropertyInfo, Action<T>>();

        public RecordReference(ITypeGenerator typeGenerator) : base(typeGenerator)
        {
            RecordReference<T>.Logger.Debug($"Entering constructor. T: {typeof(T)}");

            this.RecordType = typeof (T);

            RecordReference<T>.Logger.Debug("Exiting constructor");
        }

        public new T RecordObject => (T) base.RecordObject;

        public override bool IsExplicitlySet(PropertyInfo propertyInfo)
        {
            RecordReference<T>.Logger.Debug($"Entering IsExplicitlySet. propertyInfo: {propertyInfo}");

            bool result = this.explicitProperySetters.ContainsKey(propertyInfo);

            RecordReference<T>.Logger.Debug("Exiting IsExplicitlySet");
            return result;
        }

        public override void Populate()
        {
            RecordReference<T>.Logger.Debug("Entering Populate");

            base.RecordObject = this.TypeGenerator.GetObject<T>(this.explicitProperySetters);

            RecordReference<T>.Logger.Debug("Exiting Populate");
        }

        public virtual RecordReference<T> Set<TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression, TPropertyType value)
        {
            RecordReference<T>.Logger.Debug(
                $"Entering Set. TPropertyType: {typeof (TPropertyType)}, fieldExpression: {fieldExpression}, value: {value}");

            if (fieldExpression.Body.NodeType != ExpressionType.MemberAccess)
            {
                throw new SetExpressionException(Messages.MustBePropertyAccess);
            }

            var memberExpression = fieldExpression.Body as MemberExpression;

            if (memberExpression == null)
            {
                var unaryExpression = fieldExpression.Body as UnaryExpression;

                if (unaryExpression == null)
                {
                    throw new SetExpressionException(Messages.MustBePropertyAccess);
                }

                memberExpression = unaryExpression.Operand as MemberExpression;

                if (memberExpression == null)
                {
                    throw new SetExpressionException(Messages.MustBePropertyAccess);
                }
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;

            if (propertyInfo == null)
            {
                throw new SetExpressionException(Messages.MustBePropertyAccess);
            }

            Action<T> setter = @object => propertyInfo.SetValue(@object, value);

            this.explicitProperySetters.AddOrUpdate(propertyInfo, setter, (pi, lambda) =>
            {
                RecordReference<T>.Logger.Debug("Updatng explicitProperySetters dictionary");
                return setter;
            });

            RecordReference<T>.Logger.Debug("Exiting Set");
            return this;
        }
    }
}
