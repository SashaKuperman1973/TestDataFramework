/*
    Copyright 2016, 2017, 2018, 2019 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TestDataFramework is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TestDataFramework.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using log4net;
using TestDataFramework.Logger;

namespace TestDataFramework.RepositoryOperations
{
    public class CircularReferenceBreaker
    {
        #region Private Fields

        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(CircularReferenceBreaker));

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