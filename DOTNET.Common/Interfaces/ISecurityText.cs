using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOTNET.Common.Interfaces
{
    public interface ISecurityText
    {
        public string EncryptText(string textToEncrypt);
        public string DecryptText(string textToDecrypt);
    }
}
