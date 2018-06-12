using System;
using System.Reflection;

namespace TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService.Wrappers
{
    public class TypeInfoWrapper : MemberInfo
    {
        private readonly TypeInfo typeInfo;

        public TypeInfoWrapper(TypeInfo typeInfo)
        {
            this.typeInfo = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));
        }

        public TypeInfoWrapper(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            this.typeInfo = type.GetTypeInfo();
        }

        public TypeInfoWrapper()
        {            
        }

        public virtual AssemblyWrapper Assembly => this.typeInfo == null
            ? new AssemblyWrapper()
            : new AssemblyWrapper(this.typeInfo.Assembly);

        public virtual Type Type => this.typeInfo;

        public override MemberTypes MemberType => throw new NotImplementedException();

        public override Type DeclaringType => throw new NotImplementedException();

        public override Type ReflectedType => throw new NotImplementedException();

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override string Name => this.typeInfo.Name;

        public override string ToString()
        {
            return this.typeInfo?.ToString() ?? string.Empty;
        }

        public override bool Equals(object obj)
        {
            var toCompare = obj as TypeInfoWrapper;

            if (this.Type == null || toCompare == null)
            {
                return false;
            }

            bool result = this.typeInfo.Equals(toCompare.typeInfo);
            return result;
        }

        public override int GetHashCode()
        {
            int result = this.typeInfo == null ? 0 : this.typeInfo.GetHashCode();
            return result;
        }
    }
}