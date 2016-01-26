using System;

namespace TestDataFramework.ValueProvider
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

        string GetEmailAddress();
    }
}
