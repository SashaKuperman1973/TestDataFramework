/*
    Copyright 2016 Alexander Kuperman

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

namespace TestDataFramework.ValueProvider.Interfaces
{
    public enum PastOrFuture
    {
        Past,
        Future
    }

    public interface IValueProvider
    {
        int GetInteger(int? max);

        long GetLongInteger(long? max);

        short GetShortInteger(short? max);

        string GetString(int? length);

        char GetCharacter();

        decimal GetDecimal(int? precision);

        bool GetBoolean();

        DateTime GetDateTime(PastOrFuture? pastOrFuture, Func<long?, long> longIntegerGetter);

        byte GetByte();

        double GetDouble(int? precision);

        float GetFloat(int? precision);

        string GetEmailAddress();
    }
}
