using DOTNET.Common.Reflections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.DOTNET.Common.Reflections
{
    [TestClass]
    public class ReflectionHelperTest
    {
        [TestMethod]
        public void CopyPropertiesTest1()
        {
            CopyPropertiesModelTest1 source = new CopyPropertiesModelTest1 { Name = "Behnam", LastName = "2064", Age = 5 };

            var arg = new CopyObjectArguments<CopyPropertiesModelTest1>()
            {
                Source = source,
                CaseSensitive = StringComparison.Ordinal,
            };

            CopyPropertiesModelTest1 copy = ReflectionHelper.CopyProperties<CopyPropertiesModelTest1>(arg);

            Assert.AreEqual(copy.Name, source.Name);
            Assert.AreEqual(copy.LastName, source.LastName);
            Assert.AreEqual(copy.Age, source.Age);
        }

        [TestMethod]
        public void CopyPropertiesTest2()
        {
            CopyPropertiesModelTest1 source = new CopyPropertiesModelTest1 { Name = "Behnam", LastName = null, Age = 5 };

            var arg = new CopyObjectArguments<CopyPropertiesModelTest1>()
            {
                Source = source,
                CaseSensitive = StringComparison.Ordinal,
            };

            CopyPropertiesModelTest1 copy = ReflectionHelper.CopyProperties<CopyPropertiesModelTest1>(arg);

            Assert.AreEqual(copy.Name, source.Name);
            Assert.AreEqual(copy.LastName, source.LastName);
            Assert.AreEqual(copy.Age, source.Age);
        }

        [TestMethod]
        public void CopyPropertiesTest3()
        {
            CopyPropertiesModelTest1 source = new CopyPropertiesModelTest1 { Name = "Behnam", LastName = "Bing", Age = 8 };
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("Name", "Username");
            map.Add("LastName", "Family");
            map.Add("Age", "Age");

            var arg = new CopyObjectArguments<CopyPropertiesModelTest2>()
            {
                Source = source,
                CaseSensitive = StringComparison.Ordinal,
                Map = map,
            };

            CopyPropertiesModelTest2 copy = ReflectionHelper.CopyProperties<CopyPropertiesModelTest2>(arg);

            Assert.AreEqual(copy.Username, source.Name);
            Assert.AreEqual(copy.Family, source.LastName);
            Assert.AreEqual(copy.Age, source.Age);
        }

        [TestMethod]
        public void CopyPropertiesTest4()
        {
            CopyPropertiesModelTest1 source = new CopyPropertiesModelTest1 { Name = "Jack", LastName = "Bing", Age = -100 };

            var arg = new CopyObjectArguments<CopyPropertiesModelTest3>()
            {
                Source = source,
                CaseSensitive = StringComparison.OrdinalIgnoreCase,
            };

            CopyPropertiesModelTest3 copy = ReflectionHelper.CopyProperties<CopyPropertiesModelTest3>(arg);

            Assert.AreEqual(copy.name, source.Name);
            Assert.AreEqual(copy.lastName, source.LastName);
            Assert.AreEqual(copy.age, source.Age);
        }

        [TestMethod]
        public void CopyPropertiesTest5()
        {
            CopyPropertiesModelTest1 source = new CopyPropertiesModelTest1 { Name = "Jack", LastName = "Bing", Age = -100 };
            CopyPropertiesModelTest1 target = new CopyPropertiesModelTest1() { Name = "Behnam" };
            var arg = new CopyObjectArguments<CopyPropertiesModelTest1>()
            {
                Source = source,
                CaseSensitive = StringComparison.OrdinalIgnoreCase,
                TResult = target,
                IgnoreVariablesAreNotNull = true,
            };

            CopyPropertiesModelTest1 copy = ReflectionHelper.CopyProperties<CopyPropertiesModelTest1>(arg);

            Assert.AreNotEqual(copy.Name, source.Name);
            Assert.AreEqual(copy.LastName, source.LastName);
            Assert.AreNotEqual(copy.Age, source.Age);
        }

        [TestMethod]
        public void CopyPropertiesTest6()
        {
            CopyPropertiesModelTest1 source = new CopyPropertiesModelTest1 { Name = "Jack", LastName = "Bing", Age = -100 };
            CopyPropertiesModelTest3 target = new CopyPropertiesModelTest3();
            var arg = new CopyObjectArguments<CopyPropertiesModelTest3>()
            {
                Source = source,
                CaseSensitive = StringComparison.OrdinalIgnoreCase,
                TResult = target,
                IgnoreVariablesAreNotNull = false,
            };

            CopyPropertiesModelTest3 copy = ReflectionHelper.CopyProperties<CopyPropertiesModelTest3>(arg);

            Assert.AreEqual(copy.name, source.Name);
            Assert.AreEqual(copy.lastName, source.LastName);
            Assert.AreEqual(copy.age, source.Age);
        }

        [TestMethod]
        public void CopyPropertiesTest7()
        {
            CopyPropertiesModelTest1 source = new CopyPropertiesModelTest1 { Name = "Jack", LastName = "Bing", Age = -100 };
            CopyPropertiesModelTest1 result = new CopyPropertiesModelTest1();
            IList<string> MapIgnore = new List<string>
            {
                "Name",
                "Age"
            };
            var arg = new CopyObjectArguments<CopyPropertiesModelTest1>()
            {
                Source = source,
                CaseSensitive = StringComparison.OrdinalIgnoreCase,
                TResult = result,
                Ignore = MapIgnore,
            };

            CopyPropertiesModelTest1 copy = ReflectionHelper.CopyProperties<CopyPropertiesModelTest1>(arg);

            Assert.AreNotEqual(copy.Name, source.Name);
            Assert.AreEqual(copy.LastName, source.LastName);
            Assert.AreNotEqual(copy.Age, source.Age);
        }

    }

    public class CopyPropertiesModelTest1
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }

    public class CopyPropertiesModelTest2
    {
        public string Username { get; set; }
        public string Family { get; set; }
        public int Age { get; set; }
    }

    public class CopyPropertiesModelTest3
    {
        public string name { get; set; }
        public string lastName { get; set; }
        public int age { get; set; }
    }

}
