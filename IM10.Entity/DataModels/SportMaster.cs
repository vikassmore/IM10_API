using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class SportMaster
{
    public long SportId { get; set; }

    public string? SportName { get; set; }

    public virtual ICollection<Category> Categories { get; } = new List<Category>();

    public virtual ICollection<PlayerDetail> PlayerDetails { get; } = new List<PlayerDetail>();
}
