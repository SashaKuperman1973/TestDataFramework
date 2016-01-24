using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TestDataFramework.Populator
{
    public interface IPopulator
    {
        void Bind();

        IList<RecordReference<T>> Add<T>(int copies, RecordReference primaryRecordReference = null) where T : new();

        RecordReference<T> Add<T>(RecordReference primaryRecordReference = null) where T : new();
    }
}