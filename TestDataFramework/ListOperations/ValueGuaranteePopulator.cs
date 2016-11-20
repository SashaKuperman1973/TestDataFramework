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

            List<Tuple<IEnumerable<object>, int>> valuesPerPercentageSet =
                guaranteedValuesList.Where(valueSet => valueSet.FrequencyPercentage.HasValue)
                    .Select(
                        valueSet =>
                        {
                            int quantity = (int) ((float) references.Count*valueSet.FrequencyPercentage.Value/100f);

                            quantity = quantity >= 1 ? quantity : 1;

                            var percentageValuesResult = new Tuple<IEnumerable<object>, int>(valueSet.Values, quantity);

                            return percentageValuesResult;
                        }).ToList();

            List<Tuple<IEnumerable<object>, int>> valuesPerTotalSet =
                guaranteedValuesList.Where(valueSet => valueSet.TotalFrequency.HasValue)
                    .Select(
                        valueSet => new Tuple<IEnumerable<object>, int>(valueSet.Values, valueSet.TotalFrequency.Value))
                    .ToList();

            IEnumerable<Tuple<List<object>, int>> allValues =
                valuesPerPercentageSet.Concat(valuesPerTotalSet)
                    .Select(valueSet => new Tuple<List<object>, int>(valueSet.Item1.ToList(), valueSet.Item2))
                    .ToList();

            int totalQuantityOfGuaranteedValues =
                allValues.Sum(tuple => tuple.Item2);

            if (totalQuantityOfGuaranteedValues > references.Count)
            {
                throw new ValueGuaranteeException(Messages.TooFewReferencesForValueGuarantee);
            }

            var workingList = new OperableList<T>(references, this);

            var random = new Random();

            foreach (Tuple<List<object>, int> valueAndPopulationQuantity in allValues)
            {
                int valueIndex = 0;
                for (int i = 0; i < valueAndPopulationQuantity.Item2; i++)
                {
                    int referenceIndex = random.Next(workingList.Count);
                    RecordReference reference = workingList[referenceIndex];

                    object subject =
                        valueAndPopulationQuantity.Item1[valueIndex++%valueAndPopulationQuantity.Item1.Count];

                    Func<object> objectFunc;

                    Type subjectType = subject.GetType();

                    if (subjectType.IsGenericType && subjectType.GetGenericTypeDefinition() == typeof(Func<>))
                    {
                        objectFunc = (Func<object>) subject;

                        Type[] typeArgs = subject.GetType().GetGenericArguments();
                        if (typeArgs.Length > 1 || typeArgs[0] != typeof(T))
                        {
                            throw new ValueGuaranteeException(string.Format(Messages.GuaranteedTypeNotOfListType,
                                typeof(T), typeArgs[0], ValueGuaranteePopulator.GetValue(objectFunc)));
                        }
                    }
                    else
                    {
                        if (subjectType != typeof(T))
                        {
                            throw new ValueGuaranteeException(string.Format(Messages.GuaranteedTypeNotOfListType,
                                typeof(T), subject.GetType(), subject));
                        }

                        objectFunc = () => subject;
                    }

                    reference.PreBoundObject = objectFunc;

                    workingList.RemoveAt(referenceIndex);
                }
            }
        }

        private static object GetValue(Func<object> objectFunc)
        {
            object value;

            try
            {
                value = objectFunc();
            }
            catch (Exception ex)
            {
                value =
                    $"Value func evaluation resulted in exception: {ex.GetType()}, Message: {ex.Message}, Stack trace: {ex.StackTrace}";
            }

            return value;
        }
    }
}
