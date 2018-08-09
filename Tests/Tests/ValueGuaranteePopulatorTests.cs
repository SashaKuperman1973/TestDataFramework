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
using Moq;
using System.Collections.Generic;
using System.Linq;
using TestDataFramework.DeepSetting;
using TestDataFramework.Exceptions;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.ListOperations.Interfaces;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Concrete.OperableList;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class ValueGuaranteePopulatorTests
    {
        private Mock<IValueGauranteePopulatorContextService> contextServiceMock;

        [TestInitialize]
        public void Initialize()
        {
            this.contextServiceMock = new Mock<IValueGauranteePopulatorContextService>();

            this.contextServiceMock
                .Setup(m => m.FilterInWorkingListOfReferfences(It.IsAny<OperableList<SubjectClass>>(),
                    It.IsAny<List<GuaranteedValues>>()))
                .Returns<OperableList<SubjectClass>, List<GuaranteedValues>>(
                    (references, values) => references.ToList())
                .Verifiable();
        }

        [TestMethod]
        public void NeitherPercentageNorTotalGiven_Exception_Test()
        {
            var valueGuaranteePopulator = new ValueGuaranteePopulator();

            var values = new List<GuaranteedValues> {new GuaranteedValues()};

            Helpers.ExceptionTest(() => valueGuaranteePopulator.Bind<object>(null, values, this.contextServiceMock.Object),
                typeof(ValueGuaranteeException),
                Messages.NeitherPercentageNorTotalGiven);
        }

        [TestMethod]
        public void Bind_Test()
        {
            var valuesByPercentage1 = new GuaranteedValues
            {
                FrequencyPercentage = 15,
                Values = new object[] { (Func<SubjectClass>)(() => new SubjectClass()), new SubjectClass(), new SubjectClass(), },
                ValueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall
            };

            var valuesByPercentage2 = new GuaranteedValues
            {
                FrequencyPercentage = 15,
                Values = new object[] { new SubjectClass(), (Func<SubjectClass>)(() => new SubjectClass()), new SubjectClass(), },
                ValueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall
            };

            var valuesByFixedAmount1 = new GuaranteedValues
            {
                TotalFrequency= 4,
                Values = new object[] { new SubjectClass(), new SubjectClass(), (Func<SubjectClass>)(() => new SubjectClass()), },
                ValueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall
            };

            var valuesByFixedAmount2 = new GuaranteedValues
            {
                TotalFrequency = 5,
                Values = new object[] { new SubjectClass(), new SubjectClass(), new SubjectClass(), },
                ValueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall

            };

            var guaranteedValues = new List<GuaranteedValues>
            {
                valuesByPercentage1,
                valuesByPercentage2,
                valuesByFixedAmount1,
                valuesByFixedAmount2,
            };

            var operableList = Helpers.GetObject<OperableList<SubjectClass>>();

            for (int i = 0; i < 20; i++)
            {
                var reference = new RecordReference<SubjectClass>(null, null, null, null, null, null);

                if (i == 3 || i == 7 || i == 15)
                {
                    reference.ExplicitPropertySetters.Add(new ExplicitPropertySetter());
                }

                operableList.AddItem(reference);
            }

            var results = new List<Tuple<RecordReference<SubjectClass>, object>>();

            this.contextServiceMock
                .Setup(m => m.SetRecordReference(It.IsAny<RecordReference<SubjectClass>>(), It.IsAny<object>()))
                .Callback<RecordReference<SubjectClass>, object>(
                    (reference, value) => results.Add(
                        new Tuple<RecordReference<SubjectClass>, object>(reference, value)));
                

            var valueGuaranteePopulator = new ValueGuaranteePopulator();

            // Act

            valueGuaranteePopulator.Bind(operableList, guaranteedValues, this.contextServiceMock.Object);

            // Assert

            Assert.AreEqual(15, results.Count);
        }

        [TestMethod]
        public void Bind_Frequency_LessThan_RequestCount_Throws_Test()
        {
            this.Bind_Frequency_LessThan_RequestCount_Test(ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall, frequencyPercentage: 10);

            this.Bind_Frequency_LessThan_RequestCount_Test(ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall, totalFrequency: 2);

            this.Bind_Frequency_LessThan_RequestCount_Test(ValueCountRequestOption.DoNotThrow, frequencyPercentage: 10);

            this.Bind_Frequency_LessThan_RequestCount_Test(ValueCountRequestOption.DoNotThrow, totalFrequency: 2);
        }

        private void Bind_Frequency_LessThan_RequestCount_Test(ValueCountRequestOption valueCountRequestOption, 
            int? frequencyPercentage = null, int? totalFrequency = null)
        {
            var frequency = new GuaranteedValues
            {
                FrequencyPercentage = frequencyPercentage,
                TotalFrequency = totalFrequency,
                Values = new object[] { (Func<SubjectClass>)(() => new SubjectClass()), new SubjectClass(), new SubjectClass(), },
                ValueCountRequestOption = valueCountRequestOption
            };

            var valueGuaranteePopulator = new ValueGuaranteePopulator();

            var operableList = Helpers.GetObject<OperableList<SubjectClass>>();

            for (int i = 0; i < 20; i++)
            {
                var reference = Helpers.GetObject<RecordReference<SubjectClass>>();

                operableList.AddItem(reference);
            }

            // Act/Assert

            if (valueCountRequestOption == ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
            {
                Helpers.ExceptionTest(() =>
                        valueGuaranteePopulator.Bind(operableList, new[] {frequency},
                            this.contextServiceMock.Object),
                    typeof(ValueGuaranteeException),
                    Messages.PercentFrequencyTooSmall.Substring(0, Messages.PercentFrequencyTooSmall.IndexOf('.')),
                    MessageOption.MessageStartsWith);

                return;
            }

            valueGuaranteePopulator.Bind(operableList, new[] { frequency }, this.contextServiceMock.Object);

            this.contextServiceMock.Verify();
        }

        [TestMethod]
        public void Bind_Total_RequestCount_Overflows_Available_Elements_Test()
        {
            var guaranteedValues = new GuaranteedValues
            {
                TotalFrequency = 21,
                Values = new object[] { (Func<SubjectClass>)(() => new SubjectClass()), new SubjectClass(), new SubjectClass(), },
                ValueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall
            };

            var valueGuaranteePopulator = new ValueGuaranteePopulator();

            var operableList = Helpers.GetObject<OperableList<SubjectClass>>();

            for (int i = 0; i < 20; i++)
            {
                var reference = new RecordReference<SubjectClass>(null, null, null, null, null, null);

                operableList.AddItem(reference);
            }

            // Act/Assert

            Helpers.ExceptionTest(() =>
                    valueGuaranteePopulator.Bind(operableList, new[] {guaranteedValues}, this.contextServiceMock.Object),
                typeof(ValueGuaranteeException), Messages.TooFewReferencesForValueGuarantee);
        }
    }
}
