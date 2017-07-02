/*
    Copyright 2016, 2017 Alexander Kuperman

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
        public virtual void Bind<T>(OperableList<T> references, List<GuaranteedValues> guaranteedValuesList)
        {
            if (
                guaranteedValuesList.Any(
                    valueSet => !valueSet.FrequencyPercentage.HasValue && !valueSet.TotalFrequency.HasValue))
            {
                throw new ValueGuaranteeException(Messages.NeitherPercentageNorTotalGiven);
            }

            List<RecordReference<T>> workingList  =
                references.Where(reference => !reference.ExplicitProperySetters.Any()).ToList();

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

            if (totalQuantityOfGuaranteedValues > workingList.Count)
            {
                throw new ValueGuaranteeException(Messages.TooFewReferencesForValueGuarantee);
            }

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
