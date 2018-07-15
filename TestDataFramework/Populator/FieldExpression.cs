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
using TestDataFramework.DeepSetting;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.ListOperations.Concrete;
using TestDataFramework.Populator.Concrete;

namespace TestDataFramework.Populator
{
    public abstract class FieldExpression<TListElement, TProperty>
    {
        protected FieldExpression<TListElement, TProperty> Parent;

        protected readonly Expression<Func<TListElement, TProperty>> Expression;

        protected readonly IObjectGraphService ObjectGraphService;

        protected OperableList<TListElement> ThisOperableList;

        protected OperableList<TListElement> OperableList
        {
            get
            {
                FieldExpression<TListElement, TProperty> fieldExpression = this;

                while (fieldExpression.Parent != null)
                {
                    fieldExpression = fieldExpression.Parent;
                }

                return fieldExpression.ThisOperableList;
            }
        }

        protected FieldExpression(Expression<Func<TListElement, TProperty>> expression,
            OperableList<TListElement> operableList, IObjectGraphService objectGraphService)
        {
            this.Expression = expression;
            this.ThisOperableList = operableList;
            this.ObjectGraphService = objectGraphService;
        }

        public virtual IEnumerable<TListElement> RecordObjects => this.ThisOperableList.RecordObjects;

        protected void GuaranteePropertiesByFixedQuantity(
            IEnumerable<Func<TProperty>> guaranteedValues, int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            List<PropertyInfo> setterObjectGraph = this.ObjectGraphService.GetObjectGraph(this.Expression);

            guaranteedValues = guaranteedValues.ToList();

            if (fixedQuantity == 0)
                fixedQuantity = guaranteedValues.Count();

            this.ThisOperableList.GuaranteedPropertySetters.Add(new GuaranteedValues
            {
                TotalFrequency = fixedQuantity,
                Values = guaranteedValues.Select(value => new ExplicitPropertySetter
                {
                    PropertyChain = setterObjectGraph,
                    Action = @object => setterObjectGraph.Last().SetValue(@object, value())
                }),
                ValueCountRequestOption = valueCountRequestOption
            });
        }

        protected void GuaranteePropertiesByFixedQuantity(IEnumerable<object> guaranteedValues,
            int fixedQuantity, ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            List<PropertyInfo> setterObjectGraph = this.ObjectGraphService.GetObjectGraph(this.Expression);

            guaranteedValues = guaranteedValues.ToList();

            if (fixedQuantity == 0)
                fixedQuantity = guaranteedValues.Count();

            this.ThisOperableList.GuaranteedPropertySetters.Add(new GuaranteedValues
            {
                TotalFrequency = fixedQuantity,
                Values = guaranteedValues.Select(value => FieldExpressionHelper
                    .GetFuncOrValueBasedExlicitPropertySetter<TProperty>(value, setterObjectGraph)),
                ValueCountRequestOption = valueCountRequestOption
            });
        }

        protected void GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<Func<TProperty>> guaranteedValues, int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            List<PropertyInfo> setterObjectGraph = this.ObjectGraphService.GetObjectGraph(this.Expression);

            this.ThisOperableList.GuaranteedPropertySetters.Add(new GuaranteedValues
            {
                FrequencyPercentage = frequencyPercentage,

                Values = guaranteedValues.Select(value => new ExplicitPropertySetter
                {
                    PropertyChain = setterObjectGraph,
                    Action = @object => setterObjectGraph.Last().SetValue(@object, value())
                }),
                ValueCountRequestOption = valueCountRequestOption
            });
        }

        protected void GuaranteePropertiesByPercentageOfTotal(
            IEnumerable<object> guaranteedValues, int frequencyPercentage,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            List<PropertyInfo> setterObjectGraph = this.ObjectGraphService.GetObjectGraph(this.Expression);

            guaranteedValues = guaranteedValues.ToList();

            this.ThisOperableList.GuaranteedPropertySetters.Add(new GuaranteedValues
            {
                FrequencyPercentage = frequencyPercentage,
                Values = guaranteedValues.Select(value => FieldExpressionHelper
                    .GetFuncOrValueBasedExlicitPropertySetter<TProperty>(value, setterObjectGraph)),
                ValueCountRequestOption = valueCountRequestOption
            });
        }

        protected void SetRange<TPropertyValue>(
            Expression<Func<TListElement, TPropertyValue>> fieldExpression,
            Func<IEnumerable<TPropertyValue>> rangeFactory)
        {
            this.ThisOperableList.AddRange(fieldExpression, rangeFactory);
        }
    }
}