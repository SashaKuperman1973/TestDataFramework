/*
    Copyright 2016, 2017, 2018, 2019, 2023 Alexander Kuperman

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
using TestDataFramework.Helpers.FieldExpressionValidator.Concrete;
using TestDataFramework.Logger;
using TestDataFramework.TypeGenerator.Concrete;

namespace TestDataFramework.DeepSetting.Concrete
{
    public class ObjectGraphService : IObjectGraphService
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(ObjectGraphService));

        private readonly PropertySetFieldExpressionValidator fieldExpressionValidator =
            new PropertySetFieldExpressionValidator();

        public List<PropertyInfoProxy> GetObjectGraph<T, TPropertyValue>(Expression<Func<T, TPropertyValue>> fieldExpression)
        {
            ObjectGraphService.Logger.Entering(nameof(this.GetObjectGraph), fieldExpression);

            var propertyChain = new ObjectGraphNodeList();
            this.GetMemberInfo(propertyChain, fieldExpression.Body);

            ObjectGraphService.Logger.Exiting(nameof(this.GetObjectGraph), propertyChain);
            return propertyChain;
        }

        public bool DoesPropertyHaveSetter(List<PropertyInfoProxy> objectGraphNodeList, IEnumerable<ExplicitPropertySetter> explicitPropertySetters)
        {
            ObjectGraphService.Logger.Entering(nameof(this.DoesPropertyHaveSetter), objectGraphNodeList);

            foreach (ExplicitPropertySetter aSetter in explicitPropertySetters)
            {
                ObjectGraphService.Logger.Debug("Setter property chain: " + aSetter.PropertyChain);

                if (objectGraphNodeList.Count > aSetter.PropertyChain.Count)
                {
                    ObjectGraphService.Logger.Debug("Property graph is beyond current setter property chain.");
                    continue;
                }

                ObjectGraphService.Logger.Debug("Setter/Graph loop starts.");
                bool result = true;
                for (int i = 0; i < objectGraphNodeList.Count; i++)
                {
                    Type graphPropertyType = objectGraphNodeList[i].PropertyType;
                    Type setterChainPropertyType = aSetter.PropertyChain[i].PropertyType;

                    ObjectGraphService.Logger.Debug(
                        $"Graph PropertyType = {graphPropertyType} - Setter chain propertyType = {setterChainPropertyType}");

                    if (graphPropertyType == setterChainPropertyType)
                    {
                        continue;                        
                    }

                    ObjectGraphService.Logger.Debug("Setter/Graph property mismatch. Continuing setter loop.");

                    result = false;
                    break;
                }

                if (result)
                {
                    ObjectGraphService.Logger.Exiting(nameof(this.DoesPropertyHaveSetter), true);
                    return true;
                }
            }

            ObjectGraphService.Logger.Exiting(nameof(this.DoesPropertyHaveSetter), false);
            return false;
        }

        private void GetMemberInfo(ICollection<PropertyInfoProxy> list, Expression expression)
        {
            if (expression.NodeType == ExpressionType.Parameter || expression.NodeType == ExpressionType.Convert)
                return;

            MemberExpression memberExpression =
                this.fieldExpressionValidator.ValidateMemberAccessExpression(expression);

            this.GetMemberInfo(list, memberExpression.Expression);

            if (memberExpression.Member is PropertyInfo)
            {
                list.Add(new PropertyInfoProxy((PropertyInfo)memberExpression.Member));
                return;
            }

            if (memberExpression.Member is FieldInfo)
            {
                list.Add(new PropertyInfoProxy((FieldInfo)memberExpression.Member));
                return;
            }

            throw new Exceptions.BadExpressionMemberException(Exceptions.Messages.BadExpressionMember);
        }
    }
}