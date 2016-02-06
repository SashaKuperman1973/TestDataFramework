using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntegrationTests.Tests;
using TestDataFramework.Populator;
using Tests.TestModels;

namespace IntegrationTests
{
    public static class Helpers
    {
        public static void Dump<T>(IEnumerable<RecordReference<T>> recordReference)
        {
            recordReference.ToList().ForEach(r => Helpers.Dump(r.RecordObject));
        }

        public static void Dump(object target)
        {
            Console.WriteLine();
            Console.WriteLine(target.GetType().Name);
            target.GetType().GetProperties().ToList().ForEach(p =>
            {
                try
                {
                    Console.WriteLine(p.Name + " = " + p.GetValue(target));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(p.Name + ": " + Helpers.GetMessage(ex));
                }
            });

            Console.WriteLine();
        }

        private static string GetMessage(Exception ex)
        {
            if (ex.InnerException != null)
            {
                return Helpers.GetMessage(ex.InnerException);
            }

            return ex.Message;
        }
    }
}
