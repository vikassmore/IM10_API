using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class Country
{
    public int CountryId { get; set; }

    public string Name { get; set; } = null!;

    public string CountryCode { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<AdvContentDetail> AdvContentDetails { get; } = new List<AdvContentDetail>();

    public virtual ICollection<ListingDetail> ListingDetails { get; } = new List<ListingDetail>();

    public virtual ICollection<State> States { get; } = new List<State>();
}
