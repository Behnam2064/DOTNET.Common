using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOTNET.Common.Extensions.Streams
{
    public static class StreamExtensions
    {
        public static void Save(this Stream stream, string file)
        {
            using (var fileStream = File.Create(file))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
            }
        }
    }
}
