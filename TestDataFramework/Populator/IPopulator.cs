using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TestDataFramework.Populator
{
    public interface IPopulator
    {
        void Bind();

        RecordReference<T> Add<T>(params Expression<Action<T>>[] assignmentLambdaExpressions) where T : new();

        RecordReference<T> Add<T>(RecordReference primaryRecordReference,
            params Expression<Action<T>>[] assignmentLambdaExpressions) where T : new();

        RecordReference<T> Add<T>(params SetExpression<T>[] setExpressions) where T : new();

        RecordReference<T> Add<T>(RecordReference primaryRecordReference,
            params SetExpression<T>[] setExpressions) where T : new();

        IList<RecordReference<T>> Add<T>(int copies, params Expression<Action<T>>[] assignmentLambdaExpressions)
            where T : new();

        IList<RecordReference<T>> Add<T>(int copies, RecordReference primaryRecordReference,
            params Expression<Action<T>>[] assignmentLambdaExpressions) where T : new();
    }
}