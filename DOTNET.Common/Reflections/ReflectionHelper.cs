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

        public static object? GetValue(object obj, string ProperrtyName)
        {
            return obj.GetType().GetProperties().Where(x => x.Name.Equals(ProperrtyName)).FirstOrDefault()?.GetValue(obj);
        }

        public static bool IsContains(object? obj, string ProperrtyName, StringComparison stringComparison)
        {
            try
            {
                PropertyInfo[] properties = obj.GetType().GetType().GetProperties();
                return properties.Any(x => x.Name.Equals(ProperrtyName, StringComparison.Ordinal));
            }
            catch
            {
                return false;
            }
        }

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

        public static TResultClass CopyProperties<TResultClass>(CopyObjectArguments<TResultClass> arg) where TResultClass : class
        {
            #region All the properties 

            //All the properties of the source class
            PropertyInfo[] propSource = arg.Source.GetType().GetProperties();
            //All the properties of the result class
            PropertyInfo[] propResult = typeof(TResultClass).GetProperties();

            if (arg.Ignore != null)
            {
                //It was written in this section for more speed
                #region Ignore selected items (From source - Key)
                //Items that should not be ignored
                List<PropertyInfo> _temppropSource = new List<PropertyInfo>();
                foreach (var item in propSource)
                {
                    IEnumerable<string> q = arg.Ignore.Where(x => x.Equals(item.Name));
                    if (!q.Any())
                        _temppropSource.Add(item);
                }
                propSource = _temppropSource.ToArray();
                #endregion

                #region Ignore selected items (From result - value)

                //Items that should not be ignored
                List<PropertyInfo> _temppropResult = new List<PropertyInfo>();
                foreach (var item in propResult)
                {
                    IEnumerable<string> q = arg.Ignore.Where(x => x.Equals(item.Name));
                    if (!q.Any())
                        _temppropResult.Add(item);
                }
                propResult = _temppropResult.ToArray();
                #endregion
            }

            #endregion

            #region Instance of result class

            TResultClass? t = null;
            //You can copy the values to a class that has already been created, or create the class and then copy the values.
            if (arg.TResult == null)
                t = Activator.CreateInstance(typeof(TResultClass)) as TResultClass;
            else
                t = arg.TResult;


            #endregion

            #region Create map

            //string key => propSource[i].Name
            //string Value => propResult[i].Name
            Dictionary<string, string>? Map = new Dictionary<string, string>();

            if (arg.Map != null)
            {
                Map = arg.Map;
            }
            else
            {
                foreach (PropertyInfo piSource in propSource)
                {
                    IEnumerable<PropertyInfo> queryResult = propResult.Where(x => x.Name.Equals(piSource.Name, arg.CaseSensitive));
                    if (queryResult.Any())
                    {
                        PropertyInfo? ptarget = queryResult.FirstOrDefault();
                        if (ptarget != null)
                            Map.Add(piSource.Name, ptarget.Name);
                    }
                }
            }


            #endregion

            #region Copy Data

            foreach (KeyValuePair<string, string> item in Map)
            {
                //Find a variable with the same name in the source class
                PropertyInfo? PSource = propSource.FirstOrDefault(x => x.Name.Equals(item.Key));
                //Find a variable with the same name in the result class
                PropertyInfo? PResult = propResult.FirstOrDefault(x => x.Name.Equals(item.Value));

                //Whether the result variable has a value or not
                object? PResultValue = PResult.GetValue(t);
                //If the user does not want the variables that have values to change (In TResultClass object), ignore them
                if (arg.TResult != null && arg.IgnoreVariablesAreNotNull && PResultValue != null)
                {
                    continue;
                }
                else
                {
                    //copy data
                    object? copy = PSource.GetValue(arg.Source);
                    PResult.SetValue(t, copy);
                }

            }

            #endregion

            return t;
        }
    }

    public class CopyObjectArguments<TResultClass> where TResultClass : class
    {
        public required object Source { get; set; }

        /// <summary>
        /// If the names of the properties of the origin and destination classes are different from each other,
        /// it can be specified in this section
        /// The key is equal to the source property name and the value is equal to the destination property name of the destination class
        /// Map.Key     => Source
        /// Map.Value   => Result
        /// </summary>
        public Dictionary<string, string>? Map { get; set; }

        public IList<string>? Ignore { get; set; }

        public StringComparison CaseSensitive { get; set; }

        /// <summary>
        /// Ignore variables(in TResultClass object) that are not null (It depends on the TResult variable)
        /// This function is available if the TResult variable has a value
        /// </summary>
        public bool IgnoreVariablesAreNotNull { get; set; }

        /// <summary>
        /// This variable can be null
        /// </summary>
        public TResultClass? TResult { get; set; }

        public CopyObjectArguments()
        {

        }

        public CopyObjectArguments(object source)
        {
            this.Source = source;
        }

    }
}
