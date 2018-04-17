using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Exceptions;
using TestDataFramework.Populator.Concrete;

namespace Tests.Tests.ImmediateTests
{
    [TestClass]
    public class DeepCollectionSettingConverterTests
    {
        private class ConverterTestSubject
        {
            public IEnumerable<int> AnIEnumerable { get; set; }

            public List<int> AList { get; set; }

            public int[] AnArray { get; set; }

            public HashSet<int> AHashSet { get; set; }
        }

        [TestMethod]
        public void Convert_Test()
        {
            var converter = new DeepCollectionSettingConverter();

            var input = new[] {1, 2, 3, 4, 5};

            IEnumerable<int> iEnumerableResult = converter.Convert(input,
                typeof(ConverterTestSubject).GetProperty(nameof(ConverterTestSubject.AnIEnumerable)));
            Assert.IsNotNull(iEnumerableResult);

            IEnumerable<int> listResult = converter.Convert(input,
                typeof(ConverterTestSubject).GetProperty(nameof(ConverterTestSubject.AList)));
            Assert.AreEqual(typeof(List<int>), listResult.GetType());

            IEnumerable<int> arrayResult = converter.Convert(input,
                typeof(ConverterTestSubject).GetProperty(nameof(ConverterTestSubject.AnArray)));
            Assert.AreEqual(typeof(int[]), arrayResult.GetType());
        }

        [TestMethod]
        public void UnsupportedType_Throws()
        {
            var converter = new DeepCollectionSettingConverter();

            var input = new[] { 1, 2, 3, 4, 5 };

            Helpers.ExceptionTest(
                () => converter.Convert(input,
                    typeof(ConverterTestSubject).GetProperty(nameof(ConverterTestSubject.AHashSet))),
                typeof(ArgumentException),
                string.Format(Messages.TypeNotSupportedForDeepCollectionSetting,
                    typeof(ConverterTestSubject).GetProperty(nameof(ConverterTestSubject.AHashSet)).PropertyType));
        }
    }
}
