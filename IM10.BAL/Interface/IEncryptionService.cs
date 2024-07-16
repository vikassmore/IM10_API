using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Interface
{
    public interface IEncryptionService
    {
        string GetEncryptedId(string Id);
        string GetDecryotedId(string Id);
        public (long? DecryptedPlayerId, HttpStatusCode StatusCode, string Message) DecryptPlayerId(string Id);

    }
}
