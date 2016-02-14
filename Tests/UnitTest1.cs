using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Reflection;
using Castle.Components.DictionaryAdapter.Xml;
using Castle.Core.Internal;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataFramework;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var tableAttributeDataDictionary = new Dictionary<string, Type>();

            DateTime start = DateTime.Now;

            Assembly thisAssembly = this.GetType().Assembly;
            List<AssemblyName> assemblyNameList = thisAssembly.GetReferencedAssemblies().ToList();
            assemblyNameList.Add(thisAssembly.GetName());

            AppDomain domain = AppDomain.CreateDomain("MyDomain", null, AppDomain.CurrentDomain.SetupInformation);

            try
            {
                foreach (AssemblyName assemblyName in assemblyNameList)
                {
                    Assembly assembly = domain.Load(assemblyName);

                    foreach (TypeInfo definedType in assembly.DefinedTypes)
                    {
                        var tableAttribute =
                            definedType.GetCustomAttribute<TableAttribute>();

                        if (tableAttribute != null)
                        {
                            tableAttributeDataDictionary.Add(tableAttribute.Name, definedType);
                        }
                    }
                }

                AppDomain.Unload(domain);
            }
            catch (ReflectionTypeLoadException ex)
            {
                ex.LoaderExceptions.ForEach(Console.WriteLine);

                throw;
            }

            TimeSpan duration = DateTime.Now - start;

            Console.WriteLine(tableAttributeDataDictionary.Count);
            Console.WriteLine(duration);

            Console.Write(tableAttributeDataDictionary["DbTable"]);
        }
    }
}
