using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestDataFramework.RepositoryOperations.Model
{
    public class PropertyAttribute<T> where T : Attribute
    {
        public PropertyInfo PropertyInfo { get; set; }
        public T Attribute { get; set; }
    }
}
