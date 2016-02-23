using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using TestDataFramework.AttributeDecorator;

namespace TestDataFramework.Populator
{
    public abstract class BasePopulator
    {
        protected readonly IAttributeDecorator AttributeDecorator;

        private readonly ConcurrentDictionary<Type, Decorator> decoratorDictionary = new ConcurrentDictionary<Type, Decorator>();

        protected BasePopulator(IAttributeDecorator attributeDecorator)
        {
            this.AttributeDecorator = attributeDecorator;
        }

        public class Decorator { }

        public class Decorator<T> : Decorator
        {
            private readonly IAttributeDecorator attributeDecorator;

            public Decorator(IAttributeDecorator attributeDecorator)
            {
                this.attributeDecorator = attributeDecorator;
            }

            public Decorator<T> AddAttributeToMember<TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression, Attribute attribute)
            {
                this.attributeDecorator.DecorateMember(fieldExpression, attribute);

                return this;
            }

            public Decorator<T> AddAttributeToType(Attribute attribute)
            {
                this.attributeDecorator.DecorateType(typeof(T), attribute);

                return this;
            }
        }

        public Decorator<T> DecorateType<T>()
        {
            Decorator result = this.decoratorDictionary.GetOrAdd(typeof (T), new Decorator<T>(this.AttributeDecorator));
            return (Decorator<T>)result;
        }
    }
}
