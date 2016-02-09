using System;
using System.Reflection;
using TestDataFramework.Helpers;

namespace TestDataFramework.RepositoryOperations.Model
{
    public class PropertyAttribute<T> where T : Attribute
    {
        public PropertyInfo PropertyInfo { get; set; }
        public T Attribute { get; set; }

        public override string ToString()
        {
            string result =
                $"PropertyInfo: {this.PropertyInfo.GetExtendedMemberInfoString()}. Attribute: {this.Attribute}";

            return result;
        }
    }
}
