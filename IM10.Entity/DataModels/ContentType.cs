using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class ContentType
{
    public int ContentTypeId { get; set; }

    public string? ContentName { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<AdvContentDetail> AdvContentDetails { get; } = new List<AdvContentDetail>();

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

    public virtual ICollection<ContentDetail> ContentDetails { get; } = new List<ContentDetail>();

    public virtual ICollection<ContentFlag> ContentFlags { get; } = new List<ContentFlag>();

    public virtual ICollection<ContentView> ContentViews { get; } = new List<ContentView>();

    public virtual ICollection<MarketingCampaign> MarketingCampaigns { get; } = new List<MarketingCampaign>();
}
