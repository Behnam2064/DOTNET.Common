using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DOTNET.Common.Extensions.Assemblies
{
    public static class AssemblyExtensions
    {

        /// <summary>
        /// https://stackoverflow.com/questions/3314140/how-to-read-embedded-resource-text-file
        /// Parameter Build Action with value Embedded Resource
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task<string> GetResourceStringAsync(this Assembly assembly, string name)
        {
            using StreamReader reader = new(GetResourceStreamAsync(assembly,name));
            return await reader.ReadToEndAsync();
        }

        /// <summary>
        /// https://stackoverflow.com/questions/3314140/how-to-read-embedded-resource-text-file
        /// Parameter Build Action with value Embedded Resource
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Stream? GetResourceStreamAsync(this Assembly assembly, string name)
        {
            // Determine path
            string resourcePath = name;
            // Format: "{Namespace}.{Folder}.{filename}.{Extension}"
            //if (!name.StartsWith(nameof(SignificantDrawerCompiler)))
            if (!name.StartsWith("SignificantDrawerCompiler"))
            {
                resourcePath = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith(name));
            }

            return assembly.GetManifestResourceStream(resourcePath);
        }
    }

}
