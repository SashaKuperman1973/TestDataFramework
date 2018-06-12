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
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Exceptions;
using TestDataFramework.Factories;
using TestDataFramework.ListOperations;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Interfaces;

namespace Tests.Tests.ImmediateTests
{
    [TestClass]
    public class ValueGuaranteePopulatorTests
    {
        [TestMethod]
        public void NeitherPercentageNorTotalGiven_Exception_Test()
        {
            var valueGuaranteePopulator = new ValueGuaranteePopulator();

            var values = new List<GuaranteedValues> {new GuaranteedValues()};

            Helpers.ExceptionTest(() => valueGuaranteePopulator.Bind<object>(null, values),
                typeof(ValueGuaranteeException),
                Messages.NeitherPercentageNorTotalGiven);
        }

        [TestMethod]
        public void GuaranteedTypeNotOfListType_Exception_ByLiteralValue_Test()
        {
            var valueGuaranteePopulator = new ValueGuaranteePopulator();

            var values = new List<GuaranteedValues>
            {
                new GuaranteedValues
                {
                    TotalFrequency = 5,
                    Values = new object[] {1, 2, "Hello", 4}
                }
            };

            var operableList = new OperableList<int>(null, null)
            {
                new RecordReference<int>(null, null, null, null, null, null),
                new RecordReference<int>(null, null, null, null, null, null),
                new RecordReference<int>(null, null, null, null, null, null),
                new RecordReference<int>(null, null, null, null, null, null),
                new RecordReference<int>(null, null, null, null, null, null)
            };

            Helpers.ExceptionTest(() => valueGuaranteePopulator.Bind(operableList, values),
                typeof(ValueGuaranteeException),
                string.Format(Messages.GuaranteedTypeNotOfListType, "System.Int32", "System.String", "Hello"));
        }

        [TestMethod]
        public void GuaranteedTypeNotOfListType_Exception_ByDelegate_Test()
        {
            var valueGuaranteePopulator = new ValueGuaranteePopulator();

            var values = new List<GuaranteedValues>
            {
                new GuaranteedValues
                {
                    TotalFrequency = 5,
                    Values = new object[] {1, 2, (Func<string>) (() => "Hello"), 4}
                }
            };

            var operableList = new OperableList<int>(null, null)
            {
                new RecordReference<int>(null, null, null, null, null, null),
                new RecordReference<int>(null, null, null, null, null, null),
                new RecordReference<int>(null, null, null, null, null, null),
                new RecordReference<int>(null, null, null, null, null, null),
                new RecordReference<int>(null, null, null, null, null, null)
            };

            Helpers.ExceptionTest(() => valueGuaranteePopulator.Bind(operableList, values),
                typeof(ValueGuaranteeException),
                string.Format(Messages.GuaranteedTypeNotOfListType, "System.Int32", "System.String", "Hello"));
        }

        [TestMethod]
        public void TotalFrequency_LessThan_RequiredValues_LessThan_TotalListElements()
        {
            var guaranteedValuesSet = new[]
            {
                new GuaranteedValues
                {
                    TotalFrequency = 10
                },
                new GuaranteedValues
                {
                    FrequencyPercentage = 25
                }
            };

            ValueGuaranteePopulatorTests.Test(22, guaranteedValuesSet);
        }

        [TestMethod]
        public void BoundaryCondition_TotalFrequency_EqualTo_RequiredValues_LessThan_TotalListElements()
        {
            var guaranteedValuesSet = new[]
            {
                new GuaranteedValues
                {
                    TotalFrequency = 10
                },
                new GuaranteedValues
                {
                    FrequencyPercentage = 80
                }
            };

            ValueGuaranteePopulatorTests.Test(50, guaranteedValuesSet);
        }

        [TestMethod]
        public void ValueGuaranteeException_TooFewReferences()
        {
            var guaranteedValuesSet = new[]
            {
                new GuaranteedValues
                {
                    TotalFrequency = 8
                },
                new GuaranteedValues
                {
                    FrequencyPercentage = 86
                }
            };

            Helpers.ExceptionTest(() => ValueGuaranteePopulatorTests.Test(-1, guaranteedValuesSet),
                typeof(ValueGuaranteeException));
        }

        private static void Test(int expectedCount, params GuaranteedValues[] guaranteedValuesSet)
        {
            IPopulator populator = StaticPopulatorFactory.CreateMemoryPopulator();

            var valueGuaranteePopulator = new ValueGuaranteePopulator();

            List<RecordReference<int>> setOfInts = populator.Add<int>(50).ToList();

            var operableList = new OperableList<int>(setOfInts, valueGuaranteePopulator, null);

            var values = new List<int>();
            guaranteedValuesSet.ToList().ForEach(guaranteedValues =>
            {
                List<int> innerValues = populator.Add<int>(10).Make().ToList();
                guaranteedValues.Values = innerValues.Cast<object>();
                values.AddRange(innerValues);
            });

            // Act

            valueGuaranteePopulator.Bind(operableList, guaranteedValuesSet.ToList());

            // Assert

            List<int?> subjectValues = operableList.Where(reference => reference.RecordObject != default(int))
                .Select(reference => (int?) reference.RecordObject).ToList();

            var found = 0;
            int index;
            values.ForEach(value =>
            {
                while ((index = subjectValues.IndexOf(value)) > -1)
                {
                    found++;
                    subjectValues.RemoveAt(index);
                }
            });

            Assert.AreEqual(expectedCount, found);
        }
    }
}