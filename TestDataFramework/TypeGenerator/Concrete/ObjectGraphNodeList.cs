using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace TestDataFramework.TypeGenerator.Concrete
{
    public class ObjectGraphNodeList : List<PropertyInfo>
    {
        public ObjectGraphNodeList()
        {
        }

        public ObjectGraphNodeList(IEnumerable<PropertyInfo> collection) : base(collection)
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Start ObjectGraphNodeList");
            this.ForEach(pi => sb.AppendLine($"  {pi.DeclaringType}.{pi.Name}"));
            sb.AppendLine("End ObjectGraphNodeList");

            return sb.ToString();
        }
    }
}
