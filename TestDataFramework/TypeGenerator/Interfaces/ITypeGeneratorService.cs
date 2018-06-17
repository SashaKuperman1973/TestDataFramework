using System.Collections.Generic;
using TestDataFramework.DeepSetting;

namespace TestDataFramework.TypeGenerator.Interfaces
{
    public interface ITypeGeneratorService
    {
        IEnumerable<ExplicitPropertySetters> IsPropertyExplicitlySet(
            IEnumerable<ExplicitPropertySetters> explicitPropertySetters,
            ObjectGraphNode objectGraphNode);
    }
}
