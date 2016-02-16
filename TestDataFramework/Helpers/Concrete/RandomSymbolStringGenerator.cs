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
using log4net;
using TestDataFramework.Helpers.Interfaces;

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
