using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class EndorsmentDetailModel
    {
        public int EndorsmentId { get; set; }
        public long PlayerId { get; set; }
        public long ListingId { get; set; }
        public string? CompanyName { get; set; }

        public string? EndorsmentType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? FinalPrice { get; set; }
        public string? Notes { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
