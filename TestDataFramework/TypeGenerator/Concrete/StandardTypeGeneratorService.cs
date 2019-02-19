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
using System.Reflection;
using TestDataFramework.DeepSetting;
using TestDataFramework.TypeGenerator.Interfaces;

namespace TestDataFramework.TypeGenerator.Concrete
{
    public class StandardTypeGeneratorService : ITypeGeneratorService
    {
        public IEnumerable<ExplicitPropertySetter> GetExplicitlySetPropertySetters(
            IEnumerable<ExplicitPropertySetter> explicitPropertySetters,
            ObjectGraphNode objectGraphNode)
        {
            if (objectGraphNode == null)
                return Enumerable.Empty<ExplicitPropertySetter>();

            IEnumerable<ExplicitPropertySetter> result =
                explicitPropertySetters.Where(
                    setter => StandardTypeGeneratorService.IsPropertyExplicitlySet(setter, objectGraphNode));
            return result;
        }

        private static bool IsPropertyExplicitlySet(ExplicitPropertySetter explicitPropertySetter,
            ObjectGraphNode objectGraphNode)
        {
            var stack = new Stack<PropertyInfo>(explicitPropertySetter.PropertyChain);

            while (objectGraphNode?.PropertyInfo != null)
            {
                if (!stack.Any())
                    return false;

                PropertyInfo setterProperty = stack.Pop();

                if (!objectGraphNode.PropertyInfo.Name.Equals(setterProperty.Name, StringComparison.Ordinal))
                    return false;

                objectGraphNode = objectGraphNode.Parent;
            }

            return !stack.Any();
        }
    }
}