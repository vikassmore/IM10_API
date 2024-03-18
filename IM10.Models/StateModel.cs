using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class StateModel
    {
        public int StateId { get; set; }

        public string Name { get; set; } = null!;

        public int CountryId { get; set; }

        public string StateCode { get; set; } = null!;
    }
}
