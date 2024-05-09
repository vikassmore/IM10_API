using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class ContentUpdateModel
    {
     
        public long ContentLogId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public long ContentId { get; set; }
        public string? ContentTitle { get; set; }
        public string? Comment { get; set; }

        public bool? Approved { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public int? ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }


    }

    public class ContentTitleModel
    {
        public long ContentId { get; set; }
        public string? Title { get; set; }
    }


    public class ContentUpdateComment
    {
        public long ContentLogId { get; set; }
        public string? Comment { get; set; }

    }
}
