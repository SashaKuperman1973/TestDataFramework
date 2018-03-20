using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.ListOperations;
using TestDataFramework.TypeGenerator.Interfaces;

namespace TestDataFramework.Populator.Concrete
{
    public class RangeOperableList<T> : OperableList<T>
    {
        protected readonly IObjectGraphService ObjectGraphService;

        public RangeOperableList(ValueGuaranteePopulator valueGuaranteePopulator, BasePopulator populator, ITypeGenerator typeGenerator, IAttributeDecorator attributeDecorator,
            IObjectGraphService objectGraphService) : base(valueGuaranteePopulator, populator, typeGenerator, attributeDecorator, objectGraphService)
        {
            this.ObjectGraphService = objectGraphService;
        }

        private List<Range> Ranges { get; } = new List<Range>();

        internal Func<List<T>> Set<TPropertyType>(Expression<Func<T, TPropertyType>> fieldExpression, Func<TPropertyType> valueFactory, int copies)
        {
            List<T> Setter()
            {
                Range[] ranges = this.Ranges.OrderBy(r => r.StartPosition).ToArray();

                if (ranges.Last().EndPosition + 1 > copies)
                {
                    throw new ArgumentOutOfRangeException("Setting a collection: Range is greater than copies requested.");
                }

                var setterResult = new List<T>();

                int lastEndPosition = 0;
                IEnumerable<T> autoPopulated;
                foreach (Range range in ranges)
                {
                    if (lastEndPosition > range.StartPosition)
                    {
                        throw new ArgumentOutOfRangeException("Setting a collection: Ranges overlap.");
                    }

                    autoPopulated = this.Populator.Add<T>(range.StartPosition - lastEndPosition).Make();
                    lastEndPosition = range.EndPosition;
                    setterResult.AddRange(autoPopulated);

                    for (int i = range.StartPosition; i <= range.EndPosition; i++)
                    {
                        var recordReference = new RecordReference<T>(this.TypeGenerator, this.AttributeDecorator,
                            this.Populator, this.ObjectGraphService, this.ValueGuaranteePopulator);

                        this.InternalList.Add(recordReference);
                        recordReference.Set(fieldExpression, valueFactory);
                        recordReference.Populate();
                        setterResult.Add(recordReference.RecordObject);
                    }
                }

                autoPopulated = this.Populator.Add<T>(copies - ranges.Last().EndPosition - 1).Make();
                setterResult.AddRange(autoPopulated);

                return setterResult;
            }

            return Setter;
        }
    }
}
