/*
    Copyright 2016, 2017 Alexander Kuperman

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

using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.PropertyValueAccumulator;
using TestDataFramework.UniqueValueGenerator;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class BaseUniqueValueGeneratorTests
    {
        private Mock<IDeferredValueGenerator<LargeInteger>> deferredValueGeneratorMock;
        private Mock<IPropertyValueAccumulator> propertyValueAccumulatorMock;

        private UniqueValueGenerator uniqueValueGenerator;
        private UniqueValueGenerator generatorThrowsIfUnhandledType;

        [TestInitialize]
        public void Initialize()
        {
            this.propertyValueAccumulatorMock = new Mock<IPropertyValueAccumulator>();
            this.deferredValueGeneratorMock = new Mock<IDeferredValueGenerator<LargeInteger>>();

            this.uniqueValueGenerator = new UniqueValueGenerator(this.propertyValueAccumulatorMock.Object,
                this.deferredValueGeneratorMock.Object, false);

            this.generatorThrowsIfUnhandledType = new UniqueValueGenerator(this.propertyValueAccumulatorMock.Object,
                this.deferredValueGeneratorMock.Object, true);
        }

        [TestMethod]
        public void DeferValue_Test()
        {
            // Arrange

            PropertyInfo propertyInfo = typeof(PrimaryTable).GetProperty("Text");

            DeferredValueGetterDelegate<LargeInteger> inputDelegate = null;

            this.deferredValueGeneratorMock.Setup(
                    m => m.AddDelegate(propertyInfo, It.IsAny<DeferredValueGetterDelegate<LargeInteger>>()))
                .Callback<PropertyInfo, DeferredValueGetterDelegate<LargeInteger>>((pi, d) => inputDelegate = d)
                .Verifiable();

            const long initialCount = 5;

            this.propertyValueAccumulatorMock.Setup(m => m.GetValue(propertyInfo, initialCount)).Verifiable();

            // Act

            this.uniqueValueGenerator.DeferValue(propertyInfo);
            inputDelegate(initialCount);

            // Assert

            this.deferredValueGeneratorMock.Verify();
            this.propertyValueAccumulatorMock.Verify();
        }

        [TestMethod]
        public void GetValue_Test()
        {
            // Arrange

            PropertyInfo propertyInfo = typeof(PrimaryTable).GetProperty("Text");

            // Act

            this.uniqueValueGenerator.GetValue(propertyInfo);

            // Assert

            this.propertyValueAccumulatorMock.Verify(
                m => m.GetValue(propertyInfo, Helper.DefaultInitalCount),
                Times.Once);
        }

        [TestMethod]
        public void UnhandledTypeCheck_Throws_Test()
        {
            // Arrange

            this.propertyValueAccumulatorMock.Setup(m => m.IsTypeHandled(typeof(SecondClass))).Returns(false);

            // Act

            Helpers.ExceptionTest(() =>
                this.generatorThrowsIfUnhandledType.GetValue(typeof(SubjectClass).GetProperty(nameof(SubjectClass.SecondObject))),
                typeof(UnHandledTypeException), Messages.UnhandledUniqueKeyType.Substring(0, 30), MessageOption.MessageStartsWith);
        }

        private class UniqueValueGenerator : BaseUniqueValueGenerator
        {
            public UniqueValueGenerator(IPropertyValueAccumulator accumulator,
                IDeferredValueGenerator<LargeInteger> deferredValueGenerator, bool throwIfUnhandledType) : base(accumulator,
                deferredValueGenerator, throwIfUnhandledType)
            {
            }

            public new void DeferValue(PropertyInfo propertyInfo)
            {
                base.DeferValue(propertyInfo);
            }
        }
    }
}