using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Customware
{
    public class AesFileEncryption
    {
        private const int bufferSize = 1048576; // 1 MB

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        /// <param name="password"></param>
        public void EncryptFile(string inputFile, string outputFile, string password)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                    byte[] salt = new byte[32];

                    using (var rng = new RNGCryptoServiceProvider())
                    {
                        rng.GetBytes(salt);
                    }

                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(passwordBytes, salt, 1000);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);

                    using (FileStream fsInput = new FileStream(inputFile, FileMode.Open))
                    using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create))
                    {
                        fsOutput.Write(salt, 0, salt.Length);

                        using (CryptoStream cs = new CryptoStream(fsOutput, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            byte[] buffer = new byte[bufferSize];
                            int read;

                            while ((read = fsInput.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                cs.Write(buffer, 0, read);
                            }

                            cs.FlushFinalBlock();
                        }
                    }
                }
            }
            catch
            {

            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        /// <param name="password"></param>
        public void DecryptFile(string inputFile, string outputFile, string password)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                    byte[] salt = new byte[32];

                    using (FileStream fsInput = new FileStream(inputFile, FileMode.Open))
                    using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create))
                    {
                        fsInput.Read(salt, 0, salt.Length);

                        Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(passwordBytes, salt, 1000);
                        aes.Key = key.GetBytes(aes.KeySize / 8);
                        aes.IV = key.GetBytes(aes.BlockSize / 8);

                        using (CryptoStream cs = new CryptoStream(fsOutput, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            byte[] buffer = new byte[bufferSize];
                            int read;

                            while ((read = fsInput.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                cs.Write(buffer, 0, read);
                            }

                            cs.FlushFinalBlock();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
            }

        }
    }
}
