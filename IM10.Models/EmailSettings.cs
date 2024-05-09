using System;
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


        public String UsernameEmail { get; set; }

        public String UsernamePassword { get; set; }


        public String ToEmail { get; set; }

        public String CcEmail { get; set; }

        public bool EnableSsl { get; set; }
        public bool UseDefaultCredentials { get; set; }

    }
}
