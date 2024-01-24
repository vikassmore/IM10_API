﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class EmailSettings
    {
        public String PrimaryDomain { get; set; }

        public int PrimaryPort { get; set; }

        public String SecondayDomain { get; set; }

        public int SecondaryPort { get; set; }

        public String UsernameEmail { get; set; }

        public String UsernamePassword { get; set; }

        public String FromEmail { get; set; }

        public String ToEmail { get; set; }

        public String CcEmail { get; set; }

        // mailgun properties
        public string ApiKey { get; set; }
        public string BaseUri { get; set; }
        public string Domain { get; set; }
        public string RequestUri { get; set; }
        public string From { get; set; }

        public bool EnableSsl { get; set; }
        public bool UseDefaultCredentials { get; set; }

    }


    public class TwilioSettings
    {
        public string AccountID { get; set; }
        public string AuthToken { get; set; }
        public string PhoneNumber { get; set; }
        public string WhatsAppNumber { get; set; }
        public int OtpExpiryMinutes { get; set; }
    
    }
}
