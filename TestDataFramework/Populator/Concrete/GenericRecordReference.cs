﻿/*
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using log4net;
using TestDataFramework.Logger;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.DeepSetting;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.Persistence.Interfaces;
using TestDataFramework.TypeGenerator.Interfaces;

namespace TestDataFramework.Populator.Concrete
{
    public class RecordReference<T> : RecordReference
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof (RecordReference<T>));

        protected BasePopulator Populator;

        private readonly IObjectGraphService objectGraphService;

        protected internal readonly List<ExplicitPropertySetters> ExplicitPropertySetters =
            new List<ExplicitPropertySetters>();

        public RecordReference(ITypeGenerator typeGenerator, IAttributeDecorator attributeDecorator,
            BasePopulator populator, IObjectGraphService objectGraphService) : base(typeGenerator, attributeDecorator)
        {
            RecordReference<T>.Logger.Debug($"Entering constructor. T: {typeof(T)}");

            this.RecordType = typeof(T);
            this.Populator = populator;
            this.objectGraphService = objectGraphService;

            RecordReference<T>.Logger.Debug("Exiting constructor");
        }

        public new virtual T RecordObject => (T) (base.RecordObject ?? default(T));

        // Caller is responsible for ensuring Declaring Type is a type of the property being set.
        // This method only checks if the given property on the record reference object is set, 
        // not sub-properties.
        protected internal override bool IsExplicitlySet(PropertyInfo propertyInfo)
        {
            RecordReference<T>.Logger.Debug($"Entering IsExplicitlySet. propertyInfo: {propertyInfo}");

            bool result = this.ExplicitPropertySetters.Any(setter =>
                setter.PropertyChain.FirstOrDefault()?.Name.Equals(propertyInfo.Name) ?? false);

            RecordReference<T>.Logger.Debug("Exiting IsExplicitlySet");
            return result;
        }

        protected internal override void Populate()
        {
            RecordReference<T>.Logger.Debug("Entering Populate");

            if (this.IsPopulated)
            {
                RecordReference<T>.Logger.Debug("Is Populated. Exiting.");
                return;
            }

            base.RecordObject = this.TypeGenerator.GetObject<T>(this.ExplicitPropertySetters);
            this.IsPopulated = true;

            RecordReference<T>.Logger.Debug("Exiting Populate");
        }

        public virtual RecordReference<T> Set<TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression, TPropertyType value)
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

            this.AddToExplicitPropertySetters(fieldExpression, valueFactory);

            RecordReference<T>.Logger.Debug("Exiting Set(fieldExpression, valueFactory)");
            return this;
        }

        public virtual void Set<TPropertyType>(Expression<Func<T, IList<TPropertyType>>> fieldExpression,
            Func<TPropertyType> valueFactory, int? position = null)
        {
            
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

            this.AddToExplicitPropertySetters(fieldExpression, () => RecordReference<T>.ChooseElementInRange(rangeFactory()));

            RecordReference<T>.Logger.Debug("Exiting SetRange(fieldExpression, rangeFactory)");
            return this;
        }

        public virtual RecordReference<T> Ignore<TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression)
        {
            RecordReference<T>.Logger.Debug(
                $"Entering Ignore(fieldExpression). TPropertyType: {typeof(TPropertyType)}, fieldExpression: {fieldExpression}");

            this.AddToExplicitPropertySetters(fieldExpression, () => ExplicitlyIgnoredValue.Instance);

            RecordReference<T>.Logger.Debug("Exiting Ignore(fieldExpression)");
            return this;
        }

        public virtual T BindAndMake()
        {
            this.Populator.Bind();
            return this.RecordObject;
        }

        public virtual T Make()
        {
            this.Populator.Bind(this);
            return this.RecordObject;
        }

        private static TPropertyType ChooseElementInRange<TPropertyType>(IEnumerable<TPropertyType> elements)
        {
            elements = elements.ToList();
            int index = new Random().Next(elements.Count());
            TPropertyType result = elements.ElementAt(index);
            return result;
        }

        private void AddToExplicitPropertySetters<TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression, Func<TPropertyType> valueFactory)
        {
            object ObjectValueFactory()
            {
                var objectValueFactoryResult = valueFactory();
                return objectValueFactoryResult;
            }

            this.AddToExplicitPropertySetters(fieldExpression, ObjectValueFactory);
        }

        private void AddToExplicitPropertySetters<TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression, Func<object> valueFactory)
        {
            List<PropertyInfo> setterObjectGraph = this.objectGraphService.GetObjectGraph(fieldExpression);

            void Setter(object @object)
            {
                object value = valueFactory();

                if (value is ExplicitlyIgnoredValue)
                {
                    return;
                }

                setterObjectGraph.Last().SetValue(@object, value);
            }

            this.ExplicitPropertySetters.Add(new ExplicitPropertySetters { PropertyChain = setterObjectGraph, Action = Setter });
        }

        protected internal override void AddToReferences(IList<RecordReference> collection)
        {
            collection.Add(this);
        }

        private void SetList<TPropertyType>(int copies, Range[] ranges, Func<object> valueFactory)
        {
            ranges = ranges.OrderBy(r => r.StartPosition).ToArray();

            if (ranges.Last().EndPosition + 1 > copies)
            {
                throw new ArgumentOutOfRangeException("Setting a collection: Range is greater than copies requested.");
            }

            void Setter(object @object)
            {
                var setterResult = new List<TPropertyType>();

                int lastEndPosition = 0;
                IEnumerable<TPropertyType> autoPopulated;
                foreach (Range range in ranges)
                {
                    if (lastEndPosition > range.StartPosition)
                    {
                        throw new ArgumentOutOfRangeException("Setting a collection: Ranges overlap.");
                    }

                    autoPopulated = this.Populator.Add<TPropertyType>(range.StartPosition - lastEndPosition).Make();
                    lastEndPosition = range.EndPosition;
                    setterResult.AddRange(autoPopulated);

                    for (int i = range.StartPosition; i <= range.EndPosition; i++)
                    {
                        setterResult.Add((TPropertyType)valueFactory());
                    }
                }

                autoPopulated = this.Populator.Add<TPropertyType>(copies - ranges.Last().EndPosition - 1).Make();
                setterResult.AddRange(autoPopulated);
            }
        }
    }
}
