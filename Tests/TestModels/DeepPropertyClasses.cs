namespace Tests.TestModels
{
    public class FirstDeepPropertyTable
    {
        public string Value { get; set; }
    }

    public class SecondDeepPropertyTable
    {
        public FirstDeepPropertyTable Deep1 { get; set; }
    }

    public class ThirdDeepPropertyTable
    {
        public SecondDeepPropertyTable Deep2 { get; set; }
    }
}
