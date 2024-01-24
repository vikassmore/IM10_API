using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class AdvContentMapping
{
    public long AdvContentMapId { get; set; }

    public long ContentId { get; set; }

    public long AdvertiseContentId { get; set; }

    public int CategoryId { get; set; }

    public int SubCategoryId { get; set; }

    public int? Position { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual AdvContentDetail AdvertiseContent { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ContentDetail Content { get; set; } = null!;

    public virtual SubCategory SubCategory { get; set; } = null!;
}
