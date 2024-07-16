using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class AuthModel
    {
        public long UserId { get; set; }

       // public string Username { get; set; } = null!;
        public string EmailId { get; set; } = null!;
        public int RoleId { get; set; }
        public string Role { get; set; } =null!;
        public string FullName { get;set; } = null!;
        public string? DeviceToken { get; set; }
        public string MobileNo { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? CountryCode { get; set; }
        public int? CityId { get; set; }
        public int? StateId { get; set; }

    }

    public class LoginStaus
    {
        public long UserId { get; set; }
        public bool? IsLogin { get; set; }
        public bool AccountDeletedStatus { get; set; }

    }


    public class ResendOtp
    {
        public long UserId { get; set; }
        public string MobileNo { get; set; } = null!;
        public string Otp { get; set; }

    }



    public class UserProfileModel
    {
        public long UserId { get; set; }
        public int RoleId { get; set; }
        public string FullName { get; set; } = null!;
        public string MobileNo { get; set; } = null!;
        public string? CountryCode { get; set; }
        public string CountryName { get; set; }
        public int? StateId { get; set; }
        public string StateName { get; set; }
        public int? CityId { get; set; }
        public string CityName { get; set; }
       
    }

    public class DeleteAccountModel
    {
        public bool? IsLogin { get; set; }
        public long UserId { get; set; }
        public string? DeviceToken { get; set; }
    }



    public class AuthModelForMobile
    {
        public long UserId { get; set; }

        // public string Username { get; set; } = null!;
        public string EmailId { get; set; } = null!;
        public int RoleId { get; set; }
        public string Role { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? DeviceToken { get; set; }
        public string MobileNo { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? CountryCode { get; set; }
        public int? CityId { get; set; }
        public int? StateId { get; set; }

    }
}
