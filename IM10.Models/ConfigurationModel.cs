using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class ConfigurationModel
    {
        public string HostName { get; set; }
        public string URLffmpeg { get; set; }

    }

    public class AppSettings
    {
        public string BunnyHostName { get; set; }
        public bool ProductionFlag { get; set; }
    }

}
