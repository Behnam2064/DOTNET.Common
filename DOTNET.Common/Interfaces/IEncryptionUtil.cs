using DOTNET.Common.Encryptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOTNET.Common.Interfaces
{
    public interface IEncryptionUtil
    {
        public EncryptionUtilSettings? Settings { get; set; }
        public string EncryptText(string textToEncrypt);
        public string DecryptText(string textToDecrypt);
    }
}
