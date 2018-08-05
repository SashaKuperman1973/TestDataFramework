/*
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator;
using Tests.TestModels;

namespace Tests.Tests.OperableListTests.OperableList.GuaranteePropertiesByFixedQuantity
{
    [TestClass]
    public class DoNotThrowCountRequestOption
    {
        private TestContext testContext;

        [TestInitialize]
        public void Initialize()
        {
            this.testContext = new TestContext();
        }

        [TestMethod]
        public void PropertyFuncs_DefaultQuantity_Test()
        {
            OperableList<ElementType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new Func<ElementType>[] {() => new ElementType(), () => new ElementType(),};

            OperableList<ElementType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues, ValueCountRequestOption.DoNotThrow);

            actual.Make();

            // Assert

            this.testContext.DoAssert(operableList, actual, guaranteedValues, 2, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void PropertyFuncs_ExpicitQuantity_Test()
        {
            OperableList<ElementType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new Func<ElementType>[] { () => new ElementType(), () => new ElementType(), };

            OperableList<ElementType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues, 5, ValueCountRequestOption.DoNotThrow);

            actual.Make();

            // Assert

            this.testContext.DoAssert(operableList, actual, guaranteedValues, 5, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void PropertiesAndFuncs_DefaultQuantity_Test()
        {
            OperableList<ElementType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new object[] { new ElementType(), (Func<ElementType>)(() => new ElementType()), };

            OperableList<ElementType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues, ValueCountRequestOption.DoNotThrow);

            actual.Make();

            // Assert

            this.testContext.DoAssert(operableList, actual, guaranteedValues, 2, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void PropertiesAndFuncs_ExplicitQuantity_Test()
        {
            OperableList<ElementType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new object[] { new ElementType(), (Func<ElementType>)(() => new ElementType()), };

            OperableList<ElementType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues, 5, ValueCountRequestOption.DoNotThrow);

            actual.Make();

            // Assert

            this.testContext.DoAssert(operableList, actual, guaranteedValues, 5, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void Properties_DefaultQuantity_Test()
        {
            OperableList<ElementType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new[] { new ElementType(), new ElementType(), };

            OperableList<ElementType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues, ValueCountRequestOption.DoNotThrow);

            actual.Make();

            // Assert

            this.testContext.DoAssert(operableList, actual, guaranteedValues, 2, ValueCountRequestOption.DoNotThrow);
        }

        [TestMethod]
        public void Properties_ExplicitQuantity_Test()
        {
            OperableList<ElementType> operableList = this.testContext.CreateOperableList();

            var guaranteedValues = new[] { new ElementType(), new ElementType(), };

            OperableList<ElementType> actual =
                operableList.GuaranteeByFixedQuantity(guaranteedValues, 5, ValueCountRequestOption.DoNotThrow);

            actual.Make();

            // Assert

            this.testContext.DoAssert(operableList, actual, guaranteedValues, 5, ValueCountRequestOption.DoNotThrow);
        }
    }
}
