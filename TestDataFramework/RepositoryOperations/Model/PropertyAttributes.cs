using System;
using System.Reflection;

namespace TestDataFramework.RepositoryOperations.Model
{
    public class PropertyAttributes
    {
        public PropertyInfo PropertyInfo { get; set; }
        public Attribute[] Attributes { get; set; }
    }
}
