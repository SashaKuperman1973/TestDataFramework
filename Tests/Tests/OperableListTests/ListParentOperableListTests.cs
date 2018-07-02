using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator.Concrete.OperableList;
using Tests.TestModels;
using Tests.Tests.FieldExpressionTests;

namespace Tests.Tests.OperableListTests
{
    [TestClass]
    public class ListParentOperableListTests
    {
        private ListParentOperableList<ElementType> operableList;

        [TestInitialize]
        public void Initialize()
        {
            this.operableList = new ListParentOperableList<ElementType>(null, null, null, null, null, null);
        }

        ////////////////////////////////
        /// GuaranteeByPercentageOfTotal

        [TestMethod]
        public void GuaranteeByPercentageOfTotal_Mixed_DefaultPercentage_Test()
        {
            // Arrange

            var values = new[]
            {
                new object(),
                new object(),
            };

            // Act

            ListParentOperableList<ElementType> result = this.operableList.GuaranteeByPercentageOfTotal(values);

            // Assert

            Assert.AreEqual(1, this.operableList.GuaranteedValues.Count);

            GuaranteedValues guaranteedValues = this.operableList.GuaranteedValues.Single();

            Assert.AreEqual(10, guaranteedValues.FrequencyPercentage);
            Assert.IsNull(guaranteedValues.TotalFrequency);
            Assert.AreEqual(ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall, guaranteedValues.ValueCountRequestOption);

            Assert.AreEqual(this.operableList, result);
        }

        [TestMethod]
        public void GuaranteeByPercentageOfTotal_Mixed_ExplicitPercentage_Test()
        {
            // Arrange

            var values = new[]
            {
                new object(),
                new object(),
            };

            // Act

            ListParentOperableList<ElementType> result = this.operableList.GuaranteeByPercentageOfTotal(values, 25);

            // Assert

            Assert.AreEqual(1, this.operableList.GuaranteedValues.Count);

            GuaranteedValues guaranteedValues = this.operableList.GuaranteedValues.Single();

            Assert.AreEqual(25, guaranteedValues.FrequencyPercentage);
            Assert.IsNull(guaranteedValues.TotalFrequency);
            Assert.AreEqual(ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall, guaranteedValues.ValueCountRequestOption);

            Assert.AreEqual(this.operableList, result);
        }

        [TestMethod]
        public void GuaranteeByPercentageOfTotal_Func_DefaultPercentage_Test()
        {
            // Arrange

            var values = new[]
            {

                new Func<ElementType>(() => new ElementType()),
                new Func<ElementType>(() => new ElementType()),
            };

            // Act

            ListParentOperableList<ElementType> result = this.operableList.GuaranteeByPercentageOfTotal(values);

            // Assert

            Assert.AreEqual(1, this.operableList.GuaranteedValues.Count);

            GuaranteedValues guaranteedValues = this.operableList.GuaranteedValues.Single();

            Assert.AreEqual(values, guaranteedValues.Values);
            Assert.AreEqual(10, guaranteedValues.FrequencyPercentage);
            Assert.IsNull(guaranteedValues.TotalFrequency);
            Assert.AreEqual(ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall, guaranteedValues.ValueCountRequestOption);

            Assert.AreEqual(this.operableList, result);
        }

        [TestMethod]
        public void GuaranteeByPercentageOfTotal_Func_ExplicitPercentage_Test()
        {
            // Arrange

            var values = new[]
            {

                new Func<ElementType>(() => new ElementType()),
                new Func<ElementType>(() => new ElementType()),
            };

            // Act

            ListParentOperableList<ElementType> result = this.operableList.GuaranteeByPercentageOfTotal(values, 25);

            // Assert

            Assert.AreEqual(1, this.operableList.GuaranteedValues.Count);

            GuaranteedValues guaranteedValues = this.operableList.GuaranteedValues.Single();

            Assert.AreEqual(values, guaranteedValues.Values);
            Assert.AreEqual(25, guaranteedValues.FrequencyPercentage);
            Assert.IsNull(guaranteedValues.TotalFrequency);
            Assert.AreEqual(ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall, guaranteedValues.ValueCountRequestOption);

            Assert.AreEqual(this.operableList, result);
        }

        [TestMethod]
        public void GuaranteeByPercentageOfTotal_ValueElement_DefaultPercentage_Test()
        {
            // Arrange

            var values = new[]
            {
                new ElementType(),
                new ElementType(),
            };

            // Act

            ListParentOperableList<ElementType> result = this.operableList.GuaranteeByPercentageOfTotal(values);

            // Assert

            Assert.AreEqual(1, this.operableList.GuaranteedValues.Count);

            GuaranteedValues guaranteedValues = this.operableList.GuaranteedValues.Single();

            Assert.AreEqual(values, guaranteedValues.Values);
            Assert.AreEqual(10, guaranteedValues.FrequencyPercentage);
            Assert.IsNull(guaranteedValues.TotalFrequency);
            Assert.AreEqual(ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall, guaranteedValues.ValueCountRequestOption);

            Assert.AreEqual(this.operableList, result);
        }
        
        [TestMethod]
        public void GuaranteeByPercentageOfTotal_ValueElement_ExplicitPercentage_Test()
        {
            // Arrange

            var values = new[]
            {
                new ElementType(),
                new ElementType(),
            };

            // Act

            ListParentOperableList<ElementType> result = this.operableList.GuaranteeByPercentageOfTotal(values, 25);

            // Assert

            Assert.AreEqual(1, this.operableList.GuaranteedValues.Count);

            GuaranteedValues guaranteedValues = this.operableList.GuaranteedValues.Single();

            Assert.AreEqual(values, guaranteedValues.Values);
            Assert.AreEqual(25, guaranteedValues.FrequencyPercentage);
            Assert.IsNull(guaranteedValues.TotalFrequency);
            Assert.AreEqual(ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall, guaranteedValues.ValueCountRequestOption);

            Assert.AreEqual(this.operableList, result);
        }

        ////////////////////////////
        /// GuaranteeByFixedQuantity

        [TestMethod]
        public void GuaranteeByFixedQuantity_Mixed_DefaultQuantity_Test()
        {
            // Arrange

            var values = new[]
            {
                new object(),
                new object(),
            };

            // Act

            ListParentOperableList<ElementType> result = this.operableList.GuaranteeByFixedQuantity(values);

            // Assert

            Assert.AreEqual(1, this.operableList.GuaranteedValues.Count);

            GuaranteedValues guaranteedValues = this.operableList.GuaranteedValues.Single();

            ListParentOperableListTests.AssertSetsEqual(values, guaranteedValues.Values);
            Assert.AreEqual(2, guaranteedValues.TotalFrequency);
            Assert.IsNull(guaranteedValues.FrequencyPercentage);
            Assert.AreEqual(ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall, guaranteedValues.ValueCountRequestOption);

            Assert.AreEqual(this.operableList, result);
        }

        [TestMethod]
        public void GuaranteeByFixedQuantity_Mixed_ExplicitQuantity_Test()
        {
            // Arrange

            var values = new[]
            {
                new object(),
                new object(),
            };

            // Act

            ListParentOperableList<ElementType> result = this.operableList.GuaranteeByFixedQuantity(values, 25);

            // Assert

            Assert.AreEqual(1, this.operableList.GuaranteedValues.Count);

            GuaranteedValues guaranteedValues = this.operableList.GuaranteedValues.Single();

            Assert.AreEqual(25, guaranteedValues.TotalFrequency);
            Assert.IsNull(guaranteedValues.FrequencyPercentage);
            Assert.AreEqual(ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall, guaranteedValues.ValueCountRequestOption);

            Assert.AreEqual(this.operableList, result);
        }

        [TestMethod]
        public void GuaranteeByFixedQuantity_Func_DefaultQuantity_Test()
        {
            // Arrange

            var values = new[]
            {
                new Func<ElementType>(() => new ElementType()),
                new Func<ElementType>(() => new ElementType()),
            };

            // Act

            ListParentOperableList<ElementType> result = this.operableList.GuaranteeByFixedQuantity(values);

            // Assert

            Assert.AreEqual(1, this.operableList.GuaranteedValues.Count);

            GuaranteedValues guaranteedValues = this.operableList.GuaranteedValues.Single();

            ListParentOperableListTests.AssertSetsEqual(values, guaranteedValues.Values);
            Assert.AreEqual(2, guaranteedValues.TotalFrequency);
            Assert.IsNull(guaranteedValues.FrequencyPercentage);
            Assert.AreEqual(ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall, guaranteedValues.ValueCountRequestOption);

            Assert.AreEqual(this.operableList, result);
        }

        [TestMethod]
        public void GuaranteeByFixedQuantity_Func_ExplicitQuantity_Test()
        {
            // Arrange

            var values = new[]
            {
                new ElementType(),
                new ElementType(),
            };

            // Act

            ListParentOperableList<ElementType> result = this.operableList.GuaranteeByFixedQuantity(values, 25);

            // Assert

            Assert.AreEqual(1, this.operableList.GuaranteedValues.Count);

            GuaranteedValues guaranteedValues = this.operableList.GuaranteedValues.Single();

            ListParentOperableListTests.AssertSetsEqual(values, guaranteedValues.Values);
            Assert.AreEqual(25, guaranteedValues.TotalFrequency);
            Assert.IsNull(guaranteedValues.FrequencyPercentage);
            Assert.AreEqual(ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall, guaranteedValues.ValueCountRequestOption);

            Assert.AreEqual(this.operableList, result);
        }

        [TestMethod]
        public void GuaranteeByFixedQuantity_ValueElement_DefaultQuantity_Test()
        {
            // Arrange

            var values = new[]
            {
                new ElementType(),
                new ElementType(),
            };

            // Act

            ListParentOperableList<ElementType> result = this.operableList.GuaranteeByFixedQuantity(values);

            // Assert

            Assert.AreEqual(1, this.operableList.GuaranteedValues.Count);

            GuaranteedValues guaranteedValues = this.operableList.GuaranteedValues.Single();

            Assert.AreEqual(2, guaranteedValues.TotalFrequency);
            Assert.IsNull(guaranteedValues.FrequencyPercentage);
            Assert.AreEqual(ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall, guaranteedValues.ValueCountRequestOption);

            Assert.AreEqual(this.operableList, result);
        }

        [TestMethod]
        public void GuaranteeByFixedQuantity_ValueElement_ExplicitQuantity_Test()
        {
            // Arrange

            var values = new[]
            {
                new ElementType(),
                new ElementType(),
            };

            // Act

            ListParentOperableList<ElementType> result = this.operableList.GuaranteeByFixedQuantity(values, 25);

            // Assert

            Assert.AreEqual(1, this.operableList.GuaranteedValues.Count);

            GuaranteedValues guaranteedValues = this.operableList.GuaranteedValues.Single();

            ListParentOperableListTests.AssertSetsEqual(values, guaranteedValues.Values);

            Assert.AreEqual(25, guaranteedValues.TotalFrequency);
            Assert.IsNull(guaranteedValues.FrequencyPercentage);
            Assert.AreEqual(ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall, guaranteedValues.ValueCountRequestOption);

            Assert.AreEqual(this.operableList, result);
        }

        private static void AssertSetsEqual(IEnumerable<object> left, IEnumerable<object> right)
        {
            object[] leftArray = left.ToArray();
            object[] rightArray = right.ToArray();

            Assert.AreEqual(leftArray.Length, rightArray.Length);

            for (int i = 0; i < leftArray.Length; i++)
                Assert.AreEqual(leftArray[i], rightArray[i]);
        }
    }
}
