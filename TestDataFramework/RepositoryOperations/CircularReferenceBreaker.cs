using System;
using System.Collections.Generic;
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

        public virtual bool IsVisited<T1, T2, T3>(Action<CircularReferenceBreaker, T1, T2, T3> operation)
        {
            CircularReferenceBreaker.Logger.Debug($"Executing IsVisited. operation: {operation}");

            return this.IsVisitedDelegate(operation);
        }

        public virtual void Push<T1, T2, T3>(Action<CircularReferenceBreaker, T1, T2, T3> operation)
        {
            CircularReferenceBreaker.Logger.Debug($"Executing Push. operation: {operation}");

            this.PushDelegate(operation);
        }

        public virtual void Pop()
        {
            CircularReferenceBreaker.Logger.Debug("Entering Pop");
            Delegate operation = this.callStack.Pop();
            CircularReferenceBreaker.Logger.Debug($"Exiting Pop. operation: {operation}");
        }

        public override string ToString()
        {
            CircularReferenceBreaker.Logger.Debug("Entering ToString");

            string result = string.Join("; ", this.callStack);

            CircularReferenceBreaker.Logger.Debug("Exiting ToString");
            return result;
        }

        #endregion Public methods

        #region Private Methods

        private bool IsVisitedDelegate(Delegate operation)
        {
            CircularReferenceBreaker.Logger.Debug($"Entering IsVisitedDelegate. operation: {operation}");
            bool result = this.callStack.Contains(operation);

            CircularReferenceBreaker.Logger.Debug($"Exiting IsVisitedDelegate. result: {result}");
            return result;
        }

        private void PushDelegate(Delegate operation)
        {
            CircularReferenceBreaker.Logger.Debug($"Executing Push. operation: {operation} ");
            this.callStack.Push(operation);
        }

        #endregion Private Methods
    }
}
