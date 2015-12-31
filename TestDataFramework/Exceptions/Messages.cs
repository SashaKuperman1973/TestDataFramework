namespace TestDataFramework.Exceptions
{
    public static class Messages
    {
        public const string NoDefaultConstructor = "Type has no public default constructor: ";
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
    }
}
