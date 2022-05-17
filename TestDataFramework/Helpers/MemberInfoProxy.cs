using System;
using System.Collections.Generic;
using System.Reflection;

namespace TestDataFramework.Helpers
{
    public class MemberInfoProxy
    {
        private readonly MemberInfo memberInfo;

        public MemberInfoProxy(MemberInfo memberInfo)
        {
            this.memberInfo = memberInfo;
        }

        public Type DeclaringType => this.memberInfo.DeclaringType;

        public IEnumerable<T> GetCustomAttributes<T>() where T : Attribute => this.memberInfo.GetCustomAttributes<T>();

        public IEnumerable<Attribute> GetCustomAttributes() => this.memberInfo.GetCustomAttributes();

        public MemberTypes MemberType => this.memberInfo.MemberType;

        public string Name => this.memberInfo.Name;

        public override bool Equals(object obj)
        {
            var proxy = obj as MemberInfoProxy;
            return this.memberInfo.Equals(proxy?.memberInfo);
        }

        public override int GetHashCode()
        {
            return this.memberInfo.GetHashCode();
        }
    }
}
