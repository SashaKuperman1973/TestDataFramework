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
using log4net;
using TestDataFramework.Helpers;
using TestDataFramework.Logger;
using TestDataFramework.ValueProvider.Interfaces;

namespace TestDataFramework.ValueProvider.Concrete
{
    public class AccumulatorValueProvider : IValueProvider
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(AccumulatorValueProvider));

        private static readonly LetterEncoder LetterEncoder = new LetterEncoder();

        private bool boolean;

        internal byte ByteCount = (byte) Helper.DefaultInitalCount;

        private int characterCount;

        private int countField = (int) Helper.DefaultInitalCount;

        internal int Count
        {
            get
            {
                if (this.countField != int.MaxValue)
                    return this.countField;

                this.countField = (int) Helper.DefaultInitalCount;
                return this.countField;
            }

            set => this.countField = value;
        }

        public int GetInteger(int? min, int? max)
        {
            AccumulatorValueProvider.Logger.Debug("Entering GetInteger");
            int result = this.Count++;

            AccumulatorValueProvider.Logger.Debug($"Exiting GetInteger. result: {result}");
            return result;
        }

        public long GetLongInteger(long? min, long? max)
        {
            AccumulatorValueProvider.Logger.Debug("Entering GetLongInteger");
            long result = this.Count++;

            AccumulatorValueProvider.Logger.Debug($"Exiting GetLongInteger. result: {result}");
            return result;
        }

        public short GetShortInteger(short? min, short? max)
        {
            AccumulatorValueProvider.Logger.Debug("Entering GetShortInteger");
            short result = (short) (this.Count++ % short.MaxValue);

            AccumulatorValueProvider.Logger.Debug($"Exiting GetShortInteger. result: {result}");
            return result;
        }

        public string GetString(int? length)
        {
            AccumulatorValueProvider.Logger.Debug($"Entering GetString. length: {length}");

            int lengthToUse = length ?? 10;

            string result = AccumulatorValueProvider.LetterEncoder.Encode((ulong) this.Count++, lengthToUse);

            result = result.PadRight(lengthToUse, '+');

            AccumulatorValueProvider.Logger.Debug($"Exiting GetString. result: {result}");
            return result;
        }

        public char GetCharacter()
        {
            AccumulatorValueProvider.Logger.Debug("Entering GetCharacter");

            const int startCode = 33;
            char result = (char) (startCode + this.characterCount++);

            AccumulatorValueProvider.Logger.Debug($"Exiting GetCharacter. result: {result}");
            return result;
        }

        public decimal GetDecimal(int? precision, double? min, double? max)
        {
            AccumulatorValueProvider.Logger.Debug("Entering GetDecimal");
            decimal result = this.Count++;

            AccumulatorValueProvider.Logger.Debug($"Exiting GetDecimal. result: {result}");
            return result;
        }

        public bool GetBoolean()
        {
            AccumulatorValueProvider.Logger.Debug("Entering GetBoolean");
            bool result = this.boolean;
            this.boolean = !this.boolean;

            AccumulatorValueProvider.Logger.Debug($"Exiting GetBoolean. result: {result}");
            return result;
        }

        public DateTime GetDateTime(PastOrFuture? pastOrFuture, Func<long?, long?, long> longIntegerGetter, long? min, long? max)
        {
            AccumulatorValueProvider.Logger.Debug("Entering GetDateTime");
            DateTime result = DateTime.Now.AddDays(this.Count++);

            AccumulatorValueProvider.Logger.Debug($"Exiting GetDateTime. result: {result}");
            return result;
        }

        public byte GetByte()
        {
            AccumulatorValueProvider.Logger.Debug("Entering GetByte");

            if (this.ByteCount == byte.MaxValue)
            {
                this.ByteCount = (byte) Helper.DefaultInitalCount;
                return byte.MaxValue;
            }

            byte result = this.ByteCount++;

            AccumulatorValueProvider.Logger.Debug($"Exiting GetByte. result: {result}");
            return result;
        }

        public double GetDouble(int? precision, double? min, double? max)
        {
            AccumulatorValueProvider.Logger.Debug("Entering GetDouble");
            double result = this.Count++;

            AccumulatorValueProvider.Logger.Debug($"Exiting GetDouble. result: {result}");
            return result;
        }

        public float GetFloat(int? precision, double? min, double? max)
        {
            AccumulatorValueProvider.Logger.Debug("Entering GetFloat");
            float result = this.Count++;

            AccumulatorValueProvider.Logger.Debug($"Exiting GetFloat. result: {result}");
            return result;
        }

        public string GetEmailAddress()
        {
            throw new NotSupportedException();
        }

        public Enum GetEnum(Type enumType)
        {
            throw new NotImplementedException();
        }
    }
}