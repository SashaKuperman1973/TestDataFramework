using System;
using TestDataFramework.Helpers.Interfaces;

namespace TestDataFramework.Helpers.Concrete
{
    public class RandomSymbolStringGenerator : IRandomSymbolStringGenerator
    {
        public const int DefaultLength = 10;
        private readonly int constructorSetDefaultLength;

        private readonly Random random;

        public RandomSymbolStringGenerator(Random random, int? defaultLength = RandomSymbolStringGenerator.DefaultLength)
        {
            this.random = random;
            this.constructorSetDefaultLength = defaultLength ?? RandomSymbolStringGenerator.DefaultLength;
        }

        public string GetRandomString(int? length = null)
        {
            length = length ?? this.constructorSetDefaultLength;

            var theString = new char[length.Value];

            for (var i = 0; i < length; i++)
            {
                int ascii = this.random.Next(26);
                ascii += 65;
                theString[i] = (char)ascii;
            }

            var result = new string(theString);

            return result;
        }
    }
}
