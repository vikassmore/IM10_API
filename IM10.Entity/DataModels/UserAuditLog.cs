using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class UserAuditLog
{
    public long AuditId { get; set; }

    public string Action { get; set; } = null!;

    public string Description { get; set; } = null!;

    public long UserId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual UserMaster User { get; set; } = null!;
}
