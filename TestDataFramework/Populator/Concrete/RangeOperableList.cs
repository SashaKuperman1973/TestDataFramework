using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TestDataFramework.AttributeDecorator;
using TestDataFramework.AttributeDecorator.Interfaces;
using TestDataFramework.DeepSetting.Interfaces;
using TestDataFramework.Exceptions;
using TestDataFramework.ListOperations;
using TestDataFramework.Populator.Interfaces;
using TestDataFramework.TypeGenerator.Interfaces;

namespace TestDataFramework.Populator.Concrete
{
    public class RangeOperableList<TListElement> : OperableList<TListElement>, IRangeOperableList<TListElement>
    {
        protected readonly IObjectGraphService ObjectGraphService;
        protected readonly ITypeGenerator TypeGenerator;
        protected readonly IAttributeDecorator AttributeDecorator;
        protected readonly DeepCollectionSettingConverter DeepCollectionSettingConverter;

        public RangeOperableList(int size, ValueGuaranteePopulator valueGuaranteePopulator, BasePopulator populator,
            ITypeGenerator typeGenerator, IAttributeDecorator attributeDecorator, IObjectGraphService objectGraphService,
            DeepCollectionSettingConverter deepCollectionSettingConverter)
            : base(valueGuaranteePopulator, populator)
        {
            this.TypeGenerator = typeGenerator;
            this.AttributeDecorator = attributeDecorator;
            this.ObjectGraphService = objectGraphService;
            this.InternalList = new RecordReference<TListElement>[size].ToList();
            this.DeepCollectionSettingConverter = deepCollectionSettingConverter;
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

        public virtual RangeOperableList<TListElement> Set<TProperty>(Expression<Func<TListElement, TProperty>> fieldExpression, TProperty value, params Range[] ranges)
        {
            RangeOperableList<TListElement> result = this.Set(fieldExpression, () => value, ranges);
            return result;
        }

        public virtual RangeOperableList<TListElement> Set<TProperty>(Expression<Func<TListElement, TProperty>> fieldExpression, Func<TProperty> valueFactory, params Range[] ranges)
        {
            if (!ranges.Any())
            {
                throw new ArgumentException(Messages.NoRangeOperableListPositionsPassedIn, nameof(ranges));
            }

            int[] positions = RangeOperableList<TListElement>.GetPositions(ranges);
            this.ValidatePositionBoundaries(positions, nameof(ranges));
            this.SetInternalList(positions, fieldExpression, valueFactory);

            return this;
        }

        public virtual FieldExpression<TListElement, TProperty> Set<TProperty>(Expression<Func<TListElement, TProperty>> expression)
        {
            var fieldExpression = new FieldExpression<TListElement, TProperty>(expression, this);
            return fieldExpression;
        }

        private static int[] GetPositions(IEnumerable<Range> ranges)
        {
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

            return positions;
        }

        private void ValidatePositionBoundaries(IEnumerable<int> positions, string rangesParameterName)
        {
            IOrderedEnumerable<int> orderedPositions = positions.OrderBy(i => i);
            int highestPosition = orderedPositions.Last();
            int lowestPosition = orderedPositions.First();

            if (highestPosition >= this.InternalList.Count || lowestPosition < 0)
            {
                throw new ArgumentOutOfRangeException(rangesParameterName);
            }
        }

        private void SetInternalList<TProperty>(IEnumerable<int> positions, Expression<Func<TListElement, TProperty>> fieldExpression, Func<TProperty> valueFactory)
        {
            foreach (int position in positions)
            {
                if (this.InternalList[position] == null)
                {
                    this.InternalList[position] = this.CreateRecordReference();
                }

                this.InternalList[position].Set(fieldExpression, valueFactory);
            }
        }

        private RecordReference<TListElement> CreateRecordReference()
        {
            var result = new RecordReference<TListElement>(this.TypeGenerator, this.AttributeDecorator,
                this.Populator, this.ObjectGraphService, this.ValueGuaranteePopulator, this.DeepCollectionSettingConverter);

            return result;
        }
    }
}
