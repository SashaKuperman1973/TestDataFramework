using System.Reflection;

namespace TestDataFramework.DeepSetting
{
    public class ObjectGraphNode
    {
        public ObjectGraphNode(PropertyInfo propertyInfo, ObjectGraphNode parent)
        {
            this.PropertyInfo = propertyInfo;
            this.Parent = parent;
        }

        public PropertyInfo PropertyInfo { get; }

        public ObjectGraphNode Parent { get; }
    }
}