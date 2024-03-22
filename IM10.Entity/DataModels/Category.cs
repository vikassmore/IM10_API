using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class Category
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? DisplayOrder { get; set; }

    public long? SportId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<AdvContentMapping> AdvContentMappings { get; } = new List<AdvContentMapping>();

    public virtual ICollection<ContentDetail> ContentDetails { get; } = new List<ContentDetail>();

    public virtual ICollection<ListingDetail> ListingDetails { get; } = new List<ListingDetail>();

    public virtual SportMaster? Sport { get; set; }

    public virtual ICollection<SubCategory> SubCategories { get; } = new List<SubCategory>();
}
