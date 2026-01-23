using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.DataProtection;

namespace MDUA.DataAccess.Security
{
    public class AesSecretProtector : ISecretProtector
    {
        private readonly IDataProtector _protector;

        public AesSecretProtector(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("MDUA.Courier.Credentials.v1");
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
                return null;

            return _protector.Protect(plainText);
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrWhiteSpace(cipherText))
                return null;

            return _protector.Unprotect(cipherText);
        }
    }
}
