using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Implementaion
{
    public class EncryptionHelper
    {

        /// <summary>
        /// EncryptData
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string EncryptData(string plainText, string key, string iv)
        {
            try
            {
                using (DES desAlg = DES.Create())
                {
                    desAlg.Key = Encoding.UTF8.GetBytes(key);
                    desAlg.IV = Encoding.UTF8.GetBytes(iv);

                    ICryptoTransform encryptor = desAlg.CreateEncryptor(desAlg.Key, desAlg.IV);

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(plainText);
                            }
                        }

                        byte[] encryptedBytes = msEncrypt.ToArray();
                        return Convert.ToBase64String(encryptedBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during encryption: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// DecryptData
        /// </summary>
        /// <param name="base64EncodedCipherText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string DecryptData(string base64EncodedCipherText, string key, string iv)
        {
            try
            {
                byte[] cipherTextBytes = Convert.FromBase64String(base64EncodedCipherText);

                using (DES desAlg = DES.Create())
                {
                    desAlg.Key = Encoding.UTF8.GetBytes(key);
                    desAlg.IV = Encoding.UTF8.GetBytes(iv);

                    ICryptoTransform decryptor = desAlg.CreateDecryptor(desAlg.Key, desAlg.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(cipherTextBytes))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during decryption: " + ex.Message);
                return null;
            }
        }
    }
}
