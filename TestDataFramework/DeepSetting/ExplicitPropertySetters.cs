using System;
using System.Collections.Generic;
using System.Reflection;

namespace TestDataFramework.DeepSetting
{
    public class ExplicitPropertySetter
    {
        public List<PropertyInfo> PropertyChain { get; set; }

        public Action<object> Action { get; set; }
    }
}