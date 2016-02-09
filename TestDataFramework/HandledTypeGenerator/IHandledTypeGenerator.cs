using System;

namespace TestDataFramework.HandledTypeGenerator
{
    public interface IHandledTypeGenerator
    {
        object GetObject(Type forType);
    }
}
