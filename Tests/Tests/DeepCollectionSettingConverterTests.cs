/*
    Copyright 2016, 2017, 2018, 2019 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TestDataFramework is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TestDataFramework.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Exceptions;
using TestDataFramework.Populator.Concrete;

namespace Tests.Tests
{
    [TestClass]
    public class DeepCollectionSettingConverterTests
    {
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

            var input = new[] {1, 2, 3, 4, 5};

            Helpers.ExceptionTest(
                () => converter.Convert(input,
                    typeof(ConverterTestSubject).GetProperty(nameof(ConverterTestSubject.AHashSet))),
                typeof(ArgumentException),
                string.Format(Messages.TypeNotSupportedForDeepCollectionSetting,
                    typeof(ConverterTestSubject).GetProperty(nameof(ConverterTestSubject.AHashSet)).PropertyType));
        }

        private class ConverterTestSubject
        {
            public IEnumerable<int> AnIEnumerable { get; set; }

            public List<int> AList { get; set; }

            public int[] AnArray { get; set; }

            public HashSet<int> AHashSet { get; set; }
        }
    }
}