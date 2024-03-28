using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class ContentFlagModel
    {
        public ContentFlagModel()
        {
            this.ContentDetails = new List<ContentDetailMobileModel>();
            this.advertiseMobileModels = new List<AdvertiseMobileModel>();

        }
        public long ContentFlagId { get; set; }
        public long PlayerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PlayerName { get; set; }
        public long ContentId { get; set; }
        public string? ContentTitle { get; set; }
        public string? ContentFileName { get; set; }
        public string? ContentFilePath { get; set; }
        public bool? Trending { get; set; }
        public bool? MostLiked { get; set; }
        public bool? Favourite { get; set; }
        public int? ContentSequence { get; set; }
        public int ContentTypeId { get; set; }
        public string? ContentTypeName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public int? CommentCount { get; set; }
        public int? ViewNo { get; set; }
        public int? LikedNo { get; set; }
        public int? FavouriteNo { get; set; }

        public List<ContentDetailMobileModel> ContentDetails { get; set; }
        public List<AdvertiseMobileModel> advertiseMobileModels { get; set; }

    }
    public class ArticleCategory
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public long ContentId { get; set; }
    }

    public class model
    {
        public model()
        {
            this.ContentDetails = new List<ContentDetailMobileModel>();
            this.advertiseMobileModels = new List<AdvertiseMobileModel>();
        }
        public List<ContentDetailMobileModel> ContentDetails { get; set; }
        public List<AdvertiseMobileModel> advertiseMobileModels { get; set; }
    }

    public class ContentDetailMobileModel
    {
        public ContentDetailMobileModel()
        {
        }
        public long ContentId { get; set; }

        public string? ContentFileName { get; set; }
        public string? ContentFilePath { get; set; }
        public string? ContentFileName1 { get; set; }
        public string? ContentFilePath1 { get; set; }
        //  public byte[]? Thumbnail_1 { get; set; }
        // public byte[]? Thumbnail_2 { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

    }

    public class AdvertiseMobileModel
    {
        public long ContentId { get; set; }
        public string? Thumbnail { get; set; }
        public long AdvertiseContentId { get; set; }
        public string? AdvertiseTitle { get; set; }
        public string? AdvertiseFileName { get; set; }
        public string? AdvertiseFilePath { get; set; }

    }
    public class ContentMobileModel
    {
        public long ContentId { get; set; }
        public long AdvertiseContentId { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? Thumbnail { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? Position { get; set; }

        public string? CategoryName { get; set; }

    }
    public class MobileContentData
    {
        public string? Thumbnail { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? ViewNo { get; set; }
        public int? LikedNo { get; set; }
        public int? FavouriteNo { get; set; }
        public int CategoryId { get; set; }
        public int ContentTypeId { get; set; }
        public int CommentCount { get; set; }
        public long ContentId { get; set; }
        public bool Liked { get; set; }
        public bool Favourite { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<ContentMobileModel> contentMobileModels { get; set; }
    }
    public class MobileArticleContentData
    {
        public long ContentId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? CategoryName { get; set; }
    }


    public class MobileVideoCategoryData
    {
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public int? DisplayOrder { get; set; }
        public List<MobileContentData> mobileContentDatas { get; set; }
    }
    //public class CategoryVideoModel
    //{
    //    public long ContentId { get; set; }
    //    public long AdvertiseContentId { get; set; }
    //    public string? FileName { get; set; }
    //    public string? FilePath { get; set; }
    //    public string? Thumbnail { get; set; }
    //    public string? Title { get; set; }
    //    public string? Description { get; set; }
    //    public int? Position { get; set; }
    //    public string CategoryName { get; set; }

    //}
    public class MobileArticleCategoryData
    {
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public int? DisplayOrder { get; set; }
        public List<CategoryArticleModel> categoryArticleModels { get; set; }
    }
    public class CategoryArticleModel
    {
        public long ContentId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? Thumbnail { get; set; }
        public string CategoryName { get; set; }
        public int? ViewNo { get; set; }
        public int? LikedNo { get; set; }
        public int? FavouriteNo { get; set; }
        public int CategoryId { get; set; }
        public int ContentTypeId { get; set; }
        public int CommentCount { get; set; }
        public bool Liked { get; set; }
        public bool Favourite { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
    public class ListingLogoDetailsModel
    {
        public long ListingId { get; set; }
        public long PlayerId { get; set; }
        public string? CompanyName { get; set; }
        public string? Description { get; set; }
        public string? CompanyLogoFileName { get; set; }
        public string? CompanyLogoFilePath { get; set; }
        public int? Position { get; set; }
    }

    public class MobileSearchDataModel
    {
        public List<ListingLogoDetailsModel> listingLogoDetailsModels { get; set; }
        public List<MobileContentData> mobileContentDatas { get; set; }
        public List<CategoryArticleModel> categoryArticleModels { get; set; }
        public List<ListingLogoDetailsModel> listingDetailsModels { get; set; }
    }

    public class ContentModelFlag
    {
        public long ContentFlagId { get; set; }

        public long PlayerId { get; set; }
        public long UserId { get; set; }

        public long ContentId { get; set; }

        public bool? MostLiked { get; set; }
        public bool? Favourite { get; set; }

        public int? ContentSequence { get; set; }

        public int ContentTypeId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }
    }
    public class ContentModelView
    {
        public long ContentViewId { get; set; }

        public long PlayerId { get; set; }

        public long ContentId { get; set; }

        public bool? Trending { get; set; }

        public int? ContentSequence { get; set; }

        public int ContentTypeId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }
    }
    public class PlayerMobileDataModel
    {
        public int PlayerDataId { get; set; }
        public long PlayerId { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }

    }

    public class PlayerMobileLikeFavouriteData
    {
        public List<MobileContentData> videoContentData { get; set; }
        public List<CategoryArticleModel> articleContentData { get; set; }
    }
}
