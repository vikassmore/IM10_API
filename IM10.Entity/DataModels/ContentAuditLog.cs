using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class ContentAuditLog
{
    public long ContentLogId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public long ContentId { get; set; }

    public string? Comment { get; set; }

    public bool? Approved { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ApprovedBy { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ContentDetail Content { get; set; } = null!;
}
