﻿using System.Collections.Generic;

namespace TestDataFramework.Populator
{
    internal interface IPopulator
    {
        void Bind();

        IList<RecordReference<T>> Add<T>(int copies, RecordReference primaryRecordReference = null) where T : new();

        RecordReference<T> Add<T>(RecordReference primaryRecordReference = null) where T : new();
    }
}