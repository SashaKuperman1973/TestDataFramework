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
        private readonly Expression<Func<TListElement, TProperty>> expression;
        private readonly IObjectGraphService objectGraphService;
        private readonly OperableList<TListElement> operableList;

        protected FieldExpression(Expression<Func<TListElement, TProperty>> expression,
            OperableList<TListElement> operableList, IObjectGraphService objectGraphService)
        {
            this.expression = expression;
            this.operableList = operableList;
            this.objectGraphService = objectGraphService;
        }

        public virtual IEnumerable<TListElement> RecordObjects => this.operableList.RecordObjects;

        protected void GuaranteePropertiesByFixedQuantity(
            IEnumerable<Func<TProperty>> guaranteedValues, int fixedQuantity,
            ValueCountRequestOption valueCountRequestOption = ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall)
        {
            List<PropertyInfo> setterObjectGraph = this.objectGraphService.GetObjectGraph(this.expression);

            guaranteedValues = guaranteedValues.ToList();

            if (fixedQuantity == 0)
                fixedQuantity = guaranteedValues.Count();

            this.operableList.GuaranteedPropertySetters.Add(new GuaranteedValues
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
            List<PropertyInfo> setterObjectGraph = this.objectGraphService.GetObjectGraph(this.expression);

            guaranteedValues = guaranteedValues.ToList();

            if (fixedQuantity == 0)
                fixedQuantity = guaranteedValues.Count();

            this.operableList.GuaranteedPropertySetters.Add(new GuaranteedValues
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
            List<PropertyInfo> setterObjectGraph = this.objectGraphService.GetObjectGraph(this.expression);

            this.operableList.GuaranteedPropertySetters.Add(new GuaranteedValues
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
            List<PropertyInfo> setterObjectGraph = this.objectGraphService.GetObjectGraph(this.expression);

            guaranteedValues = guaranteedValues.ToList();

            this.operableList.GuaranteedPropertySetters.Add(new GuaranteedValues
            {
                FrequencyPercentage = frequencyPercentage,
                Values = guaranteedValues.Select(value => FieldExpressionHelper
                    .GetFuncOrValueBasedExlicitPropertySetter<TProperty>(value, setterObjectGraph)),
                ValueCountRequestOption = valueCountRequestOption
            });
        }
    }
}