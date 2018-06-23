using System;
using System.Reflection;

namespace TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers
{
    public class TypeInfoWrapper : MemberInfo, IWrapper<TypeInfo>
    {
        public readonly Guid Id = Guid.NewGuid();

        public TypeInfoWrapper(TypeInfo typeInfo)
        {
            this.Wrapped = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));
        }

        public TypeInfoWrapper(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            this.Wrapped = type.GetTypeInfo();
        }

        public TypeInfoWrapper()
        {
        }

        public TypeInfo Wrapped { get; }

        public virtual AssemblyWrapper Assembly => this.Wrapped == null
            ? new AssemblyWrapper()
            : new AssemblyWrapper(this.Wrapped.Assembly);

        public virtual Type Type => this.Wrapped;

        public override MemberTypes MemberType => this.Wrapped.MemberType;

        public override Type DeclaringType => this.Wrapped.DeclaringType;

        public override Type ReflectedType => this.Wrapped.ReflectedType;

        public override string Name => this.Wrapped.Name;

        public override object[] GetCustomAttributes(bool inherit)
        {
            return this.Wrapped.GetCustomAttributes(inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return this.Wrapped.IsDefined(attributeType, inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return this.Wrapped.GetCustomAttributes(attributeType, inherit);
        }

        public override string ToString()
        {
            return this.Wrapped?.ToString() ?? $"Empty TypeInfo Wrapper. ID: {this.Id}";
        }

        public override bool Equals(object obj)
        {
            bool result = EqualityHelper.Equals<TypeInfoWrapper, TypeInfo>(this, obj);
            return result;
        }

        public override int GetHashCode()
        {
            int result = this.Wrapped == null ? 0 : this.Wrapped.GetHashCode();
            return result;
        }
    }
}