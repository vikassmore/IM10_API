using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class RoleMenuPageMapping
{
    public int RoleMenuPageId { get; set; }

    public int RoleId { get; set; }

    public int MenuPageId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual MenuPage MenuPage { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
