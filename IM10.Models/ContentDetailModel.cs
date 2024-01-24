using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class ContentDetailModel
    {
        public int ContentId { get; set; }

        public string? ContentFileName { get; set; }
        public string? ContentFilePath { get; set; }
        public string? ContentFileName1 { get; set; }

        public string? ContentFilePath1 { get; set; }

        public byte[]? Thumbnail_1 { get; set; }
        public string? Thumbnail_2 { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public byte[]? Thumbnail { get; set; }
        public string? Thumbnail1 { get; set; }


        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public int SubCategoryId { get; set; }
        public string? SubCategoryName { get; set; }

        public long PlayerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? FullName { get; set; }
        public int ContentTypeId { get; set; }
        public string? ContentTypeName { get; set; }

        public int? LanguageId { get; set; }
        public string? LanguageName { get; set; }
        public string? Comment { get; set; }

        public bool? Approved { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }
        public string? Thumbnail2 { get; set; }
        public string? Thumbnail3 { get; set; }

    }



    public class ContentDetailModel1
    {
        public int ContentId { get; set; }

        public string? ContentFileName { get; set; }

        public string? ContentFilePath { get; set; }
        public string? ContentFileName1 { get; set; }

        public string? ContentFilePath1 { get; set; }

        public byte[]? Thumbnail1 { get; set; }
        public string? Title { get; set; }

        public string? Description { get; set; }

        public byte[]? Thumbnail { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public int CategoryId { get; set; }

        public int SubCategoryId { get; set; }

        public long PlayerId { get; set; }
     
        public int ContentTypeId { get; set; }
      
        public int? LanguageId { get; set; }

        public bool Approved { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public string? Thumbnail2 { get; set; }
        public string? Thumbnail3 { get; set; }

    }

    public class ContentModel
    {
        public int ContentId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? UpdatedBy { get; set; }

        public bool Approved { get; set; }
        public long ContentLogId { get; set; }


    }

    public class VideoImageModel
    {
        public int? ContentId { get; set; }
        public string url { get; set;}
        public string Type { get; set; }
        public string thumbnail { get; set; }
        public string FileName { get; set; }
    }

    public class CommentModel
    {
        public int? ContentId { get; set; }

        public string? Comment { get; set; }

    }

}
