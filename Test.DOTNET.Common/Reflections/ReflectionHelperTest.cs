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
            CopyPropertiesModelTest1 source = new CopyPropertiesModelTest1 { Name = "Behnam", LastName ="2064" , Age= 5 };

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
    }


    public class CopyPropertiesModelTest1
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

    }
}
