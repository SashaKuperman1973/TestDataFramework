using System;

namespace TestDataFramework.TypeGenerator
{
    public interface ITypeGenerator
    {
        object GetObject(Type forType);
    }
}