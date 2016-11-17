using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDataFramework.Populator.Concrete
{
    public class OperableList<T> : List<T>
    {
        private IEnumerable<T> guaranteed;

        public virtual OperableList<T> Guarantee(IEnumerable<T> guaranteed)
        {
            this.guaranteed = guaranteed;
            return this;
        }

        public virtual IEnumerable<T> Bind()
        {
        }
    }
}
