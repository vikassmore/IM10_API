using IM10.BAL.Interface;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Implementaion
{
    public class EncryptionService:IEncryptionService
    {
        private string key = "IM10$#^@";
        private string iv = "IM10$#^@";


        /// <summary>
        /// Method is used to get the GetEncryptedId
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public string GetEncryptedId(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                throw new ArgumentException("Player ID cannot be null or empty", nameof(Id));
            }
            string encryptedPlayerId = EncryptionHelper.EncryptData(Id, key, iv);
            return encryptedPlayerId;
        }


        /// <summary>
        /// Method is used to get the GetDecryotedId
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public string GetDecryotedId(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                throw new ArgumentException("Player ID cannot be null or empty", nameof(Id));
            }
            string decryptedId = EncryptionHelper.DecryptData(Id, key, iv);
            return decryptedId;
        }


        public (long? DecryptedPlayerId, HttpStatusCode StatusCode, string Message) DecryptPlayerId(string Id)
        {

            long decryptedPlayerId;
            try
            {
                if (IsValidBase64String(Id))
                {
                    string decryptedPlayerIdString = GetDecryotedId(Id);
                    if (!long.TryParse(decryptedPlayerIdString, out decryptedPlayerId))
                    {
                        return (null, HttpStatusCode.BadRequest, "Invalid Player ID.");
                    }
                    return (decryptedPlayerId, HttpStatusCode.OK, null);
                }
                else
                {
                    return (null, HttpStatusCode.BadRequest, "Player ID is not a valid Base-64 string.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during decryption: {ex.Message}");
                return (null, HttpStatusCode.InternalServerError, "Error decrypting Player ID.");
            }
        }

        public static bool IsValidBase64String(string base64String)
        {
            if (string.IsNullOrEmpty(base64String) || base64String.Length % 4 != 0)
            {
                return false;
            }
            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
