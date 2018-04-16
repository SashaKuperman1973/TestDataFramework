using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Populator.Concrete;

namespace Tests.Tests.ImmediateTests
{
    [TestClass]
    public class ExplicitlyIgnoredValueTests
    {
        [TestMethod]
        public void ExplicitlyIgnoredValue_IsA_Singleton()
        {
            ExplicitlyIgnoredValue explicitlyIgnoredValue1 = ExplicitlyIgnoredValue.Instance;
            ExplicitlyIgnoredValue explicitlyIgnoredValue2 = ExplicitlyIgnoredValue.Instance;

            Assert.AreEqual(explicitlyIgnoredValue2, explicitlyIgnoredValue1);
        }

    }
}
