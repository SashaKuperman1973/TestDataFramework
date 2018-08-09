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
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class DataTests
    {
        private Data<SubjectClass> data;
        private DeferredValueGetterDelegate<SubjectClass> valueGetter;
        private SubjectClass subjectClass;

        [TestInitialize]
        public void Initialiaze()
        {
            this.subjectClass = new SubjectClass();
            this.valueGetter = input => this.subjectClass;
            this.data = new Data<SubjectClass>(this.valueGetter);
        }

        [TestMethod]
        public void ValueGetter_IsPopulated_Test()
        {
            Assert.IsNotNull(this.data.ValueGetter);
        }

        [TestMethod]
        public void ToString_Test()
        {
            this.data.Item = new SubjectClass();
            Assert.IsTrue(!string.IsNullOrWhiteSpace(this.data.ToString()));
        }
    }
}
