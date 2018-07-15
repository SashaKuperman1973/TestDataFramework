using System;
using System.Collections.Generic;

namespace ExplicitlySettingProperties
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
    }

    public class DeepB
    {
        public DeepC DeepC { get; set; }

        public DateTime ADateTime { get; set; }
    }

    public class DeepC
    {
        public int AnInteger { get; set; }

        public string ATextProperty { get; set; }
    }
}
