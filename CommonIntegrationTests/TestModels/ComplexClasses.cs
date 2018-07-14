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

using System.Collections.Generic;

namespace CommonIntegrationTests.TestModels
{
    public class DeepA
    {
        public int Integer { get; set; }

        public DeepB DeepB { get; set; }
    }

    public class DeepB
    {
        public string String { get; set; }

        public DeepC DeepC { get; set; }

        public List<DeepA> DeepAList { get; set; }

        public List<DeepC> DeepCList { get; set; }
    }

    public class DeepC
    {
        public string DeepString { get; set; }

        public List<int> IntList { get; set; }

        public List<DeepD> DeepDList { get; set; }
    }

    public class DeepD
    {
        public int Integer { get; set; }
    }
}