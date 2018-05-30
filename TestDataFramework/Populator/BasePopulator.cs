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
using System.Linq.Expressions;
using log4net;
using TestDataFramework.Logger;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Interfaces;
using TestDataFramework.ValueGenerator.Interfaces;

namespace TestDataFramework.Populator
{
    public abstract class BasePopulator : IPopulator
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(BasePopulator));

        protected readonly IAttributeDecorator AttributeDecorator;

        protected readonly ConcurrentDictionary<Type, Decorator> DecoratorDictionary = new ConcurrentDictionary<Type, Decorator>();

        protected BasePopulator(IAttributeDecorator attributeDecorator)
        {
            this.AttributeDecorator = attributeDecorator;
        }

        internal void AddToDecoratorDictionary(IDictionary<Type, Decorator> decoratorDictionary)
        {
            foreach (KeyValuePair<Type, Decorator> kvp in decoratorDictionary)
            {
                this.DecoratorDictionary.TryAdd(kvp.Key, kvp.Value);
            }
        }

        public class Decorator { }

        public class Decorator<T> : Decorator
        {
            private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(Decorator<T>));

            private readonly IAttributeDecorator attributeDecorator;

            public Decorator(IAttributeDecorator attributeDecorator)
            {
                this.attributeDecorator = attributeDecorator;
            }

            public Decorator<T> AddAttributeToMember<TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression, Attribute attribute)
            {
                Decorator<T>.Logger.Debug($"Entering AddAttributeToMember<TPropertyType>. Attribute: {attribute}.");

                this.attributeDecorator.DecorateMember(fieldExpression, attribute);

                Decorator<T>.Logger.Debug("Exiting AddAttributeToMember<TPropertyType>");
                return this;
            }

            public Decorator<T> AddAttributeToType(Attribute attribute)
            {
                Decorator<T>.Logger.Debug($"Entering AddAttributeToType. Attribute: {attribute}.");

                this.attributeDecorator.DecorateType(typeof(T), attribute);

                Decorator<T>.Logger.Debug("Exiting AddAttributeToType");
                return this;
            }
        }


        public Decorator<T> DecorateType<T>()
        {
            BasePopulator.Logger.Debug("Entering DecorateType<T>");

            Decorator result = this.DecoratorDictionary.GetOrAdd(typeof (T), new Decorator<T>(this.AttributeDecorator));

            BasePopulator.Logger.Debug("Exiting DecorateType<T>");
            return (Decorator<T>)result;
        }

        public abstract void Bind();

        protected internal abstract void Bind(RecordReference recordReference);

        protected internal abstract void Bind<T>(OperableList<T> operableList);
        
        public abstract OperableList<T> Add<T>(int copies, params RecordReference[] primaryRecordReferences);

        public abstract RecordReference<T> Add<T>(params RecordReference[] primaryRecordReferences);

        public abstract void Extend(Type type, HandledTypeValueGetter valueGetter);

        public abstract IValueGenerator ValueGenerator { get; }

        public abstract void Clear();
    }
}
