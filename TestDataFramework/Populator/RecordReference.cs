using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.Exceptions;
using TestDataFramework.TypeGenerator;

namespace TestDataFramework.Populator
{
    public class RecordReference
    {
        public RecordReference(object recordObject)
        {
            this.RecordObject = recordObject;
        }

        public object RecordObject { get; }

        public Type RecordType => this.RecordObject.GetType();

        public RecordReference ForeignReference { get; private set; }

        internal void AddForeignRecordReference(RecordReference foreignRecordReference)
        {
            RecordReference current = this;

            do
            {
                if (current.RecordType == foreignRecordReference.RecordType)
                {
                    throw new CircularForeignKeyReferenceException(foreignRecordReference, this);
                }

                current = current.ForeignReference;

            } while (current != null);

            this.ForeignReference = foreignRecordReference;
        }
    }
}
