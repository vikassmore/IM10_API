using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class UserModel
    {
        public long UserId { get; set; }
       
       //public string? Username { get; set; } 

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;
        public string? FullName { get; set; } = null!;

        public string EmailId { get; set; } = null!;

        public string MobileNo { get; set; } = null!;

        public DateTime Dob { get; set; }

        public string Password { get; set; } = null!;

        public int RoleId { get; set; }
        public string Name { get; set; } = null!;


        public int? CityId { get; set; }

        public int? AppId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

    }

    public class UserModel1
    {
        public long UserId { get; set; }

      //  public string? Username { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;
        public string FullName { get; set; } = null!;

        public string EmailId { get; set; } = null!;

        public string MobileNo { get; set; } = null!;

        public DateTime Dob { get; set; }

      //  public string Password { get; set; } = null!;

        public int RoleId { get; set; }
        public string Name { get; set; } = null!;
        public int? CityId { get; set; }

        public int? AppId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

    }

   

    public class changepasswordModel
    {
        public long UserId { get; set; }
        public string Password { get; set; } = null!;

    }


    public class MobileUserRegisterModel
    {
        public long UserId { get; set; }
         public string? Username { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string EmailId { get; set; } = null!;
        public string MobileNo { get; set; } = null!;
        public DateTime Dob { get; set; }
         public string Password { get; set; } = null!;
        public int RoleId { get; set; }
        public int? CityId { get; set; }
        public int? AppId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

    }



    public class ActiveUserModel
    {
        public long UserId { get; set; }
        public string Otp { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
