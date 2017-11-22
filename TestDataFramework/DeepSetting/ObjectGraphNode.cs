using System.Reflection;

namespace TestDataFramework.DeepSetting
{
    public class ObjectGraphNode
    {
        public PropertyInfo PropertyInfo { get; }

        public ObjectGraphNode Parent { get; }

        public ObjectGraphNode(PropertyInfo propertyInfo, ObjectGraphNode parent)
        {
            this.PropertyInfo = propertyInfo;
            this.Parent = parent;
        }
    }
}
