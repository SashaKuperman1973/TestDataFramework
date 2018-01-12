namespace TestDataFramework.Populator.Concrete
{
    internal class ExplicitlyIgnoredValue
    {
        private static ExplicitlyIgnoredValue instance;

        public static ExplicitlyIgnoredValue Instance => ExplicitlyIgnoredValue.instance ??
                                                         (ExplicitlyIgnoredValue.instance = new ExplicitlyIgnoredValue());
    }
}
