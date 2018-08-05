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
using System.Linq.Expressions;
using System.Reflection;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator.Concrete;

namespace TestDataFramework.Populator
{
    public class FieldExpression<TListElement, TProperty>
    {
        protected readonly Expression<Func<TListElement, TProperty>> Expression;

        protected readonly IObjectGraphService ObjectGraphService;

        public virtual OperableListEx<TListElement> OperableList { get; }

        public FieldExpression(Expression<Func<TListElement, TProperty>> expression,
            OperableListEx<TListElement> operableList, IObjectGraphService objectGraphService)
        {
            this.Expression = expression;
            this.OperableList = operableList;
            this.ObjectGraphService = objectGraphService;
        }

        public virtual IEnumerable<TListElement> RecordObjects => this.OperableList.RecordObjects;

        public virtual IEnumerable<TListElement> Make()
        {
            return this.OperableList.Make();
        }

        public virtual IEnumerable<TListElement> BindAndMake()
        {
            return this.OperableList.BindAndMake();
        }

        public virtual FieldExpression<TListElement, TProperty> GuaranteePropertiesByFixedQuantity(IEnumerable<object> guaranteedValues,
            int fixedQuantity, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            List<PropertyInfo> setterObjectGraph = this.ObjectGraphService.GetObjectGraph(this.Expression);

            guaranteedValues = guaranteedValues.ToList();

            if (fixedQuantity == 0)
                fixedQuantity = guaranteedValues.Count();

            this.OperableList.AddGuaranteedPropertySetter(new GuaranteedValues
            {
                TotalFrequency = fixedQuantity,

                Values = guaranteedValues.Select(value => FieldExpressionHelper
                    .GetFuncOrValueBasedExlicitPropertySetter<TProperty>(value, setterObjectGraph)),

                ValueCountRequestOption = valueCountRequestOption
            });

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
            List<PropertyInfo> setterObjectGraph = this.ObjectGraphService.GetObjectGraph(this.Expression);

            guaranteedValues = guaranteedValues.ToList();

            this.OperableList.AddGuaranteedPropertySetter(new GuaranteedValues
            {
                FrequencyPercentage = frequencyPercentage,

                Values = guaranteedValues.Select(value => FieldExpressionHelper
                    .GetFuncOrValueBasedExlicitPropertySetter<TProperty>(value, setterObjectGraph)),

                ValueCountRequestOption = valueCountRequestOption
            });

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

        public virtual FieldExpression<TListElement, TProperty> SetRange<TPropertyValue>(
            Expression<Func<TListElement, TPropertyValue>> fieldExpression,
            IEnumerable<TPropertyValue> range)
        {
            return this.SetRange(fieldExpression, () => range);
        }

        public virtual FieldExpression<TListElement, TProperty> SetRange<TPropertyValue>(
            Expression<Func<TListElement, TPropertyValue>> fieldExpression,
            Func<IEnumerable<TPropertyValue>> rangeFactory)
        {
            this.OperableList.AddRange(fieldExpression, rangeFactory);
            return this;
        }
    }
}