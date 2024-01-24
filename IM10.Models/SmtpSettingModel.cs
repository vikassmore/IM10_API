using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class SmtpSettingModel
    {
        public string from { get; set; }
        public string host { get; set; }
        public int port { get; set; }
        public bool defaultCredentials { get; set; }
        public bool enableSsl { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }
}
