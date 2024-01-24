using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class CampaignDetailModel
    {
        public long MarketingCampaignId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public long PlayerId { get; set; }
        public string? FullName { get; set; }

        public long ContentId { get; set; }
        public string? ContentTitle { get; set; }
        public int ContentTypeId { get; set; }
        public string? ContentTypeName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
