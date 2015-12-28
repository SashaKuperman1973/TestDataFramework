using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDataFramework.Populator
{
    public class RecordReference<T> : RecordReference
    {
        public RecordReference(T recordObject) : base(recordObject)
        {
        }

        public new T RecordObject => (T) base.RecordObject;
    }
}
