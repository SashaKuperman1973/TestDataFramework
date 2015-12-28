namespace TestDataFramework.Exceptions
{
    public static class Messages
    {
        public const string NoDefaultConstructorExceptionMessage = "Type has no public default constructor: ";
        public const string TypeRecursionExceptionMessage = "Circular reference detected generating complex type graph: {0} -> {1}";
        public const string MaxAttributeOutOfRange = "Max attribute value is out of range for {0} property";
        public const string MaxAttributeLessThanZero = "Max attribute value is less than zero";

        public const string CircularForeignKeyReferenceException =
            "Circular Foreign Key relationship detected: Key {0}, Reference List {1}";

        public const string CircularReferenceInRecordReferenceList =
            "Internal error. Circular reference in RecordReference List: {0}";
    }
}
