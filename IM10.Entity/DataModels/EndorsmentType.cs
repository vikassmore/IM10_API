using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class EndorsmentType
{
    public int EndorsmentTypeId { get; set; }

    public string? EndorsmentName { get; set; }

    public string? EndorsmentDescription { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }
}
