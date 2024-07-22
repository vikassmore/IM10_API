using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class ContentDetail
{
    public long ContentId { get; set; }

    public string? ContentFileName { get; set; }

    public string? ContentFilePath { get; set; }

    public string? ContentFileName1 { get; set; }

    public string? ContentFilePath1 { get; set; }

    public string? Thumbnail1 { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Thumbnail { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int CategoryId { get; set; }

    public int SubCategoryId { get; set; }

    public long PlayerId { get; set; }

    public int ContentTypeId { get; set; }

    public int? LanguageId { get; set; }

    public string? Comment { get; set; }

    public bool? Approved { get; set; }

    public bool? ProductionFlag { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<AdvContentMapping> AdvContentMappings { get; } = new List<AdvContentMapping>();

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

    public virtual ICollection<ContentAuditLog> ContentAuditLogs { get; } = new List<ContentAuditLog>();

    public virtual ICollection<ContentFlag> ContentFlags { get; } = new List<ContentFlag>();

    public virtual ContentType ContentType { get; set; } = null!;

    public virtual ICollection<ContentView> ContentViews { get; } = new List<ContentView>();

    public virtual Language? Language { get; set; }

    public virtual ICollection<MarketingCampaign> MarketingCampaigns { get; } = new List<MarketingCampaign>();

    public virtual PlayerDetail Player { get; set; } = null!;

    public virtual SubCategory SubCategory { get; set; } = null!;
}
