using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TestDataFramework.Helpers;

namespace TestDataFramework.TypeGenerator.Concrete
{
    public class ObjectGraphNodeList : List<PropertyInfoProxy>
    {
        public ObjectGraphNodeList()
        {
        }

        public ObjectGraphNodeList(IEnumerable<PropertyInfoProxy> collection) : base(collection)
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
