using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TestDataFramework.Helpers
{
    public class PropertyInfoProxy : MemberInfoProxy
    {
        private readonly PropertyInfo propertyInfo;
        private readonly FieldInfo fieldInfo;

        public PropertyInfoProxy(PropertyInfo propertyInfo) : base(propertyInfo)
        {
            this.propertyInfo = propertyInfo;
        }

        public PropertyInfoProxy(FieldInfo fieldInfo) : base(fieldInfo)
        {
            this.fieldInfo = fieldInfo;
        }

        public bool IsProperty => this.propertyInfo != null;
        public bool IsField => this.fieldInfo != null;

        public Type PropertyType => this.IsProperty ? this.propertyInfo.PropertyType : this.fieldInfo.FieldType;

        public object GetValue(object obj) => this.IsProperty
            ? this.propertyInfo.GetValue(obj)
            : this.fieldInfo.GetValue(obj);

        public bool CanRead => this.IsField || this.propertyInfo.CanRead;

        public bool CanWrite => this.IsField || this.propertyInfo.CanWrite;

        public ParameterInfo[] GetIndexParameters() => this.IsProperty
            ? this.propertyInfo.GetIndexParameters()
            : Enumerable.Empty<ParameterInfo>().ToArray();

        public void SetValue(object obj, object value)
        {
            if (this.IsProperty)
                this.propertyInfo.SetValue(obj, value);
            else
                this.fieldInfo.SetValue(obj, value);
        }
    }
}
