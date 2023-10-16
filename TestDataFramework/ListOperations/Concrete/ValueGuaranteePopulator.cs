/*
    Copyright 2016, 2017, 2018, 2019, 2023 Alexander Kuperman

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
using TestDataFramework.Populator.Concrete.OperableList;

namespace TestDataFramework.ListOperations.Concrete
{
    public class GuaranteedValues
    {
        public IEnumerable<object> Values { get; set; }
        public int? FrequencyPercentage { get; set; }
        public int? TotalFrequency { get; set; }
        public ValueCountRequestOption ValueCountRequestOption { get; set; }
        public object FieldSetterIdentifier { get; set; }
    }

    public enum ValueCountRequestOption
    {
        DoNotThrow,
        ThrowIfValueCountRequestedIsTooSmall
    }

    public class ValueGuaranteePopulator
    {
        private class ValuesWithQuantity
        {
            public List<object> Values { get; set; }
            public int Quantity { get; set; }
        }

        private class ValuesWithFieldSetId : ValuesWithQuantity
        {
            public object FieldSetterIdentifier { get; set; }
        }

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

            List<RecordReference<T>> workingReferenceList =
                contextService.FilterInWorkingListOfReferfences(references, guaranteedValuesList);

            List<ValuesWithFieldSetId> valuesPerPercentageSet =
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

                            var percentageValuesResult = new ValuesWithFieldSetId
                            {
                                Values = valueSet.Values.ToList(),
                                Quantity = quantity,
                                FieldSetterIdentifier = valueSet.FieldSetterIdentifier
                            };
                            
                            return percentageValuesResult;
                        }).ToList();

            List<ValuesWithFieldSetId> valuesPerTotalSet =
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

                            var totalValuesResult = new ValuesWithFieldSetId
                            {
                                Values = valueSet.Values.ToList(),
                                Quantity = valueSet.TotalFrequency.Value,
                                FieldSetterIdentifier = valueSet.FieldSetterIdentifier
                            };

                            return totalValuesResult;
                        })
                    .ToList();

            IEnumerable<ValuesWithFieldSetId> allValues = valuesPerPercentageSet.Concat(valuesPerTotalSet);

            IEnumerable<IGrouping<object, ValuesWithFieldSetId>> fieldSetGroups = allValues.GroupBy(p => p.FieldSetterIdentifier).ToArray();

            int totalQuantityOfGuaranteedValues =
                fieldSetGroups.Sum(fieldSet => fieldSet.Max(values => values.Quantity));

            if (totalQuantityOfGuaranteedValues > workingReferenceList.Count)
                throw new ValueGuaranteeException(Messages.TooFewReferencesForValueGuarantee);

            var random = new Random();

            foreach (IEnumerable<ValuesWithFieldSetId> fieldSet in fieldSetGroups)
            {
                List<ValuesWithQuantity> workingFieldSet = fieldSet.Select(value => new ValuesWithQuantity
                {
                    Values = new List<object>(value.Values),
                    Quantity = value.Quantity
                }).ToList();

                Dictionary<ValuesWithQuantity, List<object>> fieldValues =
                    workingFieldSet.ToDictionary(fieldSetParam => fieldSetParam, fieldSetParam => new List<object>(fieldSetParam.Values));

                do
                {
                    var workingValues = new List<object>();

                    for (int i = 0; i < workingFieldSet.Count; i++)
                    {
                        int selectedValueIndex = random.Next(workingFieldSet[i].Values.Count);
                        object selectedValue = workingFieldSet[i].Values[selectedValueIndex];
                        workingValues.Add(selectedValue);

                        workingFieldSet[i].Values.RemoveAt(selectedValueIndex);
                        workingFieldSet[i].Quantity--;

                        if (workingFieldSet[i].Quantity == 0)
                        {
                            workingFieldSet.RemoveAt(i);
                            i--;
                            continue;
                        }

                        if (!workingFieldSet[i].Values.Any())
                        {
                            workingFieldSet[i].Values = new List<object>(fieldValues[workingFieldSet[i]]);
                        }
                    }

                    int referenceIndex = random.Next(workingReferenceList.Count);
                    RecordReference<T> reference = workingReferenceList[referenceIndex];

                    foreach (object value in workingValues)
                    {
                        ValueGuaranteePopulator.SetRecordReference(value, contextService, reference);
                    }

                    workingReferenceList.RemoveAt(referenceIndex);

                } while (workingFieldSet.Any());
            } 
        }

        private static void SetRecordReference<T>(
            object value, 
            IValueGauranteePopulatorContextService contextService, 
            RecordReference<T> reference
        )
        {
            Type valueType = value.GetType();

            if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(Func<>))
            {
                var valueFunc = (Func<object>)value;
                contextService.SetRecordReference(reference, valueFunc());
            }
            else
            {
                contextService.SetRecordReference(reference, value);
            }
        }
    }
}