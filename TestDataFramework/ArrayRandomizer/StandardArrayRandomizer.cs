﻿/*
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
using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.Helpers;
using TestDataFramework.Logger;
using TestDataFramework.TypeGenerator.Concrete;
using TestDataFramework.ValueGenerator.Interfaces;

namespace TestDataFramework.ArrayRandomizer
{
    public class StandardArrayRandomizer : IArrayRandomizer
    {
        private const int DimensionLength = 5;
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(StandardArrayRandomizer));

        private readonly Random random;
        private readonly IValueGenerator valueGenerator;

        public StandardArrayRandomizer(Random random, IValueGenerator valueGenerator)
        {
            StandardArrayRandomizer.Logger.Debug("Entering constructor");

            this.random = random;
            this.valueGenerator = valueGenerator;

            StandardArrayRandomizer.Logger.Debug("Exiting constructor");
        }

        public object GetArray(PropertyInfoProxy propertyInfo, Type type, TypeGeneratorContext typeGeneratorContext)
        {
            StandardArrayRandomizer.Logger.Debug(
                $"Entering GetArray. propertyInfo: {propertyInfo?.GetExtendedMemberInfoString()}, type: {type}");

            Array resultArray;

            int rank = type.GetArrayRank();
            Type basicType = type.GetElementType();

            if (rank > 1)
            {
                StandardArrayRandomizer.Logger.Debug($"rank > 1: {rank}");

                var dimensionSizes = new int[rank];

                for (int i = 0; i < dimensionSizes.Length; i++)
                {
                    StandardArrayRandomizer.Logger.Debug($"dimensionSize {i}: {dimensionSizes[i]}");
                    dimensionSizes[i] = StandardArrayRandomizer.DimensionLength;
                }

                EmptyArrayResult emptyArrayResult = StandardArrayRandomizer.GetEmptyArray(type, dimensionSizes);

                do
                {
                    object resultElement = this.valueGenerator.GetValue(propertyInfo, basicType, typeGeneratorContext);
                    StandardArrayRandomizer.Logger.Debug($"Result element: {resultElement?.GetType()}");
                    emptyArrayResult.Array.SetValue(resultElement, emptyArrayResult.Indices.Value);
                    emptyArrayResult.Indices++;
                } while (!emptyArrayResult.Indices.Overflow);

                resultArray = emptyArrayResult.Array;
            }
            else
            {
                StandardArrayRandomizer.Logger.Debug($"rank <= 1: {rank}");

                ConstructorInfo constructor = type.GetConstructor(new[] {typeof(int)});
                resultArray = (Array) constructor.Invoke(new object[] {StandardArrayRandomizer.DimensionLength});

                for (int i = 0; i < StandardArrayRandomizer.DimensionLength; i++)
                {
                    object value = this.valueGenerator.GetValue(propertyInfo, basicType, typeGeneratorContext);
                    StandardArrayRandomizer.Logger.Debug("Generated Element.");
                    resultArray.SetValue(value, i);
                }
            }

            StandardArrayRandomizer.Logger.Debug("Exiting GetArray");

            return resultArray;
        }

        private static EmptyArrayResult GetEmptyArray(Type arrayType, int[] dimensionSizes)
        {
            int rank = arrayType.GetArrayRank();
            var indexTypes = new Type[rank];

            for (int i = 0; i < indexTypes.Length; i++)
                indexTypes[i] = typeof(int);

            ConstructorInfo constructor = arrayType.GetConstructor(indexTypes);

            var resultArray = (Array) constructor.Invoke(dimensionSizes.Cast<object>().ToArray());
            var indices = new IntArrayCounter(dimensionSizes);

            var result = new EmptyArrayResult {Array = resultArray, Indices = indices};
            return result;
        }

        private class EmptyArrayResult
        {
            public Array Array;
            public IntArrayCounter Indices;
        }

        private class IntArrayCounter
        {
            private readonly int[] indexSizes;

            public IntArrayCounter(int[] indexSizes)
            {
                this.indexSizes = indexSizes;
                this.Value = new int[indexSizes.Length];
                this.Overflow = false;
            }

            public bool Overflow { get; private set; }

            public int this[int index] => this.Value[index];

            public int[] Value { get; }

            public static IntArrayCounter operator ++(IntArrayCounter counter)
            {
                int i = 0;
                bool go;
                counter.Overflow = false;
                do
                {
                    go = false;
                    counter.Value[i]++;

                    if (counter.Value[i] < counter.indexSizes[i])
                        continue;

                    for (int j = 0; j <= i; j++)
                        counter.Value[j] = 0;

                    i++;

                    if (i >= counter.Value.Length)
                    {
                        counter.Overflow = true;
                        break;
                    }

                    go = true;
                } while (go);

                return counter;
            }
        }
    }
}