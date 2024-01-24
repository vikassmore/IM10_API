using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class PlayerDetailModel
    {
        public long PlayerId { get; set; }
        public string? PlayerName { get; set; }
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
        public string FullName { get; set; }
        public string? Address { get; set; }
        public string? ProfileImageFileName { get; set; }
        public string? ProfileImageFilePath { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

    }


    public class PlayerDetailModel1
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
        public string FullName { get; set; }
        public string? Address { get; set; }

        public string? ProfileImageFileName { get; set; }

        public string? ProfileImageFilePath { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

    }



}
