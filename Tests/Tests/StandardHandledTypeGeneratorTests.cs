﻿/*
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
using System.Reflection;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.Helpers;
using TestDataFramework.TypeGenerator.Concrete;
using TestDataFramework.ValueGenerator.Interfaces;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class StandardHandledTypeGeneratorTests
    {
        private Mock<IValueGenerator> accumulatorValueGeneratorMock;
        private StandardHandledTypeGenerator handledTypeGenerator;
        private Mock<IValueGenerator> valueGeneratorMock;

        [TestInitialize]
        public void Initialize()
        {
            Helpers.ConfigureLogger();

            this.valueGeneratorMock = new Mock<IValueGenerator>();
            this.accumulatorValueGeneratorMock = new Mock<IValueGenerator>();

            this.handledTypeGenerator = new StandardHandledTypeGenerator(this.valueGeneratorMock.Object,
                () => this.accumulatorValueGeneratorMock.Object,
                4);
        }

        [TestMethod]
        public void GetObject_KeyValuePair_Test()
        {
            // Arrange

            this.valueGeneratorMock.Setup(m => m.GetValue(null, typeof(int), It.IsAny<TypeGeneratorContext>())).Returns(5);
            this.valueGeneratorMock.Setup(m => m.GetValue(null, typeof(string), It.IsAny<TypeGeneratorContext>())).Returns("ABCD");

            // Act

            object result = this.handledTypeGenerator.GetObject(typeof(KeyValuePair<int, string>), null);

            // Assert

            Assert.AreEqual(new KeyValuePair<int, string>(5, "ABCD"), result);
        }

        [TestMethod]
        public void GetObject_Dictionary_ValueKey_Test()
        {
            Console.WriteLine("IDictionary");
            this.Dictionary_ValueKey_Test(typeof(IDictionary<int, string>));

            Console.WriteLine("Dictionary");
            this.Dictionary_ValueKey_Test(typeof(Dictionary<int, string>));
        }

        private void Dictionary_ValueKey_Test(Type dictionaryType)
        {
            this.Initialize();

            // Arrange

            int i = 0;
            string[] s = {"AA", "BB", "CC", "DD"};

            this.accumulatorValueGeneratorMock.Setup(m => m.GetValue(null, typeof(int), It.IsAny<TypeGeneratorContext>())).Returns(() => ++i);
            this.valueGeneratorMock.Setup(m => m.GetValue(null, typeof(string), It.IsAny<TypeGeneratorContext>())).Returns(() => s[i - 1]);

            // Act

            object result = this.handledTypeGenerator.GetObject(dictionaryType, null);

            // Assert

            var dictionary = result as Dictionary<int, string>;

            this.accumulatorValueGeneratorMock.Verify(m => m.GetValue(null, typeof(int), It.IsAny<TypeGeneratorContext>()), Times.Exactly(4));
            this.valueGeneratorMock.Verify(m => m.GetValue(null, typeof(string), It.IsAny<TypeGeneratorContext>()), Times.Exactly(4));

            Assert.IsNotNull(dictionary);
            Assert.AreEqual("AA", dictionary[1]);
            Assert.AreEqual("BB", dictionary[2]);
            Assert.AreEqual("CC", dictionary[3]);
            Assert.AreEqual("DD", dictionary[4]);
        }

        [TestMethod]
        public void GetObject_Dictionary_EnumKey_Test()
        {
            // Arrange

            int i = 0;
            string[] s = {"AA", "BB"};

            this.valueGeneratorMock.Setup(m => m.GetValue(null, typeof(string), It.IsAny<TypeGeneratorContext>())).Returns(() => s[i++]);

            // Act

            object result = this.handledTypeGenerator.GetObject(typeof(Dictionary<AnEnum, string>), null);

            // Assert

            var dictionary = result as Dictionary<AnEnum, string>;

            Assert.IsNotNull(dictionary);
            this.accumulatorValueGeneratorMock.Verify(m => m.GetValue(It.IsAny<PropertyInfoProxy>(), It.IsAny<Type>(), null),
                Times.Never);
            this.valueGeneratorMock.Verify(m => m.GetValue(null, typeof(string), It.IsAny<TypeGeneratorContext>()), Times.Exactly(2));

            Assert.AreEqual(Enum.GetNames(typeof(AnEnum)).Length, dictionary.Count);
            Assert.AreEqual("AA", dictionary[AnEnum.SymbolA]);
            Assert.AreEqual("BB", dictionary[AnEnum.SymbolB]);
        }

        [TestMethod]
        public void GetObject_Dictionary_ReferenceKey_Test()
        {
            Console.WriteLine("IDictionary");
            this.Dictionary_ReferenceKey_Test(typeof(IDictionary<object, string>));

            Console.WriteLine("Dictionary");
            this.Dictionary_ReferenceKey_Test(typeof(Dictionary<object, string>));
        }

        private void Dictionary_ReferenceKey_Test(Type dictionaryType)
        {
            this.Initialize();

            // Arrange

            int i = 0;
            object[] o = {new object(), new object(), new object(), new object()};
            string[] s = {"AA", "BB", "CC", "DD"};

            this.valueGeneratorMock.Setup(m => m.GetValue(null, typeof(object), null)).Returns(
                () =>
                    o[i++]);
            this.valueGeneratorMock.Setup(m => m.GetValue(null, typeof(string), null)).Returns(
                () =>
                    s[i - 1]);

            // Act

            object result = this.handledTypeGenerator.GetObject(dictionaryType, null);

            // Assert

            var dictionary = result as Dictionary<object, string>;

            this.valueGeneratorMock.Verify(m => m.GetValue(null, typeof(object), It.IsAny<TypeGeneratorContext>()), Times.Exactly(4));
            this.valueGeneratorMock.Verify(m => m.GetValue(null, typeof(string), It.IsAny<TypeGeneratorContext>()), Times.Exactly(4));

            Assert.IsNotNull(dictionary);
            Assert.AreEqual("AA", dictionary[o[0]]);
            Assert.AreEqual("BB", dictionary[o[1]]);
            Assert.AreEqual("CC", dictionary[o[2]]);
            Assert.AreEqual("DD", dictionary[o[3]]);
        }

        [TestMethod]
        public void GetObject_List_Test()
        {
            Console.WriteLine("List");
            this.ListTest(typeof(List<SubjectClass>));

            Console.WriteLine("IList");
            this.ListTest(typeof(IList<SubjectClass>));

            Console.WriteLine("IEnumerable");
            this.ListTest(typeof(IEnumerable<SubjectClass>));
        }

        private void ListTest(Type listType)
        {
            this.Initialize();

            // Arrange

            SubjectClass[] sc = {new SubjectClass(), new SubjectClass(), new SubjectClass(), new SubjectClass()};
            int i = 0;

            this.valueGeneratorMock.Setup(m => m.GetValue(null, typeof(SubjectClass), It.IsAny<TypeGeneratorContext>())).Returns(() => sc[i++]);

            // Act

            var list = this.handledTypeGenerator.GetObject(listType, null) as List<SubjectClass>;

            // Assert

            Assert.IsNotNull(list);
            Assert.AreEqual(sc[0], list[0]);
            Assert.AreEqual(sc[1], list[1]);
            Assert.AreEqual(sc[2], list[2]);
            Assert.AreEqual(sc[3], list[3]);
        }

        [TestMethod]
        public void GetObject_NoHandlerFound_ReturnsNull_Test()
        {
            // Act

            object result = this.handledTypeGenerator.GetObject(typeof(string), null);

            // Assert

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetObject_GetTuple_OneItem_Test()
        {
            // Arrange

            this.valueGeneratorMock.Setup(m => m.GetValue(null, typeof(string), It.IsAny<TypeGeneratorContext>())).Returns("A");

            // Act

            object result = this.handledTypeGenerator.GetObject(typeof(Tuple<string>), null);

            // Assert

            var tuple = (Tuple<string>) result;

            Assert.AreEqual("A", tuple.Item1);
        }

        [TestMethod]
        public void GetObject_GetTuple_TwoItems_Test()
        {
            // Arrange

            this.valueGeneratorMock.Setup(m => m.GetValue(null, typeof(string), It.IsAny<TypeGeneratorContext>())).Returns("A");
            this.valueGeneratorMock.Setup(m => m.GetValue(null, typeof(int), It.IsAny<TypeGeneratorContext>())).Returns(5);

            // Act

            object result = this.handledTypeGenerator.GetObject(typeof(Tuple<string, int>), null);

            // Assert

            var tuple = (Tuple<string, int>) result;

            Assert.AreEqual("A", tuple.Item1);
            Assert.AreEqual(5, tuple.Item2);
        }

        [TestMethod]
        public void GetObject_GetTuple_ThreeItems_Test()
        {
            // Arrange

            this.valueGeneratorMock.Setup(m => m.GetValue(null, typeof(string), It.IsAny<TypeGeneratorContext>())).Returns("A");
            this.valueGeneratorMock.Setup(m => m.GetValue(null, typeof(int), It.IsAny<TypeGeneratorContext>())).Returns(5);
            this.valueGeneratorMock.Setup(m => m.GetValue(null, typeof(AnEnum), It.IsAny<TypeGeneratorContext>())).Returns(AnEnum.SymbolB);

            // Act

            object result = this.handledTypeGenerator.GetObject(typeof(Tuple<string, int, AnEnum>), null);

            // Assert

            var tuple = (Tuple<string, int, AnEnum>) result;

            Assert.AreEqual("A", tuple.Item1);
            Assert.AreEqual(5, tuple.Item2);
            Assert.AreEqual(AnEnum.SymbolB, tuple.Item3);
        }
    }
}