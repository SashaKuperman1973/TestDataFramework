using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDataFramework.RepositoryOperations
{
    public class CircularReferenceBreaker
    {
        private readonly AbstractRepositoryOperation firstVisited;
        private AbstractRepositoryOperation lastVisited;

        public CircularReferenceBreaker(AbstractRepositoryOperation firstVisited)
        {
            this.firstVisited = firstVisited;
        }

        public bool Visited(AbstractRepositoryOperation visited)
        {
            if (visited == this.firstVisited)
            {
                return true;
            }

            this.lastVisited = visited;
            return false;
        }
    }
}
