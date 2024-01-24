using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class EndorsmentDetail
{
    public int EndorsmentId { get; set; }

    public long PlayerId { get; set; }

    public long ListingId { get; set; }

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

    public virtual ListingDetail Listing { get; set; } = null!;

    public virtual PlayerDetail Player { get; set; } = null!;
}
