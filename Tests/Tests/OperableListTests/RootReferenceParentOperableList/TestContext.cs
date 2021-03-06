﻿/*
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

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Concrete.OperableList;
using TestDataFramework.TypeGenerator.Interfaces;
using Tests.TestModels;

namespace Tests.Tests.OperableListTests.RootReferenceParentOperableList
{
    public class TestContext
    {
        public RootReferenceParentOperableList<ElementType, ElementParentType> CreateOperableList() =>
            new RootReferenceParentOperableList<ElementType,ElementParentType>(
                this.RootMock.Object,
                this.Inputs,
                this.ValueGuaranteePopulatorMock.Object,
                this.PopulatorMock.Object,
                this.ObjectGraphServiceMock.Object,
                null,
                null,
                this.TypeGeneratorMock.Object,
                isShallowCopy: false
                );

        private List<Mock<RecordReference<ElementType>>> inputMocks;

        public List<Mock<RecordReference<ElementType>>> InputMocks
        {
            get
            {
                if (this.inputMocks != null) return this.inputMocks;

                var result = new List<Mock<RecordReference<ElementType>>>(3);

                for (int i=0; i<3; i++)
                    result.Add(new Mock<RecordReference<ElementType>>(
                        this.TypeGeneratorMock.Object,
                        null,
                        null,
                        this.ObjectGraphServiceMock.Object,
                        null,
                        null
                        ));

                this.inputMocks = result;
                return result;
            }
        }

        public List<RecordReference<ElementType>> Inputs => this.InputMocks.Select(i => i.Object).ToList();

        public Mock<BasePopulator> PopulatorMock;
        public Mock<RecordReference<ElementParentType>> RootMock;
        public Mock<ValueGuaranteePopulator> ValueGuaranteePopulatorMock;
        public Mock<ITypeGenerator> TypeGeneratorMock;
        public Mock<IObjectGraphService> ObjectGraphServiceMock;

        public TestContext()
        {
            this.ValueGuaranteePopulatorMock = new Mock<ValueGuaranteePopulator>();
            this.TypeGeneratorMock = new Mock<ITypeGenerator>();
            this.RootMock = Helpers.GetMock<RecordReference<ElementParentType>>();
            this.PopulatorMock = Helpers.GetMock<BasePopulator>();
            this.ObjectGraphServiceMock = new Mock<IObjectGraphService>();
        }

        private static bool Check(IEnumerable<GuaranteedValues> guaranteedValues,
            IEnumerable<object> expectedGuaranteedValues, int? total, int? percentage,
            ValueCountRequestOption option)
        {
            GuaranteedValues guaranteedValue = guaranteedValues.First();

            Helpers.AssertSetsAreEqual(expectedGuaranteedValues, guaranteedValue.Values);

            bool result = guaranteedValue.FrequencyPercentage == percentage &&
                          guaranteedValue.TotalFrequency == total &&
                          guaranteedValue.ValueCountRequestOption == option &&

                          guaranteedValue.Values != null;

            return result;
        }

        public void AssertPercentage(
            OperableList<ElementType> operableList,

            OperableList<ElementType> returnResult,

            object[] guaranteedValues,

            int percentage,
            ValueCountRequestOption option)
        {
            this.DoAssert(operableList, returnResult, guaranteedValues, null, percentage, option);
        }

        public void AssertTotal(
            OperableList<ElementType> operableList,

            OperableList<ElementType> returnResult,

            object[] guaranteedValues,

            int total,
            ValueCountRequestOption option)
        {
            this.DoAssert(operableList, returnResult, guaranteedValues, total, null, option);
        }

        private void DoAssert(
            OperableList<ElementType> operableList,

            OperableList<ElementType> returnResult,

            object[] guaranteedValues,

            int? total, int? percentage,
            ValueCountRequestOption option
        )
        {
            Assert.IsNotNull(returnResult);
            Assert.AreEqual(operableList, returnResult);

            this.ValueGuaranteePopulatorMock.Verify(m => m.Bind(operableList,
                It.Is<IEnumerable<GuaranteedValues>>(n => TestContext.Check(n, guaranteedValues, total, percentage, option)),
                It.IsAny<ValueSetContextService>()));
        }
    }
}
