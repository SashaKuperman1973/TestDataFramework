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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.TypeGenerator.Interfaces;
using Tests.TestModels;

namespace Tests.Tests.OperableListTests.OperableList
{
    public class TestContext
    {
        public OperableList<ElementType> CreateOperableList() =>
            new OperableList<ElementType>(this.InputMocks.Select(m => m.Object), 
                this.MockValueGuaranteePopulator.Object,
                null,
                null,
                null,
                null,
                null
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
                        this.MockTypeGenerator.Object,
                        null,
                        null,
                        null,
                        null,
                        null
                        ));

                this.inputMocks = result;
                return result;
            }
        }

        public Mock<ValueGuaranteePopulator> MockValueGuaranteePopulator;
        public Mock<ITypeGenerator> MockTypeGenerator;
        
        public TestContext()
        {
            this.MockValueGuaranteePopulator = new Mock<ValueGuaranteePopulator>();
            this.MockTypeGenerator = new Mock<ITypeGenerator>();
        }

        private static bool Check(IEnumerable<GuaranteedValues> guaranteedValues, object[] expected, int count, ValueCountRequestOption option)
        {
            GuaranteedValues guaranteedValue = guaranteedValues.First();

            Helpers.AssertSetsAreEqual(guaranteedValue.Values, expected);
            bool result = guaranteedValue.TotalFrequency == count &&
                          guaranteedValue.ValueCountRequestOption == option;

            return result;
        }

        public void DoAssert(OperableList<ElementType> operableList, OperableList<ElementType> actual,
            object[] guaranteedValues, int count, ValueCountRequestOption option)
        {
            Assert.AreEqual(operableList, actual);

            this.MockValueGuaranteePopulator.Verify(m => m.Bind(operableList,
                It.Is<IEnumerable<GuaranteedValues>>(n => TestContext.Check(n, guaranteedValues, count, option)),
                It.IsAny<ValueSetContextService>()));
        }
    }
}
