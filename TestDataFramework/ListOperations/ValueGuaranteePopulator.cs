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
        public virtual void Bind<T>(OperableList<T> references, List<GuaranteedValues<T>> guaranteedValuesList)
        {
            if (
                guaranteedValuesList.Any(
                    valueSet => !valueSet.FrequencyPercentage.HasValue && !valueSet.TotalFrequency.HasValue))
            {
                throw new ValueGuaranteeException(Messages.NeitherPercentageNorTotalGiven);
            }

            List<Tuple<IEnumerable<T>, int>> valuesPerPercentageSet =
                guaranteedValuesList.Where(valueSet => valueSet.FrequencyPercentage.HasValue)
                    .Select(
                        valueSet =>
                        {
                            int quantity = (int)
                                ((float) references.Count*valueSet.FrequencyPercentage.Value/valueSet.Values.Count()/
                                 100f);

                            quantity = quantity >= 1 ? quantity : 1;

                            var percentageValuesResult = new Tuple<IEnumerable<T>, int>(valueSet.Values, quantity);

                            return percentageValuesResult;
                        }).ToList();

            List<Tuple<IEnumerable<T>, int>> valuesPerTotalSet =
                guaranteedValuesList.Where(valueSet => valueSet.TotalFrequency.HasValue)
                    .Select(valueSet => new Tuple<IEnumerable<T>, int>(valueSet.Values, valueSet.TotalFrequency.Value))
                    .ToList();

            int totalQuantityOfGuaranteedValues =
                valuesPerPercentageSet.Concat(valuesPerTotalSet).Sum(tuple => tuple.Item2);

            if (totalQuantityOfGuaranteedValues > references.Count)
            {
                throw new ValueGuaranteeException(Messages.TooFewReferencesForValueGuarantee);
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
