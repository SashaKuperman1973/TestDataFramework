using System;
using System.Linq;
using System.Linq.Expressions;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.ListOperations;
using TestDataFramework.TypeGenerator.Interfaces;

namespace TestDataFramework.Populator.Concrete
{
    public class RangeOperableList<TListElement> : OperableList<TListElement>
    {
        protected readonly IObjectGraphService ObjectGraphService;
        protected readonly ITypeGenerator TypeGenerator;
        protected readonly IAttributeDecorator AttributeDecorator;
        protected readonly DeepCollectionSettingConverter deepCollectionSettingConverter;

        public RangeOperableList(int size, ValueGuaranteePopulator valueGuaranteePopulator, BasePopulator populator,
            ITypeGenerator typeGenerator, IAttributeDecorator attributeDecorator, IObjectGraphService objectGraphService,
            DeepCollectionSettingConverter deepCollectionSettingConverter)
            : base(valueGuaranteePopulator, populator)
        {
            this.TypeGenerator = typeGenerator;
            this.AttributeDecorator = attributeDecorator;
            this.ObjectGraphService = objectGraphService;
            this.InternalList = new RecordReference<TListElement>[size].ToList();
            this.deepCollectionSettingConverter = deepCollectionSettingConverter;
        }

        protected internal override void Populate()
        {
            for (int i = 0; i < this.InternalList.Count; i++)
            {
                if (this.InternalList[i] == null)
                {
                    this.InternalList[i] = this.CreateRecordReference();
                }
            }

            base.Populate();
        }

        public virtual RangeOperableList<TListElement> Set<TPropertyType>(Expression<Func<TListElement, TPropertyType>> fieldExpression, TPropertyType value, params Range[] ranges)
        {
            RangeOperableList<TListElement> result = this.Set(fieldExpression, () => value, ranges);
            return result;
        }

        public virtual RangeOperableList<TListElement> Set<TPropertyType>(Expression<Func<TListElement, TPropertyType>> fieldExpression, Func<TPropertyType> valueFactory, params Range[] ranges)
        {
            if (!ranges.Any())
            {
                throw new ArgumentException("No positions passed in.", nameof(ranges));
            }

            int[] positions = ranges.SelectMany(r =>
            {
                var range = new int[r.EndPosition + 1 - r.StartPosition];

                int i = 0;
                for (int j = r.StartPosition; j <= r.EndPosition; j++)
                {
                    range[i++] = j;
                }

                return range;
            }).ToArray();

            IOrderedEnumerable<int> orderedPositions = positions.OrderBy(i => i);
            int highestPosition = orderedPositions.Last();
            int lowestPosition = orderedPositions.First();
            if (highestPosition >= this.InternalList.Count || lowestPosition < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(ranges));
            }

            foreach (int position in positions)
            {
                if (this.InternalList[position] == null)
                {
                    this.InternalList[position] = this.CreateRecordReference();
                }

                this.InternalList[position].Set(fieldExpression, valueFactory);
            }

            return this;
        }

        private RecordReference<TListElement> CreateRecordReference()
        {
            var result = new RecordReference<TListElement>(this.TypeGenerator, this.AttributeDecorator,
                this.Populator, this.ObjectGraphService, this.ValueGuaranteePopulator, this.deepCollectionSettingConverter);

            return result;
        }
    }
}
