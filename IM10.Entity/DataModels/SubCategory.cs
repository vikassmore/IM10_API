using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class SubCategory
{
    public int SubCategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int CategoryId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<AdvContentMapping> AdvContentMappings { get; } = new List<AdvContentMapping>();

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<ContentDetail> ContentDetails { get; } = new List<ContentDetail>();

    public virtual ICollection<ListingDetail> ListingDetails { get; } = new List<ListingDetail>();
}
