using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class AdvContentDetail
{
    public long AdvertiseContentId { get; set; }

    public string? Title { get; set; }

    public long PlayerId { get; set; }

    public int NationId { get; set; }

    public int StateId { get; set; }

    public int CityId { get; set; }

    public bool IsGlobal { get; set; }

    public string? AdvertiseFileName { get; set; }

    public string? AdvertiseFilePath { get; set; }

    public int ContentTypeId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? FinalPrice { get; set; }

    public bool? IsFree { get; set; }

    public string? Comment { get; set; }

    public bool? Approved { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<AdvContentMapping> AdvContentMappings { get; } = new List<AdvContentMapping>();

    public virtual City City { get; set; } = null!;

    public virtual ContentType ContentType { get; set; } = null!;

    public virtual Country Nation { get; set; } = null!;

    public virtual PlayerDetail Player { get; set; } = null!;

    public virtual State State { get; set; } = null!;
}
