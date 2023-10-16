/*
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using TestDataFramework.Exceptions;
using TestDataFramework.Helpers;
using TestDataFramework.Logger;

namespace TestDataFramework.Populator.Concrete
{
    public class DeepCollectionSettingConverter
    {
        private static readonly ILog Logger = StandardLogManager.GetLogger(typeof(DeepCollectionSettingConverter));

        public virtual IEnumerable<TListElement> Convert<TListElement>(IEnumerable<TListElement> convertInput,
            PropertyInfoProxy propertyInfo)
        {
            DeepCollectionSettingConverter.Logger.Calling(nameof(this.Convert));

            var deepListConverterKvps = new List<Kvp<TListElement>>
            {
                new Kvp<TListElement>(typeof(IEnumerable<TListElement>), input => input),
                new Kvp<TListElement>(typeof(List<TListElement>), input => input.ToList()),
                new Kvp<TListElement>(typeof(TListElement[]), input => input.ToArray())
            };

            ConverterFunction<TListElement> converterFunction = deepListConverterKvps
                .FirstOrDefault(
                    kvp => propertyInfo.PropertyType.IsAssignableFrom(kvp.Type))?.ConverterFunction;

            if (converterFunction == null)
                throw new ArgumentException(string.Format(Messages.TypeNotSupportedForDeepCollectionSetting,
                    propertyInfo.PropertyType));

            IEnumerable<TListElement> result = converterFunction(convertInput);
            return result;
        }

        private delegate IEnumerable<TListElement> ConverterFunction<TListElement>(IEnumerable<TListElement> input);

        private class Kvp<TListElement>
        {
            public Kvp(Type type, ConverterFunction<TListElement> converterFunction)
            {
                this.Type = type;
                this.ConverterFunction = converterFunction;
            }

            public Type Type { get; }

            public ConverterFunction<TListElement> ConverterFunction { get; }
        }
    }
}