using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class EndorsmentTypeModel
    {
        public int EndorsmentTypeId { get; set; }
        public string? EndorsmentName { get; set; }
        public string? EndorsmentDescription { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
