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
