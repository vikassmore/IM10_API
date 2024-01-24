using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class MarketingCampaign
{
    public long MarketingCampaignId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public long PlayerId { get; set; }

    public long ContentId { get; set; }

    public int ContentTypeId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<CampaignDetail> CampaignDetails { get; } = new List<CampaignDetail>();

    public virtual ContentDetail Content { get; set; } = null!;

    public virtual ContentType ContentType { get; set; } = null!;

    public virtual PlayerDetail Player { get; set; } = null!;
}
