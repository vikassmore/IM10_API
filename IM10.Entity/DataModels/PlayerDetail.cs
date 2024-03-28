using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class PlayerDetail
{
    public long PlayerId { get; set; }

    public string? AadharCardFileName { get; set; }

    public string? AadharCardFilePath { get; set; }

    public string? PanCardFileName { get; set; }

    public string? PanCardFilePath { get; set; }

    public string? VotingCardFileName { get; set; }

    public string? VotingCardFilePath { get; set; }

    public string? DrivingLicenceFileName { get; set; }

    public string? DrivingLicenceFilePath { get; set; }

    public string? BankAcountNo { get; set; }

    public string? PancardNo { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Address { get; set; }

    public DateTime? Dob { get; set; }

    public string? ProfileImageFileName { get; set; }

    public string? ProfileImageFilePath { get; set; }

    public long SportId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<AdvContentDetail> AdvContentDetails { get; } = new List<AdvContentDetail>();

    public virtual ICollection<ContentDetail> ContentDetails { get; } = new List<ContentDetail>();

    public virtual ICollection<ContentFlag> ContentFlags { get; } = new List<ContentFlag>();

    public virtual ICollection<ContentView> ContentViews { get; } = new List<ContentView>();

    public virtual ICollection<EndorsmentDetail> EndorsmentDetails { get; } = new List<EndorsmentDetail>();

    public virtual ICollection<Fcmnotification> Fcmnotifications { get; } = new List<Fcmnotification>();

    public virtual ICollection<ListingDetail> ListingDetails { get; } = new List<ListingDetail>();

    public virtual ICollection<MarketingCampaign> MarketingCampaigns { get; } = new List<MarketingCampaign>();

    public virtual ICollection<PlayerData> PlayerData { get; } = new List<PlayerData>();

    public virtual SportMaster Sport { get; set; } = null!;

    public virtual ICollection<UserPlayerMapping> UserPlayerMappings { get; } = new List<UserPlayerMapping>();
}
