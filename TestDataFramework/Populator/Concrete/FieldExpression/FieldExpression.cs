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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using log4net;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.Helpers;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Logger;
using TestDataFramework.Populator.Concrete.OperableList;

namespace TestDataFramework.Populator.Concrete.FieldExpression
{
    public class FieldExpression<TListElement, TProperty>
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(FieldExpression<TListElement, TProperty>));

        private readonly Expression<Func<TListElement, TProperty>> expression;

        private readonly IObjectGraphService objectGraphService;

        public virtual OperableListEx<TListElement> OperableList { get; }

        public FieldExpression(Expression<Func<TListElement, TProperty>> expression,
            OperableListEx<TListElement> operableList, IObjectGraphService objectGraphService)
        {
            this.expression = expression;
            this.OperableList = operableList;
            this.objectGraphService = objectGraphService;
        }

        public virtual IEnumerable<TListElement> RecordObjects => this.OperableList.RecordObjects;

        public virtual IEnumerable<TListElement> Make()
        {
            FieldExpression<TListElement, TProperty>.Logger.Debug("Calling Make");
            return this.OperableList.Make();
        }

        public virtual IEnumerable<TListElement> BindAndMake()
        {
            FieldExpression<TListElement, TProperty>.Logger.Debug("Calling BindAndMake");
            return this.OperableList.BindAndMake();
        }

        public virtual FieldExpression<TListElement, TProperty> GuaranteePropertiesByFixedQuantity(IEnumerable<object> guaranteedValues,
            int fixedQuantity, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            FieldExpression<TListElement, TProperty>.Logger.Entering(nameof(this.GuaranteePropertiesByFixedQuantity));

            var fieldExpressionSet =
                new FieldExpressionsSet<TListElement>(this.OperableList, this.objectGraphService);

            fieldExpressionSet.GuaranteePropertiesByFixedQuantity(this.expression, guaranteedValues, fixedQuantity,
                valueCountRequestOption);

            FieldExpression<TListElement, TProperty>.Logger.Exiting(nameof(this.GuaranteePropertiesByFixedQuantity));
            return this;
        }

        public virtual FieldExpression<TListElement, TProperty> GuaranteePropertiesByFixedQuantity(
            IEnumerable<Func<TProperty>> guaranteedValues, int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(guaranteedValues.Cast<object>(), fixedQuantity,
                valueCountRequestOption);
        }

        public virtual FieldExpression<TListElement, TProperty> GuaranteePropertiesByFixedQuantity(
            IEnumerable<TProperty> guaranteedValues, int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(guaranteedValues.Cast<object>(), fixedQuantity,
                valueCountRequestOption);
        }

        public virtual FieldExpression<TListElement, TProperty> GuaranteePropertiesByFixedQuantity(
            IEnumerable<object> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(guaranteedValues, 0, valueCountRequestOption);
        }

        public virtual FieldExpression<TListElement, TProperty> GuaranteePropertiesByFixedQuantity(
            IEnumerable<Func<TProperty>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(guaranteedValues.Cast<object>(), valueCountRequestOption);
        }

        public virtual FieldExpression<TListElement, TProperty> GuaranteePropertiesByFixedQuantity(
            IEnumerable<TProperty> guaranteedValues, 
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByFixedQuantity(guaranteedValues.Cast<object>(), valueCountRequestOption);
        }

        public virtual FieldExpression<TListElement, TProperty> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<object> guaranteedValues, int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            FieldExpression<TListElement, TProperty>.Logger.Entering(nameof(this.GuaranteePropertiesByFixedQuantity));

            var fieldExpressionSet =
                new FieldExpressionsSet<TListElement>(this.OperableList, this.objectGraphService);

            fieldExpressionSet.GuaranteePropertiesByPercentageOfTotal(this.expression, guaranteedValues, frequencyPercentage,
                valueCountRequestOption);

            FieldExpression<TListElement, TProperty>.Logger.Exiting(nameof(this.GuaranteePropertiesByFixedQuantity));
            return this;
        }

        public virtual FieldExpression<TListElement, TProperty> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<Func<TProperty>> guaranteedValues, int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(guaranteedValues.Cast<object>(), frequencyPercentage,
                valueCountRequestOption);
        }

        public virtual FieldExpression<TListElement, TProperty> GuaranteePropertiesByPercentageOfTotal(IEnumerable<TProperty> guaranteedValues,
            int frequencyPercentage, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(guaranteedValues.Cast<object>(), frequencyPercentage,
                valueCountRequestOption);
        }

        public virtual FieldExpression<TListElement, TProperty> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<object> guaranteedValues, 
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(guaranteedValues, 10, valueCountRequestOption);
        }

        public virtual FieldExpression<TListElement, TProperty> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<Func<TProperty>> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(guaranteedValues.Cast<object>(),
                valueCountRequestOption);
        }

        public virtual FieldExpression<TListElement, TProperty> GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<TProperty> guaranteedValues,
            ValueCountRequestOption valueCountRequestOption =
                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            return this.GuaranteePropertiesByPercentageOfTotal(guaranteedValues.Cast<object>(),
                valueCountRequestOption);
        }
    }
}