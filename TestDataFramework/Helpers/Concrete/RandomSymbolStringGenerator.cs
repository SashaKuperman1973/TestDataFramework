using System;
using log4net;
using TestDataFramework.Helpers.Interfaces;
using TestDataFramework.ValueGenerator;

namespace TestDataFramework.Helpers.Concrete
{
    public class RandomSymbolStringGenerator : IRandomSymbolStringGenerator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RandomSymbolStringGenerator));

        public const int DefaultLength = 10;
        private readonly int constructorSetDefaultLength;

        private readonly Random random;

        public RandomSymbolStringGenerator(Random random, int? defaultLength = RandomSymbolStringGenerator.DefaultLength)
        {
            RandomSymbolStringGenerator.Logger.Debug("Entering constructor");

            this.random = random;
            this.constructorSetDefaultLength = defaultLength ?? RandomSymbolStringGenerator.DefaultLength;

            RandomSymbolStringGenerator.Logger.Debug("Exiting constructor");
        }

        public string GetRandomString(int? length = null)
        {
            RandomSymbolStringGenerator.Logger.Debug("Entering GetRandomString");

            length = length ?? this.constructorSetDefaultLength;

            var theString = new char[length.Value];

            for (var i = 0; i < length; i++)
            {
                int ascii = this.random.Next(26);
                ascii += 65;
                theString[i] = (char)ascii;
            }

            var result = new string(theString);

            RandomSymbolStringGenerator.Logger.Debug("Exiting GetRandomString");

            return result;
        }
    }
}
