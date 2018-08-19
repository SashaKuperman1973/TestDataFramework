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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.Populator;

namespace Tests.Tests
{
    [TestClass]
    public class PopulatableTest
    {
        private class ConcretePopulatable : Populatable
        {
            internal override void Populate()
            {
                throw new NotImplementedException();
            }

            internal override void AddToReferences(IList<RecordReference> collection)
            {
                throw new NotImplementedException();
            }

            public new void PopulateChildren()
            {
                base.PopulateChildren();
            }
        }

        [TestMethod]
        public void PopulateChildren_Test()
        {
            var populatable = new ConcretePopulatable();

            Mock<Populatable> mockChild1 = Helpers.GetMock<Populatable>();
            Mock<Populatable> mockChild2 = Helpers.GetMock<Populatable>();

            populatable.AddChild(mockChild1.Object);
            populatable.AddChild(mockChild2.Object);

            populatable.PopulateChildren();

            mockChild1.Verify(m => m.Populate());
            mockChild2.Verify(m => m.Populate());
        }
    }
}
