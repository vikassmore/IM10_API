using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class AdvContentMappingModel
    {
        public long AdvContentMapId { get; set; }

        public long ContentId { get; set; }

        public long AdvertiseContentId { get; set; }

        public int CategoryId { get; set; }

        public int SubCategoryId { get; set; }

        public int? Position { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

    }

    public class AdvContentMappingModel1
    {
        public long AdvContentMapId { get; set; }

        public long ContentId { get; set; }
        public string? ContentName { get; set; }


        public long AdvertiseContentId { get; set; }
        public string? AdvertiseContentName { get; set; }

        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public int SubCategoryId { get; set; }
        public string? SubcategoryName { get; set; }

        public int? Position { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

    }

  

}
