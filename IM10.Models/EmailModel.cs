using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class EmailModel
    {
        public string ToAddress { get; set; }
        public string Body { get; set; }
        public bool isHtml { get; set; }
        public string Subject { get; set; }

    }
}
