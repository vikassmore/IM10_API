using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class MenuPage
{
    public int MenuPageId { get; set; }

    public string MenuPageName { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<RoleMenuPageMapping> RoleMenuPageMappings { get; } = new List<RoleMenuPageMapping>();
}
