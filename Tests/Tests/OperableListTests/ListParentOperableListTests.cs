using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Moq;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Concrete.FieldExpression;
using TestDataFramework.Populator.Concrete.MakeableEnumerable;
using TestDataFramework.Populator.Concrete.OperableList;
using Tests.TestModels;

namespace Tests.Tests.OperableListTests
{
    [TestClass]
    public class ListParentOperableListTests
    {
        private ListParentOperableList<ElementType> operableList;
        private List<Mock<RecordReference<ElementType>>> contentReferenceMocks;

        private Mock<IObjectGraphService> objecGraphServcieMock;
        private List<ElementType> elementTypeSet;

        [TestInitialize]
        public void Initialize()
        {
            this.objecGraphServcieMock = new Mock<IObjectGraphService>();

            this.operableList = new ListParentOperableList<ElementType>(null, null, null, null, this.objecGraphServcieMock.Object, null);

            this.contentReferenceMocks = new List<Mock<RecordReference<ElementType>>>
            {
                new Mock<RecordReference<ElementType>>(null, null, null, this.objecGraphServcieMock.Object, null, null),
                new Mock<RecordReference<ElementType>>(null, null, null, this.objecGraphServcieMock.Object, null, null),
                new Mock<RecordReference<ElementType>>(null, null, null, this.objecGraphServcieMock.Object, null, null),
            };

            this.elementTypeSet = new List<ElementType>();
            int count = 0;
            this.contentReferenceMocks.ForEach(mock =>
            {
                this.elementTypeSet.Add(new ElementType
                {
                    AProperty = new ElementType.PropertyType(),
                    AnEnumerable = new ElementType.PropertyType[0]
                });
                this.operableList.Add(mock.Object);
                mock.SetupGet(m => m.RecordObject).Returns(this.elementTypeSet[count++]);
            });
        }

        [TestMethod]
        public void Set_PropertyValue_Test()
        {
            // Arrange

            var value = new ElementType.PropertyType();

            var setExpression = (Expression<Func<ElementType, ElementType.PropertyType>>)(m => m.AProperty);

            // Act

            ListParentOperableList<ElementType> result = this.operableList.Set(setExpression, value);

            // Assert

            this.contentReferenceMocks.ForEach(mock => mock.Verify(m => m.Set(setExpression,
                It.Is<Func<ElementType.PropertyType>>(propertyValueFunc => propertyValueFunc() == value))));

            Assert.AreEqual(this.operableList, result);
        }

        [TestMethod]
        public void Set_PropertyByFunc_Test()
        {
            // Arrange

            var value = new ElementType.PropertyType();

            var setExpression = (Expression<Func<ElementType, ElementType.PropertyType>>)(m => m.AProperty);
            Func<ElementType.PropertyType> valueFactory = () => value;

            // Act

            ListParentOperableList<ElementType> result = this.operableList.Set(setExpression, valueFactory);

            // Assert

            this.contentReferenceMocks.ForEach(mock => mock.Verify(m => m.Set(setExpression, valueFactory)));

            Assert.AreEqual(this.operableList, result);
        }

        [TestMethod]
        public void Set_GetFieldExpression_Test()
        {
            // Act

            ListParentFieldExpression<ElementType, ElementType.PropertyType> result = this.operableList.Set(m => m.AProperty);

            // Assert

            Assert.AreEqual(this.operableList, result.OperableList);
        }

        [TestMethod]
        public void Ignore_Test()
        {
            var ignoreExpression = (Expression<Func<ElementType, ElementType.PropertyType>>)(m => m.AProperty);

            // Act

            OperableList<ElementType> result = this.operableList.Ignore(ignoreExpression);

            // Assert

            this.contentReferenceMocks.ForEach(mock => mock.Verify(m => m.Ignore(ignoreExpression)));

            Assert.AreEqual(this.operableList, result);
        }

        [TestMethod]
        public void Take_Test()
        {
            // Act

            ListParentOperableList<ElementType> result = this.operableList.Take(3);

            //

            Helpers.AssertSetsAreEqual(this.contentReferenceMocks.Take(3).Select(m => m.Object), result);
        }

        [TestMethod]
        public void Skip_Test()
        {
            // Act

            ListParentOperableList<ElementType> result = this.operableList.Skip(1);

            //

            Assert.AreEqual(2, result.Count);
            Helpers.AssertSetsAreEqual(this.contentReferenceMocks.Skip(1).Select(m => m.Object), result);
        }

        [TestMethod]
        public void Select_Test()
        {
            // Arrange

            var selectExpression = (Expression<Func<ElementType, IEnumerable<ElementType.PropertyType>>>)(m => m.AnEnumerable);

            // Act

            ListParentMakeableEnumerable<ListParentOperableList<ElementType.PropertyType>, ElementType> result =
                this.operableList.Select(selectExpression, 4);

            // Assert

            Assert.AreEqual(this.contentReferenceMocks.Count, result.Count);

            this.objecGraphServcieMock.Verify(m => m.GetObjectGraph(selectExpression), Times.Exactly(6));
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

            Helpers.AssertSetsAreEqual(values, guaranteedValues.Values);
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

            Helpers.AssertSetsAreEqual(values, guaranteedValues.Values);
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

            Helpers.AssertSetsAreEqual(values, guaranteedValues.Values);
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

            Helpers.AssertSetsAreEqual(values, guaranteedValues.Values);

            Assert.AreEqual(25, guaranteedValues.TotalFrequency);
            Assert.IsNull(guaranteedValues.FrequencyPercentage);
            Assert.AreEqual(ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall, guaranteedValues.ValueCountRequestOption);

            Assert.AreEqual(this.operableList, result);
        }
    }
}
