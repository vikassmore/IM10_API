using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class CampaignSocialMediaDetailModel
    {
        public long CampaignId { get; set; }

        public string? SocialMediaViews { get; set; }

        public string? ScreenShotFileName { get; set; }

        public string? ScreenShotFilePath { get; set; }

        public long MarketingCampaignId { get; set; }
        public string? CampaignTitle { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }
    }
}
