using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestDataFramework.HandledTypeGenerator;
using TestDataFramework.UniqueValueGenerator.Interface;
using TestDataFramework.ValueGenerator;
using TestDataFramework.ValueGenerator.Interface;
using Tests.TestModels;

namespace Tests.Tests
{
    [TestClass]
    public class StandardHandledTypeGeneratorTests
    {
        private StandardHandledTypeGenerator handledTypeGenerator;
        private Mock<IValueGenerator> valueGeneratorMock;
        private Mock<IValueGenerator> accumulatorValueGeneratorMock;
        private Mock<Random> randomMock;

        [TestInitialize]
        public void Initialize()
        {
            XmlConfigurator.Configure();

            this.valueGeneratorMock = new Mock<IValueGenerator>();
            this.accumulatorValueGeneratorMock = new Mock<IValueGenerator>();
            this.randomMock = new Mock<Random>();

            this.handledTypeGenerator = new StandardHandledTypeGenerator(this.valueGeneratorMock.Object,
                () => this.accumulatorValueGeneratorMock.Object, this.randomMock.Object);
        }

        [TestMethod]
        public void GetObject_KeyValuePair_Test()
        {
            // Arrange

            this.valueGeneratorMock.Setup(m => m.GetValue(null, typeof(int))).Returns(5);
            this.valueGeneratorMock.Setup(m => m.GetValue(null, typeof(string))).Returns("ABCD");

            this.randomMock.Setup(m => m.Next(It.IsAny<int>())).Returns(2);

            // Act

            object result = this.handledTypeGenerator.GetObject(typeof(KeyValuePair<int,string>));

            // Assert

            Assert.AreEqual(new KeyValuePair<int, string>(5, "ABCD"), result);
        }

        [TestMethod]
        public void GetObject_Dictionary_ValueKey_Test()
        {
            Console.WriteLine("IDictionary");
            this.Dictionary_ValueKey_Test(typeof(IDictionary<int, string>));

            Console.WriteLine("Dictionary");
            this.Dictionary_ValueKey_Test(typeof(Dictionary<int, string>));
        }

        private void Dictionary_ValueKey_Test(Type dictionaryType)
        {
            this.Initialize();

            // Arrange

            int i = 0;
            string[] s = new[] {"AA", "BB", "CC", "DD"};


            this.accumulatorValueGeneratorMock.Setup(m => m.GetValue(null, typeof(int))).Returns(() => ++i);
            this.valueGeneratorMock.Setup(m => m.GetValue(null, typeof(string))).Returns(() => s[i - 1]);

            this.randomMock.Setup(m => m.Next(It.IsAny<int>())).Returns(3);

            // Act

            object result = this.handledTypeGenerator.GetObject(dictionaryType);

            // Assert

            var dictionary = result as Dictionary<int, string>;

            this.accumulatorValueGeneratorMock.Verify(m => m.GetValue(null, typeof(int)), Times.Exactly(4));
            this.valueGeneratorMock.Verify(m => m.GetValue(null, typeof (string)), Times.Exactly(4));

            Assert.IsNotNull(dictionary);
            Assert.AreEqual("AA", dictionary[1]);
            Assert.AreEqual("BB", dictionary[2]);
            Assert.AreEqual("CC", dictionary[3]);
            Assert.AreEqual("DD", dictionary[4]);
        }

        [TestMethod]
        public void GetObject_Dictionary_ReferenceKey_Test()
        {
            Console.WriteLine("IDictionary");
            this.Dictionary_ReferenceKey_Test(typeof(IDictionary<object, string>));

            Console.WriteLine("Dictionary");
            this.Dictionary_ReferenceKey_Test(typeof(Dictionary<object, string>));
        }

        private void Dictionary_ReferenceKey_Test(Type dictionaryType)
        {
            this.Initialize();

            // Arrange

            int i = 0;
            object[] o = new[] {new object(), new object(), new object(), new object(),};
            string[] s = new[] { "AA", "BB", "CC", "DD" };

            this.valueGeneratorMock.Setup(m => m.GetValue(null, typeof(object))).Returns(() => o[i++]);
            this.valueGeneratorMock.Setup(m => m.GetValue(null, typeof(string))).Returns(() => s[i - 1]);

            this.randomMock.Setup(m => m.Next(It.IsAny<int>())).Returns(3);

            // Act

            object result = this.handledTypeGenerator.GetObject(dictionaryType);

            // Assert

            var dictionary = result as Dictionary<object, string>;

            this.valueGeneratorMock.Verify(m => m.GetValue(null, typeof (object)), Times.Exactly(4));
            this.valueGeneratorMock.Verify(m => m.GetValue(null, typeof (string)), Times.Exactly(4));

            Assert.IsNotNull(dictionary);
            Assert.AreEqual("AA", dictionary[o[0]]);
            Assert.AreEqual("BB", dictionary[o[1]]);
            Assert.AreEqual("CC", dictionary[o[2]]);
            Assert.AreEqual("DD", dictionary[o[3]]);
        }

        [TestMethod]
        public void GetObject_List_Test()
        {
            Console.WriteLine("List");
            this.ListTest(typeof(List<SubjectClass>));

            Console.WriteLine("IList");
            this.ListTest(typeof(IList<SubjectClass>));

            Console.WriteLine("IEnumerable");
            this.ListTest(typeof(IEnumerable<SubjectClass>));
        }

        private void ListTest(Type listType)
        {
            this.Initialize();

            // Arrange

            SubjectClass[] sc = new[] {new SubjectClass(), new SubjectClass(), new SubjectClass(), new SubjectClass(),};
            int i = 0;

            this.valueGeneratorMock.Setup(m => m.GetValue(null, typeof (SubjectClass))).Returns(() => sc[i++]);

            this.randomMock.Setup(m => m.Next(It.IsAny<int>())).Returns(3);

            // Act

            var list = this.handledTypeGenerator.GetObject(listType) as List<SubjectClass>;

            // Assert

            Assert.IsNotNull(list);
            Assert.AreEqual(sc[0], list[0]);
            Assert.AreEqual(sc[1], list[1]);
            Assert.AreEqual(sc[2], list[2]);
            Assert.AreEqual(sc[3], list[3]);
        }
    }
}
