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

namespace DocumentationExamples
{
    public class Subject
    {
        public DeepA DeepA { get; set; }

        public string Text { get; set; }
    }

    public class DeepA
    {
        public string TextA { get; set; }

        public DeepB DeepB { get; set; }

        public List<DeepB> DeepBCollection { get; set; }

        public string[] StringCollection { get; set; }
    }

    public class DeepB
    {
        public string TextC { get; set; }

        public DeepC DeepC { get; set; }

        public DateTime ADateTime { get; set; }

        public List<DeepC> DeepCCollection { get; set; }
    }

    public class DeepC
    {
        public int AnInteger { get; set; }

        public string ATextProperty { get; set; }

        public List<string> DeepCStringCollection { get; set; }
    }

    public class ExclusiveRangeTest
    {
        public int AnInteger { get; set; }

        public string AString { get; set; }

        public string AnotherString { get; set; }
    }

    public interface IService
    {
        
    }

    public class Service : IService
    {
        public Service(IService service)
        {
            this.ServiceField = service;
        }

        public IService InnerService { get; set; }

        public IService ServiceField;
    }
}
