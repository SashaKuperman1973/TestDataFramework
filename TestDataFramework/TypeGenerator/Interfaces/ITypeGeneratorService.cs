using System.Collections.Generic;
using TestDataFramework.DeepSetting;

namespace TestDataFramework.TypeGenerator.Interfaces
{
    public interface ITypeGeneratorService
    {
        IEnumerable<ExplicitPropertySetter> GetExplicitlySetPropertySetters(
            IEnumerable<ExplicitPropertySetter> explicitPropertySetters,
            ObjectGraphNode objectGraphNode);
    }
}