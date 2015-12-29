using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.ArrayRandomizer;
using TestDataFramework.ValueGenerator;
using Tests.TestModels;

namespace Tests
{
    [TestClass]
    public class ArrayRandomizerTests
    {
        private StandardArrayRandomizer arrayRandomizer;
        private Mock<Random> randomMock;
        private Mock<IValueGenerator> valueGeneratorMock;

        private const int Integer = 5;
        private const int ElementLength = 3;

        [TestInitialize]
        public void Initialize()
        {
            this.randomMock = new Mock<Random>();
            this.valueGeneratorMock = new Mock<IValueGenerator>();
            this.arrayRandomizer = new StandardArrayRandomizer(this.randomMock.Object, this.valueGeneratorMock.Object);

            this.valueGeneratorMock.Setup(m => m.GetValue(It.IsAny<PropertyInfo>(), It.IsAny<Type>())).Returns(ArrayRandomizerTests.Integer);
            this.valueGeneratorMock.Setup(m => m.GetValue(It.IsAny<PropertyInfo>(), It.Is<Type>(t => t.IsArray))).Returns<PropertyInfo, Type>((p, t) => arrayRandomizer.GetArray(p, t));
            this.randomMock.Setup(m => m.Next(It.IsAny<int>())).Returns(ArrayRandomizerTests.ElementLength - 1);
        }

        [TestMethod]
        public void SimpleArray_Test()
        {
            // Act

            PropertyInfo propertyInfo = typeof (SubjectClass).GetProperty("SimpleArray");
            object value = this.arrayRandomizer.GetArray(propertyInfo, propertyInfo.PropertyType) ;

            // Assert

            var result = value as int[];
            ArrayRandomizerTests.MultiDimansionalAndJaggedArrayTest(result);
        }

        [TestMethod]
        public void MultiDimansionalArray_Test()
        {
            // Act

            PropertyInfo propertyInfo = typeof(SubjectClass).GetProperty("MultiDimensionalArray");
            object value = this.arrayRandomizer.GetArray(propertyInfo, propertyInfo.PropertyType);

            // Assert

            var result = value as int[,,];
            ArrayRandomizerTests.MultiDimansionalAndJaggedArrayTest(result);
        }

        [TestMethod]
        public void JaggedArray_Test()
        {
            // Act

            PropertyInfo propertyInfo = typeof(SubjectClass).GetProperty("JaggedArray");
            object value = this.arrayRandomizer.GetArray(propertyInfo, propertyInfo.PropertyType);

            // Assert

            var result = value as int[][][];
            ArrayRandomizerTests.MultiDimansionalAndJaggedArrayTest(result);
        }

        [TestMethod]
        public void MultiDimensionalJaggedArray_Test()
        {
            // Act

            PropertyInfo propertyInfo = typeof(SubjectClass).GetProperty("MultiDimensionalJaggedArray");
            object value = this.arrayRandomizer.GetArray(propertyInfo, propertyInfo.PropertyType);

            // Assert

            var result = value as int[,,][][,];
            ArrayRandomizerTests.MultiDimansionalAndJaggedArrayTest(result);
        }

        [TestMethod]
        public void JaggedMultiDimensionalArray_Test()
        {
            // Act

            PropertyInfo propertyInfo = typeof(SubjectClass).GetProperty("JaggedMultiDimensionalArray");
            object value = this.arrayRandomizer.GetArray(propertyInfo, propertyInfo.PropertyType);

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
                {
                    index[i++] = 0;
                }
            } while (i < rank);
        }
    }
}
