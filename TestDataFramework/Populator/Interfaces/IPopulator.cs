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
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.Populator.Concrete;
using TestDataFramework.Populator.Concrete.OperableList;
using TestDataFramework.ValueGenerator.Interfaces;

namespace TestDataFramework.Populator.Interfaces
{
    public interface IPopulator
    {
        IValueGenerator ValueGenerator { get; }

        void Bind();

        T Make<T>();

        IEnumerable<T> Make<T>(int count);
            
        OperableList<T> Add<T>(int copies, params RecordReference[] primaryRecordReferences);

        RecordReference<T> Add<T>(params RecordReference[] primaryRecordReferences);

        BasePopulator.Decorator<T> DecorateType<T>();

        void Extend(Type type, HandledTypeValueGetter valueGetter);

        void Clear();
    }
}