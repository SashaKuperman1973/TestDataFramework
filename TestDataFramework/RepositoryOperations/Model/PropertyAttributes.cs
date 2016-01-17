using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestDataFramework.RepositoryOperations.Model
{
    public class PropertyAttributes
    {
        public PropertyInfo PropertyInfo { get; set; }
        public Attribute[] Attributes { get; set; }
    }
}
