using System.Reflection;

namespace TestDataFramework.AttributeDecorator.Concrete.TableTypeCacheService
{
    public class TestDataTypeInfo
    {
        internal readonly TypeInfo TypeInfo;

        public TestDataTypeInfo(TypeInfo typeInfo)
        {
            this.TypeInfo = typeInfo;
        }

        public override string ToString()
        {
            return this.TypeInfo.ToString();
        }
    }
}