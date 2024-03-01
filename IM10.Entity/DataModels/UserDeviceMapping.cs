using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class UserDeviceMapping
{
    public long UserDeviceId { get; set; }

    public long UserId { get; set; }

    public string DeviceToken { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual UserMaster User { get; set; } = null!;
}
