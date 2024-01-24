using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class OTPAuthorizationModel
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
    }
}
