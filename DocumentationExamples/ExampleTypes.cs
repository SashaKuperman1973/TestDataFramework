using System;
using System.Collections.Generic;

namespace ExampleTypes
{
    public class Subject
    {
        public DeepA DeepA { get; set; }

        public string Text { get; set; }
    }

    public class DeepA
    {
        public string TextA { get; set; }

        public List<DeepB> DeepBCollection { get; set; }

        public string[] StringCollection { get; set; }
    }

    public class DeepB
    {
        public string TextC { get; set; }

        public DeepC DeepC { get; set; }

        public DateTime ADateTime { get; set; }

        public List<DeepC> DeepCCollection { get; set; }
    }

    public class DeepC
    {
        public int AnInteger { get; set; }

        public string ATextProperty { get; set; }

        public List<string> DeepCStringCollection { get; set; }
    }
}
