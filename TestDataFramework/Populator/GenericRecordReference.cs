using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.Exceptions;
using TestDataFramework.TypeGenerator;

namespace TestDataFramework.Populator
{
    public class RecordReference<T> : RecordReference
    {
        private readonly ConcurrentDictionary<PropertyInfo, Action<T>> explicitProperySetters =
            new ConcurrentDictionary<PropertyInfo, Action<T>>();

        public RecordReference(ITypeGenerator typeGenerator) : base(typeGenerator)
        {
            this.RecordType = typeof (T);
        }

        public new T RecordObject => (T) base.RecordObject;

        public override bool IsExplicitlySet(PropertyInfo propertyInfo)
        {
            {
                return this.explicitProperySetters.ContainsKey(propertyInfo);
            }
        }

        public override void Populate()
        {
            base.RecordObject = this.TypeGenerator.GetObject<T>(this.explicitProperySetters);
        }

        public virtual RecordReference<T> Set<TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression, TPropertyType value)
        {
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

            this.explicitProperySetters.AddOrUpdate(propertyInfo, setter, (pi, lambda) => setter);

            return this;
        }
    }
}
