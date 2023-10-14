namespace IntegrationTests.CommonIntegrationTests.TestModels
{
    public class MultiPropertyClass
    {
        public int I1 { get; set; }
        public int I2 { get; set; }
        public string S { get; set; }

        public override string ToString()
        {
            return $"I1: {this.I1} - I2: {this.I2} - S: {this.S}";
        }
    }
}
