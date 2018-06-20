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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using TestDataFramework.Exceptions;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.ListOperations.Interfaces;
using TestDataFramework.Populator.Concrete;

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
    }
}