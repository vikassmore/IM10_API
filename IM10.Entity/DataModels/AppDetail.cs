using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class AppDetail
{
    public int AppDetailsId { get; set; }

    public Guid Appkey { get; set; }

    public bool IsActive { get; set; }

    public string AppName { get; set; } = null!;
}
