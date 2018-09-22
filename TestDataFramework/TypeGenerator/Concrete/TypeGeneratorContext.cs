using System.Collections.Generic;
using System.Linq;
using TestDataFramework.DeepSetting;
using TestDataFramework.DeepSetting.Interfaces;

namespace TestDataFramework.TypeGenerator.Concrete
{
    public class TypeGeneratorContext
    {
        private static readonly List<ExplicitPropertySetter> BlankList = Enumerable.Empty<ExplicitPropertySetter>().ToList();
        private List<ExplicitPropertySetter> explicitPropertySetters;

        public TypeGeneratorContext(List<ExplicitPropertySetter> explicitPropertySetters)
        {
            this.explicitPropertySetters = explicitPropertySetters;
        }

        public bool BlankSetters { get; set; } = false;

        public int BlankSetterCount = 0;

        public List<ExplicitPropertySetter> ExplicitPropertySetters
        {
            get => this.BlankSetters ? TypeGeneratorContext.BlankList : this.explicitPropertySetters;
            set => this.explicitPropertySetters = value;
        }
    }
}
