/*
    Copyright 2016 Alexander Kuperman

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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Exceptions;
using TestDataFramework.ListOperations;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;

namespace Tests.Tests.ImmediateTests
{
    [TestClass]
    public class GuaranteedValuesTests
    {
        [TestMethod]
        public void NeitherPercentageNorTotalGiven_Exception_Test()
        {
            var valueGuaranteePopulator = new ValueGuaranteePopulator();

            var values = new List<GuaranteedValues<object>> {new GuaranteedValues<object>()};

            Helpers.ExceptionTest(() => valueGuaranteePopulator.Bind(null, values), typeof(ValueGuaranteeException),
                Messages.NeitherPercentageNorTotalGiven);
        }

        [TestMethod]
        public void GuaranteedTypeNotOfListType_Exception_ByLiteralValue_Test()
        {
            var valueGuaranteePopulator = new ValueGuaranteePopulator();

            var values = new List<GuaranteedValues<int>>
            {
                new GuaranteedValues<int>
                {
                    TotalFrequency = 5,
                    Values = new object[] {1, 2, "Hello", 4}
                }
            };

            var operableList = new OperableList<int>(null)
            {
                new RecordReference<int>(null, null),
                new RecordReference<int>(null, null),
                new RecordReference<int>(null, null),
                new RecordReference<int>(null, null),
                new RecordReference<int>(null, null),
            };

            Helpers.ExceptionTest(() => valueGuaranteePopulator.Bind(operableList, values),
                typeof(ValueGuaranteeException),
                string.Format(Messages.GuaranteedTypeNotOfListType, "System.Int32", "System.String", "Hello"));
        }

        [TestMethod]
        public void GuaranteedTypeNotOfListType_Exception_ByDelegate_Test()
        {
            var valueGuaranteePopulator = new ValueGuaranteePopulator();

            var values = new List<GuaranteedValues<int>>
            {
                new GuaranteedValues<int>
                {
                    TotalFrequency = 5,
                    Values = new object[] {1, 2, (Func<string>)(() => "Hello"), 4}
                }
            };

            var operableList = new OperableList<int>(null)
            {
                new RecordReference<int>(null, null),
                new RecordReference<int>(null, null),
                new RecordReference<int>(null, null),
                new RecordReference<int>(null, null),
                new RecordReference<int>(null, null),
            };

            Helpers.ExceptionTest(() => valueGuaranteePopulator.Bind(operableList, values),
                typeof(ValueGuaranteeException),
                string.Format(Messages.GuaranteedTypeNotOfListType, "System.Int32", "System.String", "Hello"));
        }

        [TestMethod]
        public void GuaranteedTypeNotOfListType_Exception_DelegateThrowsException_Test()
        {
            var valueGuaranteePopulator = new ValueGuaranteePopulator();

            var values = new List<GuaranteedValues<int>>
            {
                new GuaranteedValues<int>
                {
                    TotalFrequency = 5,
                    Values = new object[] {1, 2, (Func<string>)(() => { throw new NotImplementedException(); }), 4}
                }
            };

            var operableList = new OperableList<int>(null)
            {
                new RecordReference<int>(null, null),
                new RecordReference<int>(null, null),
                new RecordReference<int>(null, null),
                new RecordReference<int>(null, null),
                new RecordReference<int>(null, null),
            };

            Helpers.ExceptionTest(() => valueGuaranteePopulator.Bind(operableList, values),
                typeof(ValueGuaranteeException),
                string.Format(Messages.GuaranteedTypeNotOfListType, "System.Int32", "System.String", 
                "Value func evaluation resulted in exception: System.NotImplementedException, "+
                "Message: The method or operation is not implemented.,"+
                " Stack trace:    at Tests.Tests.ImmediateTests.GuaranteedValuesTests.<>c."+
                "<GuaranteedTypeNotOfListType_Exception_DelegateThrowsException_Test>b__3_0() "+
                "in C:\\Users\\Sasha\\documents\\visual studio 2015\\Projects\\TestDataFramework\\Tests\\Tests\\ImmediateTests\\"+
                "GuaranteedValuesTests.cs:line 91\r\n   at TestDataFramework.ListOperations.ValueGuaranteePopulator.GetValue(Func`1 objectFunc) "+
                "in C:\\Users\\Sasha\\documents\\visual studio 2015\\Projects\\TestDataFramework\\TestDataFramework\\"+
                "ListOperations\\ValueGuaranteePopulator.cs:line 110"));
        }
    }
}
