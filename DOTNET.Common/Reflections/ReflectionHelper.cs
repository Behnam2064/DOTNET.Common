using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DOTNET.Common.Reflections
{
    public class ReflectionHelper
    {

        /// <summary>
        /// https://stackoverflow.com/questions/10261824/how-can-i-get-all-constants-of-a-type-by-reflection
        /// How can I get all constants of a type by reflection?
        /// 
        /// https://stackoverflow.com/questions/33477163/get-value-of-constant-by-name
        /// To get field values or call members on static types using reflection you pass null as the instance reference.
        ///     typeof(Test).GetField("Value").GetValue(null).Dump();
        /// </summary>
        /// <param name="type">The type in which it is to be searched</param>
        /// <returns>Constants found</returns>
        public static List<FieldInfo> GetConstants(System.Type type)
        => type.GetFields(BindingFlags.Public | BindingFlags.Static |
                  BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
        
    }
}
