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