/*
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.DeepSetting;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator.Concrete;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class ValueSetContextServiceTests
    {
        [TestMethod]
        public void SetRecordReference_Test()
        {
            var service = ValueSetContextService.Instance;

            var input = new RecordReference<SubjectClass>(null, null, null, null, null, null);

            service.SetRecordReference(input, new SubjectClass());

            Assert.IsTrue(input.IsPopulated);
        }

        [TestMethod]
        public void FilterInWorkingListOfReferfences_Test()
        {
            var service = ValueSetContextService.Instance;

            var references = new List<RecordReference<SubjectClass>>();

            for (int i = 0; i < 20; i++)
            {
                var reference = new RecordReference<SubjectClass>(null, null, null, null, null, null);
                references.Add(reference);

                if (i == 10 || i == 11 || i == 12)
                {
                    reference.ExplicitPropertySetters.Add(new ExplicitPropertySetter());
                }
            }

            List<RecordReference<SubjectClass>> result = service.FilterInWorkingListOfReferfences(references, null);

            Assert.AreEqual(17, result.Count);
        }
    }
}
