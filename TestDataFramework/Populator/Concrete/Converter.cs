using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TestDataFramework.Exceptions;

namespace TestDataFramework.Populator.Concrete
{
    public class DeepCollectionSettingConverter
    {
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

        public virtual IEnumerable<TListElement> Convert<TListElement>(IEnumerable<TListElement> convertInput, PropertyInfo propertyInfo)
        {
            var deepListConverterKvps = new List<Kvp<TListElement>>
            {
                new Kvp<TListElement>(typeof(IEnumerable<TListElement>), input => input),
                new Kvp<TListElement>(typeof(List<TListElement>), input => input.ToList()),
                new Kvp<TListElement>(typeof(TListElement[]), input => input.ToArray()),
            };

            ConverterFunction<TListElement> converterFunction = deepListConverterKvps
                .FirstOrDefault(
                    kvp => propertyInfo.PropertyType.IsAssignableFrom(kvp.Type))?.ConverterFunction;
                
            if (converterFunction == null)
            {
                throw new ArgumentException(string.Format(Messages.TypeNotSupportedForDeepCollectionSetting,
                    propertyInfo.PropertyType));
            }
            
            IEnumerable<TListElement> result = converterFunction(convertInput);
            return result;
        }
    }
}
