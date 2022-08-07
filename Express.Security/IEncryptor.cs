using System;
using System.IO;

namespace Express.Security
{
    public interface IEncryptor
    {
        public string Encrypt(string value);
        public string Decrypt(string value);
        public byte[] Encrypt(byte[] inputFile);
        public byte[] Decrypt(byte[] inputFile);
    }
}
