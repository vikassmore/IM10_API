using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class State
{
    public int StateId { get; set; }

    public string Name { get; set; } = null!;

    public int CountryId { get; set; }

    public string StateCode { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<AdvContentDetail> AdvContentDetails { get; } = new List<AdvContentDetail>();

    public virtual ICollection<City> Cities { get; } = new List<City>();

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<ListingDetail> ListingDetails { get; } = new List<ListingDetail>();
}
