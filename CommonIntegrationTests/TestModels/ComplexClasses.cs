using System.Collections.Generic;

namespace CommonIntegrationTests.TestModels
{
    public class DeepA
    {
        public int Integer { get; set; }

        public DeepB DeepB { get; set; }
    }

    public class DeepB
    {
        public string String { get; set; }

        public DeepC DeepC { get; set; }

        public List<DeepA> DeepAList { get; set; }

        public List<DeepC> DeepCList { get; set; }
    }

    public class DeepC
    {
        public string DeepString { get; set; }

        public List<int> IntList { get; set; }
    }
}