using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework.Helpers;

namespace TestDataFramework.RepositoryOperations
{
    public class CircularReferenceBreaker
    {
        #region Private Fields

        private static readonly ILog Logger = LogManager.GetLogger(typeof(CircularReferenceBreaker));

        private readonly Stack<Delegate> callStack = new Stack<Delegate>();

        #endregion Private Fields

        #region Public methods

        public virtual bool IsVisited<T1, T2, T3>(Action<CircularReferenceBreaker, T1, T2, T3> operation) => this.IsVisitedDelegate(operation);

        public virtual void Push<T1, T2, T3>(Action<CircularReferenceBreaker, T1, T2, T3> operation) => this.PushDelegate(operation);

        public virtual void Pop()
        {
            Delegate operation = this.callStack.Pop();
            CircularReferenceBreaker.Logger.Debug("Popped " + Helper.DumpMethod(operation));
        }

        #endregion Public methods

        #region Private Methods

        private bool IsVisitedDelegate(Delegate operation) => this.callStack.Contains(operation);

        private void PushDelegate(Delegate operation)
        {
            CircularReferenceBreaker.Logger.Debug("Pushing " + Helper.DumpMethod(operation));
            this.callStack.Push(operation);
        }

        #endregion Private Methods
    }
}
