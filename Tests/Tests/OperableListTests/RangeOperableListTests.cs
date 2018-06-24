using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.TypeGenerator.Interfaces;
using Tests.TestModels;
using Range = TestDataFramework.Populator.Concrete.Range;

namespace Tests.Tests.OperableListTests
{
    [TestClass]
    public class SizeOperableListTests
    {
        private const int ListSize = 20;
        private Mock<IAttributeDecorator> attributeDecoratorMock;
        private Mock<DeepCollectionSettingConverter> deepCollectionSettingConverterMock;
        private Mock<IObjectGraphService> objectGraphServiceMock;
        private Mock<BasePopulator> populatorMock;
        private OperableList<SubjectClass> operableList;
        private Mock<ITypeGenerator> typeGeneratorMock;

        private Mock<ValueGuaranteePopulator> valueGuaranteePopulatorMock;

        [TestInitialize]
        public void Initialize()
        {
            this.valueGuaranteePopulatorMock = new Mock<ValueGuaranteePopulator>();
            this.populatorMock = new Mock<BasePopulator>(null);
            this.typeGeneratorMock = new Mock<ITypeGenerator>();
            this.attributeDecoratorMock = new Mock<IAttributeDecorator>();
            this.objectGraphServiceMock = new Mock<IObjectGraphService>();
            this.deepCollectionSettingConverterMock = new Mock<DeepCollectionSettingConverter>();

            this.operableList = new OperableList<SubjectClass>(
                SizeOperableListTests.ListSize,
                this.valueGuaranteePopulatorMock.Object,
                this.populatorMock.Object,
                this.typeGeneratorMock.Object,
                this.attributeDecoratorMock.Object,
                this.objectGraphServiceMock.Object,
                this.deepCollectionSettingConverterMock.Object
            );
        }

        [TestMethod]
        public void Set_SetsProperty_Test()
        {
            // Arrange

            this.objectGraphServiceMock.Setup(m => m.GetObjectGraph<SubjectClass, int>(p => p.Integer))
                .Returns(new List<PropertyInfo> {typeof(SubjectClass).GetProperty(nameof(SubjectClass.Integer))});

            // Act

            var range1 = Range.StartAndEndPositions(3, 5);
            var range2 = Range.StartAndEndPositions(7, 9);
            var range3 = Range.StartAndEndPositions(13, 17);

            this.operableList.Set(p => p.Integer, () => 7, range1, range2, range3);

            List<RecordReference<SubjectClass>> internalList = this.operableList.InternalList;
            SizeOperableListTests.CheckRecordReference(internalList[3]);
        }

        private static void CheckRecordReference(RecordReference<SubjectClass> recordReference)
        {
            var subject = new SubjectClass();
            recordReference.ExplicitPropertySetters.First().Action(subject);
            Assert.AreEqual(7, subject.Integer);
        }

        [TestMethod]
        public void Set_NoRanges_Test()
        {
            Helpers.ExceptionTest(() => this.operableList.Set(null, () => 1), typeof(ArgumentException),
                Messages.NoRangeOperableListPositionsPassedIn + "\r\nParameter name: ranges");
        }

        [TestMethod]
        public void Set_ValidateAgainstUpperBoundary_Test()
        {
            Helpers.ExceptionTest(() =>
                    this.operableList.Set(null, () => 1,
                        Range.StartAndEndPositions(SizeOperableListTests.ListSize - 3, SizeOperableListTests.ListSize)
                    ),
                typeof(ArgumentOutOfRangeException),
                "Specified argument was out of the range of valid values.\r\nParameter name: ranges"
            );
        }

        [TestMethod]
        public void Set_ValidateAgainstLowerBoundary_Test()
        {
            Helpers.ExceptionTest(() =>
                    this.operableList.Set(null, () => 1, Range.StartAndEndPositions(-2, 3)),
                typeof(ArgumentOutOfRangeException),
                "Specified argument was out of the range of valid values.\r\nParameter name: ranges"
            );
        }

        [TestMethod]
        public void Set_BoundaryConditions_Test()
        {
            this.operableList.Set(p => p.Integer, () => 1, Range.StartAndEndPositions(0, SizeOperableListTests.ListSize - 1));
        }

        [TestMethod]
        public void Set_TProperty_Test()
        {
            // Arrange

            this.objectGraphServiceMock.Setup(m => m.GetObjectGraph<SubjectClass, int>(p => p.Integer))
                .Returns(new List<PropertyInfo> { typeof(SubjectClass).GetProperty(nameof(SubjectClass.Integer)) });

            // Act

            this.operableList.Set(p => p.Integer, 7, 0);

            List<RecordReference<SubjectClass>> internalList = this.operableList.InternalList;
            SizeOperableListTests.CheckRecordReference(internalList[0]);
        }

        [TestMethod]
        public void Set_Return_FieldExpression_Test()
        {
            // Act

            FieldExpression<SubjectClass, int> resultFieldExpression = this.operableList.Set(m => m.Integer);

            // Assert

            Assert.IsNotNull(resultFieldExpression);
        }
    }
}