using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DOTNET.Common.Extensions.Strings
{
    public static class StringExtensions
    {
        /// <summary>
        /// Remove illegal characters for a file name
        /// </summary>
        /// <param name="FileName">Your file name</param>
        /// <param name="Replace">Alternate character</param>
        /// <returns>String without illegal characters</returns>
        public static string RemoveIllegalCharacters(this string FileName, char Replace)
        {

            if (string.IsNullOrEmpty(FileName) || string.IsNullOrWhiteSpace(FileName))
                return FileName;

            // length file name exp

            char[] Illegal = { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };

            if (Illegal.Any(x => x == Replace))
                throw new System.IO.IOException("Illegal file name");

            if (!Illegal.Any(x => FileName.Contains(x)))
                return FileName;

            foreach (char item in Illegal)
            {
                FileName = FileName.Replace(item, Replace);
            }

            return FileName;

        }

        public static string ReplaceArabicWithPersian(this string str)
            => str?.Replace("ي", "ی").Replace("ي", "ی").Replace("ك", "ک").Replace("ک", "ک").Replace("؟", "ی");

        public static string ReplacePersianWithArabic(this string str)
            => str?.Replace("ی", "ي").Replace("ی", "ي").Replace("ک", "ك").Replace("ک", "ک").Replace("ی", "؟");

    }
}
