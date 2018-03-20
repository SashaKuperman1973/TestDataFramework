using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.ListOperations;
using TestDataFramework.TypeGenerator.Interfaces;

namespace TestDataFramework.Populator.Concrete
{
    public class RangeOperableList<TListElement, TPropertyType> : OperableList<TListElement>
    {
        protected readonly IObjectGraphService ObjectGraphService;
        protected readonly ITypeGenerator typeGenerator;
        protected readonly IAttributeDecorator attributeDecorator;
        private readonly int copies;

        public RangeOperableList(int copies, ValueGuaranteePopulator valueGuaranteePopulator, BasePopulator populator,
            ITypeGenerator typeGenerator, IAttributeDecorator attributeDecorator, IObjectGraphService objectGraphService)
            : base(valueGuaranteePopulator, populator)
        {
            this.copies = copies;
            this.typeGenerator = typeGenerator;
            this.attributeDecorator = attributeDecorator;
            this.ObjectGraphService = objectGraphService;
        }

        private List<Range> Ranges { get; } = new List<Range>();

        public virtual Func<IList<TListElement>> GetListSetter(Expression<Func<TListElement, TPropertyType>> fieldExpression, Func<TPropertyType> valueFactory)
        {
            List<TListElement> Setter()
            {
                if (!this.Ranges.Any())
                {
                    return Enumerable.Empty<TListElement>().ToList();
                }

                Range[] ranges = this.Ranges.OrderBy(r => r.StartPosition).ToArray();

                if (ranges.Last().EndPosition + 1 > this.copies)
                {
                    throw new ArgumentOutOfRangeException("Setting a collection: Range is greater than copies requested.");
                }

                var setterResult = new List<TListElement>();

                int lastEndPosition = 0;
                IEnumerable<TListElement> autoPopulated;
                foreach (Range range in ranges)
                {
                    if (lastEndPosition > range.StartPosition)
                    {
                        throw new ArgumentOutOfRangeException("Setting a collection: Ranges overlap.");
                    }

                    autoPopulated = this.Populator.Add<TListElement>(range.StartPosition - lastEndPosition).Make();
                    lastEndPosition = range.EndPosition;
                    setterResult.AddRange(autoPopulated);

                    for (int i = range.StartPosition; i <= range.EndPosition; i++)
                    {
                        var recordReference = new RecordReference<TListElement>(this.typeGenerator, this.attributeDecorator,
                            this.Populator, this.ObjectGraphService, this.ValueGuaranteePopulator);

                        this.InternalList.Add(recordReference);
                        recordReference.Set(fieldExpression, valueFactory);
                        recordReference.Populate();
                        setterResult.Add(recordReference.RecordObject);
                    }
                }

                autoPopulated = this.Populator.Add<TListElement>(this.copies - ranges.Last().EndPosition - 1).Make();
                setterResult.AddRange(autoPopulated);

                return setterResult;
            }

            return Setter;
        }
    }
}
