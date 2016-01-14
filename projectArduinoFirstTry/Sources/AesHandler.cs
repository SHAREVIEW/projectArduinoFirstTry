using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace projectArduinoFirstTry.Sources
{
    public static class AesHandler
    {
        private static byte[] _aesKey = new byte[] {
            0x80, 0x59, 0x43, 0xFA, 0x9D, 0x2B, 0x3F, 0x01, 0x00, 0x45, 0x89, 0x7A, 0x0A, 0x3C, 0xE2, 0x54
        };

        private static byte[] _iv = new byte[] {
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F
        };

        public static string DecryptStringFromBytes(byte[] cipherText)
        {
            // Check arguments. 
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (_aesKey == null || _aesKey.Length <= 0)
                throw new ArgumentNullException("Key");
            if (_iv == null || _iv.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold 
            // the decrypted text. 
            string plaintext = null;

            // Create an RijndaelManaged object 
            // with the specified key and IV. 
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = _aesKey;
                rijAlg.IV = _iv;
                rijAlg.Padding = PaddingMode.Zeros;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption. 
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream 
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }
            return plaintext;
        }

        public static byte[] AesKey
        {
            set { _aesKey = value;}
        }

        public static byte[] IV
        {
            set { _iv = value; }
        }

    }
}
