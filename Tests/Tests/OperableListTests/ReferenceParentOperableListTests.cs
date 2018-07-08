using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    public class ReferenceParentOperableListTests
    {
        private ReferenceParentOperableList<ElementType, SubjectClass> operableList;

        private Mock<RecordReference<SubjectClass>> parentRefernceMock;
        private Mock<IObjectGraphService> objecGraphServcieMock;
        private List<Mock<RecordReference<ElementType>>> contentReferenceMocks;

        [TestInitialize]
        public void Initialize()
        {
            this.parentRefernceMock = new Mock<RecordReference<SubjectClass>>(null, null, null, null, null, null);

            this.operableList =
                new ReferenceParentOperableList<ElementType, SubjectClass>(this.parentRefernceMock.Object, null, null, null, null, null, null);

            this.objecGraphServcieMock = new Mock<IObjectGraphService>();

            this.contentReferenceMocks = new List<Mock<RecordReference<ElementType>>>
            {
                new Mock<RecordReference<ElementType>>(null, null, null, this.objecGraphServcieMock.Object, null, null),
                new Mock<RecordReference<ElementType>>(null, null, null, this.objecGraphServcieMock.Object, null, null),
                new Mock<RecordReference<ElementType>>(null, null, null, this.objecGraphServcieMock.Object, null, null),
            };

            this.contentReferenceMocks.ForEach(mock => this.operableList.Add(mock.Object));
        }

        [TestMethod]
        public void Make_Test()
        {
            // Act

            this.operableList.Make();

            // Assert

            this.parentRefernceMock.Verify(m => m.Make());
        }

        [TestMethod]
        public void BindAndMake_Test()
        {
            // Act

            this.operableList.BindAndMake();

            // Assert

            this.parentRefernceMock.Verify(m => m.BindAndMake());
        }

        [TestMethod]
        public void Set_PropertyValue_Test()
        {
            // Arrange

            var value = new ElementType.PropertyType();

            var setExpression = (Expression<Func<ElementType, ElementType.PropertyType>>)(m => m.AProperty);

            // Act

            this.operableList.Set(setExpression, value);

            // Assert

            this.contentReferenceMocks.ForEach(mock => mock.Verify(m => m.Set(setExpression,
                It.Is<Func<ElementType.PropertyType>>(propertyValueFunc => propertyValueFunc() == value))));
        }

        [TestMethod]
        public void Set_PropertyByFunc_Test()
        {
            // Arrange

            var value = new ElementType.PropertyType();

            var setExpression = (Expression<Func<ElementType, ElementType.PropertyType>>)(m => m.AProperty);
            Func<ElementType.PropertyType> valueFactory = () => value;

            // Act

            this.operableList.Set(setExpression, valueFactory);

            // Assert

            this.contentReferenceMocks.ForEach(mock => mock.Verify(m => m.Set(setExpression, valueFactory)));
        }

        [TestMethod]
        public void Set_GetFieldExpression_Test()
        {
            // Act

            ReferenceParentFieldExpression<ElementType, ElementType.PropertyType, SubjectClass> result =
                this.operableList.Set(m => m.AProperty);

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

            ReferenceParentOperableList<ElementType, SubjectClass> result = this.operableList.Take(3);

            //

            Helpers.AssertSetsAreEqual(this.contentReferenceMocks.Take(3).Select(m => m.Object), result);
        }

        [TestMethod]
        public void Skip_Test()
        {
            // Act

            ReferenceParentOperableList<ElementType, SubjectClass> result = this.operableList.Skip(1);

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

            ReferenceParentMakeableEnumerable<ReferenceParentOperableList<ElementType.PropertyType, SubjectClass>,
                SubjectClass> result =
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

            ReferenceParentOperableList<ElementType, SubjectClass> result = this.operableList.GuaranteeByPercentageOfTotal(values);

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

            ReferenceParentOperableList<ElementType, SubjectClass> result = this.operableList.GuaranteeByPercentageOfTotal(values, 25);

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

            ReferenceParentOperableList<ElementType, SubjectClass> result = this.operableList.GuaranteeByPercentageOfTotal(values);

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

            ReferenceParentOperableList<ElementType, SubjectClass> result = this.operableList.GuaranteeByPercentageOfTotal(values, 25);

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

            ReferenceParentOperableList<ElementType, SubjectClass> result = this.operableList.GuaranteeByPercentageOfTotal(values);

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

            ReferenceParentOperableList<ElementType, SubjectClass> result = this.operableList.GuaranteeByPercentageOfTotal(values, 25);

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

            ReferenceParentOperableList<ElementType, SubjectClass> result = this.operableList.GuaranteeByFixedQuantity(values);

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

            ReferenceParentOperableList<ElementType, SubjectClass> result = this.operableList.GuaranteeByFixedQuantity(values, 25);

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

            ReferenceParentOperableList<ElementType, SubjectClass> result = this.operableList.GuaranteeByFixedQuantity(values);

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

            ReferenceParentOperableList<ElementType, SubjectClass> result = this.operableList.GuaranteeByFixedQuantity(values, 25);

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

            ReferenceParentOperableList<ElementType, SubjectClass> result = this.operableList.GuaranteeByFixedQuantity(values);

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

            ReferenceParentOperableList<ElementType, SubjectClass> result = this.operableList.GuaranteeByFixedQuantity(values, 25);

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
