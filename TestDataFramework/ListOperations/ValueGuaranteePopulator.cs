using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestDataFramework.Exceptions;
using TestDataFramework.Populator;
using TestDataFramework.Populator.Concrete;

namespace TestDataFramework.ListOperations
{
    public class ValueGuaranteePopulator
    {
        public virtual void Bind<T>(OperableList<T> references, IEnumerable<T> guaranteedValues,
            int frequencyPercentage)
        {
            guaranteedValues = guaranteedValues.ToList();

            int referencesPerValueQuantity =
                (int) ((float) references.Count*frequencyPercentage/guaranteedValues.Count()/100f);

            if (referencesPerValueQuantity == 0)
            {
                referencesPerValueQuantity = 1;
            }

            int totalReferencesQuantity = referencesPerValueQuantity*guaranteedValues.Count();

            if (totalReferencesQuantity > references.Count)
            {
                throw new TooFewReferencesForValueGuaranteeException();
            }

            var workingList = new OperableList<T>(references);

            var random = new Random();

            foreach (T value in guaranteedValues)
            {
                for (int i = 0; i < referencesPerValueQuantity; i++)
                {
                    int referenceIndex = random.Next(workingList.Count);
                    RecordReference reference = workingList[referenceIndex];
                    reference.RecordObject = value;
                    reference.IsAPrePopulatedValue = true;
                    workingList.RemoveAt(referenceIndex);
                }
            }
        }
    }
}
