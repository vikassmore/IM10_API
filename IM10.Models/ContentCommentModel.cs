using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class ContentCommentModel
    {
        public long CommentId { get; set; }
       public long UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public string? EmailId { get; set; }
        public string? MobileNo { get; set; }
        public long ContentId { get; set; }
        public string? ContentFileName { get; set; }
        public string? ContentFilePath { get; set; }
        public string? Thumbnail1 { get; set; }

        public int ContentTypeId { get; set; }
        public string? ContentTypeName { get; set; }

        public string? DeviceId { get; set; }

        public string? Location { get; set; }

        public bool? Liked { get; set; }

        public string? Comment1 { get; set; }

        public bool? Shared { get; set; }

        public bool? IsPublic { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public long? ParentCommentId { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

    }


    public class ContentCommentModel1
    {
        public long CommentId { get; set; }
        public long UserId { get; set; }
        public long ContentId { get; set; }
        public int ContentTypeId { get; set; }
        public string? DeviceId { get; set; }
        public string? Location { get; set; }
        public bool? Liked { get; set; }
        public string? Comment1 { get; set; }
        public bool? Shared { get; set; }
        public bool? IsPublic { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public long? ParentCommentId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

    }
    public class ContentCommentModelData
    {
        public long CommentId { get; set; }
        public List<ContentCommentModel> contentCommentModels { get; set; }

    }
}
