﻿using System.Collections.Generic;
using log4net;
using TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.Logger;

namespace TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService
{
    public class TableTypeLookup
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(TableTypeLookup));

        private readonly IAttributeDecoratorBase attributeDecorator;

        public TableTypeLookup(IAttributeDecoratorBase attributeDecorator)
        {
            this.attributeDecorator = attributeDecorator;
        }

        public virtual TypeInfoWrapper GetTableTypeByCriteria(Table table,
            TypeDictionaryEqualityComparer.EqualsCriteriaDelegate matchCriteria,
            AssemblyLookupContext assemblyLookupContext)
        {
            TableTypeLookup.Logger.Debug("Entering GetTableTypeByCriteria.");

            TypeInfoWrapper result;
            IList<TypeInfoWrapper> collisionTypes;

            assemblyLookupContext.TypeDictionaryEqualityComparer.SetEqualsCriteria(matchCriteria);

            if (assemblyLookupContext.CollisionDictionary.TryGetValue(table, out collisionTypes))
                throw new TableTypeCacheException(Messages.DuplicateTableName, collisionTypes);

            TableTypeLookup.Logger.Debug("Exiting GetTableTypeByCriteria.");
            return assemblyLookupContext.TypeDictionary.TryGetValue(table, out result) ? result : null;
        }

        public virtual TypeInfoWrapper GetTableTypeWithCatalogue(Table table,
            AssemblyLookupContext assemblyLookupContext)
        {
            TableTypeLookup.Logger.Debug("Entering GetTableTypeWithCatalogue.");

            if (table.HasCatalogueName)
            {
                TableTypeLookup.Logger.Debug("Table has a catalogue name. Exiting GetTableTypeWithCatalogue.");
                return null;
            }

            TypeInfoWrapper result;

            assemblyLookupContext.TypeDictionaryEqualityComparer.SetEqualsCriteria((fromSet, input) =>
                fromSet.HasCatalogueName);

            if (!assemblyLookupContext.TypeDictionary.TryGetValue(table, out result)) return null;

            // Test for collision where !input.HasCatalogueName and values 
            // match on input but have different catalogue names specified.

            var resultTableAttribute = this.attributeDecorator.GetSingleAttribute<TableAttribute>(result);

            assemblyLookupContext.TypeDictionaryEqualityComparer.SetEqualsCriteria(
                (fromSet, input) =>
                    fromSet.HasCatalogueName &&
                    !fromSet.CatalogueName.Equals(resultTableAttribute.CatalogueName)
            );

            TypeInfoWrapper ambigousConditionType;

            if (assemblyLookupContext.TypeDictionary.TryGetValue(table, out ambigousConditionType))
                throw new TableTypeCacheException(Messages.AmbigousTableSearchConditions, table, result,
                    ambigousConditionType);

            TableTypeLookup.Logger.Debug("Exiting GetTableTypeWithCatalogue.");
            return result;
        }
    }
}