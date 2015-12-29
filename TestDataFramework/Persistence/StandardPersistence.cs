using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.Exceptions;
using TestDataFramework.Populator;
//using TestDataFramework.Repository;

namespace TestDataFramework.Persistence
{
    public class StandardPersistence : IPersistence
    {
        //private readonly IRepository repository;

        public void Persist(IEnumerable<RecordReference> recordReferences)
        {
            foreach (RecordReference recordReference in recordReferences)
            {
                this.GenerateAutoPrimaryKey(recordReference);
            }
        }

        private void GenerateAutoPrimaryKey(RecordReference recordReference)
        {
            throw new NotImplementedException();

            Type recordType = recordReference.RecordType;

            List<PropertyInfo> propertyList = recordType.GetProperties().ToList();

            List<PropertyInfo> autoIdentityPropertyInfoList = propertyList.Where(p =>
            {
                IEnumerable<AutoIdentityAttribute> attrs = p.GetCustomAttributes<AutoIdentityAttribute>();

                if (attrs == null)
                {
                    return false;
                }

                attrs = attrs.ToList();

                if (attrs.Count() > 1)
                {
                    throw new AmbiguousMatchException(string.Format(Messages.AmbigousAttributeMatch,
                        "AutoIdentityAttribute", p.Name, recordType));
                }

                return true;
            }).ToList();

            if (autoIdentityPropertyInfoList.Count > 1)
            {
                throw new AmbiguousMatchException(string.Format(Messages.AmbigousPropertyMatch, "AutoIdentityAttribute",
                    recordType));
            }

            PropertyInfo autoIdentityProerty = autoIdentityPropertyInfoList.FirstOrDefault();

            if (autoIdentityProerty == null)
            {
                return;
            }

            string repositoryTableName = recordType.GetCustomAttribute<TableAttribute>()?.Name ?? recordType.Name;
        }
    }
}
