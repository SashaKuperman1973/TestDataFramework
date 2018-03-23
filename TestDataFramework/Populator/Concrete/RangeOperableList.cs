﻿using System;
using System.Collections.Generic;
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

        public RangeOperableList(int size, ValueGuaranteePopulator valueGuaranteePopulator, BasePopulator populator,
            ITypeGenerator typeGenerator, IAttributeDecorator attributeDecorator, IObjectGraphService objectGraphService)
            : base(valueGuaranteePopulator, populator)
        {
            this.TypeGenerator = typeGenerator;
            this.AttributeDecorator = attributeDecorator;
            this.ObjectGraphService = objectGraphService;
            this.InternalList = new List<RecordReference<TListElement>>(size);
        }

        //Expression<Func<TListElement, TPropertyType>> fieldExpression
        //Func<TPropertyType> valueFactory
        public virtual Func<IList<TListElement>> GetListSetter()
        {
            List<TListElement> Setter()
            {
                var setterResult = new List<TListElement>();

                foreach (RecordReference<TListElement> recordReference in this.InternalList)
                {
                    if (recordReference == null)
                    {
                        
                    }
                }

                //////////

                int lastEndPosition = 0;
                IEnumerable<TListElement> autoPopulated;
                foreach (Range range in ranges)
                {
                    if (lastEndPosition > range.StartPosition)
                    {
                        throw new ArgumentOutOfRangeException("Setting a collection: Ranges overlap.");
                    }

                    autoPopulated = this.Populator.Add<TListElement>().Make();
                    lastEndPosition = range.EndPosition;
                    setterResult.AddRange(autoPopulated);

                    for (int i = range.StartPosition; i <= range.EndPosition; i++)
                    {
                        var recordReference = new RecordReference<TListElement>(this.TypeGenerator, this.AttributeDecorator,
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
