using System.Data.Common;
using System.Reflection;

namespace TestDataFramework.DeferredValueGenerator.Interfaces
{
    public delegate T HandlerDelegate<out T>(PropertyInfo propertyInfo, DbCommand dbCommand);

    public interface IDeferredValueGeneratorHandler<out T>
    {
        T NumberHandler(PropertyInfo propertyInfo, DbCommand command);

        T StringHandler(PropertyInfo propertyInfo, DbCommand command);
    }
}