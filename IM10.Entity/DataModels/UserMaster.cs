using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class UserMaster
{
    public long UserId { get; set; }

    public string? Username { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string EmailId { get; set; } = null!;

    public string MobileNo { get; set; } = null!;

    public DateTime Dob { get; set; }

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    public int? CityId { get; set; }

    public int? AppId { get; set; }

    public string? DeviceToken { get; set; }

    public string? CountryCode { get; set; }

    public bool? IsLogin { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

    public virtual ICollection<ContentFlag> ContentFlags { get; } = new List<ContentFlag>();

    public virtual ICollection<Otpautherization> Otpautherizations { get; } = new List<Otpautherization>();

    public virtual ICollection<UserAuditLog> UserAuditLogs { get; } = new List<UserAuditLog>();

    public virtual ICollection<UserPlayerMapping> UserPlayerMappings { get; } = new List<UserPlayerMapping>();
}
