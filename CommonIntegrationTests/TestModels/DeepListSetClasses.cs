﻿/*
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
    public class ListSetterBaseType
    {
        public ListSetterBaseTypeB B { get; set; }

        public int AnInt { get; set; }
    }

    public class ListSetterBaseTypeB
    {
        public ListType WithCollection { get; set; }

        public int AnInt { get; set; }
    }

    public class ListType
    {
        public List<ListElementType> ElementList { get; set; }

        public ListElementType[] ElementArray { get; set; }
    }

    public class ListElementType
    {
        public string AString { get; set; }

        public SubElementType SubElement { get; set; }
    }

    public class SubElementType
    {
        public string AString { get; set; }

        public int AnInt { get; set; }
    }
}