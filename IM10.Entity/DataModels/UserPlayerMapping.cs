﻿using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class UserPlayerMapping
{
    public long UserPlayerId { get; set; }

    public long UserId { get; set; }

    public long PlayerId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual PlayerDetail Player { get; set; } = null!;

    public virtual UserMaster User { get; set; } = null!;
}
