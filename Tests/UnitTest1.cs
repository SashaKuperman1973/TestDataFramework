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
                        string tableName = null;

                        CustomAttributeData tableAttributeData =
                            definedType.CustomAttributes.FirstOrDefault(
                                attributeData =>
                                    attributeData.AttributeType == typeof (TableAttribute) &&
                                    !string.IsNullOrWhiteSpace(tableName = (string)
                                        attributeData.NamedArguments.First(
                                            args =>
                                                args.MemberName.Equals("Name",
                                                    StringComparison.OrdinalIgnoreCase)).TypedValue.Value)
                                );

                        if (tableAttributeData != null)
                        {
                            tableAttributeDataDictionary.Add(tableName, definedType);
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
