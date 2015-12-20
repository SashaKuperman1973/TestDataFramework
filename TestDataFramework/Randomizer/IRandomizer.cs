using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDataFramework.Randomizer
{
    public interface IRandomizer
    {
        int RandomizeInteger();
        long RandomizeLongInteger();
        short RandomizeShortInteger();
        string RandomizeString(int? length);
        char RandomizeCharacter();
        decimal RandomizeDecimal(int? precision);
        bool RandomizeBoolean();
    }
}
