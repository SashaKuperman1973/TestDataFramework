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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using TestDataFramework.DeepSetting;
using TestDataFramework.Exceptions;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.ListOperations.Interfaces;
using TestDataFramework.Populator.Concrete;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class ValueGuaranteePopulatorTests
    {
        private Mock<IValueGauranteePopulatorContextService> contextService;

        [TestInitialize]
        public void Initialize()
        {
            this.contextService = new Mock<IValueGauranteePopulatorContextService>();
        }

        [TestMethod]
        public void NeitherPercentageNorTotalGiven_Exception_Test()
        {
            var valueGuaranteePopulator = new ValueGuaranteePopulator();

            var values = new List<GuaranteedValues> {new GuaranteedValues()};

            Helpers.ExceptionTest(() => valueGuaranteePopulator.Bind<object>(null, values, this.contextService.Object),
                typeof(ValueGuaranteeException),
                Messages.NeitherPercentageNorTotalGiven);
        }

        [TestMethod]
        public void Bind_Test()
        {
            var valuesByPercentage1 = new GuaranteedValues
            {
                FrequencyPercentage = 10,
                Values = new object[] { (Func<SubjectClass>)(() => new SubjectClass()), new SubjectClass(), new SubjectClass(), },
            };

            var valuesByPercentage2 = new GuaranteedValues
            {
                FrequencyPercentage = 10,
                Values = new object[] { new SubjectClass(), (Func<SubjectClass>)(() => new SubjectClass()), new SubjectClass(), }
            };

            var valuesByFixedAmount1 = new GuaranteedValues
            {
                TotalFrequency= 4,
                Values = new object[] { new SubjectClass(), new SubjectClass(), (Func<SubjectClass>)(() => new SubjectClass()), }
            };

            var valuesByFixedAmount2 = new GuaranteedValues
            {
                TotalFrequency = 5,
                Values = new object[] { new SubjectClass(), new SubjectClass(), new SubjectClass(), }
            };

            var guaranteedValues = new List<GuaranteedValues>
            {
                valuesByPercentage1,
                valuesByPercentage2,
                valuesByFixedAmount1,
                valuesByFixedAmount2,
            };

            var operableList = new OperableList<SubjectClass>(null, null);

            for (int i = 0; i < 20; i++)
            {
                var reference = new RecordReference<SubjectClass>(null, null, null, null, null, null);

                if (i == 3 || i == 7 || i == 15)
                {
                    reference.ExplicitPropertySetters.Add(new ExplicitPropertySetter());
                }

                operableList.InternalList.Add(reference);
            }

            var results = new List<Tuple<RecordReference<SubjectClass>, object>>();

            this.contextService
                .Setup(m => m.SetRecordReference(It.IsAny<RecordReference<SubjectClass>>(), It.IsAny<object>()))
                .Callback<RecordReference<SubjectClass>, object>(
                    (reference, value) => results.Add(
                        new Tuple<RecordReference<SubjectClass>, object>(reference, value)));
                

            var valueGuaranteePopulator = new ValueGuaranteePopulator();

            // Act

            valueGuaranteePopulator.Bind(operableList, guaranteedValues, this.contextService.Object);

            // Assert

            Assert.AreEqual(13, results.Count);

            //int c = results.Count(r => values r.Item2 );

            //this.contextService.Verify(m => m.SetRecordReference(It.IsAny<RecordReference<SubjectClass>>(), It.IsAny<SubjectClass>()), tim);
        }
    }
}