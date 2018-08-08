﻿/*
    Copyright 2016, 2017, 2018 Alexander Kuperman

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
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.ListOperations.Interfaces;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;
using Tests.TestModels;

namespace Tests.Tests.OperableListTests.OperableList.MainTests
{
    [TestClass]
    public class Common
    {
        private TestContext testContext;

        [TestInitialize]
        public void Initialize()
        {
            this.testContext = new TestContext();
        }

        [TestMethod]
        public void AddToReferences_Test()
        {
            // Arrange

            OperableList<ElementType> operableList = this.testContext.CreateOperableList();
            var references = new List<RecordReference>();

            // Act

            operableList.AddToReferences(references);

            // Assert

            Helpers.AssertSetsAreEqual(this.testContext.Inputs, references);
        }

        [TestMethod]
        public void AddGuaranteedPorpertySetter_Test()
        {
            // Arrange

            OperableList<ElementType> operableList = this.testContext.CreateOperableList();
            var guaranteedValues = new GuaranteedValues();
            
            // Act

            operableList.AddGuaranteedPropertySetter(guaranteedValues);

            operableList.Populate();

            // Assert

            this.testContext.ValueGuaranteePopulatorMock.Verify(m => m.Bind(operableList,
                It.Is<IEnumerable<GuaranteedValues>>(v => v.Single() == guaranteedValues),
                It.Is<IValueGauranteePopulatorContextService>(s => s is ExplicitPropertySetterContextService)));
        }

        [TestMethod]
        public void RecordObjects_Test()
        {
            // Arrange

            OperableList<ElementType> operableList = this.testContext.CreateOperableList();

            foreach (Mock<RecordReference<ElementType>> inputMock in this.testContext.InputMocks)
            {
                inputMock.Setup(m => m.RecordObject).Returns(new ElementType());
            }

            // Act

            IEnumerable<ElementType> result = operableList.RecordObjects;

            // Assert

            Helpers.AssertSetsAreEqual(this.testContext.InputObjects, result);
        }

        [TestMethod]
        public void AddRange_RangeCollection_Test()
        {
            // Arrange

            OperableList<ElementType> operableList = this.testContext.CreateOperableList();
            IEnumerable<ElementType.PropertyType> range = Enumerable.Empty<ElementType.PropertyType>();

            var expression = (Expression<Func<ElementType, ElementType.PropertyType>>)(m => m.AProperty);

            // Act

            operableList.AddRange(expression, range);

            // Assert

            this.testContext.InputMocks.ForEach(m => m.Verify(n => n.SetRange(expression, range)));
        }

        [TestMethod]
        public void AddRange_RangeFactory_Test()
        {
            // Arrange

            OperableList<ElementType> operableList = this.testContext.CreateOperableList();
            Func<IEnumerable<ElementType.PropertyType>> rangeFactory = Enumerable.Empty<ElementType.PropertyType>;

            var expression = (Expression<Func<ElementType, ElementType.PropertyType>>)(m => m.AProperty);

            // Act

            operableList.AddRange(expression, rangeFactory);

            // Assert

            this.testContext.InputMocks.ForEach(m => m.Verify(n => n.SetRange(expression, rangeFactory)));
        }
    }
}
