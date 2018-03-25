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

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;
using Tests.TestModels;

namespace Tests.Tests.ImmediateTests
{
    [TestClass]
    public class OperableListTests
    {
        private Mock<BasePopulator> populatorMock;

        [TestInitialize]
        public void Initialize()
        {
            this.populatorMock = new Mock<BasePopulator>(null);
        }

        [TestMethod]
        public void BindAndMake_Test()
        {
            // Arrange

            var operableList = new OperableList<SubjectClass>(null, this.populatorMock.Object);
            var subjectReference = new RecordReference<SubjectClass>(null, null, null, null, null);
            operableList.Add(subjectReference);

            // Act

            IEnumerable<SubjectClass> resultSet = operableList.BindAndMake();

            // Assert

            this.populatorMock.Verify(m => m.Bind());
        }

        [TestMethod]
        public void Make_Test()
        {
            // Arrange

            var operableList = new OperableList<SubjectClass>(null, this.populatorMock.Object);

            // Act

            IEnumerable<SubjectClass> resultSet = operableList.Make();

            // Assert

            this.populatorMock.Verify(m => m.Bind(operableList));
        }
    }
}
