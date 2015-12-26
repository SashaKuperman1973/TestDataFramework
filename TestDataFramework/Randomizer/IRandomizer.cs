using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDataFramework.Randomizer
{
    public enum PastOrFuture
    {
        Past,
        Future
    }

    public interface IRandomizer
    {
        int RandomizeInteger(int? max);

        long RandomizeLongInteger(long? max);

        short RandomizeShortInteger(short? max);

        string RandomizeString(int? length);

        char RandomizeCharacter();

        decimal RandomizeDecimal(int? precision);

        bool RandomizeBoolean();

        DateTime RandomizeDateTime(PastOrFuture? pastOrFuture);

        byte RandomizeByte();

        double RandomizeDouble(int? precision);

        object RandomizeEmailAddress();
    }
}
