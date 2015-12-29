using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace TestDataFramework.RepositoryOperations
{
    public class CircularReferenceBreaker
    {
        #region Private Fields

        private static readonly ILog Logger = LogManager.GetLogger(typeof(CircularReferenceBreaker));

        private readonly Stack<Delegate> callStack = new Stack<Delegate>();

        #endregion Private Fields

        #region Public methods

        public bool IsVisited<T>(Func<CircularReferenceBreaker, T> operation) => this.IsVisitedDelegate(operation);

        public void Push<T>(Func<CircularReferenceBreaker, T> operation) => this.PushDelegate(operation);

        public void Pop()
        {
            Delegate operation = this.callStack.Pop();
            CircularReferenceBreaker.Logger.Debug("Popped " + operation);
        }

        #endregion Public methods

        #region Private Methods

        private bool IsVisitedDelegate(Delegate operation) => this.callStack.Contains(operation);

        private void PushDelegate(Delegate operation)
        {
            CircularReferenceBreaker.Logger.Debug("Pushing " + operation);
            this.callStack.Push(operation);
        }

        #endregion Private Methods
    }
}
