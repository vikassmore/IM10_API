using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "EmailId is required")]
        public string EmailId { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
   
    public class MobileLoginModel
    {
        public string MobileNo { get; set; }
        public string? DeviceToken { get; set; }
        public string? CountryCode { get; set; }
        public long PlayerId { get; set; }
        public int? CityId { get; set; }
        public int? StateId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

    }
}
