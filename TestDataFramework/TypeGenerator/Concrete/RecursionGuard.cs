using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.DeepSetting;
using TestDataFramework.Logger;

namespace TestDataFramework.TypeGenerator.Concrete
{
    public class RecursionGuard
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(RecursionGuard));

        private Stack<Type> guardTypes = new Stack<Type>();
        private readonly Stack<Stack<Type>> guardStack = new Stack<Stack<Type>>();

        public virtual bool Push(Type forType, IEnumerable<ExplicitPropertySetter> explicitPropertySetters,
            ObjectGraphNode objectGraphNode)
        {
            if (this.guardTypes.Contains(forType))
            {
                if (!RecursionGuard.IsThereAnExplicitSetterforCircularReference(objectGraphNode, explicitPropertySetters))
                {
                    RecursionGuard.Logger.Debug("Circular reference encountered. Type: " + forType);
                    return false;
                }

                this.guardStack.Push(this.guardTypes);
                this.guardTypes = new Stack<Type>();
            }

            this.guardTypes.Push(forType);
            return true;
        }

        public virtual void Pop()
        {
            this.guardTypes.Pop();
            if (!this.guardTypes.Any() && this.guardStack.Any())
            {
                this.guardTypes = this.guardStack.Pop();
            }
        }

        private static bool IsThereAnExplicitSetterforCircularReference(ObjectGraphNode objectGraphNode,
            IEnumerable<ExplicitPropertySetter> explicitPropertySetters)
        {
            if (objectGraphNode == null)
                return false;

            var objectGraphNodeList = new List<PropertyInfo>();
            while (objectGraphNode.PropertyInfo != null)
            {
                objectGraphNodeList.Add(objectGraphNode.PropertyInfo);
                objectGraphNode = objectGraphNode.Parent;
            }

            objectGraphNodeList.Reverse();

            foreach (ExplicitPropertySetter aSetter in explicitPropertySetters)
            {
                if (objectGraphNodeList.Count > aSetter.PropertyChain.Count)
                    continue;

                bool result = true;
                for (int i = 0; i < objectGraphNodeList.Count; i++)
                {
                    if (objectGraphNodeList[i].PropertyType == aSetter.PropertyChain[i].PropertyType) continue;

                    result = false;
                    break;
                }

                if (result)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
