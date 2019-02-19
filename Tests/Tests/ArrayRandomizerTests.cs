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
using System.Reflection;
using Castle.Components.DictionaryAdapter.Xml;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.ArrayRandomizer;
using TestDataFramework.TypeGenerator.Concrete;
using TestDataFramework.ValueGenerator.Interfaces;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class ArrayRandomizerTests
    {
        private const int Integer = 5;
        private const int ElementLength = 3;
        private StandardArrayRandomizer arrayRandomizer;
        private Mock<Random> randomMock;
        private Mock<IValueGenerator> valueGeneratorMock;

        [TestInitialize]
        public void Initialize()
        {
            this.randomMock = new Mock<Random>();
            this.valueGeneratorMock = new Mock<IValueGenerator>();
            this.arrayRandomizer = new StandardArrayRandomizer(this.randomMock.Object, this.valueGeneratorMock.Object);

            this.valueGeneratorMock.Setup(m => m.GetValue(It.IsAny<PropertyInfo>(), It.IsAny<Type>(), It.IsAny<TypeGeneratorContext>()))
                .Returns(ArrayRandomizerTests.Integer);

            this.valueGeneratorMock.Setup(m => m.GetValue(It.IsAny<PropertyInfo>(), It.Is<Type>(t => t.IsArray), It.IsAny<TypeGeneratorContext>()))
                .Returns<PropertyInfo, Type, TypeGeneratorContext>((p, t, c) => this.arrayRandomizer.GetArray(p, t, c));

            this.randomMock.Setup(m => m.Next(It.IsAny<int>())).Returns(ArrayRandomizerTests.ElementLength - 1);

            XmlConfigurator.Configure();
        }

        [TestMethod]
        public void SimpleArray_Test()
        {
            // Act

            PropertyInfo propertyInfo = typeof(SubjectClass).GetProperty("SimpleArray");
            object value = this.arrayRandomizer.GetArray(propertyInfo, propertyInfo.PropertyType, null);

            // Assert

            var result = value as int[];
            ArrayRandomizerTests.MultiDimansionalAndJaggedArrayTest(result);
        }

        [TestMethod]
        public void MultiDimansionalArray_Test()
        {
            // Act

            PropertyInfo propertyInfo = typeof(SubjectClass).GetProperty("MultiDimensionalArray");
            object value = this.arrayRandomizer.GetArray(propertyInfo, propertyInfo.PropertyType, null);

            // Assert

            var result = value as int[,,];
            ArrayRandomizerTests.MultiDimansionalAndJaggedArrayTest(result);
        }

        [TestMethod]
        public void JaggedArray_Test()
        {
            // Act

            PropertyInfo propertyInfo = typeof(SubjectClass).GetProperty("JaggedArray");
            object value = this.arrayRandomizer.GetArray(propertyInfo, propertyInfo.PropertyType, null);

            // Assert

            var result = value as int[][][];
            ArrayRandomizerTests.MultiDimansionalAndJaggedArrayTest(result);
        }

        [TestMethod]
        public void MultiDimensionalJaggedArray_Test()
        {
            // Act

            PropertyInfo propertyInfo = typeof(SubjectClass).GetProperty("MultiDimensionalJaggedArray");
            object value = this.arrayRandomizer.GetArray(propertyInfo, propertyInfo.PropertyType, null);

            // Assert

            var result = value as int[,,][][,];
            ArrayRandomizerTests.MultiDimansionalAndJaggedArrayTest(result);
        }

        [TestMethod]
        public void JaggedMultiDimensionalArray_Test()
        {
            // Act

            PropertyInfo propertyInfo = typeof(SubjectClass).GetProperty("JaggedMultiDimensionalArray");
            object value = this.arrayRandomizer.GetArray(propertyInfo, propertyInfo.PropertyType, null);

            // Assert

            var result = value as int[][,,][];
            ArrayRandomizerTests.MultiDimansionalAndJaggedArrayTest(result);
        }

        private static void MultiDimansionalAndJaggedArrayTest(Array result)
        {
            Assert.IsNotNull(result);

            int rank = result.Rank;

            var index = new int[rank];

            int i;
            do
            {
                object value = result.GetValue(index);
                var array = value as Array;
                if (array != null)
                {
                    ArrayRandomizerTests.MultiDimansionalAndJaggedArrayTest(array);
                    return;
                }

                Assert.AreEqual(ArrayRandomizerTests.Integer, value);

                i = 0;
                while (i < rank && ++index[i] >= ArrayRandomizerTests.ElementLength)
                    index[i++] = 0;
            } while (i < rank);
        }
    }
}