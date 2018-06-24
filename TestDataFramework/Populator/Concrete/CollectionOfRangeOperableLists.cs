using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace TestDataFramework.Populator.Concrete
{
    public class CollectionOfRangeOperableLists<TListElement>
    {
        private readonly List<OperableList<TListElement>> input = new List<OperableList<TListElement>>();

        public virtual void Add(OperableList<TListElement> list)
        {
            this.input.Add(list);
        }

        public virtual CollectionOfRangeOperableLists<TListElement> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, TProperty value, params Range[] ranges)
        {
            this.input.ToList().ForEach(list => list.Set(fieldExpression, value, ranges));
            return this;
        }

        public virtual CollectionOfRangeOperableLists<TListElement> Set<TProperty>(
            Expression<Func<TListElement, TProperty>> fieldExpression, Func<TProperty> valueFactory,
            params Range[] ranges)
        {
            this.input.ToList().ForEach(list => list.Set(fieldExpression, valueFactory, ranges));
            return this;
        }
    }
}
