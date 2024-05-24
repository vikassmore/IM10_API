using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class ListingDetail
{
    public long ListingId { get; set; }

    public long PlayerId { get; set; }

    public string? CompanyName { get; set; }

    public string? Description { get; set; }

    public string? ContactPersonName { get; set; }

    public string? ContactPersonEmailId { get; set; }

    public string? ContactPersonMobile { get; set; }

    public string? CompanyEmailId { get; set; }

    public string? CompanyMobile { get; set; }

    public string? CompanyPhone { get; set; }

    public string? CompanyWebSite { get; set; }

    public string? CompanyLogoFileName { get; set; }

    public string? CompanyLogoFilePath { get; set; }

    public int? NationId { get; set; }

    public int? StateId { get; set; }

    public int? CityId { get; set; }

    public bool? IsGlobal { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? FinalPrice { get; set; }

    public int? Position { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual City? City { get; set; }

    public virtual ICollection<EndorsmentDetail> EndorsmentDetails { get; } = new List<EndorsmentDetail>();

    public virtual Country? Nation { get; set; }

    public virtual PlayerDetail Player { get; set; } = null!;

    public virtual State? State { get; set; }
}
