using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IM10.Models.MobileModel;

namespace IM10.Models
{
    public class MobileModel
    {

        public MobileModel()
        {
            this.Articles = new List<ArticleModel>();
        }

        public long PlayerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PlayerName { get; set; }
        public List<ArticleModel> Articles { get; set; }


        public class ArticleModel
        {
            public long AdvContentMapId { get; set; }
            public long ContentId { get; set; }
            public DateTime? CreatedDate { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public string? ContentFileName { get; set; }
            public string? ContentFilePath { get; set; }
            public long AdvertiseContentId { get; set; }
            public string AdvertiseFileName { get; set; }
            public string? AdvertiseFilePath { get; set; }
        }

    }

    public class AppModel
    {
        public AppModel()
        {
            this.Videos = new List<VideosModel>();
        }
        public long PlayerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PlayerName { get; set; }
        public List<VideosModel> Videos { get; set; }

        public class VideosModel
        {
            public long AdvContentMapId { get; set; }
            public long ContentId { get; set; }
            public DateTime? CreatedDate { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public string? ContentFileName { get; set; }
            public string? ContentFilePath { get; set; }
            public long AdvertiseContentId { get; set; }
            public string AdvertiseFileName { get; set; }
            public string? AdvertiseFilePath { get; set; }
        }
    }
}
