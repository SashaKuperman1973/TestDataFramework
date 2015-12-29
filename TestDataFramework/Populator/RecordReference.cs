using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.Exceptions;
using TestDataFramework.TypeGenerator;

namespace TestDataFramework.Populator
{
    public abstract class RecordReference
    {
        protected RecordReference(object recordObject)
        {
            this.RecordObject = recordObject;
        }

        public object RecordObject { get; }

        public Type RecordType => this.RecordObject.GetType();

        public RecordReference PrimaryKeyReference { get; private set; }

        public void AddPrimaryRecordReference(RecordReference primaryRecordReference)
        {
            RecordReference current = this;

            do
            {
                if (current.RecordType == primaryRecordReference.RecordType)
                {
                    throw new CircularForeignKeyReferenceException(primaryRecordReference, this);
                }

                current = current.PrimaryKeyReference;

            } while (current != null);

            this.PrimaryKeyReference = primaryRecordReference;
        }
    }
}
