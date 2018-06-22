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
using TestDataFramework.Exceptions;
using TestDataFramework.ListOperations.Interfaces;
using TestDataFramework.Populator.Concrete;

namespace TestDataFramework.ListOperations.Concrete
{
    public class GuaranteedValues
    {
        public IEnumerable<object> Values { get; set; }
        public int? FrequencyPercentage { get; set; }
        public int? TotalFrequency { get; set; }
        public ValueCountRequestOption ValueCountRequestOption { get; set; }
    }

    public enum ValueCountRequestOption
    {
        DoNotThrow,
        ThrowIfValueCountRequestedIsTooSmall
    }

    public class ValueGuaranteePopulator
    {
        public virtual void Bind<T>(OperableList<T> references, IEnumerable<GuaranteedValues> guaranteedValuesList,
            IValueGauranteePopulatorContextService contextService)
        {
            guaranteedValuesList = guaranteedValuesList.ToList();

            if (
                guaranteedValuesList.Any(
                    valueSet => !valueSet.FrequencyPercentage.HasValue && !valueSet.TotalFrequency.HasValue)
                )
            {
                throw new ValueGuaranteeException(Messages.NeitherPercentageNorTotalGiven);
            }

            List<RecordReference<T>> workingList =
                contextService.FilterInWorkingListOfReferfences(references, guaranteedValuesList);

            List<Tuple<IEnumerable<object>, int>> valuesPerPercentageSet =
                guaranteedValuesList.Where(valueSet => valueSet.FrequencyPercentage.HasValue)
                    .Select(
                        valueSet =>
                        {
                            int quantity = (int) ((float) references.Count * valueSet.FrequencyPercentage.Value / 100f);

                            quantity = quantity >= 1 ? quantity : 1;

                            if (valueSet.ValueCountRequestOption ==
                                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall
                                && quantity < valueSet.Values.Count())
                            {
                                throw new ValueGuaranteeException(
                                    string.Format(Messages.PercentFrequencyTooSmall, valueSet.Values.Count(),
                                        valueSet.FrequencyPercentage, quantity, references.Count));
                            }

                            var percentageValuesResult = new Tuple<IEnumerable<object>, int>(valueSet.Values, quantity);

                            return percentageValuesResult;
                        }).ToList();

            List<Tuple<IEnumerable<object>, int>> valuesPerTotalSet =
                guaranteedValuesList.Where(valueSet => valueSet.TotalFrequency.HasValue)
                    .Select(
                        valueSet =>
                        {
                            if (valueSet.ValueCountRequestOption ==
                                ValueCountRequestOption.ThrowIfValueCountRequestedIsTooSmall
                                && valueSet.TotalFrequency < valueSet.Values.Count())
                            {
                                throw new ValueGuaranteeException(
                                    string.Format(Messages.TotalFrequencyTooSmall, valueSet.Values.Count(),
                                        valueSet.TotalFrequency, references.Count));
                            }

                            return new Tuple<IEnumerable<object>, int>(valueSet.Values, valueSet.TotalFrequency.Value);
                        })
                    .ToList();

            IEnumerable<Tuple<List<object>, int>> allValues =
                valuesPerPercentageSet.Concat(valuesPerTotalSet)
                    .Select(valueSet => new Tuple<List<object>, int>(valueSet.Item1.ToList(), valueSet.Item2))
                    .ToList();

            int totalQuantityOfGuaranteedValues =
                allValues.Sum(tuple => tuple.Item2);

            if (totalQuantityOfGuaranteedValues > workingList.Count)
                throw new ValueGuaranteeException(Messages.TooFewReferencesForValueGuarantee);

            var random = new Random();

            foreach (Tuple<List<object>, int> valueAndPopulationQuantity in allValues)
                for (int valueIndex = 0; valueIndex < valueAndPopulationQuantity.Item2; valueIndex++)
                {
                    int referenceIndex = random.Next(workingList.Count);
                    RecordReference<T> reference = workingList[referenceIndex];

                    object subject =
                        valueAndPopulationQuantity.Item1[valueIndex % valueAndPopulationQuantity.Item1.Count];

                    Type subjectType = subject.GetType();

                    if (subjectType.IsGenericType && subjectType.GetGenericTypeDefinition() == typeof(Func<>))
                    {
                        var objectFunc = (Func<object>) subject;
                        contextService.SetRecordReference(reference, objectFunc());
                    }
                    else
                    {
                        contextService.SetRecordReference(reference, subject);
                    }

                    workingList.RemoveAt(referenceIndex);
                }
        }
    }
}