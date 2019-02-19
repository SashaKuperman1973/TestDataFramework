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
using System.Linq.Expressions;
using System.Reflection;
using log4net;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeepSetting;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.Helpers.FieldExpressionValidator.Concrete;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Logger;
using TestDataFramework.Populator.Concrete.OperableList;
using TestDataFramework.Populator.Interfaces;
using TestDataFramework.RepositoryOperations.Model;
using TestDataFramework.TypeGenerator.Concrete;
using TestDataFramework.TypeGenerator.Interfaces;

namespace TestDataFramework.Populator.Concrete
{
    public class RecordReference<T> : RecordReference, IMakeable<T>
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(RecordReference<T>));
        private readonly DeepCollectionSettingConverter deepCollectionSettingConverter;

        internal readonly List<ExplicitPropertySetter> ExplicitPropertySetters =
            new List<ExplicitPropertySetter>();

        private readonly IObjectGraphService objectGraphService;
        private readonly ValueGuaranteePopulator valueGuaranteePopulator;

        private readonly BasePopulator populator;

        private readonly PropertySetFieldExpressionValidator fieldExpressionValidator =
            new PropertySetFieldExpressionValidator();

        public RecordReference(ITypeGenerator typeGenerator, IAttributeDecorator attributeDecorator,
            BasePopulator populator, IObjectGraphService objectGraphService,
            ValueGuaranteePopulator valueGuaranteePopulator,
            DeepCollectionSettingConverter deepCollectionSettingConverter) : base(typeGenerator, attributeDecorator)
        {
            RecordReference<T>.Logger.Debug($"Entering constructor. T: {typeof(T)}");

            this.RecordType = typeof(T);
            this.populator = populator;
            this.objectGraphService = objectGraphService;
            this.valueGuaranteePopulator = valueGuaranteePopulator;
            this.deepCollectionSettingConverter = deepCollectionSettingConverter;

            RecordReference<T>.Logger.Debug("Exiting constructor");
        }

        public virtual T RecordObject => (T) (base.RecordObjectBase ?? default(T));

        public override Type RecordType { get; }

        public virtual void AddPrimaryRecordReference<TPrimaryRecord, TPrimaryKey>(
            RecordReference<TPrimaryRecord> primaryRecordReference,
            Expression<Func<T, TPrimaryKey>> foreignKeyExpression)
        {
            if (foreignKeyExpression == null || primaryRecordReference == null)
                throw new ArgumentNullException();

            MemberExpression memberExpression =
                this.fieldExpressionValidator.ValidateMemberAccessExpression(foreignKeyExpression);

            PropertyInfo foreignKeyPropertyInfo = memberExpression.Member as PropertyInfo;

            if (foreignKeyPropertyInfo == null)
                throw new MemberAccessExpressionException(Messages.PropertySetExpressionMustBePropertyAccess);

            ForeignKeyAttribute foreignKeyAttribute =
                this.AttributeDecorator.GetCustomAttribute<ForeignKeyAttribute>(foreignKeyPropertyInfo);

            if (foreignKeyAttribute == null)
            {
                throw new ArgumentException(Messages.NotAForeignKey);
            }

            string primaryRecordTableName = Helper.GetTableName(typeof(TPrimaryRecord), this.AttributeDecorator).Name;

            if (!foreignKeyAttribute.PrimaryTableName.Equals(primaryRecordTableName, StringComparison.Ordinal))
            {
                throw new ArgumentException(Messages.PrimaryForeignTableMismatch);
            }

            IEnumerable<PropertyAttribute<PrimaryKeyAttribute>> primaryKeyPropertyAttributes =
                this.AttributeDecorator.GetPropertyAttributes<PrimaryKeyAttribute>(typeof(TPrimaryRecord));

            bool isThereAForeignToPrimaryKeyMatch = primaryKeyPropertyAttributes.Any(primaryKeyPropertyAttribute =>

                foreignKeyAttribute.PrimaryKeyName.Equals(
                    Helper.GetColumnName(primaryKeyPropertyAttribute.PropertyInfo, this.AttributeDecorator),
                    StringComparison.Ordinal)

                &&

                foreignKeyPropertyInfo.PropertyType == primaryKeyPropertyAttribute.PropertyInfo.PropertyType
            );

            if (!isThereAForeignToPrimaryKeyMatch)
            {                
                throw new ArgumentException(Messages.NoForeignPrimaryKeyMatch);
            }

            if (this.ExplicitPrimaryKeyRecords.ContainsKey(foreignKeyPropertyInfo.Name))
            {
                throw new ArgumentException(
                    string.Format(Messages.ForeignKeyAlreadySet, foreignKeyPropertyInfo.DeclaringType.Name,
                        foreignKeyPropertyInfo.PropertyType.Name)
                    );
            }

            this.ExplicitPrimaryKeyRecords[foreignKeyPropertyInfo.Name] = primaryRecordReference;

            if (this.PrimaryKeyReferences.Contains(primaryRecordReference))
                return;

            this.PrimaryKeyReferences.Add(primaryRecordReference);
        }

        // Caller is responsible for ensuring Declaring Type is a type of the property being set.
        // This method only checks if the given property on the record reference object is set, 
        // not sub-properties.
        public override bool IsExplicitlySet(PropertyInfo propertyInfo)
        {
            RecordReference<T>.Logger.Debug($"Entering IsExplicitlySet. propertyInfo: {propertyInfo}");

            bool result = this.ExplicitPropertySetters.Any(setter =>
                (setter.PropertyChain.FirstOrDefault()?.Name.Equals(propertyInfo.Name) ?? false)
                && setter.PropertyChain.FirstOrDefault()?.DeclaringType == propertyInfo.DeclaringType);

            RecordReference<T>.Logger.Debug("Exiting IsExplicitlySet");
            return result;
        }

        internal override void Populate()
        {
            RecordReference<T>.Logger.Debug("Entering Populate");

            if (this.IsPopulated)
            {
                RecordReference<T>.Logger.Debug("Is Populated. Exiting.");
                return;
            }

            this.PopulateChildren();

            base.RecordObjectBase =
                this.TypeGenerator.GetObject<T>(new TypeGeneratorContext(this.ExplicitPropertySetters));

            this.IsPopulated = true;

            RecordReference<T>.Logger.Debug("Exiting Populate");
        }

        internal void AddToExplicitPropertySetters<TResultElement>(
            Expression<Func<T, IEnumerable<TResultElement>>> listFieldExpression,
            OperableList<TResultElement> list)
        {
            RecordReference<T>.Logger.Calling(nameof(this.AddToExplicitPropertySetters));

            List<PropertyInfo> objectPropertyGraph = this.objectGraphService.GetObjectGraph(listFieldExpression);

            this.AddToExplicitPropertySetters(listFieldExpression,
                () => this.deepCollectionSettingConverter.Convert(list.RecordObjects, objectPropertyGraph.Last()));
        }

        public virtual RecordReference<T> Set<TBasePropertyValue, TPropertyValue>(
            Expression<Func<T, TBasePropertyValue>> fieldExpression, TPropertyValue value)
            where TPropertyValue : TBasePropertyValue
        {
            RecordReference<T>.Logger.Debug(
                $"Entering Set(fieldExpression, value). TPropertyValue: {typeof(TPropertyValue)}, fieldExpression: {fieldExpression}, value: {value?.GetType()}");

            RecordReference<T> result = this.Set(fieldExpression, () => value);

            RecordReference<T>.Logger.Debug("Exiting Set(fieldExpression, value)");
            return result;
        }

        public virtual RecordReference<T> Set<TBasePropertyValue, TPropertyValue>(
            Expression<Func<T, TBasePropertyValue>> fieldExpression, Func<TPropertyValue> valueFactory)
            where TPropertyValue : TBasePropertyValue
        {
            RecordReference<T>.Logger.Debug(
                $"Entering Set(fieldExpression, valueFactory). TPropertyValue: {typeof(TPropertyValue)}, fieldExpression: {fieldExpression}, valueFactory: {valueFactory}");

            this.AddToExplicitPropertySetters(fieldExpression, valueFactory);

            RecordReference<T>.Logger.Debug("Exiting Set(fieldExpression, valueFactory)");
            return this;
        }

        public virtual RootReferenceParentOperableList<TListElement, T> SetList<TListElement>(
            Expression<Func<T, IEnumerable<TListElement>>> listFieldExpression, int size)
        {
            var operableList = new RootReferenceParentOperableList<TListElement, T>(
                this, 
                new RecordReference<TListElement>[size],
                this.valueGuaranteePopulator,
                this.populator,
                this.objectGraphService,
                this.AttributeDecorator,
                this.deepCollectionSettingConverter,
                this.TypeGenerator,
                isShallowCopy: false
                );

            this.AddChild(operableList);

            this.AddToExplicitPropertySetters(listFieldExpression, operableList);
            return operableList;
        }

        public virtual RecordReference<T> SetRange<TPropertyValue>(Expression<Func<T, TPropertyValue>> fieldExpression,
            IEnumerable<TPropertyValue> range)
        {
            RecordReference<T>.Logger.Debug(
                $"Entering SetRange(fieldExpression, range). TPropertyValue: {typeof(TPropertyValue)}, fieldExpression: {fieldExpression}, valueFactory: {range?.GetType()}");

            RecordReference<T> result = this.SetRange(fieldExpression, () => range);

            RecordReference<T>.Logger.Debug("Exiting Set(fieldExpression, value)");
            return result;
        }

        public virtual RecordReference<T> SetRange<TPropertyValue>(Expression<Func<T, TPropertyValue>> fieldExpression,
            Func<IEnumerable<TPropertyValue>> rangeFactory)
        {
            RecordReference<T>.Logger.Debug(
                $"Entering SetRange(fieldExpression, rangeFactory). TPropertyValue: {typeof(TPropertyValue)}, fieldExpression: {fieldExpression}, valueFactory: {rangeFactory}");

            this.AddToExplicitPropertySetters(fieldExpression,
                () => RecordReference<T>.ChooseElementInRange(rangeFactory()));

            RecordReference<T>.Logger.Debug("Exiting SetRange(fieldExpression, rangeFactory)");
            return this;
        }

        public virtual RecordReference<T> Ignore<TPropertyValue>(Expression<Func<T, TPropertyValue>> fieldExpression)
        {
            RecordReference<T>.Logger.Debug(
                $"Entering Ignore(fieldExpression). TPropertyValue: {typeof(TPropertyValue)}, fieldExpression: {fieldExpression}");

            this.AddToExplicitPropertySetters(fieldExpression, () => ExplicitlyIgnoredValue.Instance);

            RecordReference<T>.Logger.Debug("Exiting Ignore(fieldExpression)");

            return this;
        }

        public virtual T BindAndMake()
        {
            this.populator.Bind();
            return this.RecordObject;
        }

        public virtual T Make()
        {
            this.populator.Bind(this);
            return this.RecordObject;
        }

        internal override void AddToReferences(IList<RecordReference> collection)
        {
            collection.Add(this);
        }

        private static TPropertyValue ChooseElementInRange<TPropertyValue>(IEnumerable<TPropertyValue> elements)
        {
            elements = elements.ToList();
            int index = new Random(DateTime.Now.Millisecond).Next(elements.Count());
            TPropertyValue result = elements.ElementAt(index);
            return result;
        }

        private void AddToExplicitPropertySetters<TBasePropertyValue, TPropertyValue>(
            Expression<Func<T, TBasePropertyValue>> fieldExpression, Func<TPropertyValue> valueFactory)
        {
            object ObjectValueFactory() => valueFactory();

            this.AddToExplicitPropertySetters(fieldExpression, ObjectValueFactory);
        }

        private void AddToExplicitPropertySetters<TPropertyValue>(Expression<Func<T, TPropertyValue>> fieldExpression,
            Func<object> valueFactory)
        {
            List<PropertyInfo> setterObjectGraph = this.objectGraphService.GetObjectGraph(fieldExpression);

            void Setter(object @object)
            {
                object value = valueFactory();

                if (value is ExplicitlyIgnoredValue)
                    return;

                setterObjectGraph.Last().SetValue(@object, value);
            }

            this.ExplicitPropertySetters.Add(
                new ExplicitPropertySetter {PropertyChain = setterObjectGraph, Action = Setter});
        }
    }
}