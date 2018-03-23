using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.ListOperations;
using TestDataFramework.TypeGenerator.Interfaces;

namespace TestDataFramework.Populator.Concrete
{
    public class RangeOperableListHolder<TListElement>
    {
        private readonly ValueGuaranteePopulator valueGuaranteePopulator;
        private readonly BasePopulator populator;
        protected readonly IObjectGraphService objectGraphService;
        protected readonly ITypeGenerator typeGenerator;
        protected readonly IAttributeDecorator attributeDecorator;

        public RangeOperableListHolder(int size, ValueGuaranteePopulator valueGuaranteePopulator, BasePopulator populator,
            ITypeGenerator typeGenerator, IAttributeDecorator attributeDecorator, IObjectGraphService objectGraphService)
        {
            this.Size = size;
            this.valueGuaranteePopulator = valueGuaranteePopulator;
            this.populator = populator;
            this.typeGenerator = typeGenerator;
            this.attributeDecorator = attributeDecorator;
            this.objectGraphService = objectGraphService;
        }

        public int Size { get; }

        public void SetListElement<TPropertyType>(Expression<Func<TListElement, TPropertyType>> fieldExpression, Func<TPropertyType> valueFactory)
        {
            var list = new RangeOperableList<TListElement, TPropertyType>(this.Size, this.valueGuaranteePopulator, this.populator,
                this.typeGenerator, this.attributeDecorator, this.objectGraphService);

            Func<IList<TListElement>> listSetter = list.GetListSetter(fieldExpression, valueFactory);
        }
    }
}
