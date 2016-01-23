using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.Exceptions;

namespace TestDataFramework.Populator
{
    public class RecordReference<T> : RecordReference
    {
        public RecordReference(T recordObject, ICollection<PropertyInfo> explicitlySetProperties) : base(recordObject, explicitlySetProperties)
        {
        }

        public new T RecordObject => (T) base.RecordObject;
    }
}
