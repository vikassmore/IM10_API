using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using StartUpX.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using static IM10.Models.AppModel;
using static IM10.Models.MobileModel;

namespace IM10.BAL.Implementaion
{
    public class MobileService : IMobileServices
    {
        IM10DbContext context;
        private ConfigurationModel _configuration;
        private readonly IUserAuditLogService _userAuditLogService;
        /// <summary>
        /// Creating constructor and injection dbContext
        /// </summary>
        /// <param name="_context"></param>
        public MobileService(IM10DbContext _context, IOptions<ConfigurationModel> hostName, IUserAuditLogService userAuditLogService)
        {
            context = _context;
            _configuration = hostName.Value;
            _userAuditLogService = userAuditLogService;
        }
        /// <summary>
        /// Video Thumbnail
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string ThumbnailPath(string filePath)
        {
            string extension = "";
            if (!String.IsNullOrWhiteSpace(filePath))
            {
                extension = Path.GetExtension(filePath);
                if (!String.IsNullOrWhiteSpace(extension))
                {
                    filePath = filePath.Replace(extension, ".jpeg");
                    filePath = filePath.Replace("ContentFile/", "ContentFile/thumbnail/");
                }
            }

            return filePath;
        }

        /// <summary>
        /// Method is used to get all treding contentflagDetail
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<MobileContentData> GetTop5TrendingVideoContent(long playerId, long userId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var tredingEntity = (from flag in context.ContentViews
                                 join player in context.PlayerDetails on flag.PlayerId equals player.PlayerId
                                 join content in context.ContentDetails on flag.ContentId equals content.ContentId
                                 join type in context.ContentTypes on flag.ContentTypeId equals type.ContentTypeId
                                 where flag.IsDeleted == false && flag.Trending == true && flag.ContentTypeId == ContentTypeHelper.VideoContentTypeId
                                 && flag.PlayerId == playerId && content.Approved == true && content.IsDeleted==false
                                 orderby flag.UpdatedDate descending
                                 select new
                                 {
                                     content.ContentId
                                 }).ToList();
            var tredingtopfive = tredingEntity.GroupBy(x => new { x.ContentId }).Select(x => new { ContentId = x.Key, count = x.Count() }).OrderByDescending(x => x.count).Take(5).ToList();
            if (tredingtopfive.Count == 0)
            {
                tredingEntity = (from flag in context.AdvContentMappings
                                 join content in context.ContentDetails on flag.ContentId equals content.ContentId
                                 where content.ContentTypeId == ContentTypeHelper.VideoContentTypeId && flag.IsDeleted == false && content.Approved == true && content.PlayerId == playerId
                                 orderby flag.CreatedDate descending
                                 select new
                                 {
                                     flag.ContentId
                                 }).ToList();
                tredingtopfive = tredingEntity.GroupBy(x => new { x.ContentId }).Select(x => new { ContentId = x.Key, count = x.Count() }).OrderByDescending(x => x.count).Take(5).ToList();
            }
            List<MobileContentData> topList = new List<MobileContentData>();
            foreach (var item in tredingtopfive)
            {
                var topModel = new MobileContentData();
                var contentList = new List<ContentMobileModel>();
                var advertiseContent = (from advcontent in context.AdvContentMappings
                                        join adv in context.AdvContentDetails on advcontent.AdvertiseContentId equals adv.AdvertiseContentId
                                        where advcontent.ContentId == item.ContentId.ContentId && advcontent.IsDeleted == false
                                        select new
                                        {
                                            adv.AdvertiseContentId,
                                            adv.AdvertiseFileName,
                                            adv.AdvertiseFilePath,
                                            adv.Title,
                                            advcontent.ContentId,
                                            advcontent.Position,
                                        }).ToList();
                //var advertiseContent = context.AdvContentMappings.Include(x => x.Content).Include(x => x.AdvertiseContent).Where(x => x.ContentId == item.ContentId.ContentId && x.IsDeleted == false).ToList();
                var contentVideo = context.ContentDetails.Include(x => x.Category).FirstOrDefault(x => x.ContentId == item.ContentId.ContentId && x.IsDeleted == false);
                var imgmodel = new VideoImageModel();
                if (contentVideo.ContentFileName != null)
                {
                    var content1 = new ContentMobileModel();
                    imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(contentVideo.ContentFilePath) ? contentVideo.ContentFilePath : contentVideo.ContentFilePath);
                    imgmodel.Type = String.IsNullOrEmpty(contentVideo.ContentFilePath) ? "video" : "image";
                    imgmodel.FileName = (imgmodel.url);

                    content1.ContentId = contentVideo.ContentId;
                    content1.FileName = contentVideo.ContentFileName;
                    content1.FilePath = contentVideo.ContentFilePath;
                    content1.Title = contentVideo.Title;
                    content1.Description = contentVideo.Description;
                    content1.Position = Helper.FirstHalfContentPostion;
                    content1.CategoryName = contentVideo.Category.Name;
                    content1.Thumbnail = ThumbnailPath(imgmodel.url);
                    contentList.Add(content1);
                }
                if (contentVideo.ContentFileName1 != null)
                {
                    var content2 = new ContentMobileModel();
                    var imgmodel1 = new VideoImageModel();
                    imgmodel1.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(contentVideo.ContentFilePath1) ? contentVideo.ContentFilePath1 : contentVideo.ContentFilePath1);
                    imgmodel1.Type = String.IsNullOrEmpty(contentVideo.ContentFilePath1) ? "video" : "image";
                    imgmodel1.FileName = (imgmodel1.url);

                    content2.ContentId = contentVideo.ContentId;
                    content2.FileName = contentVideo.ContentFileName1;
                    content2.FilePath = contentVideo.ContentFilePath1;
                    content2.Title = contentVideo.Title;
                    content2.Description = contentVideo.Description;
                    content2.Position = Helper.SecondHalfContentPostion;
                    content2.CategoryName = contentVideo.Category.Name;
                    content2.Thumbnail = ThumbnailPath(imgmodel1.url);
                    contentList.Add(content2);
                }
                foreach (var advcontent in advertiseContent)
                {
                    var model = new ContentMobileModel();
                    var imgmodel2 = new VideoImageModel();
                    imgmodel2.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(advcontent.AdvertiseFilePath) ? advcontent.AdvertiseFilePath : advcontent.AdvertiseFilePath);
                    imgmodel2.Type = String.IsNullOrEmpty(advcontent.AdvertiseFilePath) ? "video" : "image";
                    imgmodel2.FileName = (imgmodel2.url);
                    model.AdvertiseContentId = advcontent.AdvertiseContentId;
                    model.FileName = advcontent.AdvertiseFileName;
                    model.FilePath = advcontent.AdvertiseFilePath;
                    model.Title = advcontent.Title;
                    model.Thumbnail = imgmodel2.url;
                    model.ContentId = advcontent.ContentId;
                    model.Position = advcontent.Position;
                    contentList.Add(model);
                }
                topModel.contentMobileModels = contentList.OrderBy(x => x.Position).ToList();
                topModel.ContentId = contentVideo.ContentId;
                topModel.Title = contentVideo.Title;
                topModel.Description = contentVideo.Description;
                topModel.Thumbnail = ThumbnailPath(imgmodel.url);
                topModel.CategoryId = contentVideo.CategoryId;
                topModel.ViewNo = context.ContentViews.Where(x => x.ContentId == item.ContentId.ContentId && x.Trending == true).Select(x => x.Trending).Count();
                topModel.ContentTypeId = contentVideo.ContentTypeId;
                topModel.LikedNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId.ContentId && x.MostLiked == true).Select(x => x.MostLiked).Count();
                topModel.FavouriteNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId.ContentId && x.Favourite == true).Select(x => x.Favourite).Count();
                topModel.CommentCount = context.Comments.Where(x => x.ContentId == item.ContentId.ContentId && x.IsPublic == true && x.IsDeleted == false).Select(x => x.CommentId).Count();
                topModel.Liked = context.ContentFlags.Where(x => x.ContentId == item.ContentId.ContentId && x.MostLiked == true && x.UserId == userId).Select(x => x.MostLiked).Distinct().Count() >= 1 ? true : false;
                topModel.Favourite = context.ContentFlags.Where(x => x.ContentId == item.ContentId.ContentId && x.Favourite == true && x.UserId == userId).Select(x => x.Favourite).Distinct().Count() >= 1 ? true : false;
                topModel.CreatedDate = contentVideo.CreatedDate;
                topList.Add(topModel);

            }
            if (tredingEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            return topList;
        }



        /// <summary>
        /// Method is used to get all Category top five video
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<MobileVideoCategoryData> GetAllCategoryTopFiveVideoContent(long playerId, long userId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            List<MobileVideoCategoryData> mobileVideoCategoryData = new List<MobileVideoCategoryData>();
            var categoryEnitityList = (from content in context.ContentDetails
                                       where content.ContentTypeId == ContentTypeHelper.VideoContentTypeId &&
                                             content.Approved == true &&
                                             content.PlayerId == playerId &&
                                             content.IsDeleted == false
                                       join flag in context.AdvContentMappings.Where(m => m.IsDeleted == false)
                                       on content.ContentId equals flag.ContentId into gj
                                       from subflag in gj.DefaultIfEmpty()
                                       orderby subflag != null ? subflag.CreatedDate : content.CreatedDate descending
                                       select new
                                       {
                                           ContentId = content.ContentId,
                                           CategoryId = subflag != null ? subflag.CategoryId : content.CategoryId,
                                           Name = subflag != null ? subflag.Category.Name : content.Category.Name,
                                           DisplayOrder = subflag != null ? subflag.Category.DisplayOrder : 0,
                                           HasAdvertisement = subflag != null // Indicates if there's an advertisement mapped
                                       }).ToList();
            var categoryList = categoryEnitityList.GroupBy(x => new { x.CategoryId, x.Name }).Select(x => new { CategoryId = x.Key.CategoryId, Name = x.Key.Name, DisplayOrder = x.Max(x=>x.DisplayOrder), count = x.Count() }).ToList();
            foreach (var item in categoryList)
            {
                var displayEntity = context.Categories.Where(x => x.CategoryId ==item.CategoryId).Select(z => z.DisplayOrder).FirstOrDefault();
                var categoryModel = new MobileVideoCategoryData();
                categoryModel.CategoryId = item.CategoryId;
                categoryModel.CategoryName = item.Name;
                categoryModel.DisplayOrder = displayEntity.HasValue ? displayEntity.Value : 0;
                List<MobileContentData> modelList = new List<MobileContentData>();
                var contentEntity = categoryEnitityList.Where(x => x.CategoryId == item.CategoryId).ToList();
                var contenttopfive = contentEntity.GroupBy(x => new { x.ContentId }).Select(x => new { ContentId = x.Key, count = x.Count() }).OrderByDescending(x => x.count).ToList();
                foreach (var item1 in contenttopfive)
                {
                    var topModel = new MobileContentData();
                    var contentVideo = context.ContentDetails.Include(x => x.Category).FirstOrDefault(x => x.ContentId == item1.ContentId.ContentId && x.Approved == true && x.IsDeleted == false);
                    if (contentVideo != null)
                    {
                        var contentList = new List<ContentMobileModel>();
                        var advertiseContent = (from advcontent in context.AdvContentMappings
                                                join adv in context.AdvContentDetails on advcontent.AdvertiseContentId equals adv.AdvertiseContentId
                                                where advcontent.ContentId == item1.ContentId.ContentId && advcontent.IsDeleted == false
                                                select new
                                                {
                                                    adv.AdvertiseContentId,
                                                    adv.AdvertiseFileName,
                                                    adv.AdvertiseFilePath,
                                                    adv.Title,
                                                    advcontent.ContentId,
                                                    advcontent.Position,
                                                }).ToList();
                        var imgmodel = new VideoImageModel();
                        if (contentVideo.ContentFileName != null)
                        {
                            var content1 = new ContentMobileModel();
                            imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(contentVideo.ContentFilePath) ? contentVideo.ContentFilePath : contentVideo.ContentFilePath);
                            imgmodel.Type = String.IsNullOrEmpty(contentVideo.ContentFilePath) ? "video" : "image";
                            imgmodel.FileName = (imgmodel.url);

                            content1.ContentId = contentVideo.ContentId;
                            content1.FileName = contentVideo.ContentFileName;
                            content1.FilePath = contentVideo.ContentFilePath;
                            content1.Title = contentVideo.Title;
                            content1.Description = contentVideo.Description;
                            content1.Position = Helper.FirstHalfContentPostion;
                            content1.CategoryName = contentVideo.Category.Name;
                            content1.Thumbnail = ThumbnailPath(imgmodel.url);
                            contentList.Add(content1);
                        }
                        if (contentVideo.ContentFileName1 != null)
                        {
                            var content2 = new ContentMobileModel();
                            var imgmodel1 = new VideoImageModel();
                            imgmodel1.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(contentVideo.ContentFilePath1) ? contentVideo.ContentFilePath1 : contentVideo.ContentFilePath1);
                            imgmodel1.Type = String.IsNullOrEmpty(contentVideo.ContentFilePath1) ? "video" : "image";
                            imgmodel1.FileName = (imgmodel1.url);

                            content2.ContentId = contentVideo.ContentId;
                            content2.FileName = contentVideo.ContentFileName1;
                            content2.FilePath = contentVideo.ContentFilePath1;
                            content2.Title = contentVideo.Title;
                            content2.Description = contentVideo.Description;
                            content2.Position = Helper.SecondHalfContentPostion;
                            content2.CategoryName = contentVideo.Category.Name;
                            content2.Thumbnail = ThumbnailPath(imgmodel1.url);
                            contentList.Add(content2);
                        }
                        foreach (var advcontent in advertiseContent)
                        {
                            var model = new ContentMobileModel();
                            var imgmodel2 = new VideoImageModel();
                            imgmodel2.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(advcontent.AdvertiseFilePath) ? advcontent.AdvertiseFilePath : advcontent.AdvertiseFilePath);
                            imgmodel2.Type = String.IsNullOrEmpty(advcontent.AdvertiseFilePath) ? "video" : "image";
                            imgmodel2.FileName = (imgmodel2.url);
                            model.AdvertiseContentId = advcontent.AdvertiseContentId;
                            model.FileName = advcontent.AdvertiseFileName;
                            model.FilePath = advcontent.AdvertiseFilePath;
                            model.Title = advcontent.Title;
                            model.Thumbnail = imgmodel2.url;
                            model.ContentId = advcontent.ContentId;
                            model.Position = advcontent.Position;
                            contentList.Add(model);
                        }
                        topModel.contentMobileModels = contentList.OrderBy(x => x.Position).ToList();
                        topModel.ContentId = contentVideo.ContentId;
                        topModel.Title = contentVideo.Title;
                        topModel.Description = contentVideo.Description;
                        topModel.Thumbnail = ThumbnailPath(imgmodel.url);
                        topModel.CategoryId = contentVideo.Category.CategoryId;
                        topModel.ViewNo = context.ContentViews.Where(x => x.ContentId == item1.ContentId.ContentId && x.Trending == true).Select(x => x.Trending).Count();
                        topModel.ContentTypeId = contentVideo.ContentTypeId;
                        topModel.LikedNo = context.ContentFlags.Where(x => x.ContentId == item1.ContentId.ContentId && x.MostLiked == true).Select(x => x.MostLiked).Count();
                        topModel.FavouriteNo = context.ContentFlags.Where(x => x.ContentId == item1.ContentId.ContentId && x.Favourite == true).Select(x => x.Favourite).Count();
                        topModel.CommentCount = context.Comments.Where(x => x.ContentId == item1.ContentId.ContentId).Select(x => x.CommentId).Count();
                        topModel.Liked = context.ContentFlags.Where(x => x.ContentId == item1.ContentId.ContentId && x.MostLiked == true && x.UserId == userId).Select(x => x.MostLiked).Distinct().Count() >= 1 ? true : false;
                        topModel.Favourite = context.ContentFlags.Where(x => x.ContentId == item1.ContentId.ContentId && x.Favourite == true && x.UserId == userId).Select(x => x.Favourite).Distinct().Count() >= 1 ? true : false;
                        topModel.CreatedDate = contentVideo.CreatedDate;
                        modelList.Add(topModel);
                        modelList= modelList.OrderByDescending(x => x.CreatedDate).ToList();
                    }
                }
                categoryModel.mobileContentDatas = modelList;
                if (modelList.Any())
                {
                    mobileVideoCategoryData.Add(categoryModel);
                }
            }
            if (categoryEnitityList.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            List<MobileVideoCategoryData> mobileVideoCategoryListData = mobileVideoCategoryData.OrderBy(x => x.DisplayOrder).ToList();
            return mobileVideoCategoryListData;
        }
        /// <summary>
        /// Method is used to get all video By Category
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MobileVideoCategoryData GetVideoContentByCategory(long playerId, long categoryId, long userId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            MobileVideoCategoryData videoModel = new MobileVideoCategoryData();
            var categoryEnitityList = (from content in context.ContentDetails
                                       where content.IsDeleted==false && content.ContentTypeId == ContentTypeHelper.VideoContentTypeId && content.Approved == true && content.PlayerId == playerId &&  content.CategoryId == categoryId
                                       join flag in context.AdvContentMappings .Where(m=>m.IsDeleted==false)
                                       on content.ContentId equals flag.ContentId into gj
                                       from subflag in gj.DefaultIfEmpty()
                                       orderby subflag != null ? subflag.CreatedDate : content.CreatedDate descending
                                       select new
                                       {
                                           ContentId = content.ContentId,
                                           CategoryId = subflag != null ? subflag.CategoryId : content.CategoryId,
                                           CategoryName = subflag != null ? subflag.Category.Name : content.Category.Name,
                                           DisplayOrder = subflag != null ? subflag.Category.DisplayOrder : 0,
                                           HasAdvertisement = subflag != null // Indicates if there's an advertisement mapped
                                       }).ToList();
            var contenttopfive = categoryEnitityList.GroupBy(x => new { x.ContentId, x.CategoryId, x.CategoryName, x.DisplayOrder }).Select(x => new { ContentId = x.Key, CategoryId = x.Key.CategoryId, Name = x.Key.CategoryName, DisplayOrder = x.Key.DisplayOrder, count = x.Count() }).ToList();
            List<MobileContentData> topList = new List<MobileContentData>();
            foreach (var item1 in contenttopfive)
            {
                var displayEntity = context.Categories.Where(x => x.CategoryId == categoryId).Select(z => z.DisplayOrder).FirstOrDefault();
                videoModel.CategoryName = item1.Name;
                videoModel.CategoryId = item1.CategoryId;
                videoModel.DisplayOrder = displayEntity.HasValue ? displayEntity.Value : 0;
                var topModel = new MobileContentData();
                var contentVideo = context.ContentDetails.Include(x => x.Category).FirstOrDefault(x => x.ContentId == item1.ContentId.ContentId && x.Approved == true && x.IsDeleted == false);
                if (contentVideo != null)
                {
                    var contentList = new List<ContentMobileModel>();
                    var advertiseContent = (from advcontent in context.AdvContentMappings
                                            join adv in context.AdvContentDetails on advcontent.AdvertiseContentId equals adv.AdvertiseContentId
                                            where advcontent.ContentId == item1.ContentId.ContentId && advcontent.IsDeleted == false
                                            select new
                                            {
                                                adv.AdvertiseContentId,
                                                adv.AdvertiseFileName,
                                                adv.AdvertiseFilePath,
                                                adv.Title,
                                                advcontent.ContentId,
                                                advcontent.Position,
                                            }).ToList();
                    var imgmodel = new VideoImageModel();
                    if (contentVideo.ContentFileName != null)
                    {
                        var content1 = new ContentMobileModel();
                        imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(contentVideo.ContentFilePath) ? contentVideo.ContentFilePath : contentVideo.ContentFilePath);
                        imgmodel.Type = String.IsNullOrEmpty(contentVideo.ContentFilePath) ? "video" : "image";
                        imgmodel.FileName = (imgmodel.url);

                        content1.ContentId = contentVideo.ContentId;
                        content1.FileName = contentVideo.ContentFileName;
                        content1.FilePath = contentVideo.ContentFilePath;
                        content1.Title = contentVideo.Title;
                        content1.Description = contentVideo.Description;
                        content1.Position = Helper.FirstHalfContentPostion;
                        content1.CategoryName = contentVideo.Category.Name;
                        content1.Thumbnail = ThumbnailPath(imgmodel.url);
                        contentList.Add(content1);
                    }
                    if (contentVideo.ContentFileName1 != null)
                    {
                        var content2 = new ContentMobileModel();
                        var imgmodel1 = new VideoImageModel();
                        imgmodel1.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(contentVideo.ContentFilePath1) ? contentVideo.ContentFilePath1 : contentVideo.ContentFilePath1);
                        imgmodel1.Type = String.IsNullOrEmpty(contentVideo.ContentFilePath1) ? "video" : "image";
                        imgmodel1.FileName = (imgmodel1.url);

                        content2.ContentId = contentVideo.ContentId;
                        content2.FileName = contentVideo.ContentFileName1;
                        content2.FilePath = contentVideo.ContentFilePath1;
                        content2.Title = contentVideo.Title;
                        content2.Description = contentVideo.Description;
                        content2.Position = Helper.SecondHalfContentPostion;
                        content2.CategoryName = contentVideo.Category.Name;
                        content2.Thumbnail = ThumbnailPath(imgmodel1.url);
                        contentList.Add(content2);
                    }
                    foreach (var advcontent in advertiseContent)
                    {
                        var model = new ContentMobileModel();
                        var imgmodel2 = new VideoImageModel();
                        imgmodel2.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(advcontent.AdvertiseFilePath) ? advcontent.AdvertiseFilePath : advcontent.AdvertiseFilePath);
                        imgmodel2.Type = String.IsNullOrEmpty(advcontent.AdvertiseFilePath) ? "video" : "image";
                        imgmodel2.FileName = (imgmodel2.url);
                        model.AdvertiseContentId = advcontent.AdvertiseContentId;
                        model.FileName = advcontent.AdvertiseFileName;
                        model.FilePath = advcontent.AdvertiseFilePath;
                        model.Title = advcontent.Title;
                        model.Thumbnail = imgmodel2.url;
                        model.ContentId = advcontent.ContentId;
                        model.Position = advcontent.Position;
                        contentList.Add(model);
                    }
                    topModel.contentMobileModels = contentList.OrderBy(x => x.Position).ToList();
                    topModel.ContentId = contentVideo.ContentId;
                    topModel.Title = contentVideo.Title;
                    topModel.Description = contentVideo.Description;
                    topModel.Thumbnail = ThumbnailPath(imgmodel.url);
                    topModel.ViewNo = context.ContentViews.Where(x => x.ContentId == item1.ContentId.ContentId && x.Trending == true).Select(x => x.Trending).Count();
                    topModel.ContentTypeId = contentVideo.ContentTypeId;
                    topModel.LikedNo = context.ContentFlags.Where(x => x.ContentId == item1.ContentId.ContentId && x.MostLiked == true).Select(x => x.MostLiked).Count();
                    topModel.FavouriteNo = context.ContentFlags.Where(x => x.ContentId == item1.ContentId.ContentId && x.Favourite == true).Select(x => x.Favourite).Count();
                    topModel.CommentCount = context.Comments.Where(x => x.ContentId == item1.ContentId.ContentId && x.IsPublic == true && x.IsDeleted == false).Select(x => x.CommentId).Count();
                    topModel.Liked = context.ContentFlags.Where(x => x.ContentId == item1.ContentId.ContentId && x.MostLiked == true && x.UserId == userId).Select(x => x.MostLiked).Distinct().Count() >= 1 ? true : false;
                    topModel.Favourite = context.ContentFlags.Where(x => x.ContentId == item1.ContentId.ContentId && x.Favourite == true && x.UserId == userId).Select(x => x.Favourite).Distinct().Count() >= 1 ? true : false;
                    topModel.CreatedDate = contentVideo.CreatedDate;
                    topList.Add(topModel);
                }
            }
            if (categoryEnitityList.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            videoModel.mobileContentDatas = topList.OrderByDescending(x=>x.CreatedDate).ToList();
            return videoModel;
        }
        /// <summary>
        /// Method is used to get mobileVideoview by contentId
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        public ContentFlagModel GetMobileVideoView(long contentId, ref ErrorResponseModel errorResponseModel)
        {
            string message = "";
            var articleEntity = (from flag in context.ContentFlags
                                 join content in context.ContentDetails on flag.ContentId equals content.ContentId
                                 join view in context.ContentViews on flag.ContentId equals view.ContentId
                                 where flag.ContentId == contentId && flag.IsDeleted == false && content.Approved == true && flag.ContentTypeId == ContentTypeHelper.VideoContentTypeId
                                 select new
                                 {
                                     flag.ContentFlagId,
                                     flag.PlayerId,
                                     flag.ContentId,
                                     view.Trending,
                                     flag.MostLiked,
                                     flag.ContentSequence,
                                     flag.ContentTypeId,
                                     flag.CreatedBy,
                                     flag.CreatedDate,
                                     flag.UpdatedBy,
                                     flag.UpdatedDate,
                                     content.ContentFileName,
                                     content.ContentFilePath,
                                     content.Title,
                                     content.Description,
                                 }).FirstOrDefault();
            if (articleEntity == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;
            }

            var imgmodel = new VideoImageModel();
            imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(articleEntity.ContentFilePath) ? articleEntity.ContentFilePath : articleEntity.ContentFilePath);
            imgmodel.Type = String.IsNullOrEmpty(articleEntity.ContentFilePath) ? "video" : "image";
            // imgModel.thumbnail = _configuration.HostName.TrimEnd('/') + "/thumbnail/" + imgModel.url
            imgmodel.FileName = imgmodel.url;

            return new ContentFlagModel
            {
                ContentFlagId = articleEntity.ContentFlagId,
                ContentId = articleEntity.ContentId,
                PlayerId = articleEntity.PlayerId,
                Trending = articleEntity.Trending,
                MostLiked = articleEntity.MostLiked,
                ContentSequence = articleEntity.ContentSequence,
                ContentTypeId = articleEntity.ContentTypeId,
                CreatedBy = articleEntity.CreatedBy,
                CreatedDate = articleEntity.CreatedDate,
                UpdatedBy = articleEntity.UpdatedBy,
                UpdatedDate = articleEntity.UpdatedDate,
                ContentFileName = articleEntity.ContentFileName,
                ContentFilePath = imgmodel.url,
                ContentTitle = articleEntity.Title
            };
        }
        /// <summary>
        /// Method is used to get all Category top five Article
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<MobileArticleCategoryData> GetAllCategoryTopFiveArticleContent(long playerId, long userId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            List<MobileArticleCategoryData> mobileArticleList = new List<MobileArticleCategoryData>();
            var categoryEnitityList = (from content in context.ContentDetails
                                       where content.ContentTypeId == ContentTypeHelper.ArticleContentTypeId && content.IsDeleted == false && content.Approved == true && content.PlayerId == playerId
                                       orderby content.UpdatedDate descending
                                       select new
                                       {
                                           content.ContentId,
                                           content.CategoryId,
                                           content.Category.Name,
                                           content.Category.DisplayOrder
                                       }).ToList();
            var categoryList = categoryEnitityList.GroupBy(x => new { x.CategoryId, x.Name, x.DisplayOrder }).Select(x => new { CategoryId = x.Key.CategoryId, Name = x.Key.Name, DisplayOrder = x.Key.DisplayOrder, count = x.Count() }).ToList();
            foreach (var item in categoryList)
            {
                var categoryModel = new MobileArticleCategoryData();
                categoryModel.CategoryId = item.CategoryId;
                categoryModel.CategoryName = item.Name;
                categoryModel.DisplayOrder = item.DisplayOrder;
                List<CategoryArticleModel> categoryArticleList = new List<CategoryArticleModel>();
                var contentEntity = categoryEnitityList.Where(x => x.CategoryId == item.CategoryId).ToList();
                var contenttopfive = contentEntity.GroupBy(x => new { x.ContentId }).Select(x => new { ContentId = x.Key, count = x.Count() }).OrderByDescending(x => x.count).Take(5).ToList();
                foreach (var item1 in contenttopfive)
                {
                    var contentArticle = context.ContentDetails.FirstOrDefault(x => x.ContentId == item1.ContentId.ContentId && x.Approved == true && x.IsDeleted == false);
                    if (contentArticle != null)
                    {
                        CategoryArticleModel articleModel = new CategoryArticleModel();
                        var imgmodel2 = new VideoImageModel();
                        imgmodel2.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(contentArticle.ContentFilePath1) ? contentArticle.ContentFilePath1 : contentArticle.ContentFilePath1);
                        imgmodel2.Type = String.IsNullOrEmpty(contentArticle.ContentFilePath1) ? "video" : "image";
                        imgmodel2.FileName = (imgmodel2.url);
                        articleModel.ContentId = contentArticle.ContentId;
                        articleModel.Title = contentArticle.Title;
                        articleModel.Description = contentArticle.Description;
                        articleModel.CategoryName = contentArticle.Category.Name;
                        articleModel.FileName = contentArticle.ContentFileName1;
                        articleModel.FilePath = imgmodel2.url;
                        articleModel.ViewNo = context.ContentViews.Where(x => x.ContentId == item1.ContentId.ContentId && x.Trending == true).Select(x => x.Trending).Count();
                        articleModel.ContentTypeId = contentArticle.ContentTypeId;
                        articleModel.LikedNo = context.ContentFlags.Where(x => x.ContentId == item1.ContentId.ContentId && x.MostLiked == true).Select(x => x.MostLiked).Count();
                        articleModel.FavouriteNo = context.ContentFlags.Where(x => x.ContentId == item1.ContentId.ContentId && x.Favourite == true).Select(x => x.Favourite).Count();
                        articleModel.CommentCount = context.Comments.Where(x => x.ContentId == item1.ContentId.ContentId).Select(x => x.CommentId).Count();
                        articleModel.Liked = context.ContentFlags.Where(x => x.ContentId == item1.ContentId.ContentId && x.MostLiked == true && x.UserId == userId).Select(x => x.MostLiked).Distinct().Count() >= 1 ? true : false;
                        articleModel.Favourite = context.ContentFlags.Where(x => x.ContentId == item1.ContentId.ContentId && x.Favourite == true && x.UserId == userId).Select(x => x.Favourite).Distinct().Count() >= 1 ? true : false;
                        articleModel.CreatedDate = contentArticle.CreatedDate;
                        categoryArticleList.Add(articleModel);
                    }
                }
                categoryModel.categoryArticleModels = categoryArticleList;
                mobileArticleList.Add(categoryModel);
            }
            if (categoryEnitityList.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            List<MobileArticleCategoryData> mobileArticleListData = mobileArticleList.OrderBy(x => x.DisplayOrder).ToList();
            return mobileArticleListData;
        }
        /// <summary>
        /// Method is used to get all article By Category
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="categoryId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MobileArticleCategoryData GetArticleContentByCategory(long playerId, long categoryId, long userId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            MobileArticleCategoryData categoryModel = new MobileArticleCategoryData();
            var categoryEnitityList = (from content in context.ContentDetails
                                       where content.ContentTypeId == ContentTypeHelper.ArticleContentTypeId && content.IsDeleted == false && content.Approved == true && content.PlayerId == playerId && content.CategoryId == categoryId
                                       orderby content.UpdatedDate descending
                                       select new
                                       {
                                           content.ContentId,
                                           content.CategoryId,
                                           content.Category.Name,
                                           content.Category.DisplayOrder
                                       }).ToList();
            var categoryList = categoryEnitityList.GroupBy(x => new { x.ContentId, x.CategoryId, x.Name, x.DisplayOrder }).Select(x => new { ContentId = x.Key.ContentId, CategoryId = x.Key.CategoryId, Name = x.Key.Name, DisplayOrder = x.Key.DisplayOrder, count = x.Count() }).ToList();
            List<CategoryArticleModel> categoryArticleList = new List<CategoryArticleModel>();
            foreach (var item in categoryList)
            {
                categoryModel.CategoryName = item.Name;
                categoryModel.CategoryId = item.CategoryId;
                categoryModel.DisplayOrder = item.DisplayOrder;
                var contentArticle = context.ContentDetails.FirstOrDefault(x => x.ContentId == item.ContentId && x.Approved == true && x.IsDeleted == false);
                if (contentArticle != null)
                {
                    CategoryArticleModel articleModel = new CategoryArticleModel();
                    var imgmodel2 = new VideoImageModel();
                    imgmodel2.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(contentArticle.ContentFilePath1) ? contentArticle.ContentFilePath1 : contentArticle.ContentFilePath1);
                    imgmodel2.Type = String.IsNullOrEmpty(contentArticle.ContentFilePath1) ? "video" : "image";
                    imgmodel2.FileName = (imgmodel2.url);
                    articleModel.ContentId = contentArticle.ContentId;
                    articleModel.Title = contentArticle.Title;
                    articleModel.Description = contentArticle.Description;
                    articleModel.CategoryName = contentArticle.Category.Name;
                    articleModel.FileName = contentArticle.ContentFileName1;
                    articleModel.FilePath = imgmodel2.url;
                    articleModel.ViewNo = context.ContentViews.Where(x => x.ContentId == item.ContentId && x.Trending == true).Select(x => x.Trending).Count();
                    articleModel.ContentTypeId = contentArticle.ContentTypeId;
                    articleModel.LikedNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.MostLiked == true).Select(x => x.MostLiked).Count();
                    articleModel.FavouriteNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.Favourite == true).Select(x => x.Favourite).Count();
                    articleModel.CommentCount = context.Comments.Where(x => x.ContentId == item.ContentId).Select(x => x.CommentId).Count();
                    articleModel.Liked = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.MostLiked == true && x.UserId == userId).Select(x => x.MostLiked).Distinct().Count() >= 1 ? true : false;
                    articleModel.Favourite = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.Favourite == true && x.UserId == userId).Select(x => x.Favourite).Distinct().Count() >= 1 ? true : false;
                    articleModel.CreatedDate = contentArticle.CreatedDate;
                    categoryArticleList.Add(articleModel);
                }
            }
            if (categoryEnitityList.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            categoryModel.categoryArticleModels = categoryArticleList;
            return categoryModel;
        }
        /// <summary>
        /// Method is used to get mobilearticleview by contentId
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ContentFlagModel GetMobileArticleView(long contentId, long userId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            string message = "";
            var articleEntity = (from content in context.ContentDetails
                                 where content.ContentId == contentId && content.IsDeleted == false && content.Approved == true && content.ContentTypeId == ContentTypeHelper.ArticleContentTypeId
                                 select new
                                 {
                                     content.ContentId,
                                     content.ContentFileName,
                                     content.ContentFilePath,
                                     content.Title,
                                     content.Description,
                                     content.ContentTypeId,
                                     content.CreatedDate
                                 }).FirstOrDefault();
            if (articleEntity == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;
            }

            var imgmodel = new VideoImageModel();
            imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(articleEntity.ContentFilePath) ? articleEntity.ContentFilePath : articleEntity.ContentFilePath);
            imgmodel.Type = String.IsNullOrEmpty(articleEntity.ContentFilePath) ? "video" : "image";
            // imgModel.thumbnail = _configuration.HostName.TrimEnd('/') + "/thumbnail/" + imgModel.url
            int ViewNo = context.ContentViews.Where(x => x.ContentId == articleEntity.ContentId && x.Trending == true).Select(x => x.Trending).Count();
            int Liked = context.ContentFlags.Where(x => x.ContentId == articleEntity.ContentId && x.MostLiked == true).Select(x => x.MostLiked).Count();
            int Favourite = context.ContentFlags.Where(x => x.ContentId == articleEntity.ContentId && x.Favourite == true).Select(x => x.Favourite).Count();
            int CommentCount = context.Comments.Where(x => x.ContentId == articleEntity.ContentId && x.IsPublic == true && x.IsDeleted == false).Select(x => x.CommentId).Count();
            bool Like = context.ContentFlags.Where(x => x.ContentId == articleEntity.ContentId && x.MostLiked == true && x.UserId == userId).Select(x => x.MostLiked).Distinct().Count() >= 1 ? true : false;
            bool Favourite1 = context.ContentFlags.Where(x => x.ContentId == articleEntity.ContentId && x.Favourite == true && x.UserId == userId).Select(x => x.Favourite).Distinct().Count() >= 1 ? true : false;
            imgmodel.FileName = imgmodel.url;

            return new ContentFlagModel
            {
                ContentId = articleEntity.ContentId,
                ContentFileName = articleEntity.ContentFileName,
                ContentFilePath = imgmodel.url,
                ContentTitle = articleEntity.Title,
                ContentTypeId = articleEntity.ContentTypeId,
                ViewNo = ViewNo,
                LikedNo = Liked,
                FavouriteNo = Favourite,
                CommentCount = CommentCount,
                MostLiked = Like,
                Favourite = Favourite1,
                CreatedDate = articleEntity.CreatedDate
            };
        }
        /// <summary>
        /// Method is used to get ListingDetail by playerId
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public List<ListingLogoDetailsModel> GetListingDetailByplayerId(long playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var listdetailEntity = new List<ListingLogoDetailsModel>();
            var listEntity = (from list in context.ListingDetails
                              join nation in context.Countries on list.NationId equals nation.CountryId
                              join ct in context.Cities on list.CityId equals ct.CityId
                              join st in context.States on list.StateId equals st.StateId
                              join cat in context.Categories on list.CategoryId equals cat.CategoryId
                              join subCat in context.SubCategories on list.SubCategoryId equals subCat.SubCategoryId
                              join user in context.UserMasters on list.CreatedBy equals (int)user.UserId
                              where list.IsDeleted == false && list.PlayerId == playerId
                              orderby list.UpdatedDate descending

                              select new ListingDetailModel
                              {
                                  ListingId = list.ListingId,
                                  PlayerId = list.PlayerId,
                                  RoleId = user.RoleId,
                                  CompanyEmailId = list.CompanyEmailId,
                                  CompanyLogoFileName = list.CompanyLogoFileName,
                                  CompanyName = list.CompanyName,
                                  Description = list.Description,
                                  CompanyLogoFilePath = list.CompanyLogoFilePath,
                                  CompanyWebSite = list.CompanyWebSite,
                                  CompanyMobile = list.CompanyMobile,
                                  CompanyPhone = list.CompanyPhone,
                                  IsGlobal = (list.IsGlobal == null) ? false : list.IsGlobal,
                                  NationId = list.NationId,
                                  NationName = nation.Name,
                                  StateId = list.StateId,
                                  StateName = st.Name,
                                  CityId = list.CityId,
                                  CityName = ct.Name,
                                  CategoryId = list.CategoryId,
                                  CategoryName = cat.Name,
                                  SubCategoryId = list.SubCategoryId,
                                  SubcategoryName = subCat.Name,
                                  StartDate = list.StartDate,
                                  EndDate = list.EndDate,
                                  FinalPrice = (list.FinalPrice == null) ? null : list.FinalPrice.ToString(),
                                  Position = (list.Position == null) ? null : list.Position,
                                  ContactPersonEmailId = list.ContactPersonEmailId,
                                  ContactPersonName = list.ContactPersonName,
                                  ContactPersonMobile = list.ContactPersonMobile,
                                  CreatedBy = list.CreatedBy,
                                  CreatedDate = list.CreatedDate,
                                  UpdatedBy = list.UpdatedBy,
                                  UpdatedDate = list.UpdatedDate,
                              }).ToList();
            if (listEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            listEntity.ForEach(item =>
            {
                var imgmodel = new VideoImageModel();
                imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(item.CompanyLogoFilePath) ? item.CompanyLogoFilePath : item.CompanyLogoFilePath);
                imgmodel.Type = String.IsNullOrEmpty(item.CompanyLogoFilePath) ? "video" : "image";
                imgmodel.FileName = (imgmodel.url);

                listdetailEntity.Add(new ListingLogoDetailsModel
                {
                    ListingId = item.ListingId,
                    PlayerId = item.PlayerId,
                    CompanyName = item.CompanyName,
                    Description = Regex.Replace(item.Description, @"\t|\n|\r", ""),
                    CompanyLogoFileName = item.CompanyLogoFileName,
                    CompanyLogoFilePath = imgmodel.FileName,
                    Position = item.Position
                });
            });
            return listdetailEntity;
        }

        /// <summary>
        /// Method to get ListingDetail by listingId
        /// </summary>
        /// <param name="listingId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public ListingDetailModel GetListingDetailById(long listingId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var listEntity = (from list in context.ListingDetails
                              join
                              nation in context.Countries on list.NationId equals nation.CountryId
                              join state in context.States on list.StateId equals state.StateId
                              join city in context.Cities on list.CityId equals city.CityId
                              join category in context.Categories on list.CategoryId equals category.CategoryId
                              join subcategory in context.SubCategories on list.SubCategoryId equals subcategory.SubCategoryId
                              where list.IsDeleted == false && list.ListingId == listingId
                              select new ListingDetailModel
                              {
                                  ListingId = list.ListingId,
                                  PlayerId = list.PlayerId,
                                  CompanyEmailId = list.CompanyEmailId,
                                  CompanyLogoFileName = list.CompanyLogoFileName,
                                  CompanyName = list.CompanyName,
                                  Description = list.Description,
                                  CompanyLogoFilePath = list.CompanyLogoFilePath,
                                  CompanyWebSite = list.CompanyWebSite,
                                  CompanyMobile = list.CompanyMobile,
                                  CompanyPhone = list.CompanyPhone,
                                  IsGlobal = (list.IsGlobal == null) ? false : list.IsGlobal,
                                  NationId = list.NationId,
                                  NationName = nation.Name,
                                  StateId = list.StateId,
                                  StateName = state.Name,
                                  CityId = list.CityId,
                                  CityName = city.Name,
                                  CategoryId = list.CategoryId,
                                  CategoryName = category.Name,
                                  SubCategoryId = list.SubCategoryId,
                                  SubcategoryName = subcategory.Name,
                                  StartDate = list.StartDate,
                                  EndDate = list.EndDate,
                                  FinalPrice = (list.FinalPrice == null) ? null : list.FinalPrice.ToString(),
                                  Position = list.Position,
                                  ContactPersonEmailId = list.ContactPersonEmailId,
                                  ContactPersonName = list.ContactPersonName,
                                  ContactPersonMobile = list.ContactPersonMobile,
                              }).FirstOrDefault();

            if (listEntity == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            var imgmodel = new VideoImageModel();
            imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(listEntity.CompanyLogoFilePath) ? listEntity.CompanyLogoFilePath : listEntity.CompanyLogoFilePath);
            imgmodel.Type = String.IsNullOrEmpty(listEntity.CompanyLogoFilePath) ? "video" : "image";
            imgmodel.FileName = (imgmodel.url);
            return new ListingDetailModel
            {
                ListingId = listEntity.ListingId,
                PlayerId = listEntity.PlayerId,
                CompanyEmailId = listEntity.CompanyEmailId,
                CompanyLogoFileName = listEntity.CompanyLogoFileName,
                CompanyName = listEntity.CompanyName,
                Description = Regex.Replace(listEntity.Description, @"\t|\n|\r", ""),
                CompanyLogoFilePath = imgmodel.FileName,
                CompanyWebSite = listEntity.CompanyWebSite,
                CompanyMobile = listEntity.CompanyMobile,
                CompanyPhone = listEntity.CompanyPhone,
                IsGlobal = listEntity.IsGlobal,
                NationId = listEntity.NationId,
                NationName = listEntity.NationName,
                StateId = listEntity.StateId,
                StateName = listEntity.StateName,
                CityId = listEntity.CityId,
                CityName = listEntity.CityName,
                CategoryId = listEntity.CategoryId,
                CategoryName = listEntity.CategoryName,
                SubCategoryId = listEntity.SubCategoryId,
                SubcategoryName = listEntity.SubcategoryName,
                StartDate = listEntity.StartDate,
                EndDate = listEntity.EndDate,
                FinalPrice = listEntity.FinalPrice,
                Position = listEntity.Position,
                ContactPersonEmailId = listEntity.ContactPersonEmailId,
                ContactPersonName = listEntity.ContactPersonName,
                ContactPersonMobile = listEntity.ContactPersonMobile,
            };
        }
        /// <summary>
        /// Method is used to get To20ListingDetail by playerId
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public List<ListingLogoDetailsModel> GetTop20ListingDetailByplayerId(long playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var listdetailEntity = new List<ListingLogoDetailsModel>();
            var listEntity = (from list in context.ListingDetails
                              join nation in context.Countries on list.NationId equals nation.CountryId
                              join ct in context.Cities on list.CityId equals ct.CityId
                              join st in context.States on list.StateId equals st.StateId
                              join cat in context.Categories on list.CategoryId equals cat.CategoryId
                              join subCat in context.SubCategories on list.SubCategoryId equals subCat.SubCategoryId
                              join user in context.UserMasters on list.CreatedBy equals (int)user.UserId
                              where list.IsDeleted == false && list.PlayerId == playerId
                              orderby list.UpdatedDate descending

                              select new ListingDetailModel
                              {
                                  ListingId = list.ListingId,
                                  PlayerId = list.PlayerId,
                                  RoleId = user.RoleId,
                                  CompanyEmailId = list.CompanyEmailId,
                                  CompanyLogoFileName = list.CompanyLogoFileName,
                                  CompanyName = list.CompanyName,
                                  Description = list.Description,
                                  CompanyLogoFilePath = list.CompanyLogoFilePath,
                                  CompanyWebSite = list.CompanyWebSite,
                                  CompanyMobile = list.CompanyMobile,
                                  CompanyPhone = list.CompanyPhone,
                                  IsGlobal = (list.IsGlobal == null) ? false : list.IsGlobal,
                                  NationId = list.NationId,
                                  NationName = nation.Name,
                                  StateId = list.StateId,
                                  StateName = st.Name,
                                  CityId = list.CityId,
                                  CityName = ct.Name,
                                  CategoryId = list.CategoryId,
                                  CategoryName = cat.Name,
                                  SubCategoryId = list.SubCategoryId,
                                  SubcategoryName = subCat.Name,
                                  StartDate = list.StartDate,
                                  EndDate = list.EndDate,
                                  FinalPrice = (list.FinalPrice == null) ? null : list.FinalPrice.ToString(),
                                  Position = (list.Position == null) ? null : list.Position,
                                  ContactPersonEmailId = list.ContactPersonEmailId,
                                  ContactPersonName = list.ContactPersonName,
                                  ContactPersonMobile = list.ContactPersonMobile,
                                  CreatedBy = list.CreatedBy,
                                  CreatedDate = list.CreatedDate,
                                  UpdatedBy = list.UpdatedBy,
                                  UpdatedDate = list.UpdatedDate,
                              }).ToList();

            if (listEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            var tredingtoptwenty = listEntity.OrderByDescending(x => x.Position).Take(20).ToList();
            tredingtoptwenty.ForEach(item =>
            {
                var imgmodel = new VideoImageModel();
                imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(item.CompanyLogoFilePath) ? item.CompanyLogoFilePath : item.CompanyLogoFilePath);
                imgmodel.Type = String.IsNullOrEmpty(item.CompanyLogoFilePath) ? "video" : "image";
                imgmodel.FileName = (imgmodel.url);

                listdetailEntity.Add(new ListingLogoDetailsModel
                {
                    ListingId = item.ListingId,
                    PlayerId = item.PlayerId,
                    CompanyName = item.CompanyName,
                    Description = Regex.Replace(item.Description, @"\t|\n|\r", ""),
                    CompanyLogoFileName = item.CompanyLogoFileName,
                    CompanyLogoFilePath = imgmodel.FileName,
                    Position = item.Position
                });
            });
            return listdetailEntity;
        }
        /// <summary>
        /// Method is used to get the search data by playerId and title
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="searchData"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MobileSearchDataModel GetMobileSearchDetailByplayerId(long playerId, string searchData, long userId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var mobileSearchDataModel = new MobileSearchDataModel();
            var listEntity = (from list in context.ListingDetails
                              join nation in context.Countries on list.NationId equals nation.CountryId
                              join ct in context.Cities on list.CityId equals ct.CityId
                              join st in context.States on list.StateId equals st.StateId
                              join cat in context.Categories on list.CategoryId equals cat.CategoryId
                              join subCat in context.SubCategories on list.SubCategoryId equals subCat.SubCategoryId
                              join user in context.UserMasters on list.CreatedBy equals (int)user.UserId
                              where list.IsDeleted == false && list.PlayerId == playerId && (list.Category.Name.ToLower().Contains(searchData.ToLower()) || list.CompanyName.ToLower().Contains(searchData.ToLower()))
                              orderby list.UpdatedDate descending

                              select new ListingDetailModel
                              {
                                  ListingId = list.ListingId,
                                  PlayerId = list.PlayerId,
                                  RoleId = user.RoleId,
                                  CompanyEmailId = list.CompanyEmailId,
                                  CompanyLogoFileName = list.CompanyLogoFileName,
                                  CompanyName = list.CompanyName,
                                  Description = list.Description,
                                  CompanyLogoFilePath = list.CompanyLogoFilePath,
                                  CompanyWebSite = list.CompanyWebSite,
                                  CompanyMobile = list.CompanyMobile,
                                  CompanyPhone = list.CompanyPhone,
                                  IsGlobal = (list.IsGlobal == null) ? false : list.IsGlobal,
                                  NationId = list.NationId,
                                  NationName = nation.Name,
                                  StateId = list.StateId,
                                  StateName = st.Name,
                                  CityId = list.CityId,
                                  CityName = ct.Name,
                                  CategoryId = list.CategoryId,
                                  CategoryName = cat.Name,
                                  SubCategoryId = list.SubCategoryId,
                                  SubcategoryName = subCat.Name,
                                  StartDate = list.StartDate,
                                  EndDate = list.EndDate,
                                  FinalPrice = (list.FinalPrice == null) ? null : list.FinalPrice.ToString(),
                                  Position = (list.Position == null) ? null : list.Position,
                                  ContactPersonEmailId = list.ContactPersonEmailId,
                                  ContactPersonName = list.ContactPersonName,
                                  ContactPersonMobile = list.ContactPersonMobile,
                                  CreatedBy = list.CreatedBy,
                                  CreatedDate = list.CreatedDate,
                                  UpdatedBy = list.UpdatedBy,
                                  UpdatedDate = list.UpdatedDate,
                              }).ToList();

            if (listEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            listEntity.ForEach(item =>
            {
                var imgmodel = new VideoImageModel();
                imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(item.CompanyLogoFilePath) ? item.CompanyLogoFilePath : item.CompanyLogoFilePath);
                imgmodel.Type = String.IsNullOrEmpty(item.CompanyLogoFilePath) ? "video" : "image";
                imgmodel.FileName = (imgmodel.url);

                mobileSearchDataModel.listingLogoDetailsModels.Add(new ListingLogoDetailsModel
                {
                    ListingId = item.ListingId,
                    PlayerId = item.PlayerId,
                    CompanyName = item.CompanyName,
                    Description = Regex.Replace(item.Description, @"\t|\n|\r", ""),
                    CompanyLogoFileName = item.CompanyLogoFileName,
                    CompanyLogoFilePath = imgmodel.FileName,
                    Position = item.Position
                });
            });

            var tredingEntity = (from flag in context.AdvContentMappings
                                 join content in context.ContentDetails on flag.ContentId equals content.ContentId
                                 where content.ContentTypeId == ContentTypeHelper.VideoContentTypeId && flag.IsDeleted == false && content.Approved == true && content.PlayerId == playerId
                                 && (flag.Category.Name.ToLower().Contains(searchData.ToLower()) || flag.Content.Title.ToLower().Contains(searchData.ToLower()))
                                 orderby flag.CreatedDate descending
                                 select new
                                 {
                                     flag.ContentId
                                 }).ToList();
            if (tredingEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            var tredinggroupList = tredingEntity.GroupBy(x => new { x.ContentId }).Select(x => new { ContentId = x.Key, count = x.Count() }).OrderByDescending(x => x.count).ToList();
            List<MobileContentData> topList = new List<MobileContentData>();
            foreach (var item in tredinggroupList)
            {
                var topModel = new MobileContentData();
                var contentList = new List<ContentMobileModel>();
                var contentVideo = context.ContentDetails.Include(x => x.Category).FirstOrDefault(x => x.ContentId == item.ContentId.ContentId && x.Approved == true && x.IsDeleted == false);
                if (contentVideo != null)
                {
                    var advertiseContent = (from advcontent in context.AdvContentMappings
                                            join adv in context.AdvContentDetails on advcontent.AdvertiseContentId equals adv.AdvertiseContentId
                                            where advcontent.ContentId == item.ContentId.ContentId && advcontent.IsDeleted == false
                                            select new
                                            {
                                                adv.AdvertiseContentId,
                                                adv.AdvertiseFileName,
                                                adv.AdvertiseFilePath,
                                                adv.Title,
                                                advcontent.ContentId,
                                                advcontent.Position,
                                            }).ToList();
                    var imgmodel = new VideoImageModel();
                    if (contentVideo.ContentFileName != null)
                    {
                        var content1 = new ContentMobileModel();
                        imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(contentVideo.ContentFilePath) ? contentVideo.ContentFilePath : contentVideo.ContentFilePath);
                        imgmodel.Type = String.IsNullOrEmpty(contentVideo.ContentFilePath) ? "video" : "image";
                        imgmodel.FileName = (imgmodel.url);

                        content1.ContentId = contentVideo.ContentId;
                        content1.FileName = contentVideo.ContentFileName;
                        content1.FilePath = contentVideo.ContentFilePath;
                        content1.Title = contentVideo.Title;
                        content1.Description = contentVideo.Description;
                        content1.Position = Helper.FirstHalfContentPostion;
                        content1.CategoryName = contentVideo.Category.Name;
                        content1.Thumbnail = ThumbnailPath(imgmodel.url);
                        contentList.Add(content1);
                    }
                    if (contentVideo.ContentFileName1 != null)
                    {
                        var content2 = new ContentMobileModel();
                        var imgmodel1 = new VideoImageModel();
                        imgmodel1.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(contentVideo.ContentFilePath1) ? contentVideo.ContentFilePath1 : contentVideo.ContentFilePath1);
                        imgmodel1.Type = String.IsNullOrEmpty(contentVideo.ContentFilePath1) ? "video" : "image";
                        imgmodel1.FileName = (imgmodel1.url);

                        content2.ContentId = contentVideo.ContentId;
                        content2.FileName = contentVideo.ContentFileName1;
                        content2.FilePath = contentVideo.ContentFilePath1;
                        content2.Title = contentVideo.Title;
                        content2.Description = contentVideo.Description;
                        content2.Position = Helper.SecondHalfContentPostion;
                        content2.CategoryName = contentVideo.Category.Name;
                        content2.Thumbnail = ThumbnailPath(imgmodel1.url);
                        contentList.Add(content2);
                    }
                    foreach (var advcontent in advertiseContent)
                    {
                        var model = new ContentMobileModel();
                        var imgmodel2 = new VideoImageModel();
                        imgmodel2.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(advcontent.AdvertiseFilePath) ? advcontent.AdvertiseFilePath : advcontent.AdvertiseFilePath);
                        imgmodel2.Type = String.IsNullOrEmpty(advcontent.AdvertiseFilePath) ? "video" : "image";
                        imgmodel2.FileName = (imgmodel2.url);
                        model.AdvertiseContentId = advcontent.AdvertiseContentId;
                        model.FileName = advcontent.AdvertiseFileName;
                        model.FilePath = advcontent.AdvertiseFilePath;
                        model.Title = advcontent.Title;
                        model.Thumbnail = imgmodel2.url;
                        model.ContentId = advcontent.ContentId;
                        model.Position = advcontent.Position;
                        contentList.Add(model);
                    }
                    topModel.contentMobileModels = contentList.OrderBy(x => x.Position).ToList();
                    topModel.ContentId = contentVideo.ContentId;
                    topModel.Title = contentVideo.Title;
                    topModel.Description = contentVideo.Description;
                    topModel.Thumbnail = ThumbnailPath(imgmodel.url);
                    topModel.CategoryId = contentVideo.CategoryId;
                    topModel.ViewNo = context.ContentViews.Where(x => x.ContentId == item.ContentId.ContentId && x.Trending == true).Select(x => x.Trending).Count();
                    topModel.ContentTypeId = contentVideo.ContentTypeId;
                    topModel.LikedNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId.ContentId && x.MostLiked == true).Select(x => x.MostLiked).Count();
                    topModel.FavouriteNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId.ContentId && x.Favourite == true).Select(x => x.Favourite).Count();
                    topModel.CommentCount = context.Comments.Where(x => x.ContentId == item.ContentId.ContentId).Select(x => x.CommentId).Count();
                    topModel.Liked = context.ContentFlags.Where(x => x.ContentId == item.ContentId.ContentId && x.MostLiked == true && x.UserId == userId).Select(x => x.MostLiked).Distinct().Count() >= 1 ? true : false;
                    topModel.Favourite = context.ContentFlags.Where(x => x.ContentId == item.ContentId.ContentId && x.Favourite == true && x.UserId == userId).Select(x => x.Favourite).Distinct().Count() >= 1 ? true : false;
                    topModel.CreatedDate = contentVideo.CreatedDate;
                    topList.Add(topModel);
                }
            }
            mobileSearchDataModel.mobileContentDatas = topList;
            List<MobileArticleCategoryData> mobileArticleList = new List<MobileArticleCategoryData>();
            var categoryEnitityList = (from content in context.ContentDetails
                                       where content.ContentTypeId == ContentTypeHelper.ArticleContentTypeId && content.IsDeleted == false && content.Approved == true && content.PlayerId == playerId
                                       && (content.Category.Name.ToLower().Contains(searchData.ToLower()) || content.Title.ToLower().Contains(searchData.ToLower()))
                                       orderby content.UpdatedDate descending
                                       select new
                                       {
                                           content.ContentId,
                                           content.CategoryId,
                                           content.Category.Name,
                                           content.Category.DisplayOrder
                                       }).ToList();
            var categoryList = categoryEnitityList.GroupBy(x => new { x.ContentId, x.CategoryId, x.Name, x.DisplayOrder }).Select(x => new { ContentId = x.Key.ContentId, CategoryId = x.Key.CategoryId, Name = x.Key.Name, DisplayOrder = x.Key.DisplayOrder, count = x.Count() }).ToList();
            if (categoryEnitityList.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            List<CategoryArticleModel> categoryArticleList = new List<CategoryArticleModel>();
            foreach (var item in categoryList)
            {
                var contentEntity = categoryEnitityList.Where(x => x.CategoryId == item.CategoryId).ToList();
                foreach (var item1 in contentEntity)
                {
                    var contentArticle = context.ContentDetails.FirstOrDefault(x => x.ContentId == item1.ContentId && x.Approved == true && x.IsDeleted == false);
                    if (contentArticle != null)
                    {
                        CategoryArticleModel articleModel = new CategoryArticleModel();
                        var imgmodel2 = new VideoImageModel();
                        imgmodel2.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(contentArticle.ContentFilePath1) ? contentArticle.ContentFilePath1 : contentArticle.ContentFilePath1);
                        imgmodel2.Type = String.IsNullOrEmpty(contentArticle.ContentFilePath1) ? "video" : "image";
                        imgmodel2.FileName = (imgmodel2.url);
                        articleModel.ContentId = contentArticle.ContentId;
                        articleModel.Title = contentArticle.Title;
                        articleModel.Description = contentArticle.Description;
                        articleModel.CategoryName = contentArticle.Category.Name;
                        articleModel.FileName = contentArticle.ContentFileName1;
                        articleModel.FilePath = imgmodel2.url;
                        articleModel.ViewNo = context.ContentViews.Where(x => x.ContentId == item1.ContentId && x.Trending == true).Select(x => x.Trending).Count();
                        articleModel.ContentTypeId = contentArticle.ContentTypeId;
                        articleModel.LikedNo = context.ContentFlags.Where(x => x.ContentId == item1.ContentId && x.MostLiked == true).Select(x => x.MostLiked).Count();
                        articleModel.FavouriteNo = context.ContentFlags.Where(x => x.ContentId == item1.ContentId && x.Favourite == true).Select(x => x.Favourite).Count();
                        articleModel.CommentCount = context.Comments.Where(x => x.ContentId == item1.ContentId).Select(x => x.CommentId).Count();
                        articleModel.Liked = context.ContentFlags.Where(x => x.ContentId == item1.ContentId && x.MostLiked == true && x.UserId == userId).Select(x => x.MostLiked).Distinct().Count() >= 1 ? true : false;
                        articleModel.Favourite = context.ContentFlags.Where(x => x.ContentId == item1.ContentId && x.Favourite == true && x.UserId == userId).Select(x => x.Favourite).Distinct().Count() >= 1 ? true : false;
                        articleModel.CreatedDate = contentArticle.CreatedDate;
                        categoryArticleList.Add(articleModel);
                    }
                }
            }
            mobileSearchDataModel.categoryArticleModels = categoryArticleList;
            listEntity.ForEach(item =>
            {
                var imgmodel = new VideoImageModel();
                imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(item.CompanyLogoFilePath) ? item.CompanyLogoFilePath : item.CompanyLogoFilePath);
                imgmodel.Type = String.IsNullOrEmpty(item.CompanyLogoFilePath) ? "video" : "image";
                imgmodel.FileName = (imgmodel.url);

                mobileSearchDataModel.listingLogoDetailsModels.Add(new ListingLogoDetailsModel
                {
                    ListingId = item.ListingId,
                    PlayerId = item.PlayerId,
                    CompanyName = item.CompanyName,
                    Description = Regex.Replace(item.Description, @"\t|\n|\r", ""),
                    CompanyLogoFileName = item.CompanyLogoFileName,
                    CompanyLogoFilePath = imgmodel.FileName,
                    Position = item.Position
                });
            });
            return mobileSearchDataModel;
        }
        /// <summary>
        /// Method is used to add/edit comment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string AddMobileContentComment(ContentCommentModel1 model, ref ErrorResponseModel errorResponseModel)
        {
            string message = "";
            if (model.CommentId == 0)
            {
                Comment comment = new Comment();
                comment.UserId = model.UserId;
                comment.ContentId = model.ContentId;
                comment.ContentTypeId = model.ContentTypeId;
                comment.DeviceId = model.DeviceId;
                comment.Location = model.Location;
                comment.Liked = model.Liked;
                comment.Comment1 = model.Comment1;
                comment.Shared = false;
                comment.CreatedBy = model.CreatedBy;
                comment.CreatedDate = DateTime.Now;
                comment.UpdatedDate = DateTime.Now;
                comment.IsDeleted = false;
                comment.ParentCommentId = null;
                comment.IsPublic = false;
                context.Comments.Add(comment);
                context.SaveChanges();
                message = GlobalConstants.CommentSaveSuccessfully;
                var userAuditLog = new UserAuditLogModel();
                userAuditLog.Action = " Add Mobile Content Comment Details";
                userAuditLog.Description = "Mobile Content Comment Details Added";
                userAuditLog.UserId = (int)model.CreatedBy;
                userAuditLog.CreatedBy = model.CreatedBy;
                userAuditLog.CreatedDate = DateTime.Now;
                _userAuditLogService.AddUserAuditLog(userAuditLog);
            }
            else
            {
                var commentEntity = context.Comments.FirstOrDefault(x => x.CommentId == model.CommentId);
                if (commentEntity != null)
                {
                    commentEntity.CommentId = model.CommentId;
                    commentEntity.UserId = model.UserId;
                    commentEntity.ContentId = model.ContentId;
                    commentEntity.ContentTypeId = model.ContentTypeId;
                    commentEntity.DeviceId = model.DeviceId;
                    commentEntity.Location = model.Location;
                    commentEntity.Liked = false;
                    commentEntity.Comment1 = model.Comment1;
                    commentEntity.Shared = false;
                    commentEntity.UpdatedBy = model.UpdatedBy;
                    commentEntity.UpdatedDate = DateTime.Now;
                    commentEntity.IsDeleted = false;
                    commentEntity.ParentCommentId = null;
                    commentEntity.IsPublic = model.IsPublic;
                    context.Comments.Update(commentEntity);
                    context.SaveChanges();
                    message = GlobalConstants.CommentUpdateSuccessfully;
                    var userAuditLog = new UserAuditLogModel();
                    userAuditLog.Action = " Update Content Comment Details";
                    userAuditLog.Description = " Content Comment Details Updated";
                    userAuditLog.UserId = (int)model.CreatedBy;
                    userAuditLog.CreatedBy = model.CreatedBy;
                    userAuditLog.CreatedDate = DateTime.Now;
                    _userAuditLogService.AddUserAuditLog(userAuditLog);
                }
            }
            return message;
        }


        /// <summary>
        /// Method is used to get contentcomment by contentId
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        public CommentListData GetMobileCommentByContentId(long contentId, long UserId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            List<ContentCommentModelData> commentModelList = new List<ContentCommentModelData>();
            List<ContentCommentModel> ContentcommentList = new List<ContentCommentModel>();
            CommentListData commentListData = new CommentListData();
            commentListData.commentCount = 0;
            var PubliccommentEntity = (from comment in context.Comments
                                 join content in context.ContentDetails on comment.ContentId equals content.ContentId
                                 join user in context.UserMasters on comment.UserId equals user.UserId
                                 join contenttype in context.ContentTypes on comment.ContentTypeId equals contenttype.ContentTypeId
                                 join player in context.PlayerDetails on content.PlayerId equals player.PlayerId
                                 where comment.ContentId == contentId && comment.IsDeleted == false
                                 && comment.IsPublic == true
                                 orderby comment.CreatedDate descending
                                 select new ContentCommentModel
                                 {
                                     CommentId = comment.CommentId,
                                     ContentId = comment.ContentId,
                                     UserId = comment.UserId,
                                     EmailId = user.EmailId,
                                     FirstName = user.FirstName,
                                     LastName = user.LastName,
                                     FullName = user.FirstName + " " + user.LastName,
                                     DeviceId = comment.DeviceId,
                                     Location = comment.Location,
                                     Liked = comment.Liked,
                                     Comment1 = comment.Comment1,
                                     Shared = comment.Shared,
                                     IsPublic = comment.IsPublic,
                                     CreatedBy = comment.CreatedBy,
                                     CreatedDate = comment.CreatedDate,
                                     ParentCommentId = comment.ParentCommentId,
                                     ContentTypeId = content.ContentTypeId,
                                     ContentTypeName = contenttype.ContentName,
                                     UpdatedDate = comment.UpdatedDate,
                                     UpdatedBy = comment.UpdatedBy,
                                 }).ToList();

            var PrivatecommentEntity = (from comment in context.Comments
                                        join content in context.ContentDetails on comment.ContentId equals content.ContentId
                                        join user in context.UserMasters on comment.UserId equals user.UserId
                                        join contenttype in context.ContentTypes on comment.ContentTypeId equals contenttype.ContentTypeId
                                        join player in context.PlayerDetails on content.PlayerId equals player.PlayerId
                                        where comment.ContentId == contentId && comment.IsDeleted == false
                                        && comment.IsPublic == false && comment.UserId == UserId
                                        orderby comment.CreatedDate descending
                                        select new ContentCommentModel
                                        {
                                            CommentId = comment.CommentId,
                                            ContentId = comment.ContentId,
                                            UserId = comment.UserId,
                                            EmailId = user.EmailId,
                                            FirstName = user.FirstName,
                                            LastName = user.LastName,
                                            FullName = user.FirstName + " " + user.LastName,
                                            DeviceId = comment.DeviceId,
                                            Location = comment.Location,
                                            Liked = comment.Liked,
                                            Comment1 = comment.Comment1,
                                            Shared = comment.Shared,
                                            IsPublic = comment.IsPublic,
                                            CreatedBy = comment.CreatedBy,
                                            CreatedDate = comment.CreatedDate,
                                            ParentCommentId = comment.ParentCommentId,
                                            ContentTypeId = content.ContentTypeId,
                                            ContentTypeName = contenttype.ContentName,
                                            UpdatedDate = comment.UpdatedDate,
                                            UpdatedBy = comment.UpdatedBy,
                                        }).ToList();

            if (PubliccommentEntity.Count == 0 && PrivatecommentEntity.Count==0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return new CommentListData();
            }

            ContentcommentList.AddRange(PubliccommentEntity.ToList());
            ContentcommentList.AddRange(PrivatecommentEntity.ToList());
            ContentcommentList = ContentcommentList.OrderByDescending(x => x.CreatedDate).ToList();

            foreach (var item in ContentcommentList)
            {

                ContentCommentModelData modelData = new ContentCommentModelData();
                if (item.CommentId != null && item.ParentCommentId == null)
                {
                    modelData.CommentId = item.CommentId;
                    List<ContentCommentModel> commentList = new List<ContentCommentModel>();
                    var model = new ContentCommentModel();
                    model.CommentId = item.CommentId;
                    model.ContentId = item.ContentId;
                    model.UserId = item.UserId;
                    model.Comment1 = item.Comment1;
                    model.ParentCommentId = item.ParentCommentId;
                    model.DeviceId = item.DeviceId;
                    model.Location = item.Location;
                    model.Liked = item.Liked;
                    model.Shared = item.Shared;
                    model.IsPublic = item.IsPublic;
                    model.EmailId = item.EmailId;
                    model.CreatedDate = item.CreatedDate;
                    model.CreatedBy = item.CreatedBy;
                    model.FirstName = item.FirstName;
                    model.LastName = item.LastName;
                    model.FullName = item.FirstName + " " + item.LastName;
                    model.ContentTypeId = item.ContentTypeId;
                    model.ContentTypeName = item.ContentTypeName;
                    model.UpdatedBy = item.UpdatedBy;
                    model.UpdatedDate = item.UpdatedDate;
                    commentList.Add(model);
                    var commentreplyEntity = (from comment in context.Comments
                                              join content in context.ContentDetails on comment.ContentId equals content.ContentId
                                              join user in context.UserMasters on comment.UserId equals user.UserId
                                              join contenttype in context.ContentTypes on comment.ContentTypeId equals contenttype.ContentTypeId
                                              join player in context.PlayerDetails on content.PlayerId equals player.PlayerId
                                              where comment.ContentId == contentId && comment.IsDeleted == false
                                              && comment.ParentCommentId == item.CommentId

                                              select new ContentCommentModel
                                              {
                                                  CommentId = comment.CommentId,
                                                  ContentId = comment.ContentId,
                                                  UserId = comment.UserId,
                                                  EmailId = user.EmailId,
                                                  FirstName = user.FirstName,
                                                  ReplyedAdminName = "IM" + player.FirstName,
                                                  LastName = user.LastName,
                                                  FullName = user.FirstName + " " + user.LastName,
                                                  DeviceId = comment.DeviceId,
                                                  Location = comment.Location,
                                                  CommentedUserId = model.UserId,
                                                  Liked = comment.Liked,
                                                  Comment1 = comment.Comment1,
                                                  Shared = comment.Shared,
                                                  IsPublic = comment.IsPublic,
                                                  CreatedBy = comment.CreatedBy,
                                                  CreatedDate = comment.CreatedDate,
                                                  ParentCommentId = comment.ParentCommentId,
                                                  ContentTypeId = content.ContentTypeId,
                                                  ContentTypeName = contenttype.ContentName,
                                                  UpdatedDate = comment.UpdatedDate,
                                                  UpdatedBy = comment.UpdatedBy,
                                              }).ToList();
                    // var commentreplyEntity = context.Comments.FirstOrDefault(x => x.ParentCommentId == item.CommentId);
                    if (commentreplyEntity.Count != 0)
                    {
                        foreach (var item1 in commentreplyEntity)
                        {
                            var model1 = new ContentCommentModel();
                            model1.CommentId = item1.CommentId;
                            model1.ContentId = item1.ContentId;
                            model1.UserId = item1.UserId;
                            model1.Comment1 = item1.Comment1;
                            model1.ParentCommentId = item1.ParentCommentId;
                            model1.DeviceId = item1.DeviceId;
                            model1.Location = item1.Location;
                            model1.Liked = item1.Liked;
                            model1.ReplyedAdminName = item1.ReplyedAdminName;
                            model1.CommentedUserId = item1.CommentedUserId;
                            model1.Shared = item1.Shared;
                            model1.IsPublic = item1.IsPublic;
                            model1.EmailId = item1.EmailId;
                            model1.CreatedDate = item1.CreatedDate;
                            model1.CreatedBy = item1.CreatedBy;
                            model1.FirstName = item1.FirstName;
                            model1.LastName = item1.LastName;
                            model1.FullName = item1.FirstName + " " + item1.LastName;
                            model1.ContentTypeId = item1.ContentTypeId;
                            model1.ContentTypeName = item1.ContentTypeName;
                            model1.UpdatedBy = item1.UpdatedBy;
                            model1.UpdatedDate = item1.UpdatedDate;
                            commentList.Add(model1);
                        }
                    }
                    modelData.contentCommentModels = commentList;
                    modelData.CommentCount = commentList.Count();
                    commentListData.commentCount = modelData.CommentCount + commentListData.commentCount;
                }

                commentModelList.Add(modelData);
            }
            List<ContentCommentModelData> commentModelList1 = new List<ContentCommentModelData>();
            commentModelList1 = commentModelList.Where(x => x.CommentId != 0).ToList();
            commentListData.lstdatamodel = commentModelList1;
            return commentListData;
        }

        /// <summary>
        /// Method is used to get contentcomment by playerId
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public List<ContentCommentModel> GetMobileCommentByPlayerId(long playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var contentList = new List<ContentCommentModel>();
            var contentEntity = (from comment in context.Comments
                                 join content in context.ContentDetails on comment.ContentId equals content.ContentId
                                 join user in context.UserMasters on comment.UserId equals user.UserId
                                 join contenttype in context.ContentTypes on comment.ContentTypeId equals contenttype.ContentTypeId
                                 join player in context.PlayerDetails on content.PlayerId equals player.PlayerId
                                 where content.PlayerId == playerId && comment.IsDeleted == false && comment.IsPublic == true
                                 orderby comment.CreatedDate descending
                                 select new ContentCommentModel
                                 {
                                     CommentId = comment.CommentId,
                                     ContentId = comment.ContentId,
                                     UserId = comment.UserId,
                                     EmailId = user.EmailId,
                                     FirstName = user.FirstName,
                                     LastName = user.LastName,
                                     FullName = user.FirstName + " " + user.LastName,
                                     DeviceId = comment.DeviceId,
                                     Location = comment.Location,
                                     Liked = comment.Liked,
                                     Comment1 = comment.Comment1,
                                     Shared = comment.Shared,
                                     IsPublic = comment.IsPublic,
                                     CreatedBy = comment.CreatedBy,
                                     CreatedDate = comment.CreatedDate,
                                     ParentCommentId = comment.ParentCommentId,
                                     ContentTypeId = content.ContentTypeId,
                                     ContentTypeName = contenttype.ContentName,
                                     UpdatedDate = comment.UpdatedDate,
                                     UpdatedBy = comment.UpdatedBy,
                                 }).ToList();
            if (contentEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return new List<ContentCommentModel>();
            }
            contentEntity.ForEach(item =>
            {
                contentList.Add(new ContentCommentModel
                {
                    CommentId = item.CommentId,
                    ContentId = item.ContentId,
                    UserId = item.UserId,
                    Comment1 = item.Comment1,
                    ParentCommentId = item.ParentCommentId,
                    DeviceId = item.DeviceId,
                    Location = item.Location,
                    Liked = item.Liked,
                    Shared = item.Shared,
                    IsPublic = item.IsPublic,
                    EmailId = item.EmailId,
                    CreatedDate = item.CreatedDate,
                    CreatedBy = item.CreatedBy,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    FullName = item.FirstName + " " + item.LastName,
                    ContentTypeId = item.ContentTypeId,
                    ContentTypeName = item.ContentTypeName,
                    UpdatedBy = item.UpdatedBy,
                    UpdatedDate = item.UpdatedDate,
                });
            });
            return contentList;
        }

        /// <summary>
        /// Method is used to add view flag
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string AddMobileViewCount(ContentModelView model, ref ErrorResponseModel errorResponseModel)
        {
            string message = "";
            if (model.ContentViewId == 0)
            {
                ContentView content = new ContentView();
                content.PlayerId = model.PlayerId;
                content.ContentId = model.ContentId;
                content.ContentTypeId = model.ContentTypeId;
                content.Trending = model.Trending;
                content.CreatedBy = model.CreatedBy;
                content.CreatedDate = DateTime.Now;
                content.UpdatedDate = DateTime.Now;
                content.IsDeleted = false;
                context.ContentViews.Add(content);
                context.SaveChanges();
                message = GlobalConstants.MobileUserSaveMessage;
            }
            return message;
        }

        /// <summary>
        /// Method is used to add like flag
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string AddMobileLikeFavouriteCount(ContentModelFlag model, ref ErrorResponseModel errorResponseModel)
        {
            string message = "";
            var contentEntity = context.ContentFlags.FirstOrDefault(x => x.PlayerId == model.PlayerId && x.ContentId == model.ContentId && x.UserId == model.UserId && x.IsDeleted == false);
            if (contentEntity == null)
            {
                ContentFlag contentFlag = new ContentFlag();
                contentFlag.PlayerId = model.PlayerId;
                contentFlag.ContentId = model.ContentId;
                contentFlag.UserId = model.UserId;
                contentFlag.ContentTypeId = model.ContentTypeId;
                contentFlag.CreatedBy = model.CreatedBy;
                contentFlag.CreatedDate = DateTime.Now;
                contentFlag.IsDeleted = false;
                contentFlag.MostLiked = model.MostLiked;
                contentFlag.Favourite = model.Favourite;
                context.ContentFlags.Add(contentFlag);
                context.SaveChanges();
                message = GlobalConstants.SaveMessage;
            }
            else
            {
                var contentFlagEntity = context.ContentFlags.FirstOrDefault(x => x.PlayerId == model.PlayerId && x.ContentId == model.ContentId && x.UserId == model.UserId);
                if (contentFlagEntity != null)
                {
                    contentFlagEntity.PlayerId = model.PlayerId;
                    contentFlagEntity.ContentId = model.ContentId;
                    contentFlagEntity.ContentTypeId = model.ContentTypeId;
                    contentFlagEntity.CreatedBy = model.CreatedBy;
                    contentFlagEntity.CreatedDate = DateTime.Now;
                    contentFlagEntity.IsDeleted = false;
                    contentFlagEntity.UserId = model.UserId;
                    contentFlagEntity.MostLiked = model.MostLiked;
                    contentFlagEntity.Favourite = model.Favourite;
                    context.ContentFlags.Update(contentFlagEntity);
                    context.SaveChanges();
                    message = GlobalConstants.SaveMessage;
                }
            }
            return message;
        }

        /// <summary>
        /// Method is used to get splash screen by playerid
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public PlayerMobileDataModel GetMobileSplashScreenByplayerId(long playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var playerSplashEntity = (from content in context.PlayerData
                                      where content.PlayerId == playerId && content.IsDeleted == false && content.FileCategoryTypeId == FileCategoryTypeHelper.SplashScreenTypeId
                                      select new
                                      {
                                          content.PlayerDataId,
                                          content.PlayerId,
                                          content.FileName,
                                          content.FilePath
                                      }).FirstOrDefault();
            if (playerSplashEntity == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return new PlayerMobileDataModel();
            }

            var imgmodel = new VideoImageModel();
            imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(playerSplashEntity.FilePath) ? playerSplashEntity.FilePath : playerSplashEntity.FilePath);
            imgmodel.Type = String.IsNullOrEmpty(playerSplashEntity.FilePath) ? "video" : "image";
            imgmodel.thumbnail = ThumbnailPath(imgmodel.url);
            return new PlayerMobileDataModel
            {
                PlayerDataId = playerSplashEntity.PlayerDataId,
                PlayerId = playerSplashEntity.PlayerId,
                FilePath = imgmodel.url,
                FileName = playerSplashEntity.FileName
            };
        }
        /// <summary>
        /// Method is used to get logo image by playerid
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public PlayerMobileDataModel GetMobileLogoImageByplayerId(long playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var playerLogoEntity = (from content in context.PlayerData
                                    where content.PlayerId == playerId && content.IsDeleted == false && content.FileCategoryTypeId == FileCategoryTypeHelper.LogoTypeId
                                    select new
                                    {
                                        content.PlayerDataId,
                                        content.PlayerId,
                                        content.FileName,
                                        content.FilePath
                                    }).FirstOrDefault();
            if (playerLogoEntity == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return new PlayerMobileDataModel();
            }

            var imgmodel = new VideoImageModel();
            imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(playerLogoEntity.FilePath) ? playerLogoEntity.FilePath : playerLogoEntity.FilePath);
            imgmodel.Type = String.IsNullOrEmpty(playerLogoEntity.FilePath) ? "video" : "image";
            imgmodel.thumbnail = imgmodel.url;
            return new PlayerMobileDataModel
            {
                PlayerDataId = playerLogoEntity.PlayerDataId,
                PlayerId = playerLogoEntity.PlayerId,
                FilePath = imgmodel.thumbnail,
                FileName = playerLogoEntity.FileName
            };
        }
        /// <summary>
        /// Method is used to get slide image by playerid
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<PlayerMobileDataModel> GetMobileSlideImageByplayerId(long playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            List<PlayerMobileDataModel> playerSlideModelList = new List<PlayerMobileDataModel>();
            var playerSlideImageEntity = (from content in context.PlayerData
                                          where content.PlayerId == playerId && content.IsDeleted == false && content.FileCategoryTypeId == FileCategoryTypeHelper.SlideImageTypeId
                                          select new
                                          {
                                              content.PlayerDataId,
                                              content.PlayerId,
                                              content.FileName,
                                              content.FilePath
                                          }).ToList();
            if (playerSlideImageEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return new List<PlayerMobileDataModel>();
            }
            foreach (var item in playerSlideImageEntity)
            {
                var imgmodel = new VideoImageModel();
                imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(item.FilePath) ? item.FilePath : item.FilePath);
                imgmodel.Type = String.IsNullOrEmpty(item.FilePath) ? "video" : "image";
                imgmodel.thumbnail = imgmodel.url;
                var model = new PlayerMobileDataModel();
                model.PlayerDataId = item.PlayerDataId;
                model.PlayerId = item.PlayerId;
                model.FilePath = imgmodel.thumbnail;
                model.FileName = item.FileName;
                playerSlideModelList.Add(model);
            }
            return playerSlideModelList;
        }

        /// <summary>
        /// Method is used to get the liked data by playerId and userId
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PlayerMobileLikeFavouriteData GetMobileLikeVideoArticle(long playerId, long userId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            PlayerMobileLikeFavouriteData playerMobileLikeFavouriteData = new PlayerMobileLikeFavouriteData();
            var likedVideoEntity = (from flag in context.ContentFlags
                                    join player in context.PlayerDetails on flag.PlayerId equals player.PlayerId
                                    join content in context.ContentDetails on flag.ContentId equals content.ContentId
                                    join type in context.ContentTypes on flag.ContentTypeId equals type.ContentTypeId
                                    where flag.IsDeleted == false && flag.MostLiked == true && flag.ContentTypeId == ContentTypeHelper.VideoContentTypeId && flag.PlayerId == playerId && content.Approved == true && flag.UserId == userId
                                    orderby flag.CreatedDate descending
                                    select new
                                    {
                                        content.ContentId
                                    }).ToList();

            List<MobileContentData> topList = new List<MobileContentData>();
            foreach (var item in likedVideoEntity)
            {
                var topModel = new MobileContentData();
                var contentList = new List<ContentMobileModel>();
                var advertiseContent = (from advcontent in context.AdvContentMappings
                                        join adv in context.AdvContentDetails on advcontent.AdvertiseContentId equals adv.AdvertiseContentId
                                        where advcontent.ContentId == item.ContentId && advcontent.IsDeleted == false
                                        select new
                                        {
                                            adv.AdvertiseContentId,
                                            adv.AdvertiseFileName,
                                            adv.AdvertiseFilePath,
                                            adv.Title,
                                            advcontent.ContentId,
                                            advcontent.Position,
                                        }).ToList();
                var contentVideo = context.ContentDetails.Include(x => x.Category).FirstOrDefault(x => x.ContentId == item.ContentId && x.IsDeleted == false);
                var imgmodel = new VideoImageModel();
                if (contentVideo.ContentFileName != null)
                {
                    var content1 = new ContentMobileModel();
                    imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(contentVideo.ContentFilePath) ? contentVideo.ContentFilePath : contentVideo.ContentFilePath);
                    imgmodel.Type = String.IsNullOrEmpty(contentVideo.ContentFilePath) ? "video" : "image";
                    imgmodel.FileName = (imgmodel.url);

                    content1.ContentId = contentVideo.ContentId;
                    content1.FileName = contentVideo.ContentFileName;
                    content1.FilePath = contentVideo.ContentFilePath;
                    content1.Title = contentVideo.Title;
                    content1.Description = contentVideo.Description;
                    content1.Position = Helper.FirstHalfContentPostion;
                    content1.CategoryName = contentVideo.Category.Name;
                    content1.Thumbnail = ThumbnailPath(imgmodel.url);
                    contentList.Add(content1);
                }
                if (contentVideo.ContentFileName1 != null)
                {
                    var content2 = new ContentMobileModel();
                    var imgmodel1 = new VideoImageModel();
                    imgmodel1.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(contentVideo.ContentFilePath1) ? contentVideo.ContentFilePath1 : contentVideo.ContentFilePath1);
                    imgmodel1.Type = String.IsNullOrEmpty(contentVideo.ContentFilePath1) ? "video" : "image";
                    imgmodel1.FileName = (imgmodel1.url);

                    content2.ContentId = contentVideo.ContentId;
                    content2.FileName = contentVideo.ContentFileName1;
                    content2.FilePath = contentVideo.ContentFilePath1;
                    content2.Title = contentVideo.Title;
                    content2.Description = contentVideo.Description;
                    content2.Position = Helper.SecondHalfContentPostion;
                    content2.CategoryName = contentVideo.Category.Name;
                    content2.Thumbnail = ThumbnailPath(imgmodel1.url);
                    contentList.Add(content2);
                }
                foreach (var advcontent in advertiseContent)
                {
                    var model = new ContentMobileModel();
                    var imgmodel2 = new VideoImageModel();
                    imgmodel2.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(advcontent.AdvertiseFilePath) ? advcontent.AdvertiseFilePath : advcontent.AdvertiseFilePath);
                    imgmodel2.Type = String.IsNullOrEmpty(advcontent.AdvertiseFilePath) ? "video" : "image";
                    imgmodel2.FileName = (imgmodel2.url);
                    model.AdvertiseContentId = advcontent.AdvertiseContentId;
                    model.FileName = advcontent.AdvertiseFileName;
                    model.FilePath = advcontent.AdvertiseFilePath;
                    model.Title = advcontent.Title;
                    model.Thumbnail = imgmodel2.url;
                    model.ContentId = advcontent.ContentId;
                    model.Position = advcontent.Position;
                    contentList.Add(model);
                }
                topModel.contentMobileModels = contentList.OrderBy(x => x.Position).ToList();
                topModel.ContentId = contentVideo.ContentId;
                topModel.Title = contentVideo.Title;
                topModel.Description = contentVideo.Description;
                topModel.Thumbnail = ThumbnailPath(imgmodel.url);
                topModel.CategoryId = contentVideo.CategoryId;
                topModel.ViewNo = context.ContentViews.Where(x => x.ContentId == item.ContentId && x.Trending == true).Select(x => x.Trending).Count();
                topModel.ContentTypeId = contentVideo.ContentTypeId;
                topModel.LikedNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.MostLiked == true).Select(x => x.MostLiked).Count();
                topModel.FavouriteNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.Favourite == true).Select(x => x.Favourite).Count();
                topModel.CommentCount = context.Comments.Where(x => x.ContentId == item.ContentId).Select(x => x.CommentId).Count();
                topModel.Liked = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.MostLiked == true && x.UserId == userId).Select(x => x.MostLiked).Distinct().Count() >= 1 ? true : false;
                topModel.Favourite = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.Favourite == true && x.UserId == userId).Select(x => x.Favourite).Distinct().Count() >= 1 ? true : false;
                topModel.CreatedDate = contentVideo.CreatedDate;
                topList.Add(topModel);
            }
            playerMobileLikeFavouriteData.videoContentData = topList;
            var likedArticleEntity = (from flag in context.ContentFlags
                                      join player in context.PlayerDetails on flag.PlayerId equals player.PlayerId
                                      join content in context.ContentDetails on flag.ContentId equals content.ContentId
                                      join type in context.ContentTypes on flag.ContentTypeId equals type.ContentTypeId
                                      where flag.IsDeleted == false && flag.MostLiked == true && flag.ContentTypeId == ContentTypeHelper.ArticleContentTypeId && flag.PlayerId == playerId && content.Approved == true && flag.UserId == userId
                                      orderby flag.CreatedDate descending
                                      select new
                                      {
                                          content.ContentId
                                      }).ToList();
            List<CategoryArticleModel> categoryArticleList = new List<CategoryArticleModel>();
            foreach (var item in likedArticleEntity)
            {
                var contentArticle = context.ContentDetails.FirstOrDefault(x => x.ContentId == item.ContentId && x.Approved == true && x.IsDeleted == false);
                if (contentArticle != null)
                {
                    CategoryArticleModel articleModel = new CategoryArticleModel();
                    var imgmodel2 = new VideoImageModel();
                    imgmodel2.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(contentArticle.ContentFilePath1) ? contentArticle.ContentFilePath1 : contentArticle.ContentFilePath1);
                    imgmodel2.Type = String.IsNullOrEmpty(contentArticle.ContentFilePath1) ? "video" : "image";
                    imgmodel2.FileName = (imgmodel2.url);
                    articleModel.ContentId = contentArticle.ContentId;
                    articleModel.Title = contentArticle.Title;
                    articleModel.Description = contentArticle.Description;
                    articleModel.CategoryName = contentArticle.Category.Name;
                    articleModel.FileName = contentArticle.ContentFileName1;
                    articleModel.FilePath = imgmodel2.url;
                    articleModel.ViewNo = context.ContentViews.Where(x => x.ContentId == item.ContentId && x.Trending == true).Select(x => x.Trending).Count();
                    articleModel.ContentTypeId = contentArticle.ContentTypeId;
                    articleModel.LikedNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.MostLiked == true).Select(x => x.MostLiked).Count();
                    articleModel.FavouriteNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.Favourite == true).Select(x => x.Favourite).Count();
                    articleModel.CommentCount = context.Comments.Where(x => x.ContentId == item.ContentId).Select(x => x.CommentId).Count();
                    articleModel.Liked = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.MostLiked == true && x.UserId == userId).Select(x => x.MostLiked).Distinct().Count() >= 1 ? true : false;
                    articleModel.Favourite = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.Favourite == true && x.UserId == userId).Select(x => x.Favourite).Distinct().Count() >= 1 ? true : false;
                    articleModel.CreatedDate = contentArticle.CreatedDate;
                    categoryArticleList.Add(articleModel);
                }
            }
            playerMobileLikeFavouriteData.articleContentData = categoryArticleList;
            if (likedVideoEntity.Count == 0 && likedArticleEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            return playerMobileLikeFavouriteData;
        }

        /// <summary>
        /// Method is used to get the favourite data by playerId and userId
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PlayerMobileLikeFavouriteData GetMobileFavouriteVideoArticle(long playerId, long userId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            PlayerMobileLikeFavouriteData playerMobileLikeFavouriteData = new PlayerMobileLikeFavouriteData();
            var likedVideoEntity = (from flag in context.ContentFlags
                                    join player in context.PlayerDetails on flag.PlayerId equals player.PlayerId
                                    join content in context.ContentDetails on flag.ContentId equals content.ContentId
                                    join type in context.ContentTypes on flag.ContentTypeId equals type.ContentTypeId
                                    where flag.IsDeleted == false && flag.Favourite == true && flag.ContentTypeId == ContentTypeHelper.VideoContentTypeId && flag.PlayerId == playerId && content.Approved == true && flag.UserId == userId
                                    orderby flag.CreatedDate descending
                                    select new
                                    {
                                        content.ContentId
                                    }).ToList();

            List<MobileContentData> topList = new List<MobileContentData>();
            foreach (var item in likedVideoEntity)
            {
                var topModel = new MobileContentData();
                var contentList = new List<ContentMobileModel>();
                var advertiseContent = (from advcontent in context.AdvContentMappings
                                        join adv in context.AdvContentDetails on advcontent.AdvertiseContentId equals adv.AdvertiseContentId
                                        where advcontent.ContentId == item.ContentId && advcontent.IsDeleted == false
                                        select new
                                        {
                                            adv.AdvertiseContentId,
                                            adv.AdvertiseFileName,
                                            adv.AdvertiseFilePath,
                                            adv.Title,
                                            advcontent.ContentId,
                                            advcontent.Position,
                                        }).ToList();
                var contentVideo = context.ContentDetails.Include(x => x.Category).FirstOrDefault(x => x.ContentId == item.ContentId && x.IsDeleted == false);
                var imgmodel = new VideoImageModel();
                if (contentVideo.ContentFileName != null)
                {
                    var content1 = new ContentMobileModel();
                    imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(contentVideo.ContentFilePath) ? contentVideo.ContentFilePath : contentVideo.ContentFilePath);
                    imgmodel.Type = String.IsNullOrEmpty(contentVideo.ContentFilePath) ? "video" : "image";
                    imgmodel.FileName = (imgmodel.url);

                    content1.ContentId = contentVideo.ContentId;
                    content1.FileName = contentVideo.ContentFileName;
                    content1.FilePath = contentVideo.ContentFilePath;
                    content1.Title = contentVideo.Title;
                    content1.Description = contentVideo.Description;
                    content1.Position = Helper.FirstHalfContentPostion;
                    content1.CategoryName = contentVideo.Category.Name;
                    content1.Thumbnail = ThumbnailPath(imgmodel.url);
                    contentList.Add(content1);
                }
                if (contentVideo.ContentFileName1 != null)
                {
                    var content2 = new ContentMobileModel();
                    var imgmodel1 = new VideoImageModel();
                    imgmodel1.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(contentVideo.ContentFilePath1) ? contentVideo.ContentFilePath1 : contentVideo.ContentFilePath1);
                    imgmodel1.Type = String.IsNullOrEmpty(contentVideo.ContentFilePath1) ? "video" : "image";
                    imgmodel1.FileName = (imgmodel1.url);

                    content2.ContentId = contentVideo.ContentId;
                    content2.FileName = contentVideo.ContentFileName1;
                    content2.FilePath = contentVideo.ContentFilePath1;
                    content2.Title = contentVideo.Title;
                    content2.Description = contentVideo.Description;
                    content2.Position = Helper.SecondHalfContentPostion;
                    content2.CategoryName = contentVideo.Category.Name;
                    content2.Thumbnail = ThumbnailPath(imgmodel1.url);
                    contentList.Add(content2);
                }
                foreach (var advcontent in advertiseContent)
                {
                    var model = new ContentMobileModel();
                    var imgmodel2 = new VideoImageModel();
                    imgmodel2.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(advcontent.AdvertiseFilePath) ? advcontent.AdvertiseFilePath : advcontent.AdvertiseFilePath);
                    imgmodel2.Type = String.IsNullOrEmpty(advcontent.AdvertiseFilePath) ? "video" : "image";
                    imgmodel2.FileName = (imgmodel2.url);
                    model.AdvertiseContentId = advcontent.AdvertiseContentId;
                    model.FileName = advcontent.AdvertiseFileName;
                    model.FilePath = advcontent.AdvertiseFilePath;
                    model.Title = advcontent.Title;
                    model.Thumbnail = imgmodel2.url;
                    model.ContentId = advcontent.ContentId;
                    model.Position = advcontent.Position;
                    contentList.Add(model);
                }
                topModel.contentMobileModels = contentList.OrderBy(x => x.Position).ToList();
                topModel.ContentId = contentVideo.ContentId;
                topModel.Title = contentVideo.Title;
                topModel.Description = contentVideo.Description;
                topModel.Thumbnail = ThumbnailPath(imgmodel.url);
                topModel.CategoryId = contentVideo.CategoryId;
                topModel.ViewNo = context.ContentViews.Where(x => x.ContentId == item.ContentId && x.Trending == true).Select(x => x.Trending).Count();
                topModel.ContentTypeId = contentVideo.ContentTypeId;
                topModel.LikedNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.MostLiked == true).Select(x => x.MostLiked).Count();
                topModel.FavouriteNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.Favourite == true).Select(x => x.Favourite).Count();
                topModel.CommentCount = context.Comments.Where(x => x.ContentId == item.ContentId).Select(x => x.CommentId).Count();
                topModel.Liked = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.MostLiked == true && x.UserId == userId).Select(x => x.MostLiked).Distinct().Count() >= 1 ? true : false;
                topModel.Favourite = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.Favourite == true && x.UserId == userId).Select(x => x.Favourite).Distinct().Count() >= 1 ? true : false;
                topModel.CreatedDate = contentVideo.CreatedDate;
                topList.Add(topModel);
            }
            playerMobileLikeFavouriteData.videoContentData = topList;
            var likedArticleEntity = (from flag in context.ContentFlags
                                      join player in context.PlayerDetails on flag.PlayerId equals player.PlayerId
                                      join content in context.ContentDetails on flag.ContentId equals content.ContentId
                                      join type in context.ContentTypes on flag.ContentTypeId equals type.ContentTypeId
                                      where flag.IsDeleted == false && flag.Favourite == true && flag.ContentTypeId == ContentTypeHelper.ArticleContentTypeId && flag.PlayerId == playerId && content.Approved == true && flag.UserId == userId
                                      orderby flag.CreatedDate descending
                                      select new
                                      {
                                          content.ContentId
                                      }).ToList();
            List<CategoryArticleModel> categoryArticleList = new List<CategoryArticleModel>();
            foreach (var item in likedArticleEntity)
            {
                var contentArticle = context.ContentDetails.FirstOrDefault(x => x.ContentId == item.ContentId && x.Approved == true && x.IsDeleted == false);
                if (contentArticle != null)
                {
                    CategoryArticleModel articleModel = new CategoryArticleModel();
                    var imgmodel2 = new VideoImageModel();
                    imgmodel2.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(contentArticle.ContentFilePath1) ? contentArticle.ContentFilePath1 : contentArticle.ContentFilePath1);
                    imgmodel2.Type = String.IsNullOrEmpty(contentArticle.ContentFilePath1) ? "video" : "image";
                    imgmodel2.FileName = (imgmodel2.url);
                    articleModel.ContentId = contentArticle.ContentId;
                    articleModel.Title = contentArticle.Title;
                    articleModel.Description = contentArticle.Description;
                    articleModel.CategoryName = contentArticle.Category.Name;
                    articleModel.FileName = contentArticle.ContentFileName1;
                    articleModel.FilePath = imgmodel2.url;
                    articleModel.ViewNo = context.ContentViews.Where(x => x.ContentId == item.ContentId && x.Trending == true).Select(x => x.Trending).Count();
                    articleModel.ContentTypeId = contentArticle.ContentTypeId;
                    articleModel.LikedNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.MostLiked == true).Select(x => x.MostLiked).Count();
                    articleModel.FavouriteNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.Favourite == true).Select(x => x.Favourite).Count();
                    articleModel.CommentCount = context.Comments.Where(x => x.ContentId == item.ContentId).Select(x => x.CommentId).Count();
                    articleModel.Liked = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.MostLiked == true && x.UserId == userId).Select(x => x.MostLiked).Distinct().Count() >= 1 ? true : false;
                    articleModel.Favourite = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.Favourite == true && x.UserId == userId).Select(x => x.Favourite).Distinct().Count() >= 1 ? true : false;
                    articleModel.CreatedDate = contentArticle.CreatedDate;
                    categoryArticleList.Add(articleModel);
                }
            }
            playerMobileLikeFavouriteData.articleContentData = categoryArticleList;
            if (likedVideoEntity.Count == 0 && likedArticleEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            return playerMobileLikeFavouriteData;
        }

        public CommentCountData GetMobileCommentCount(long contentId, long userId, ref ErrorResponseModel errorResponseModel)
        {
                errorResponseModel = new ErrorResponseModel();
                List<ContentCommentModelData> commentModelList = new List<ContentCommentModelData>();
                List<ContentCommentModel> ContentcommentList = new List<ContentCommentModel>();
                CommentCountData commentListData = new CommentCountData();
                commentListData.CommentListCount = 0;
                var PubliccommentEntity = (from comment in context.Comments
                                     join content in context.ContentDetails on comment.ContentId equals content.ContentId
                                     join user in context.UserMasters on comment.UserId equals user.UserId
                                     join contenttype in context.ContentTypes on comment.ContentTypeId equals contenttype.ContentTypeId
                                     join player in context.PlayerDetails on content.PlayerId equals player.PlayerId
                                     where comment.ContentId == contentId && comment.IsDeleted == false
                                     && comment.IsPublic == true
                                     orderby comment.CreatedDate descending
                                     select new ContentCommentModel
                                     {
                                         CommentId = comment.CommentId,
                                         ContentId = comment.ContentId,
                                         UserId = comment.UserId,
                                         EmailId = user.EmailId,
                                         FirstName = user.FirstName,
                                         LastName = user.LastName,
                                         FullName = user.FirstName + " " + user.LastName,
                                         DeviceId = comment.DeviceId,
                                         Location = comment.Location,
                                         Liked = comment.Liked,
                                         Comment1 = comment.Comment1,
                                         Shared = comment.Shared,
                                         IsPublic = comment.IsPublic,
                                         CreatedBy = comment.CreatedBy,
                                         CreatedDate = comment.CreatedDate,
                                         ParentCommentId = comment.ParentCommentId,
                                         ContentTypeId = content.ContentTypeId,
                                         ContentTypeName = contenttype.ContentName,
                                         UpdatedDate = comment.UpdatedDate,
                                         UpdatedBy = comment.UpdatedBy,
                                     }).ToList();

                var PrivatecommentEntity = (from comment in context.Comments
                                            join content in context.ContentDetails on comment.ContentId equals content.ContentId
                                            join user in context.UserMasters on comment.UserId equals user.UserId
                                            join contenttype in context.ContentTypes on comment.ContentTypeId equals contenttype.ContentTypeId
                                            join player in context.PlayerDetails on content.PlayerId equals player.PlayerId
                                            where comment.ContentId == contentId && comment.IsDeleted == false
                                            && comment.IsPublic == false && comment.UserId == userId
                                            orderby comment.CreatedDate descending
                                            select new ContentCommentModel
                                            {
                                                CommentId = comment.CommentId,
                                                ContentId = comment.ContentId,
                                                UserId = comment.UserId,
                                                EmailId = user.EmailId,
                                                FirstName = user.FirstName,
                                                LastName = user.LastName,
                                                FullName = user.FirstName + " " + user.LastName,
                                                DeviceId = comment.DeviceId,
                                                Location = comment.Location,
                                                Liked = comment.Liked,
                                                Comment1 = comment.Comment1,
                                                Shared = comment.Shared,
                                                IsPublic = comment.IsPublic,
                                                CreatedBy = comment.CreatedBy,
                                                CreatedDate = comment.CreatedDate,
                                                ParentCommentId = comment.ParentCommentId,
                                                ContentTypeId = content.ContentTypeId,
                                                ContentTypeName = contenttype.ContentName,
                                                UpdatedDate = comment.UpdatedDate,
                                                UpdatedBy = comment.UpdatedBy,
                                            }).ToList();

                if (PubliccommentEntity.Count == 0 && PrivatecommentEntity.Count==0)
                {
                    errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                    errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                    return new CommentCountData();
                }

                ContentcommentList.AddRange(PubliccommentEntity.ToList());
                ContentcommentList.AddRange(PrivatecommentEntity.ToList());

                foreach (var item in ContentcommentList)
                {
                    ContentCommentModelData modelData = new ContentCommentModelData();
                    if (item.CommentId != null && item.ParentCommentId == null)
                    {
                        modelData.CommentId = item.CommentId;
                        List<ContentCommentModel> commentList = new List<ContentCommentModel>();
                        var model = new ContentCommentModel();
                        model.CommentId = item.CommentId;
                        model.ContentId = item.ContentId;
                        model.UserId = item.UserId;
                        model.Comment1 = item.Comment1;
                        model.ParentCommentId = item.ParentCommentId;
                        model.DeviceId = item.DeviceId;
                        model.Location = item.Location;
                        model.Liked = item.Liked;
                        model.Shared = item.Shared;
                        model.IsPublic = item.IsPublic;
                        model.EmailId = item.EmailId;
                        model.CreatedDate = item.CreatedDate;
                        model.CreatedBy = item.CreatedBy;
                        model.FirstName = item.FirstName;
                        model.LastName = item.LastName;
                        model.FullName = item.FirstName + " " + item.LastName;
                        model.ContentTypeId = item.ContentTypeId;
                        model.ContentTypeName = item.ContentTypeName;
                        model.UpdatedBy = item.UpdatedBy;
                        model.UpdatedDate = item.UpdatedDate;
                        commentList.Add(model);
                        var commentreplyEntity = (from comment in context.Comments
                                                  join content in context.ContentDetails on comment.ContentId equals content.ContentId
                                                  join user in context.UserMasters on comment.UserId equals user.UserId
                                                  join contenttype in context.ContentTypes on comment.ContentTypeId equals contenttype.ContentTypeId
                                                  join player in context.PlayerDetails on content.PlayerId equals player.PlayerId
                                                  where comment.ContentId == contentId && comment.IsDeleted == false
                                                  && comment.ParentCommentId == item.CommentId

                                                  select new ContentCommentModel
                                                  {
                                                      CommentId = comment.CommentId,
                                                      ContentId = comment.ContentId,
                                                      UserId = comment.UserId,
                                                      EmailId = user.EmailId,
                                                      FirstName = user.FirstName,
                                                      ReplyedAdminName = "IM" + player.FirstName,
                                                      LastName = user.LastName,
                                                      FullName = user.FirstName + " " + user.LastName,
                                                      DeviceId = comment.DeviceId,
                                                      Location = comment.Location,
                                                      CommentedUserId = model.UserId,
                                                      Liked = comment.Liked,
                                                      Comment1 = comment.Comment1,
                                                      Shared = comment.Shared,
                                                      IsPublic = comment.IsPublic,
                                                      CreatedBy = comment.CreatedBy,
                                                      CreatedDate = comment.CreatedDate,
                                                      ParentCommentId = comment.ParentCommentId,
                                                      ContentTypeId = content.ContentTypeId,
                                                      ContentTypeName = contenttype.ContentName,
                                                      UpdatedDate = comment.UpdatedDate,
                                                      UpdatedBy = comment.UpdatedBy,
                                                  }).ToList();
                        // var commentreplyEntity = context.Comments.FirstOrDefault(x => x.ParentCommentId == item.CommentId);
                        if (commentreplyEntity.Count != 0)
                        {
                            foreach (var item1 in commentreplyEntity)
                            {
                                var model1 = new ContentCommentModel();
                                model1.CommentId = item1.CommentId;
                                model1.ContentId = item1.ContentId;
                                model1.UserId = item1.UserId;
                                model1.Comment1 = item1.Comment1;
                                model1.ParentCommentId = item1.ParentCommentId;
                                model1.DeviceId = item1.DeviceId;
                                model1.Location = item1.Location;
                                model1.Liked = item1.Liked;
                                model1.ReplyedAdminName = item1.ReplyedAdminName;
                                model1.CommentedUserId = item1.CommentedUserId;
                                model1.Shared = item1.Shared;
                                model1.IsPublic = item1.IsPublic;
                                model1.EmailId = item1.EmailId;
                                model1.CreatedDate = item1.CreatedDate;
                                model1.CreatedBy = item1.CreatedBy;
                                model1.FirstName = item1.FirstName;
                                model1.LastName = item1.LastName;
                                model1.FullName = item1.FirstName + " " + item1.LastName;
                                model1.ContentTypeId = item1.ContentTypeId;
                                model1.ContentTypeName = item1.ContentTypeName;
                                model1.UpdatedBy = item1.UpdatedBy;
                                model1.UpdatedDate = item1.UpdatedDate;
                                commentList.Add(model1);
                            }
                        }
                        modelData.contentCommentModels = commentList;
                        modelData.CommentCount = commentList.Count();
                        commentListData.CommentListCount = modelData.CommentCount + commentListData.CommentListCount;
                    }
                    commentModelList.Add(modelData);
                }
                List<ContentCommentModelData> commentModelList1 = new List<ContentCommentModelData>();
                commentModelList1 = commentModelList.Where(x => x.CommentId != 0).ToList();
                return commentListData;       
        }
    }
}
