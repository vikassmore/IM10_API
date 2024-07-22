using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class AdvContentDetailsModel
    {
        public long AdvertiseContentId { get; set; }
        public string? Title { get; set; }
        public long PlayerId { get; set; }
        public int NationId { get; set; }
        public string NationName { get; set; } = null!;
        public int StateId { get; set; }
        public string StateName { get; set; } = null!;
        public int CityId { get; set; }
        public string CityName { get; set; } = null!;
        public bool IsGlobal { get; set; }
        public string? AdvertiseFileName { get; set; }
        public string? AdvertiseFilePath { get; set; }
        public int ContentTypeId { get; set; }
        public string ContentName { get; set; } = null!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Comment { get; set; }
        public bool? Approved { get; set; }
        public string? FinalPrice { get; set; }
        public bool? IsFree { get; set; }
        public bool? ProductionFlag { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
       public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }

    }


    public class AdvContentDetailsModel1
    {
        public long AdvertiseContentId { get; set; }
        public string? Title { get; set; }
        public long PlayerId { get; set; }
        public int NationId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public bool IsGlobal { get; set; }
        public string AdvertiseFileName { get; set; }
        public string? AdvertiseFilePath { get; set; }
        public int ContentTypeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? FinalPrice { get; set; }
        public bool? IsFree { get; set; }
        public bool? ProductionFlag { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
    }


    public class AdvContentComment
    {
        public long AdvertiseContentId { get; set; }
        public string? Comment { get; set; }

    }
}
