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
        public IEnumerable<ExplicitPropertySetters> IsPropertyExplicitlySet(
            IEnumerable<ExplicitPropertySetters> explicitPropertySetters,
            ObjectGraphNode objectGraphNode)
        {
            if (objectGraphNode == null)
                return Enumerable.Empty<ExplicitPropertySetters>();

            IEnumerable<ExplicitPropertySetters> result =
                explicitPropertySetters.Where(
                    setters => StandardTypeGeneratorService.IsPropertyExplicitlySet(setters, objectGraphNode));
            return result;
        }

        private static bool IsPropertyExplicitlySet(ExplicitPropertySetters explicitPropertySetters,
            ObjectGraphNode objectGraphNode)
        {
            var stack = new Stack<PropertyInfo>(explicitPropertySetters.PropertyChain);

            while (objectGraphNode.PropertyInfo != null)
            {
                if (!stack.Any())
                    return false;

                PropertyInfo setters = stack.Pop();

                if (!objectGraphNode.PropertyInfo.Name.Equals(setters.Name, StringComparison.Ordinal))
                    return false;

                objectGraphNode = objectGraphNode.Parent;
            }

            return !stack.Any();
        }
    }
}
