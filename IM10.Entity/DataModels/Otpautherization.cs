using System;
using System.Collections.Generic;

namespace IM10.Entity.DataModels;

public partial class Otpautherization
{
    public int Otpid { get; set; }

    public long UserId { get; set; }

    public string Otp { get; set; } = null!;

    public DateTime? OtpvalidDateTime { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsActive { get; set; }

    public virtual UserMaster User { get; set; } = null!;
}
