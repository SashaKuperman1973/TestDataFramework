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
namespace TestDataFramework.Exceptions
{
    public static class Messages
    {
        public const string TypeRecursion = "Circular reference detected generating complex type graph: {0} -> {1}";
        public const string MaxAttributeOutOfRange = "Max attribute value is out of range for {0} property";
        public const string MaxAttributeLessThanZero = "Max attribute value is less than zero";

        public const string CircularForeignKeyReference =
            "Circular Foreign Key relationship detected: Key {0}, Reference List {1}";

        public const string CircularReferenceInRecordReferenceList =
            "Internal error. Circular reference in RecordReference List: {0}";

        public const string AmbigousPropertyAttributeMatch = "More than one {0} found on property {1} in type {2}";
        public const string AmbigousTypeAttributeMatch = "More than one {0} found on type {1}";
        public const string AmbigousAttributeMatch = "More than one {0} found on element {1}";

        public const string AmbigousPropertyMatch = "More than one property with {0} found in type {1}";
        public const string NoForeignKeys = "No foreign keys in type {0}";

        public const string NoReferentialIntegrity =
            "Referential integrity error in schema between primary type {0} and foreign type {1}. Can also happen if foreign->primary keys are different types.";

        public const string NotInATransaction =
            "Ambient tranactions being enforced and persitence code not running in one. Possibly committing to underlying data source unintentionally. You can specify that you want to skip transaction checking in the API.";

        public const string StringGeneratorOverflow = "input {0} resulted in overflow for string length {1}";

        public const string UnknownPastOrFutureEnumValue = "Unrecognized PastOrFuture enum value";

        public const string DeferredValueGeneratorExecuted = "Cannot invoke Deferred value generator again after Execute method called";

        public const string UnexpectedHandlerType =
            "Unexpected type fetching value from DB. Property: {0}, Actual value: {1}";

        public const string PropertyNotFound = "Count key not found. Key property: {0}";

        public const string UpdatePropertyNotFound = "Count key for update not found. Key property: {0}";

        public const string PropertyKeyNotFound =  "Property type not handled: {0}";

        public const string Underflow = "Underflow";

        public const string DataCountsDoNotMatch =
            "Db result counts doesn't match input collection count in FillData method";

        public const string MultipleKeysNotAllowedWithAutoKey =
            "Composite primary keys are not allowed when there is an Auto key. Type: {0}";

        public const string SetExpressionNotAssignment =
            "Set expression must be an assignment expression (and not a +=, etc. type of expression)";

        public const string PropertySetExpressionMustBePropertyAccess = "Set operation expression must be a property access expression";

        public const string AddAttributeExpressionMustBePropertyAccess = "Add attribute expression must be a property access expression";

        public const string NoSetter = "The property given has no setter";

        public const string ColumnNotInInputList = "Primary key property doesn't match any input element. {0}";

        public const string InsertionDoesNotSupportType = "Insertion doesn't support type <{0}>, value <{1}>.";

        public const string RequestForMoreThan2UniqueBooleanValues = "Request for more than two unique Boolean values";

        public const string ByteUniqueValueOverflow = "Overflow generating unique byte value.";

        public const string UnhandledUniqueKeyType = "Unhandled type when attempting to ensure uniqueness: {0}";

        public const string LargeIntegerUnderFlow = "Underflow when subtracting LargeIntegers";

        public const string FloatPrecisionOutOfRange =
            "Input value out of range. Max pricision for a C# float is 7 digits";

        public const string PrecisionMustBeNonNegative = "Precision must be non-negative";

        public const string AutoKeyMustBeInteger = "Only integral integer types can be used as Auto increment keys. {0}";

        public const string TypeTooNarrow = "Type too narrow attempting to set property {0} to result from database: {1}";

        public const string CannotResolveForeignKey =
            "Cannot resolve foreign key. Foreign Key Attribute: {0}, foreign type: {1}";

        public const string DuplicateTableName =
            "Attempted to add a type that resolves to an existing type's table definition. You can use TableAttribute to differentiate table names for identically named types in different namespaces. Types involved: {0}";

        public const string ErrorGettingDefinedType =
            "Internal Error. Attimpted to get defined type from dictionary. Table variable: {0}";

        public const string AssemblyCacheNotPopulated = "Cache for assembly does not exist. Assembly: {0}";

        public const string CatalogueAndNoSchema =
            "Cannot have catalogue specified without a schema given. Catalogue: {0}, Table name: {1}";

        public const string AmbigousTableSearchConditions =
            "Ambigous type match when attempting to resolve primary-foreign type/table reference. Foreign Table object used: [{0}], Types involved in the collision: [{1}], [{2}]";

        public const string ForeignKeyRecordWithNoPrimaryKeyRecord =
            "Foreign key in record found without corresponding record/primary key assigned. Foreign type: {0}, Property name: {1}";

        public const string TooFewReferencesForValueGuarantee =
            "Number of guaranteed values would exceed number of given references minus number of referneces with explicit properties";

        public const string NeitherPercentageNorTotalGiven =
            "Internal error: Neither a percentage or a fix value given for gauranteed value quantity";

        public const string GuaranteedTypeNotOfListType =
            "Guaranteed input type not of collection type. Expected type: {0}, Actual type: {1}, Value: {2}";

        public const string TypeNotSupportedForDeepCollectionSetting =
            "Type {0} is not supported for deep collection setting";

        public const string NoRangeOperableListPositionsPassedIn = "No positions passed in.";
    }
}
