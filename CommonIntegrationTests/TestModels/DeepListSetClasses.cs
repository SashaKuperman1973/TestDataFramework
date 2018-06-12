using System.Collections.Generic;

namespace CommonIntegrationTests.TestModels
{
    public class ListSetterBaseType
    {
        public ListSetterBaseTypeB B { get; set; }

        public int AnInt { get; set; }
    }

    public class ListSetterBaseTypeB
    {
        public ListType WithCollection { get; set; }

        public int AnInt { get; set; }
    }

    public class ListType
    {
        public List<ListElementType> ElementList { get; set; }

        public ListElementType[] ElementArray { get; set; }
    }

    public class ListElementType
    {
        public string AString { get; set; }

        public SubElementType SubElement { get; set; }
    }

    public class SubElementType
    {
        public string AString { get; set; }

        public int AnInt { get; set; }
    }
}