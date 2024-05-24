using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class ListingDetailModel
    {
        public long ListingId { get; set; }

        public long PlayerId { get; set; }
        public int RoleId { get; set; }

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
        public string NationName { get; set; } = null!;

        public int? StateId { get; set; }
        public string StateName { get; set; } = null!;


        public int? CityId { get; set; }
        public string CityName { get; set; } = null!;

        public bool? IsGlobal { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? FinalPrice { get; set; }       
        public int? Position { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }

    }

    public class ListingDetailModel1
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

        public int NationId { get; set; }

        public int StateId { get; set; }


        public int CityId { get; set; }

        public bool IsGlobal { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? FinalPrice { get; set; }

        public int? Position { get; set; }

        public int? CreatedBy { get; set; }


        public int? UpdatedBy { get; set; }
    }

}
