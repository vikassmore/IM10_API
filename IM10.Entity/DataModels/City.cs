using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class City
{
    public int CityId { get; set; }

    public string Name { get; set; } = null!;

    public int StateId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<AdvContentDetail> AdvContentDetails { get; } = new List<AdvContentDetail>();

    public virtual ICollection<ListingDetail> ListingDetails { get; } = new List<ListingDetail>();

    public virtual State State { get; set; } = null!;
}
