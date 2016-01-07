using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using TestDataFramework.DeferredValueGenerator.Interfaces;
using TestDataFramework.Exceptions;

namespace TestDataFramework.DeferredValueGenerator.Concrete
{
    public class HandlerDictionary<T> : IHandlerDictionary<T>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(HandlerDictionary<T>));

        private static Dictionary<Type, HandlerDelegate<T>> propertyTypeHandlerDictionary;
        private readonly IDeferredValueGeneratorHandler<T> generatorHandler;

        public HandlerDictionary(IDeferredValueGeneratorHandler<T> generatorHandler)
        {
            this.generatorHandler = generatorHandler;
            this.FillDictionary();
        }

        public HandlerDelegate<T> this[Type type]
        {
            get
            {
                HandlerDictionary<T>.Logger.Debug("Entering Type indexer");

                HandlerDelegate<T> handler;
                if (!HandlerDictionary<T>.propertyTypeHandlerDictionary.TryGetValue(type, out handler))
                {
                    throw new KeyNotFoundException(string.Format(Messages.PropertyKeyNotFound, type));
                }

                HandlerDictionary<T>.Logger.Debug("Exiting Type indexer");
                return handler;
            }
        }

        private void FillDictionary()
        {
            HandlerDictionary<T>.Logger.Debug("Entering FillDictionary");

            if (HandlerDictionary<T>.propertyTypeHandlerDictionary != null)
            {
                HandlerDictionary<T>.Logger.Debug(
                    "propertyTypeHandlerDictionary already populated. Exiting FillDictionary");

                return;
            }

            HandlerDictionary<T>.propertyTypeHandlerDictionary = new Dictionary<Type, HandlerDelegate<T>>()
            {
                {typeof (int), this.generatorHandler.NumberHandler},
                {typeof (short), this.generatorHandler.NumberHandler},
                {typeof (long), this.generatorHandler.NumberHandler},
                {typeof (byte), this.generatorHandler.NumberHandler},
                {typeof (string), this.generatorHandler.StringHandler},
            };

            HandlerDictionary<T>.Logger.Debug("Exiting FillDictionary");
        }
    }
}
