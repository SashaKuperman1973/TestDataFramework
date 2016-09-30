/*
    Copyright 2016 Alexander Kuperman

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
using TestDataFramework.Helpers;
using TestDataFramework.PropertyValueAccumulator;
using TestDataFramework.UniqueValueGenerator;
using Tests.TestModels;

namespace Tests.Tests.ImmediateTests
{
    [TestClass]
    public class BaseUniqueValueGeneratorTests
    {
        private class UniqueValueGenerator : BaseUniqueValueGenerator
        {
            public UniqueValueGenerator(IPropertyValueAccumulator accumulator,
                IDeferredValueGenerator<LargeInteger> deferredValueGenerator) : base(accumulator, deferredValueGenerator, throwIfUnhandledType: false)
            {
            }

            public new void DeferValue(PropertyInfo propertyInfo)
            {
                base.DeferValue(propertyInfo);
            }
        }

        private UniqueValueGenerator uniqueValueGenerator;
        private Mock<IPropertyValueAccumulator> propertyValueAccumulatorMock;
        private Mock<IDeferredValueGenerator<LargeInteger>> deferredValueGeneratorMock;

        [TestInitialize]
        public void Initialize()
        {
            this.propertyValueAccumulatorMock = new Mock<IPropertyValueAccumulator>();
            this.deferredValueGeneratorMock = new Mock<IDeferredValueGenerator<LargeInteger>>();

            this.uniqueValueGenerator = new UniqueValueGenerator(this.propertyValueAccumulatorMock.Object,
                this.deferredValueGeneratorMock.Object);
        }

        [TestMethod]
        public void DeferValue_Test()
        {
            // Arrange

            PropertyInfo propertyInfo = typeof(PrimaryTable).GetProperty("Text");

            DeferredValueGetterDelegate<LargeInteger> inputDelegate = null;

            this.deferredValueGeneratorMock.Setup(
                m => m.AddDelegate(propertyInfo, It.IsAny<DeferredValueGetterDelegate<LargeInteger>>()))
                .Callback<PropertyInfo, DeferredValueGetterDelegate<LargeInteger>>((pi, d) => inputDelegate = d).Verifiable();

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

            PropertyInfo propertyInfo = typeof (PrimaryTable).GetProperty("Text");

            // Act

            this.uniqueValueGenerator.GetValue(propertyInfo);

            // Assert

            this.propertyValueAccumulatorMock.Verify(
                m => m.GetValue(propertyInfo, Helper.DefaultInitalCount), 
                Times.Once);
        }
    }
}
