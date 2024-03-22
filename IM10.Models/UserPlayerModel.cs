using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class UserPlayerModel
    {
        public long UserPlayerId { get; set; }
        public long UserId { get; set; }
        //  public string Username { get; set; } = null!;
        public string? UserFirstName { get; set; }
        public string? UserLastName { get; set; }
        public string? UserFullName { get; set; }
        public long? RoleId { get; set; }
        public string RoleName { get; set; }
        public long PlayerId { get; set; }
        public string? FullName { get; set; }
        public string EmailId { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public long? SportId { get; set; }
        public string? SportName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string? ProfileImageFileName { get; set; }
        public string? ProfileImageFilePath { get; set; }
    }

    public class UserPlayerModel1
    {
        public string UserPlayerIds { get; set; }
        public string UncheckedPlayerIds { get; set; }
        public long UserPlayerId { get; set; }
        public long UserId { get; set; }
        public long PlayerId { get; set; }   
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }



    public class UserPlayerMappingModel
    {
        public string UncheckedPlayerIds { get; set; }
        public long UserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }




    public class UserPlayerModel2
    {    
        public long UserId { get; set; }
        public List<long> lstPlayers { get; set; }
        public List<string> lstPlayer { get; set; }
    }
    public class UserPlayerModel3
    {
        public long UserPlayerId { get; set; }
        public long UserId { get; set; }
        public long PlayerId { get; set; }
        public string PlayerIds { get; set; }
        public string? FullName { get; set; }
        public string EmailId { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string? ProfileImageFileName { get; set; }
        public string? ProfileImageFilePath { get; set; }

    }


    public class SportUserPlayerModel
    {
        public long UserPlayerId { get; set; }
        public long UserId { get; set; }
        public long PlayerId { get; set; }
        public string? FullName { get; set; }
        public string EmailId { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public long? SportId { get; set; }
        public string? SportName { get; set; }
        public string? ProfileImageFileName { get; set; }
        public string? ProfileImageFilePath { get; set; }
    }

}
