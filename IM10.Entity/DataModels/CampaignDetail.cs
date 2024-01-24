using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class CampaignDetail
{
    public long CampaignId { get; set; }

    public string? SocialMediaViews { get; set; }

    public string? ScreenShotFileName { get; set; }

    public string? ScreenShotFilePath { get; set; }

    public long MarketingCampaignId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual MarketingCampaign MarketingCampaign { get; set; } = null!;
}
