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
using System.Linq.Expressions;
using TestDataFramework.Populator.Concrete;

namespace TestDataFramework.Populator.Interfaces
{
    public interface IRangeOperableList<T>
    {
        IEnumerable<T> RecordObjects { get; }

        OperableList<T> GuaranteeByPercentageOfTotal(IEnumerable<object> guaranteedValues,
            int frequencyPercentage = 10);

        OperableList<T> GuaranteeByPercentageOfTotal(IEnumerable<Func<T>> guaranteedValues,
            int frequencyPercentage = 10);

        OperableList<T> GuaranteeByPercentageOfTotal(IEnumerable<T> guaranteedValues, int frequencyPercentage = 10);

        OperableList<T> GuaranteeByFixedQuantity(IEnumerable<object> guaranteedValues, int fixedQuantity = 0);

        OperableList<T> GuaranteeByFixedQuantity(IEnumerable<Func<T>> guaranteedValues, int fixedQuantity = 0);

        OperableList<T> GuaranteeByFixedQuantity(IEnumerable<T> guaranteedValues, int fixedQuantity = 0);

        void Ignore<TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression);

        IEnumerable<T> BindAndMake();

        IEnumerable<T> Make();

        RangeOperableList<T> Set<TProperty>(Expression<Func<T, TProperty>> fieldExpression,
            TProperty value, params Range[] ranges);

        RangeOperableList<T> Set<TProperty>(Expression<Func<T, TProperty>> fieldExpression,
            Func<TProperty> valueFactory, params Range[] ranges);

        FieldExpression<T, TProperty> Set<TProperty>(Expression<Func<T, TProperty>> expression);
    }
}