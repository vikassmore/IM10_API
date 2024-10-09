using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using StartUpX.Common;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using static IM10.Models.MobileModel;


namespace IM10.BAL.Implementaion
{
    public class MobileService : IMobileServices
    {
        IM10DbContext context;
        private ConfigurationModel _configuration;
        private readonly IUserAuditLogService _userAuditLogService;
        private readonly IEncryptionService _encryptionService;
        private readonly AppSettings _config;


        /// <summary>
        /// Creating constructor and injection dbContext
        /// </summary>
        /// <param name="_context"></param>
        public MobileService(IM10DbContext _context, IOptions<AppSettings> config, IEncryptionService encryptionService, IOptions<ConfigurationModel> hostName, IUserAuditLogService userAuditLogService)
        {
            context = _context;
            _configuration = hostName.Value;
            _userAuditLogService = userAuditLogService;
            _encryptionService = encryptionService;
            _config = config.Value;
        }


        private string GetHostName(bool? flag)
        {
            return (bool)flag ? _config.BunnyHostName : _configuration.HostName;
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
        public List<MobileContentData> GetTop5TrendingVideoContent(string playerId, long userId, int countNumber, ref ErrorResponseModel errorResponseModel)
        {
            bool userIsLoggedIn = context.UserMasters.Where(z => z.UserId == userId && z.IsDeleted == false)
                                 .Select(z => z.IsLogin).FirstOrDefault() ?? false;
            errorResponseModel = new ErrorResponseModel();
            var decryptResult = _encryptionService.DecryptPlayerId(playerId);
            if (decryptResult.DecryptedPlayerId == null)
            {
                errorResponseModel.StatusCode = decryptResult.StatusCode;
                errorResponseModel.Message = decryptResult.Message;
                return new List<MobileContentData>();
            }

            long decryptplayerId = decryptResult.DecryptedPlayerId.Value;


            var tredingEntity = (from flag in context.ContentViews
                                 join player in context.PlayerDetails on flag.PlayerId equals player.PlayerId
                                 join content in context.ContentDetails on flag.ContentId equals content.ContentId
                                 join type in context.ContentTypes on flag.ContentTypeId equals type.ContentTypeId
                                 where flag.IsDeleted == false && flag.Trending == true && flag.ContentTypeId == ContentTypeHelper.VideoContentTypeId
                                 && flag.PlayerId == decryptplayerId && content.Approved == true && content.IsDeleted == false
                                 orderby flag.UpdatedDate descending
                                 select new
                                 {
                                     content.ContentId
                                 }).ToList();
            var tredingtopfive = tredingEntity.GroupBy(x => new { x.ContentId }).Select(x => new { ContentId = x.Key, count = x.Count() }).OrderByDescending(x => x.count).Take(countNumber).ToList();
            if (tredingtopfive.Count == 0)
            {
                tredingEntity = (from flag in context.AdvContentMappings
                                 join content in context.ContentDetails on flag.ContentId equals content.ContentId
                                 where content.ContentTypeId == ContentTypeHelper.VideoContentTypeId && flag.IsDeleted == false && content.Approved == true && content.PlayerId == decryptplayerId
                                 orderby flag.CreatedDate descending
                                 select new
                                 {
                                     flag.ContentId
                                 }).ToList();
                tredingtopfive = tredingEntity.GroupBy(x => new { x.ContentId }).Select(x => new { ContentId = x.Key, count = x.Count() }).OrderByDescending(x => x.count).Take(countNumber).ToList();
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
                                            adv.ProductionFlag
                                        }).ToList();
                //var advertiseContent = context.AdvContentMappings.Include(x => x.Content).Include(x => x.AdvertiseContent).Where(x => x.ContentId == item.ContentId.ContentId && x.IsDeleted == false).ToList();
                var contentVideo = context.ContentDetails.Include(x => x.Category).FirstOrDefault(x => x.ContentId == item.ContentId.ContentId && x.IsDeleted == false);
                var imgmodel = new VideoImageModel();
                if (contentVideo.ContentFileName != null)
                {
                    var content1 = new ContentMobileModel();
                    var hostname1 = GetHostName(contentVideo.ProductionFlag);
                    imgmodel.url = hostname1.TrimEnd('/') + (String.IsNullOrEmpty(contentVideo.ContentFilePath) ? contentVideo.ContentFilePath : contentVideo.ContentFilePath);
                    imgmodel.Type = String.IsNullOrEmpty(contentVideo.ContentFilePath) ? "video" : "image";
                    imgmodel.FileName = (imgmodel.url);

                    content1.ContentId = contentVideo.ContentId;
                    content1.FileName = contentVideo.ContentFileName;
                    content1.FilePath = hostname1 + contentVideo.ContentFilePath;
                    content1.Title = contentVideo.Title;
                    content1.Description = contentVideo.Description;
                    content1.Position = Helper.FirstHalfContentPostion;
                    content1.CategoryName = contentVideo.Category.Name;
                    content1.Thumbnail = hostname1 + contentVideo.Thumbnail1; //ThumbnailPath(imgmodel.url);
                    contentList.Add(content1);
                }
                if (contentVideo.ContentFileName1 != null)
                {
                    var content2 = new ContentMobileModel();
                    var imgmodel1 = new VideoImageModel();
                    var hostname12 = GetHostName(contentVideo.ProductionFlag);
                    imgmodel1.url = hostname12.TrimEnd('/') + (String.IsNullOrEmpty(contentVideo.ContentFilePath1) ? contentVideo.ContentFilePath1 : contentVideo.ContentFilePath1);
                    imgmodel1.Type = String.IsNullOrEmpty(contentVideo.ContentFilePath1) ? "video" : "image";
                    imgmodel1.FileName = (imgmodel1.url);

                    content2.ContentId = contentVideo.ContentId;
                    content2.FileName = contentVideo.ContentFileName1;
                    content2.FilePath = hostname12 + contentVideo.ContentFilePath1;
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
                    var hostname123 = GetHostName(advcontent.ProductionFlag);
                    imgmodel2.url = hostname123.TrimEnd('/') + (String.IsNullOrEmpty(advcontent.AdvertiseFilePath) ? advcontent.AdvertiseFilePath : advcontent.AdvertiseFilePath);
                    imgmodel2.Type = String.IsNullOrEmpty(advcontent.AdvertiseFilePath) ? "video" : "image";
                    imgmodel2.FileName = _configuration.HostName + (imgmodel2.url);
                    model.AdvertiseContentId = advcontent.AdvertiseContentId;
                    model.FileName = advcontent.AdvertiseFileName;
                    model.FilePath = hostname123 + advcontent.AdvertiseFilePath;
                    model.Title = advcontent.Title;
                    model.Thumbnail = imgmodel2.url;
                    model.ContentId = advcontent.ContentId;
                    model.Position = advcontent.Position;
                    contentList.Add(model);
                }
                var hostname = GetHostName(contentVideo.ProductionFlag);

                topModel.contentMobileModels = contentList.OrderBy(x => x.Position).ToList();
                topModel.ContentId = contentVideo.ContentId;
                topModel.Title = contentVideo.Title;
                topModel.Description = contentVideo.Description;
                topModel.Thumbnail = hostname + contentVideo.Thumbnail1; //ThumbnailPath(imgmodel.url);
                topModel.CategoryId = contentVideo.CategoryId;
                topModel.ViewNo = context.ContentViews.Where(x => x.ContentId == item.ContentId.ContentId && x.Trending == true).Select(x => x.Trending).Count();
                topModel.ContentTypeId = contentVideo.ContentTypeId;
                topModel.LikedNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId.ContentId && x.MostLiked == true && x.IsDeleted==false).Select(x => x.MostLiked).Count();
                topModel.FavouriteNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId.ContentId && x.Favourite == true && x.IsDeleted == false).Select(x => x.Favourite).Count();
                var publicEntity = context.Comments.Where(x => x.ContentId == item.ContentId.ContentId && x.IsPublic == true && x.IsDeleted == false).Select(x => x.CommentId).ToList();

                if (userIsLoggedIn == true)
                {
                    var commentList = context.Comments.Where(x => x.ContentId == item.ContentId.ContentId && x.UserId == userId && x.IsDeleted == false && x.IsPublic == false).Select(x => x.CommentId).ToList();
                    var commentData = context.Comments.Where(x => x.ContentId == item.ContentId.ContentId && x.UserId == userId && x.IsDeleted == false).Select(x => x.CommentId).ToList();
                    var replyData = context.Comments.Where(x => commentData.Contains((long)x.ParentCommentId) && x.IsPublic == false && x.IsDeleted == false).ToList();
                    topModel.CommentCount = commentList.Count() + replyData.Count() + publicEntity.Count();
                }
                else
                {
                    topModel.CommentCount = publicEntity.Count();
                }

                topModel.Liked = context.ContentFlags.Where(x => x.ContentId == item.ContentId.ContentId && x.MostLiked == true && x.UserId == userId && x.IsDeleted == false).Select(x => x.MostLiked).Distinct().Count() >= 1 ? true : false;
                topModel.Favourite = context.ContentFlags.Where(x => x.ContentId == item.ContentId.ContentId && x.Favourite == true && x.UserId == userId && x.IsDeleted == false).Select(x => x.Favourite).Distinct().Count() >= 1 ? true : false;
                topModel.CreatedDate = contentVideo.CreatedDate;
                topList.Add(topModel);

            }
            var newtoplist = topList.OrderByDescending(z => z.CreatedDate).ToList();
            var groupedContent = newtoplist.GroupBy(c => c.CategoryId)
                   .Select(g => new
                   {
                       Category = g.Key,
                       Contents = g.ToList()
                   }).ToList();

            var indexedContent = new List<MobileContentData>();

            foreach (var group in groupedContent)
            {
                int index = 0;
                foreach (var content in group.Contents)
                {
                    content.AutoIndex = index;
                    indexedContent.Add(content);
                    index++;
                }
            }
            if (tredingEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            return indexedContent;
        }



        /// <summary>
        /// Method is used to get all Category top five video
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<MobileVideoCategoryData> GetAllCategoryTopFiveVideoContent(string playerId, long userId, ref ErrorResponseModel errorResponseModel)
        {
            bool userIsLoggedIn = context.UserMasters.Where(z => z.UserId == userId && z.IsDeleted == false)
                                 .Select(z => z.IsLogin).FirstOrDefault() ?? false;

            errorResponseModel = new ErrorResponseModel();
            var decryptResult = _encryptionService.DecryptPlayerId(playerId);
            if (decryptResult.DecryptedPlayerId == null)
            {
                errorResponseModel.StatusCode = decryptResult.StatusCode;
                errorResponseModel.Message = decryptResult.Message;
                return new List<MobileVideoCategoryData>();
            }

            long decryptplayerId = decryptResult.DecryptedPlayerId.Value;

            List<MobileVideoCategoryData> mobileVideoCategoryData = new List<MobileVideoCategoryData>();
            var categoryEnitityList = (from content in context.ContentDetails
                                       where content.ContentTypeId == ContentTypeHelper.VideoContentTypeId &&
                                             content.Approved == true &&
                                             content.PlayerId == decryptplayerId &&
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
                                           HasAdvertisement = subflag != null
                                       }).ToList();

            var categoryList = categoryEnitityList.GroupBy(x => new { x.CategoryId, x.Name }).Select(x => new { CategoryId = x.Key.CategoryId, Name = x.Key.Name, DisplayOrder = x.Max(x => x.DisplayOrder), count = x.Count() }).ToList();
            foreach (var item in categoryList)
            {
                var displayEntity = context.Categories.Where(x => x.CategoryId == item.CategoryId).Select(z => z.DisplayOrder).FirstOrDefault();
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
                                                    adv.ProductionFlag
                                                }).ToList();
                        var imgmodel = new VideoImageModel();
                        if (contentVideo.ContentFileName != null)
                        {
                            var hostname1 = GetHostName(contentVideo.ProductionFlag);
                            var content1 = new ContentMobileModel();
                            imgmodel.url = hostname1.TrimEnd('/') + (String.IsNullOrEmpty(contentVideo.ContentFilePath) ? contentVideo.ContentFilePath : contentVideo.ContentFilePath);
                            imgmodel.Type = String.IsNullOrEmpty(contentVideo.ContentFilePath) ? "video" : "image";
                            imgmodel.FileName = (imgmodel.url);

                            content1.ContentId = contentVideo.ContentId;
                            content1.FileName = contentVideo.ContentFileName;
                            content1.FilePath = hostname1 + contentVideo.ContentFilePath;
                            content1.Title = contentVideo.Title;
                            content1.Description = contentVideo.Description;
                            content1.Position = Helper.FirstHalfContentPostion;
                            content1.CategoryName = contentVideo.Category.Name;
                            content1.Thumbnail = hostname1 + contentVideo.Thumbnail1;// ThumbnailPath(imgmodel.url);
                            contentList.Add(content1);
                        }
                        if (contentVideo.ContentFileName1 != null)
                        {
                            var hostname12 = GetHostName(contentVideo.ProductionFlag);
                            var content2 = new ContentMobileModel();
                            var imgmodel1 = new VideoImageModel();
                            imgmodel1.url = hostname12.TrimEnd('/') + (String.IsNullOrEmpty(contentVideo.ContentFilePath1) ? contentVideo.ContentFilePath1 : contentVideo.ContentFilePath1);
                            imgmodel1.Type = String.IsNullOrEmpty(contentVideo.ContentFilePath1) ? "video" : "image";
                            imgmodel1.FileName = (imgmodel1.url);

                            content2.ContentId = contentVideo.ContentId;
                            content2.FileName = contentVideo.ContentFileName1;
                            content2.FilePath = hostname12 + contentVideo.ContentFilePath1;
                            content2.Title = contentVideo.Title;
                            content2.Description = contentVideo.Description;
                            content2.Position = Helper.SecondHalfContentPostion;
                            content2.CategoryName = contentVideo.Category.Name;
                            content2.Thumbnail = ThumbnailPath(imgmodel1.url);
                            contentList.Add(content2);
                        }
                        foreach (var advcontent in advertiseContent)
                        {
                            var hostname123 = GetHostName(advcontent.ProductionFlag);
                            var model = new ContentMobileModel();
                            var imgmodel2 = new VideoImageModel();
                            imgmodel2.url = hostname123.TrimEnd('/') + (String.IsNullOrEmpty(advcontent.AdvertiseFilePath) ? advcontent.AdvertiseFilePath : advcontent.AdvertiseFilePath);
                            imgmodel2.Type = String.IsNullOrEmpty(advcontent.AdvertiseFilePath) ? "video" : "image";
                            imgmodel2.FileName = (imgmodel2.url);
                            model.AdvertiseContentId = advcontent.AdvertiseContentId;
                            model.FileName = advcontent.AdvertiseFileName;
                            model.FilePath = imgmodel2.FileName;
                            model.Title = advcontent.Title;
                            model.Thumbnail = imgmodel2.url;
                            model.ContentId = advcontent.ContentId;
                            model.Position = advcontent.Position;
                            contentList.Add(model);
                        }
                        var hostname = GetHostName(contentVideo.ProductionFlag);
                        topModel.contentMobileModels = contentList.OrderBy(x => x.Position).ToList();
                        topModel.ContentId = contentVideo.ContentId;
                        topModel.Title = contentVideo.Title;
                        topModel.Description = contentVideo.Description;
                        topModel.Thumbnail = hostname + contentVideo.Thumbnail1; //ThumbnailPath(imgmodel.url);
                        topModel.CategoryId = contentVideo.Category.CategoryId;
                        topModel.ViewNo = context.ContentViews.Where(x => x.ContentId == item1.ContentId.ContentId && x.Trending == true).Select(x => x.Trending).Count();
                        topModel.ContentTypeId = contentVideo.ContentTypeId;
                        topModel.LikedNo = context.ContentFlags.Where(x => x.ContentId == item1.ContentId.ContentId && x.MostLiked == true && x.IsDeleted == false).Select(x => x.MostLiked).Count();
                        topModel.FavouriteNo = context.ContentFlags.Where(x => x.ContentId == item1.ContentId.ContentId && x.Favourite == true && x.IsDeleted == false).Select(x => x.Favourite).Count();
                        // topModel.CommentCount = context.Comments.Where(x => x.ContentId == item1.ContentId.ContentId).Select(x => x.CommentId).Count();
                        var publicEntity = context.Comments.Where(x => x.ContentId == item1.ContentId.ContentId && x.IsPublic == true && x.IsDeleted == false).Select(x => x.CommentId).ToList();

                        if (userIsLoggedIn == true)
                        {
                            var commentList = context.Comments.Where(x => x.ContentId == item1.ContentId.ContentId && x.UserId == userId && x.IsDeleted == false && x.IsPublic == false).Select(x => x.CommentId).ToList();
                            var commentData = context.Comments.Where(x => x.ContentId == item1.ContentId.ContentId && x.UserId == userId && x.IsDeleted == false).Select(x => x.CommentId).ToList();
                            var replyData = context.Comments.Where(x => commentData.Contains((long)x.ParentCommentId) && x.IsPublic == false && x.IsDeleted == false).ToList();
                            topModel.CommentCount = commentList.Count() + replyData.Count() + publicEntity.Count();
                        }
                        else
                        {
                            topModel.CommentCount = publicEntity.Count();
                        }

                        topModel.Liked = context.ContentFlags.Where(x => x.ContentId == item1.ContentId.ContentId && x.MostLiked == true && x.UserId == userId && x.IsDeleted == false).Select(x => x.MostLiked).Distinct().Count() >= 1 ? true : false;
                        topModel.Favourite = context.ContentFlags.Where(x => x.ContentId == item1.ContentId.ContentId && x.Favourite == true && x.UserId == userId && x.IsDeleted == false).Select(x => x.Favourite).Distinct().Count() >= 1 ? true : false;
                        topModel.CreatedDate = contentVideo.CreatedDate;
                        modelList.Add(topModel);
                        modelList = modelList.OrderByDescending(x => x.CreatedDate).ToList();
                    }
                }
                var groupedContent = modelList.GroupBy(c => c.CategoryId)
                   .Select(g => new
                   {
                       Category = g.Key,
                       Contents = g.ToList()
                   }).ToList();

                var indexedContent = new List<MobileContentData>();

                foreach (var group in groupedContent)
                {
                    int index = 0;
                    foreach (var content in group.Contents)
                    {
                        content.AutoIndex = index;
                        indexedContent.Add(content);
                        index++;
                    }
                }

                categoryModel.mobileContentDatas = indexedContent;
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
        public MobileVideoCategoryData GetVideoContentByCategory(string playerId, long categoryId, long userId, int countNumber, ref ErrorResponseModel errorResponseModel)
        {
            bool userIsLoggedIn = context.UserMasters.Where(z => z.UserId == userId && z.IsDeleted == false)
                                  .Select(z => z.IsLogin).FirstOrDefault() ?? false;

            errorResponseModel = new ErrorResponseModel();
            var decryptResult = _encryptionService.DecryptPlayerId(playerId);
            if (decryptResult.DecryptedPlayerId == null)
            {
                errorResponseModel.StatusCode = decryptResult.StatusCode;
                errorResponseModel.Message = decryptResult.Message;
                return new MobileVideoCategoryData();
            }

            long decryptplayerId = decryptResult.DecryptedPlayerId.Value;

            if (categoryId == 0)
            {
                // Call the GetTop5TrendingVideoContent method if categoryId is 0
                var trendingContent = GetTop5TrendingVideoContent(playerId, userId, countNumber, ref errorResponseModel);
                MobileVideoCategoryData trendingVideoModel = new MobileVideoCategoryData
                {
                    CategoryName = "Trending",
                    CategoryId = 0,
                    DisplayOrder = 0,
                    mobileContentDatas = trendingContent
                };
                return trendingVideoModel;
            }
            MobileVideoCategoryData videoModel = new MobileVideoCategoryData();

            var categoryEnitityList = (from content in context.ContentDetails
                                       where content.IsDeleted == false && content.ContentTypeId == ContentTypeHelper.VideoContentTypeId && content.Approved == true && content.PlayerId == decryptplayerId && content.CategoryId == categoryId
                                       join flag in context.AdvContentMappings.Where(m => m.IsDeleted == false)
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
                                                adv.ProductionFlag
                                            }).ToList();
                    var imgmodel = new VideoImageModel();
                    if (contentVideo.ContentFileName != null)
                    {
                        var hostname1 = GetHostName(contentVideo.ProductionFlag);
                        var content1 = new ContentMobileModel();
                        imgmodel.url = hostname1.TrimEnd('/') + (String.IsNullOrEmpty(contentVideo.ContentFilePath) ? contentVideo.ContentFilePath : contentVideo.ContentFilePath);
                        imgmodel.Type = String.IsNullOrEmpty(contentVideo.ContentFilePath) ? "video" : "image";
                        imgmodel.FileName = (imgmodel.url);
                        content1.ContentId = contentVideo.ContentId;
                        content1.FileName = contentVideo.ContentFileName;
                        content1.FilePath = hostname1 + contentVideo.ContentFilePath;
                        content1.Title = contentVideo.Title;
                        content1.Description = contentVideo.Description;
                        content1.Position = Helper.FirstHalfContentPostion;
                        content1.CategoryName = contentVideo.Category.Name;
                        content1.Thumbnail = hostname1 + contentVideo.Thumbnail1; //ThumbnailPath(imgmodel.url);
                        contentList.Add(content1);
                    }
                    if (contentVideo.ContentFileName1 != null)
                    {
                        var hostname12 = GetHostName(contentVideo.ProductionFlag);
                        var content2 = new ContentMobileModel();
                        var imgmodel1 = new VideoImageModel();
                        imgmodel1.url = hostname12.TrimEnd('/') + (String.IsNullOrEmpty(contentVideo.ContentFilePath1) ? contentVideo.ContentFilePath1 : contentVideo.ContentFilePath1);
                        imgmodel1.Type = String.IsNullOrEmpty(contentVideo.ContentFilePath1) ? "video" : "image";
                        imgmodel1.FileName = (imgmodel1.url);

                        content2.ContentId = contentVideo.ContentId;
                        content2.FileName = contentVideo.ContentFileName1;
                        content2.FilePath = hostname12 + contentVideo.ContentFilePath1;
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
                        var hostname123 = GetHostName(advcontent.ProductionFlag);
                        imgmodel2.url = hostname123.TrimEnd('/') + (String.IsNullOrEmpty(advcontent.AdvertiseFilePath) ? advcontent.AdvertiseFilePath : advcontent.AdvertiseFilePath);
                        imgmodel2.Type = String.IsNullOrEmpty(advcontent.AdvertiseFilePath) ? "video" : "image";
                        imgmodel2.FileName = (imgmodel2.url);
                        model.AdvertiseContentId = advcontent.AdvertiseContentId;
                        model.FileName = advcontent.AdvertiseFileName;
                        model.FilePath = hostname123 + advcontent.AdvertiseFilePath;
                        model.Title = advcontent.Title;
                        model.Thumbnail = imgmodel2.url;
                        model.ContentId = advcontent.ContentId;
                        model.Position = advcontent.Position;
                        contentList.Add(model);
                    }
                    var hostname = GetHostName(contentVideo.ProductionFlag);
                    topModel.contentMobileModels = contentList.OrderBy(x => x.Position).ToList();
                    topModel.ContentId = contentVideo.ContentId;
                    topModel.Title = contentVideo.Title;
                    topModel.CategoryId = item1.CategoryId;
                    topModel.Description = contentVideo.Description;
                    topModel.Thumbnail = hostname + contentVideo.Thumbnail1; //ThumbnailPath(imgmodel.url);
                    topModel.ViewNo = context.ContentViews.Where(x => x.ContentId == item1.ContentId.ContentId && x.Trending == true).Select(x => x.Trending).Count();
                    topModel.ContentTypeId = contentVideo.ContentTypeId;
                    topModel.LikedNo = context.ContentFlags.Where(x => x.ContentId == item1.ContentId.ContentId && x.MostLiked == true && x.IsDeleted == false).Select(x => x.MostLiked).Count();
                    topModel.FavouriteNo = context.ContentFlags.Where(x => x.ContentId == item1.ContentId.ContentId && x.Favourite == true && x.IsDeleted == false).Select(x => x.Favourite).Count();
                    var publicEntity = context.Comments.Where(x => x.ContentId == item1.ContentId.ContentId && x.IsPublic == true && x.IsDeleted == false).Select(x => x.CommentId).ToList();

                    if (userIsLoggedIn == true)
                    {
                        var commentList = context.Comments.Where(x => x.ContentId == item1.ContentId.ContentId && x.UserId == userId && x.IsDeleted == false && x.IsPublic==false).Select(x => x.CommentId).ToList();
                        var commentData = context.Comments.Where(x => x.ContentId == item1.ContentId.ContentId && x.UserId == userId && x.IsDeleted == false).Select(x => x.CommentId).ToList();
                        var replyData = context.Comments.Where(x => commentData.Contains((long)x.ParentCommentId) && x.IsPublic==false && x.IsDeleted == false).ToList();
                        topModel.CommentCount = commentList.Count() + replyData.Count() + publicEntity.Count();
                    }
                    else
                    {
                        topModel.CommentCount = publicEntity.Count();
                    }

                    topModel.Liked = context.ContentFlags.Where(x => x.ContentId == item1.ContentId.ContentId && x.MostLiked == true && x.UserId == userId && x.IsDeleted == false).Select(x => x.MostLiked).Distinct().Count() >= 1 ? true : false;
                    topModel.Favourite = context.ContentFlags.Where(x => x.ContentId == item1.ContentId.ContentId && x.Favourite == true && x.UserId == userId && x.IsDeleted == false).Select(x => x.Favourite).Distinct().Count() >= 1 ? true : false;
                    topModel.CreatedDate = contentVideo.CreatedDate;
                    topList.Add(topModel);
                }
            }
            var newtoplist = topList.OrderByDescending(z => z.CreatedDate).ToList();
            var groupedContent = newtoplist.GroupBy(c => c.CategoryId)
                   .Select(g => new
                   {
                       Category = g.Key,
                       Contents = g.ToList()
                   }).ToList();

            var indexedContent = new List<MobileContentData>();

            foreach (var group in groupedContent)
            {
                int index = 0;
                foreach (var content in group.Contents)
                {
                    content.AutoIndex = index;
                    indexedContent.Add(content);
                    index++;
                }
            }
            if (categoryEnitityList.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            videoModel.mobileContentDatas = indexedContent;
            return videoModel;
        }



        /// <summary>
        /// Method is used to get mobileVideoview by contentId
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        public ContentFlagModel GetMobileVideoView(long contentId, long userId, ref ErrorResponseModel errorResponseModel)
        {
            bool userIsLoggedIn = context.UserMasters.Where(z => z.UserId == userId && z.IsDeleted == false)
                                 .Select(z => z.IsLogin).FirstOrDefault() ?? false;

            errorResponseModel = new ErrorResponseModel();
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
                                     flag.Favourite,
                                     flag.MostLiked,
                                     flag.ContentSequence,
                                     flag.ContentTypeId,
                                     flag.CreatedBy,
                                     flag.CreatedDate,
                                     flag.UpdatedBy,
                                     flag.UpdatedDate,
                                     content.ContentFileName,
                                     content.ContentFilePath,
                                     content.ContentFileName1,
                                     content.ContentFilePath1,
                                     content.Title,
                                     content.Description,
                                     content.Category.Name,
                                     content.Thumbnail1,
                                     content.ProductionFlag
                                 }).FirstOrDefault();

            if (articleEntity == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;
            }

            var contentMobileModels = new List<ContentMobileModel>();

            if (articleEntity.ContentFileName != null)
            {
                var content1 = new ContentMobileModel();
                var imgmodel = new VideoImageModel();
                var hostname1 = GetHostName(articleEntity.ProductionFlag);

                imgmodel.url = hostname1.TrimEnd('/') + (String.IsNullOrEmpty(articleEntity.ContentFilePath) ? articleEntity.ContentFilePath : articleEntity.ContentFilePath);
                imgmodel.Type = String.IsNullOrEmpty(articleEntity.ContentFilePath) ? "video" : "image";
                imgmodel.FileName = (imgmodel.url);

                content1.ContentId = articleEntity.ContentId;
                content1.FileName = articleEntity.ContentFileName;
                content1.FilePath = imgmodel.FileName;
                content1.Title = articleEntity.Title;
                content1.Description = articleEntity.Description;
                content1.Position = Helper.FirstHalfContentPostion;
                content1.CategoryName = articleEntity.Name;
                content1.Thumbnail = hostname1 + articleEntity.Thumbnail1;
                contentMobileModels.Add(content1);
            }

            if (articleEntity.ContentFileName1 != null)
            {
                var content2 = new ContentMobileModel();
                var imgmodel1 = new VideoImageModel();
                var hostname12 = GetHostName(articleEntity.ProductionFlag);

                imgmodel1.url = hostname12.TrimEnd('/') + (String.IsNullOrEmpty(articleEntity.ContentFilePath1) ? articleEntity.ContentFilePath1 : articleEntity.ContentFilePath1);
                imgmodel1.Type = String.IsNullOrEmpty(articleEntity.ContentFilePath1) ? "video" : "image";
                imgmodel1.FileName = (imgmodel1.url);

                content2.ContentId = articleEntity.ContentId;
                content2.FileName = articleEntity.ContentFileName1;
                content2.FilePath = imgmodel1.FileName;
                content2.Title = articleEntity.Title;
                content2.Description = articleEntity.Description;
                content2.Position = Helper.SecondHalfContentPostion;
                content2.CategoryName = articleEntity.Name;
                content2.Thumbnail = ThumbnailPath(imgmodel1.url);
                contentMobileModels.Add(content2);
            }

            var advertiseContent = (from advcontent in context.AdvContentMappings
                                    join adv in context.AdvContentDetails on advcontent.AdvertiseContentId equals adv.AdvertiseContentId
                                    where advcontent.ContentId == articleEntity.ContentId && advcontent.IsDeleted == false
                                    select new
                                    {
                                        adv.AdvertiseContentId,
                                        adv.AdvertiseFileName,
                                        adv.AdvertiseFilePath,
                                        adv.Title,
                                        advcontent.ContentId,
                                        advcontent.Position,
                                        adv.ProductionFlag
                                    }).ToList();

            foreach (var adv in advertiseContent)
            {
                var model = new ContentMobileModel();
                var imgmodel2 = new VideoImageModel();
                var hostname123 = GetHostName(adv.ProductionFlag);

                imgmodel2.url = hostname123.TrimEnd('/') + (String.IsNullOrEmpty(adv.AdvertiseFilePath) ? adv.AdvertiseFilePath : adv.AdvertiseFilePath);
                imgmodel2.Type = String.IsNullOrEmpty(adv.AdvertiseFilePath) ? "video" : "image";
                imgmodel2.FileName = (imgmodel2.url);

                model.AdvertiseContentId = adv.AdvertiseContentId;
                model.FileName = adv.AdvertiseFileName;
                model.FilePath = imgmodel2.FileName;
                model.Title = adv.Title;
                model.Thumbnail = imgmodel2.FileName;
                model.ContentId = adv.ContentId;
                model.Position = adv.Position;
                contentMobileModels.Add(model);
            }
            int ViewNo = context.ContentViews.Where(x => x.ContentId == articleEntity.ContentId && x.Trending == true).Select(x => x.Trending).Count();
            int Liked = context.ContentFlags.Where(x => x.ContentId == articleEntity.ContentId && x.MostLiked == true && x.IsDeleted == false).Select(x => x.MostLiked).Count();
            int Favourite = context.ContentFlags.Where(x => x.ContentId == articleEntity.ContentId && x.Favourite == true && x.IsDeleted == false).Select(x => x.Favourite).Count();
            int CommentCount=0;

            var publicEntity = context.Comments.Where(x => x.ContentId == articleEntity.ContentId && x.IsPublic == true && x.IsDeleted == false).Select(x => x.CommentId).ToList();

            if (userIsLoggedIn == true)
            {
                var commentList = context.Comments.Where(x => x.ContentId == articleEntity.ContentId && x.UserId == userId && x.IsDeleted == false && x.IsPublic == false).Select(x => x.CommentId).ToList();
                var commentData = context.Comments.Where(x => x.ContentId == articleEntity.ContentId && x.UserId == userId && x.IsDeleted == false).Select(x => x.CommentId).ToList();
                var replyData = context.Comments.Where(x => commentData.Contains((long)x.ParentCommentId) && x.IsPublic == false && x.IsDeleted == false).ToList();
                CommentCount = commentList.Count() + replyData.Count() + publicEntity.Count();
            }
            else
            {
               CommentCount = publicEntity.Count();
            }

            var hostname = GetHostName(articleEntity.ProductionFlag);

            return new ContentFlagModel
            {
                ContentFlagId = articleEntity.ContentFlagId,
                ContentId = articleEntity.ContentId,
                PlayerId = _encryptionService.GetEncryptedId(articleEntity.PlayerId.ToString()),
                Trending = articleEntity.Trending,
                MostLiked = articleEntity.MostLiked,
                ViewNo = ViewNo,
                LikedNo = Liked,
                CommentCount = CommentCount,
                FavouriteNo = Favourite,
                Favourite = articleEntity.Favourite,
                ContentSequence = articleEntity.ContentSequence,
                ContentTypeId = articleEntity.ContentTypeId,
                CreatedBy = articleEntity.CreatedBy,
                CreatedDate = articleEntity.CreatedDate,
                UpdatedBy = articleEntity.UpdatedBy,
                UpdatedDate = articleEntity.UpdatedDate,
                ContentFileName = articleEntity.ContentFileName,
                ContentFilePath = hostname + articleEntity.ContentFilePath,
                ContentTitle = articleEntity.Title,
                contentMobileModels = contentMobileModels.OrderBy(z => z.Position).ToList()
            };
        }


        /// <summary>
        /// Method is used to get all Category top five Article
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<MobileArticleCategoryData> GetAllCategoryTopFiveArticleContent(string playerId, long userId, int countNumber, ref ErrorResponseModel errorResponseModel)
        {
            bool userIsLoggedIn = context.UserMasters.Where(z => z.UserId == userId && z.IsDeleted == false)
                                 .Select(z => z.IsLogin).FirstOrDefault() ?? false;

            errorResponseModel = new ErrorResponseModel();
            var decryptResult = _encryptionService.DecryptPlayerId(playerId);
            if (decryptResult.DecryptedPlayerId == null)
            {
                errorResponseModel.StatusCode = decryptResult.StatusCode;
                errorResponseModel.Message = decryptResult.Message;
                return new List<MobileArticleCategoryData>();
            }

            long decryptplayerId = decryptResult.DecryptedPlayerId.Value;

            List<MobileArticleCategoryData> mobileArticleList = new List<MobileArticleCategoryData>();
            var categoryEnitityList = (from content in context.ContentDetails
                                       where content.ContentTypeId == ContentTypeHelper.ArticleContentTypeId && content.IsDeleted == false && content.Approved == true && content.PlayerId == decryptplayerId
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

                var contenttopfive = contentEntity.GroupBy(x => new { x.ContentId }).Select(x => new { ContentId = x.Key, count = x.Count() }).OrderByDescending(x => x.count).Take(countNumber).ToList();
                foreach (var item1 in contenttopfive)
                {
                    var contentArticle = context.ContentDetails.FirstOrDefault(x => x.ContentId == item1.ContentId.ContentId && x.Approved == true && x.IsDeleted == false);
                    if (contentArticle != null)
                    {
                        CategoryArticleModel articleModel = new CategoryArticleModel();
                        var hostname1 = GetHostName(contentArticle.ProductionFlag);

                        var imgmodel2 = new VideoImageModel();
                        imgmodel2.url = hostname1.TrimEnd('/') + (String.IsNullOrEmpty(contentArticle.ContentFilePath) ? contentArticle.ContentFilePath : contentArticle.ContentFilePath);
                        imgmodel2.Type = String.IsNullOrEmpty(contentArticle.ContentFilePath) ? "video" : "image";
                        imgmodel2.FileName = (imgmodel2.url);

                        articleModel.ContentId = contentArticle.ContentId;
                        articleModel.Title = contentArticle.Title;
                        articleModel.CategoryId = contentArticle.CategoryId;
                        articleModel.Description = contentArticle.Description;
                        articleModel.CategoryName = contentArticle.Category.Name;
                        articleModel.FileName = contentArticle.ContentFileName;
                        articleModel.FilePath = imgmodel2.FileName;
                        articleModel.Thumbnail = hostname1 + contentArticle.Thumbnail1;
                        articleModel.ViewNo = context.ContentViews.Where(x => x.ContentId == item1.ContentId.ContentId && x.Trending == true).Select(x => x.Trending).Count();
                        articleModel.ContentTypeId = contentArticle.ContentTypeId;
                        articleModel.LikedNo = context.ContentFlags.Where(x => x.ContentId == item1.ContentId.ContentId && x.MostLiked == true && x.IsDeleted == false).Select(x => x.MostLiked).Count();
                        articleModel.FavouriteNo = context.ContentFlags.Where(x => x.ContentId == item1.ContentId.ContentId && x.Favourite == true && x.IsDeleted == false).Select(x => x.Favourite).Count();
                        var publicEntity = context.Comments.Where(x => x.ContentId == item1.ContentId.ContentId && x.IsPublic == true && x.IsDeleted == false).Select(x => x.CommentId).ToList();

                        if (userIsLoggedIn == true)
                        {
                            var commentList = context.Comments.Where(x => x.ContentId == item1.ContentId.ContentId && x.UserId == userId && x.IsDeleted == false && x.IsPublic == false).Select(x => x.CommentId).ToList();
                            var commentData = context.Comments.Where(x => x.ContentId == item1.ContentId.ContentId && x.UserId == userId && x.IsDeleted == false).Select(x => x.CommentId).ToList();
                            var replyData = context.Comments.Where(x => commentData.Contains((long)x.ParentCommentId) && x.IsPublic == false && x.IsDeleted == false).ToList();
                            articleModel.CommentCount = commentList.Count() + replyData.Count() + publicEntity.Count();
                        }
                        else
                        {
                            articleModel.CommentCount = publicEntity.Count();
                        }

                        // articleModel.CommentCount = context.Comments.Where(x => x.ContentId == item1.ContentId.ContentId).Select(x => x.CommentId).Count();
                        articleModel.Liked = context.ContentFlags.Where(x => x.ContentId == item1.ContentId.ContentId && x.MostLiked == true && x.UserId == userId && x.IsDeleted == false).Select(x => x.MostLiked).Distinct().Count() >= 1 ? true : false;
                        articleModel.Favourite = context.ContentFlags.Where(x => x.ContentId == item1.ContentId.ContentId && x.Favourite == true && x.UserId == userId && x.IsDeleted == false).Select(x => x.Favourite).Distinct().Count() >= 1 ? true : false;
                        articleModel.CreatedDate = contentArticle.CreatedDate;
                        categoryArticleList.Add(articleModel);
                    }
                }
                var newtoplist1 = categoryArticleList.OrderByDescending(z => z.CreatedDate).ToList();
                var groupedContent1 = newtoplist1.GroupBy(c => c.CategoryId)
                       .Select(g => new
                       {
                           Category = g.Key,
                           Contents = g.ToList()
                       }).ToList();

                var indexedContent1 = new List<CategoryArticleModel>();

                foreach (var group in groupedContent1)
                {
                    int index = 0;
                    foreach (var content in group.Contents)
                    {
                        content.CategoryAutoIndex = index;
                        indexedContent1.Add(content);
                        index++;
                    }
                }
                categoryModel.categoryArticleModels = indexedContent1;
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
        public MobileArticleCategoryData GetArticleContentByCategory(string playerId, long categoryId, long userId, ref ErrorResponseModel errorResponseModel)
        {
            bool userIsLoggedIn = context.UserMasters.Where(z => z.UserId == userId && z.IsDeleted == false)
                                 .Select(z => z.IsLogin).FirstOrDefault() ?? false;

            errorResponseModel = new ErrorResponseModel();
            var decryptResult = _encryptionService.DecryptPlayerId(playerId);
            if (decryptResult.DecryptedPlayerId == null)
            {
                errorResponseModel.StatusCode = decryptResult.StatusCode;
                errorResponseModel.Message = decryptResult.Message;
                return new MobileArticleCategoryData();
            }

            long decryptplayerId = decryptResult.DecryptedPlayerId.Value;

            MobileArticleCategoryData categoryModel = new MobileArticleCategoryData();
            var categoryEnitityList = (from content in context.ContentDetails
                                       where content.ContentTypeId == ContentTypeHelper.ArticleContentTypeId && content.IsDeleted == false && content.Approved == true && content.PlayerId == decryptplayerId && content.CategoryId == categoryId
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
                    var hostname1 = GetHostName(contentArticle.ProductionFlag);

                    var imgmodel2 = new VideoImageModel();
                    imgmodel2.url = hostname1.TrimEnd('/') + (String.IsNullOrEmpty(contentArticle.ContentFilePath) ? contentArticle.ContentFilePath : contentArticle.ContentFilePath);
                    imgmodel2.Type = String.IsNullOrEmpty(contentArticle.ContentFilePath) ? "video" : "image";
                    imgmodel2.FileName = (imgmodel2.url);

                    articleModel.Thumbnail = hostname1 + contentArticle.Thumbnail1;
                    articleModel.ContentId = contentArticle.ContentId;
                    articleModel.Title = contentArticle.Title;
                    articleModel.Description = contentArticle.Description;
                    articleModel.CategoryName = contentArticle.Category.Name;
                    articleModel.CategoryId = item.CategoryId;
                    articleModel.FileName = contentArticle.ContentFileName;
                    articleModel.FilePath = imgmodel2.FileName;
                    articleModel.ViewNo = context.ContentViews.Where(x => x.ContentId == item.ContentId && x.Trending == true).Select(x => x.Trending).Count();
                    articleModel.ContentTypeId = contentArticle.ContentTypeId;
                    articleModel.LikedNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.MostLiked == true && x.IsDeleted == false).Select(x => x.MostLiked).Count();
                    articleModel.FavouriteNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.Favourite == true && x.IsDeleted == false).Select(x => x.Favourite).Count();
                    var publicEntity = context.Comments.Where(x => x.ContentId == item.ContentId && x.IsPublic == true && x.IsDeleted == false).Select(x => x.CommentId).ToList();

                    if (userIsLoggedIn == true)
                    {
                        var commentList = context.Comments.Where(x => x.ContentId == item.ContentId && x.UserId == userId && x.IsDeleted == false && x.IsPublic == false).Select(x => x.CommentId).ToList();
                        var commentData = context.Comments.Where(x => x.ContentId == item.ContentId && x.UserId == userId && x.IsDeleted == false).Select(x => x.CommentId).ToList();
                        var replyData = context.Comments.Where(x => commentData.Contains((long)x.ParentCommentId) && x.IsPublic == false && x.IsDeleted == false).ToList();
                        articleModel.CommentCount = commentList.Count() + replyData.Count() + publicEntity.Count();
                    }
                    else
                    {
                        articleModel.CommentCount = publicEntity.Count();
                    }

                    // articleModel.CommentCount = context.Comments.Where(x => x.ContentId == item.ContentId && x.IsDeleted==false).Select(x => x.CommentId).Count();
                    articleModel.Liked = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.MostLiked == true && x.UserId == userId && x.IsDeleted == false).Select(x => x.MostLiked).Distinct().Count() >= 1 ? true : false;
                    articleModel.Favourite = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.Favourite == true && x.UserId == userId && x.IsDeleted == false).Select(x => x.Favourite).Distinct().Count() >= 1 ? true : false;
                    articleModel.CreatedDate = contentArticle.CreatedDate;
                    categoryArticleList.Add(articleModel);
                }
            }
            var newtoplist1 = categoryArticleList.OrderByDescending(z => z.CreatedDate).ToList();
            var groupedContent1 = newtoplist1.GroupBy(c => c.CategoryId)
                   .Select(g => new
                   {
                       Category = g.Key,
                       Contents = g.ToList()
                   }).ToList();

            var indexedContent1 = new List<CategoryArticleModel>();

            foreach (var group in groupedContent1)
            {
                int index = 0;
                foreach (var content in group.Contents)
                {
                    content.CategoryAutoIndex = index;
                    indexedContent1.Add(content);
                    index++;
                }
            }
            if (categoryEnitityList.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            categoryModel.categoryArticleModels = indexedContent1;
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
            bool userIsLoggedIn = context.UserMasters.Where(z => z.UserId == userId && z.IsDeleted == false)
                                .Select(z => z.IsLogin).FirstOrDefault() ?? false;

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
                                     content.CreatedDate,
                                     content.ProductionFlag
                                 }).FirstOrDefault();
            if (articleEntity == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;
            }

            var imgmodel = new VideoImageModel();
            var hostname1 = GetHostName(articleEntity.ProductionFlag);

            imgmodel.url = hostname1.TrimEnd('/') + (String.IsNullOrEmpty(articleEntity.ContentFilePath) ? articleEntity.ContentFilePath : articleEntity.ContentFilePath);
            imgmodel.Type = String.IsNullOrEmpty(articleEntity.ContentFilePath) ? "video" : "image";

            int ViewNo = context.ContentViews.Where(x => x.ContentId == articleEntity.ContentId && x.Trending == true).Select(x => x.Trending).Count();
            int Liked = context.ContentFlags.Where(x => x.ContentId == articleEntity.ContentId && x.MostLiked == true && x.IsDeleted == false).Select(x => x.MostLiked).Count();
            int Favourite = context.ContentFlags.Where(x => x.ContentId == articleEntity.ContentId && x.Favourite == true && x.IsDeleted == false).Select(x => x.Favourite).Count();
            var publicEntity = context.Comments.Where(x => x.ContentId == articleEntity.ContentId && x.IsPublic == true && x.IsDeleted == false).Select(x => x.CommentId).ToList();
            int CommentCount = 0;
            if (userIsLoggedIn == true)
            {
                var commentList = context.Comments.Where(x => x.ContentId == articleEntity.ContentId && x.UserId == userId && x.IsDeleted == false && x.IsPublic == false).Select(x => x.CommentId).ToList();
                var commentData = context.Comments.Where(x => x.ContentId == articleEntity.ContentId && x.UserId == userId && x.IsDeleted == false).Select(x => x.CommentId).ToList();
                var replyData = context.Comments.Where(x => commentData.Contains((long)x.ParentCommentId) && x.IsPublic == false && x.IsDeleted == false).ToList();
                CommentCount = commentList.Count() + replyData.Count() + publicEntity.Count();
            }
            else
            {
                CommentCount = publicEntity.Count();
            }

            //int CommentCount = context.Comments.Where(x => x.ContentId == articleEntity.ContentId && x.IsPublic == true && x.IsDeleted == false).Select(x => x.CommentId).Count();
            bool Like = context.ContentFlags.Where(x => x.ContentId == articleEntity.ContentId && x.MostLiked == true && x.UserId == userId && x.IsDeleted == false).Select(x => x.MostLiked).Distinct().Count() >= 1 ? true : false;
            bool Favourite1 = context.ContentFlags.Where(x => x.ContentId == articleEntity.ContentId && x.Favourite == true && x.UserId == userId && x.IsDeleted == false).Select(x => x.Favourite).Distinct().Count() >= 1 ? true : false;
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
                Description = articleEntity.Description,
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
        public List<ListingLogoDetailsModel> GetListingDetailByplayerId(string playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var decryptResult = _encryptionService.DecryptPlayerId(playerId);
            if (decryptResult.DecryptedPlayerId == null)
            {
                errorResponseModel.StatusCode = decryptResult.StatusCode;
                errorResponseModel.Message = decryptResult.Message;
                return new List<ListingLogoDetailsModel>();
            }

            long decryptplayerId = decryptResult.DecryptedPlayerId.Value;

            var listdetailEntity = new List<ListingLogoDetailsModel>();
            var listEntity1 = context.ListingDetails.Where(z => z.PlayerId == decryptplayerId && z.IsDeleted == false).OrderByDescending(d => d.UpdatedDate).ToList();

            if (listEntity1.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            listEntity1.ForEach(item =>
            {
                var imgmodel = new VideoImageModel();
                imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(item.CompanyLogoFilePath) ? item.CompanyLogoFilePath : item.CompanyLogoFilePath);
                imgmodel.Type = String.IsNullOrEmpty(item.CompanyLogoFilePath) ? "video" : "image";
                imgmodel.FileName = (imgmodel.url);

                listdetailEntity.Add(new ListingLogoDetailsModel
                {
                    ListingId = item.ListingId,
                    PlayerId = playerId,
                    CompanyName = item.CompanyName,
                    Description = Regex.Replace(item.Description, @"\t|\n|\r", ""),
                    CompanyLogoFileName = item.CompanyLogoFileName,
                    CompanyLogoFilePath = imgmodel.FileName,
                    Position = item.Position
                });
            });
            return listdetailEntity.OrderBy(z => z.Position).ToList();
        }



        /// <summary>
        /// Method to get ListingDetail by listingId
        /// </summary>
        /// <param name="listingId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public MobileListingDetailModel GetListingDetailById(long listingId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var listEntity = (from list in context.ListingDetails join
                              nation in context.Countries on list.NationId equals nation.CountryId
                              join state in context.States on list.StateId equals state.StateId
                              join city in context.Cities on list.CityId equals city.CityId
                              where list.IsDeleted == false && list.ListingId == listingId
                              select new MobileListingDetailModel
                              {
                                  ListingId = list.ListingId,
                                  PlayerId = _encryptionService.GetEncryptedId(list.PlayerId.ToString()),
                                  CompanyEmailId = list.CompanyEmailId,
                                  CompanyLogoFileName = list.CompanyLogoFileName,
                                  CompanyName = list.CompanyName,
                                  Description = Regex.Replace(list.Description, @"\t|\n|\r", ""),
                                  CompanyLogoFilePath = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(list.CompanyLogoFilePath) ? list.CompanyLogoFilePath : list.CompanyLogoFilePath),
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
            return listEntity;
        }



        /// <summary>
        /// Method is used to get To20ListingDetail by playerId
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="countNumber"></param>
        /// <returns></returns>
        public List<ListingLogoDetailsModel> GetTop20ListingDetailByplayerId(string playerId, int countNumber, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();

            var decryptResult = _encryptionService.DecryptPlayerId(playerId);
            if (decryptResult.DecryptedPlayerId == null)
            {
                errorResponseModel.StatusCode = decryptResult.StatusCode;
                errorResponseModel.Message = decryptResult.Message;
                return new List<ListingLogoDetailsModel>();
            }

            long decryptplayerId = decryptResult.DecryptedPlayerId.Value;

            var listdetailEntity = new List<ListingLogoDetailsModel>();
            var listEntity = context.ListingDetails.Where(z => z.PlayerId == decryptplayerId && z.IsDeleted == false).OrderByDescending(d => d.UpdatedDate).ToList();

            if (listEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }

            var tredingtoptwenty = listEntity.OrderByDescending(x => x.Position).Take(countNumber).ToList();
            tredingtoptwenty.ForEach(item =>
            {
                var imgmodel = new VideoImageModel();
                imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(item.CompanyLogoFilePath) ? item.CompanyLogoFilePath : item.CompanyLogoFilePath);
                imgmodel.Type = String.IsNullOrEmpty(item.CompanyLogoFilePath) ? "video" : "image";
                imgmodel.FileName = (imgmodel.url);

                listdetailEntity.Add(new ListingLogoDetailsModel
                {
                    ListingId = item.ListingId,
                    PlayerId = playerId,
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
        public MobileSearchDataModel GetMobileSearchDetailByplayerId(string playerId, string searchData, long userId, ref ErrorResponseModel errorResponseModel)
        {
            bool userIsLoggedIn = context.UserMasters.Where(z => z.UserId == userId && z.IsDeleted == false)
                                 .Select(z => z.IsLogin).FirstOrDefault() ?? false;
            errorResponseModel = new ErrorResponseModel();

            var mobileSearchDataModel = new MobileSearchDataModel
            {
                listingLogoDetailsModels = new List<ListingLogoDetailsModel>(),
                mobileContentDatas = new List<MobileContentData>(),
                categoryArticleModels = new List<CategoryArticleModel>(),
            };

            var decryptResult = _encryptionService.DecryptPlayerId(playerId);
            if (decryptResult.DecryptedPlayerId == null)
            {
                errorResponseModel.StatusCode = decryptResult.StatusCode;
                errorResponseModel.Message = decryptResult.Message;
                return mobileSearchDataModel;
            }

            long decryptplayerId = decryptResult.DecryptedPlayerId.Value;

            var listEntity = (from list in context.ListingDetails
                              join nation in context.Countries on list.NationId equals nation.CountryId
                              join ct in context.Cities on list.CityId equals ct.CityId
                              join st in context.States on list.StateId equals st.StateId
                              join user in context.UserMasters on list.CreatedBy equals (int)user.UserId
                              where list.IsDeleted == false && list.PlayerId == decryptplayerId && (list.CompanyName.ToLower().Contains(searchData.ToLower()) || list.Description.ToLower().Contains(searchData.ToLower()))
                              orderby list.UpdatedDate descending

                              select new MobileListingDetailModel
                              {
                                  ListingId = list.ListingId,
                                  PlayerId = playerId,
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
                    PlayerId = playerId,
                    CompanyName = item.CompanyName,
                    Description = Regex.Replace(item.Description, @"\t|\n|\r", ""),
                    CompanyLogoFileName = item.CompanyLogoFileName,
                    CompanyLogoFilePath = imgmodel.FileName,
                    Position = item.Position
                });
            });

            var contentIdsWithMapping = (from flag in context.AdvContentMappings
                                         where !flag.IsDeleted
                                            && (flag.Category.Name.ToLower().Contains(searchData.ToLower()) ||
                                                flag.Content.Title.ToLower().Contains(searchData.ToLower()) ||
                                                flag.Content.Description.ToLower().Contains(searchData.ToLower()))
                                         select flag.ContentId).Distinct();

            var tredingEntity = (from content in context.ContentDetails
                                 where content.ContentTypeId == ContentTypeHelper.VideoContentTypeId
                                       && content.Approved == true
                                       && content.PlayerId == decryptplayerId
                                       && (content.PlayerId == decryptplayerId)
                                       && (contentIdsWithMapping.Contains(content.ContentId) ||
                                           content.Title.ToLower().Contains(searchData.ToLower()) ||
                                           content.Description.ToLower().Contains(searchData.ToLower()))
                                 orderby contentIdsWithMapping.Contains(content.ContentId) descending
                                 select new
                                 {
                                     ContentId = content.ContentId,
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
                                                adv.ProductionFlag
                                            }).ToList();
                    var imgmodel = new VideoImageModel();
                    if (contentVideo.ContentFileName != null)
                    {
                        var content1 = new ContentMobileModel();
                        var hostname1 = GetHostName(contentVideo.ProductionFlag);

                        imgmodel.url = hostname1.TrimEnd('/') + (String.IsNullOrEmpty(contentVideo.ContentFilePath) ? contentVideo.ContentFilePath : contentVideo.ContentFilePath);
                        imgmodel.Type = String.IsNullOrEmpty(contentVideo.ContentFilePath) ? "video" : "image";
                        imgmodel.FileName = (imgmodel.url);

                        content1.ContentId = contentVideo.ContentId;
                        content1.FileName = contentVideo.ContentFileName;
                        content1.FilePath = hostname1 + contentVideo.ContentFilePath;
                        content1.Title = contentVideo.Title;
                        content1.Description = contentVideo.Description;
                        content1.Position = Helper.FirstHalfContentPostion;
                        content1.CategoryName = contentVideo.Category.Name;
                        content1.Thumbnail = hostname1 + contentVideo.Thumbnail1;//ThumbnailPath(imgmodel.url);
                        contentList.Add(content1);
                    }
                    if (contentVideo.ContentFileName1 != null)
                    {
                        var content2 = new ContentMobileModel();
                        var hostname12 = GetHostName(contentVideo.ProductionFlag);

                        var imgmodel1 = new VideoImageModel();
                        imgmodel1.url = hostname12.TrimEnd('/') + (String.IsNullOrEmpty(contentVideo.ContentFilePath1) ? contentVideo.ContentFilePath1 : contentVideo.ContentFilePath1);
                        imgmodel1.Type = String.IsNullOrEmpty(contentVideo.ContentFilePath1) ? "video" : "image";
                        imgmodel1.FileName = (imgmodel1.url);

                        content2.ContentId = contentVideo.ContentId;
                        content2.FileName = contentVideo.ContentFileName1;
                        content2.FilePath = hostname12 + contentVideo.ContentFilePath1;
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
                        var hostname123 = GetHostName(advcontent.ProductionFlag);

                        var imgmodel2 = new VideoImageModel();
                        imgmodel2.url = hostname123.TrimEnd('/') + (String.IsNullOrEmpty(advcontent.AdvertiseFilePath) ? advcontent.AdvertiseFilePath : advcontent.AdvertiseFilePath);
                        imgmodel2.Type = String.IsNullOrEmpty(advcontent.AdvertiseFilePath) ? "video" : "image";
                        imgmodel2.FileName = (imgmodel2.url);

                        model.AdvertiseContentId = advcontent.AdvertiseContentId;
                        model.FileName = advcontent.AdvertiseFileName;
                        model.FilePath = hostname123 + advcontent.AdvertiseFilePath;
                        model.Title = advcontent.Title;
                        model.Thumbnail = imgmodel2.url;
                        model.ContentId = advcontent.ContentId;
                        model.Position = advcontent.Position;
                        contentList.Add(model);
                    }
                    var hostname = GetHostName(contentVideo.ProductionFlag);

                    topModel.contentMobileModels = contentList.OrderBy(x => x.Position).ToList();
                    topModel.ContentId = contentVideo.ContentId;
                    topModel.Title = contentVideo.Title;
                    topModel.Description = contentVideo.Description;
                    topModel.Thumbnail = hostname + contentVideo.Thumbnail1;//ThumbnailPath(imgmodel.url);
                    topModel.CategoryId = contentVideo.CategoryId;
                    topModel.ViewNo = context.ContentViews.Where(x => x.ContentId == item.ContentId.ContentId && x.Trending == true).Select(x => x.Trending).Count();
                    topModel.ContentTypeId = contentVideo.ContentTypeId;
                    topModel.LikedNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId.ContentId && x.MostLiked == true && x.IsDeleted == false).Select(x => x.MostLiked).Count();
                    topModel.FavouriteNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId.ContentId && x.Favourite == true && x.IsDeleted == false).Select(x => x.Favourite).Count();
                    var publicEntity = context.Comments.Where(x => x.ContentId == item.ContentId.ContentId && x.IsPublic == true && x.IsDeleted == false).Select(x => x.CommentId).ToList();

                    if (userIsLoggedIn == true)
                    {
                        var commentList = context.Comments.Where(x => x.ContentId == item.ContentId.ContentId && x.UserId == userId && x.IsDeleted == false && x.IsPublic == false).Select(x => x.CommentId).ToList();
                        var commentData = context.Comments.Where(x => x.ContentId == item.ContentId.ContentId && x.UserId == userId && x.IsDeleted == false).Select(x => x.CommentId).ToList();
                        var replyData = context.Comments.Where(x => commentData.Contains((long)x.ParentCommentId) && x.IsPublic == false && x.IsDeleted == false).ToList();
                        topModel.CommentCount = commentList.Count() + replyData.Count() + publicEntity.Count();
                    }
                    else
                    {
                        topModel.CommentCount = publicEntity.Count();
                    }

                    // topModel.CommentCount = context.Comments.Where(x => x.ContentId == item.ContentId.ContentId).Select(x => x.CommentId).Count();
                    topModel.Liked = context.ContentFlags.Where(x => x.ContentId == item.ContentId.ContentId && x.MostLiked == true && x.UserId == userId && x.IsDeleted == false).Select(x => x.MostLiked).Distinct().Count() >= 1 ? true : false;
                    topModel.Favourite = context.ContentFlags.Where(x => x.ContentId == item.ContentId.ContentId && x.Favourite == true && x.UserId == userId && x.IsDeleted == false).Select(x => x.Favourite).Distinct().Count() >= 1 ? true : false;
                    topModel.CreatedDate = contentVideo.CreatedDate;

                    topList.Add(topModel);
                }

            }
            var newtoplist = topList.OrderByDescending(z => z.CreatedDate).ToList();
            var groupedContent = newtoplist.GroupBy(c => c.CategoryId)
                   .Select(g => new
                   {
                       Category = g.Key,
                       Contents = g.ToList()
                   }).ToList();

            var indexedContent = new List<MobileContentData>();

            foreach (var group in groupedContent)
            {
                int index = 0;
                foreach (var content in group.Contents)
                {
                    content.AutoIndex = index;
                    indexedContent.Add(content);
                    index++;
                }
            }

            mobileSearchDataModel.mobileContentDatas = indexedContent;

            List<MobileArticleCategoryData> mobileArticleList = new List<MobileArticleCategoryData>();
            var categoryEnitityList = (from content in context.ContentDetails
                                       where content.ContentTypeId == ContentTypeHelper.ArticleContentTypeId && content.IsDeleted == false && content.Approved == true && content.PlayerId == decryptplayerId
                                       && (content.Category.Name.ToLower().Contains(searchData.ToLower()) || content.Title.ToLower().Contains(searchData.ToLower()) || content.Description.ToLower().Contains(searchData.ToLower()))
                                       orderby content.UpdatedDate descending
                                       select new
                                       {
                                           content.ContentId,
                                           content.CategoryId,
                                           content.Category.Name,
                                           content.Category.DisplayOrder,

                                       }).ToList();
            var categoryList = categoryEnitityList.GroupBy(x => new { x.ContentId, x.CategoryId, x.Name, x.DisplayOrder }).Select(x => new { ContentId = x.Key.ContentId, CategoryId = x.Key.CategoryId, Name = x.Key.Name, DisplayOrder = x.Key.DisplayOrder, count = x.Count() }).ToList();
            if (categoryEnitityList.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            List<CategoryArticleModel> categoryArticleList = new List<CategoryArticleModel>();
            var distinctCategoryid = categoryList.DistinctBy(x => x.CategoryId).ToList();
            foreach (var item in distinctCategoryid)
            {

                var contentEntity = categoryEnitityList.Where(x => x.CategoryId == item.CategoryId).ToList();
                foreach (var item1 in contentEntity)
                {
                    var contentArticle = context.ContentDetails.FirstOrDefault(x => x.ContentId == item1.ContentId && x.Approved == true && x.IsDeleted == false);
                    if (contentArticle != null)
                    {
                        CategoryArticleModel articleModel = new CategoryArticleModel();
                        var hostname1 = GetHostName(contentArticle.ProductionFlag);

                        var imgmodel2 = new VideoImageModel();
                        imgmodel2.url = hostname1.TrimEnd('/') + (String.IsNullOrEmpty(contentArticle.ContentFilePath) ? contentArticle.ContentFilePath : contentArticle.ContentFilePath);
                        imgmodel2.Type = String.IsNullOrEmpty(contentArticle.ContentFilePath) ? "video" : "image";
                        imgmodel2.FileName = (imgmodel2.url);

                        articleModel.ContentId = contentArticle.ContentId;
                        articleModel.Title = contentArticle.Title;
                        articleModel.Description = contentArticle.Description;
                        articleModel.CategoryName = contentArticle.Category.Name;
                        articleModel.FileName = contentArticle.ContentFileName1;
                        articleModel.FilePath = imgmodel2.url;
                        articleModel.CategoryId = contentArticle.CategoryId;
                        articleModel.Thumbnail = hostname1 + contentArticle.Thumbnail1;
                        articleModel.ViewNo = context.ContentViews.Where(x => x.ContentId == item1.ContentId && x.Trending == true).Select(x => x.Trending).Count();
                        articleModel.ContentTypeId = contentArticle.ContentTypeId;
                        articleModel.LikedNo = context.ContentFlags.Where(x => x.ContentId == item1.ContentId && x.MostLiked == true && x.IsDeleted == false).Select(x => x.MostLiked).Count();
                        articleModel.FavouriteNo = context.ContentFlags.Where(x => x.ContentId == item1.ContentId && x.Favourite == true && x.IsDeleted == false).Select(x => x.Favourite).Count();
                        var publicEntity = context.Comments.Where(x => x.ContentId == item1.ContentId && x.IsPublic == true && x.IsDeleted == false).Select(x => x.CommentId).ToList();

                        if (userIsLoggedIn == true)
                        {
                            var commentList = context.Comments.Where(x => x.ContentId == item1.ContentId && x.UserId == userId && x.IsDeleted == false && x.IsPublic == false).Select(x => x.CommentId).ToList();
                            var commentData = context.Comments.Where(x => x.ContentId == item1.ContentId && x.UserId == userId && x.IsDeleted == false).Select(x => x.CommentId).ToList();
                            var replyData = context.Comments.Where(x => commentData.Contains((long)x.ParentCommentId) && x.IsPublic == false && x.IsDeleted == false).ToList();
                            articleModel.CommentCount = commentList.Count() + replyData.Count() + publicEntity.Count();
                        }
                        else
                        {
                            articleModel.CommentCount = publicEntity.Count();
                        }

                        //articleModel.CommentCount = context.Comments.Where(x => x.ContentId == item1.ContentId).Select(x => x.CommentId).Count();
                        articleModel.Liked = context.ContentFlags.Where(x => x.ContentId == item1.ContentId && x.MostLiked == true && x.UserId == userId && x.IsDeleted == false).Select(x => x.MostLiked).Distinct().Count() >= 1 ? true : false;
                        articleModel.Favourite = context.ContentFlags.Where(x => x.ContentId == item1.ContentId && x.Favourite == true && x.UserId == userId && x.IsDeleted == false).Select(x => x.Favourite).Distinct().Count() >= 1 ? true : false;
                        articleModel.CreatedDate = contentArticle.CreatedDate;
                        categoryArticleList.Add(articleModel);
                    }
                }
            }
            var newtoplist1 = categoryArticleList.OrderByDescending(z => z.CreatedDate).ToList();
            var groupedContent1 = newtoplist1.GroupBy(c => c.CategoryId)
                   .Select(g => new
                   {
                       Category = g.Key,
                       Contents = g.ToList()
                   }).ToList();

            var indexedContent1 = new List<CategoryArticleModel>();

            foreach (var group in groupedContent1)
            {
                int index = 0;
                foreach (var content in group.Contents)
                {
                    content.CategoryAutoIndex = index;
                    indexedContent1.Add(content);
                    index++;
                }
            }
            mobileSearchDataModel.categoryArticleModels = indexedContent1;
            return mobileSearchDataModel;
        }

        /// <summary>
        /// Method is used to add/edit comment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string AddMobileContentComment(ContentCommentModel1 model, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();

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
                comment.CreatedBy = (int?)model.UserId;
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
                userAuditLog.Description = "Mobile Content Comment Details Added Successfully";
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

            bool userIsLoggedIn = context.UserMasters.Where(z => z.UserId == UserId && z.IsDeleted == false)
                                .Select(z => z.IsLogin).FirstOrDefault() ?? false;

            List<ContentCommentModelData> commentModelList = new List<ContentCommentModelData>();
            List<ContentCommentModel> ContentcommentList = new List<ContentCommentModel>();
            CommentListData commentListData = new CommentListData();
            commentListData.commentCount = 0;
            var PubliccommentEntity = (from comment in context.Comments
                                       join content in context.ContentDetails on comment.ContentId equals content.ContentId
                                       join user in context.UserMasters on comment.UserId equals user.UserId
                                       where user.IsDeleted == false
                                       join contenttype in context.ContentTypes on comment.ContentTypeId equals contenttype.ContentTypeId
                                       join player in context.PlayerDetails on content.PlayerId equals player.PlayerId
                                       where player.IsDeleted == false
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
                                        where user.IsDeleted == false
                                        join contenttype in context.ContentTypes on comment.ContentTypeId equals contenttype.ContentTypeId
                                        join player in context.PlayerDetails on content.PlayerId equals player.PlayerId
                                        where player.IsDeleted == false
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

            if (PubliccommentEntity.Count == 0 && PrivatecommentEntity.Count == 0)
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
                    var model = new ContentCommentModel
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
                        UpdatedDate = item.UpdatedDate
                    };
                    commentList.Add(model);

                    List<ContentCommentModel> filteredReplies = new List<ContentCommentModel>();

                    // Fetch Public Replies
                    var publicReplies = (from comment in context.Comments
                                         join content in context.ContentDetails on comment.ContentId equals content.ContentId
                                         join user in context.UserMasters on comment.UserId equals user.UserId
                                         where user.IsDeleted == false
                                         join contenttype in context.ContentTypes on comment.ContentTypeId equals contenttype.ContentTypeId
                                         join player in context.PlayerDetails on content.PlayerId equals player.PlayerId
                                         where player.IsDeleted == false
                                         where comment.ContentId == contentId && comment.IsDeleted == false
                                         && comment.ParentCommentId == item.CommentId && comment.IsPublic == true
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
                                             UpdatedBy = comment.UpdatedBy
                                         }).ToList();

                    // Fetch Private Replies
                    if (item.UserId == UserId && userIsLoggedIn)
                    {
                        var privateReplies = (from comment in context.Comments
                                              join content in context.ContentDetails on comment.ContentId equals content.ContentId
                                              join user in context.UserMasters on comment.UserId equals user.UserId
                                              where user.IsDeleted == false
                                              join contenttype in context.ContentTypes on comment.ContentTypeId equals contenttype.ContentTypeId
                                              join player in context.PlayerDetails on content.PlayerId equals player.PlayerId
                                              where player.IsDeleted == false
                                              where comment.ContentId == contentId && comment.IsDeleted == false
                                              && comment.ParentCommentId == item.CommentId && comment.IsPublic == false
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
                                                  UpdatedBy = comment.UpdatedBy
                                              }).ToList();
                                   filteredReplies.AddRange(privateReplies);                       
                    }

                    filteredReplies.AddRange(publicReplies);
                    if (filteredReplies.Count != 0)
                    {
                        foreach (var item1 in filteredReplies)
                        {
                            var model1 = new ContentCommentModel
                            {
                                CommentId = item1.CommentId,
                                ContentId = item1.ContentId,
                                UserId = item1.UserId,
                                Comment1 = item1.Comment1,
                                ParentCommentId = item1.ParentCommentId,
                                DeviceId = item1.DeviceId,
                                Location = item1.Location,
                                Liked = item1.Liked,
                                ReplyedAdminName = item1.ReplyedAdminName,
                                CommentedUserId = item1.CommentedUserId,
                                Shared = item1.Shared,
                                IsPublic = item1.IsPublic,
                                EmailId = item1.EmailId,
                                CreatedDate = item1.CreatedDate,
                                CreatedBy = item1.CreatedBy,
                                FirstName = item1.FirstName,
                                LastName = item1.LastName,
                                FullName = item1.FirstName + " " + item1.LastName,
                                ContentTypeId = item1.ContentTypeId,
                                ContentTypeName = item1.ContentTypeName,
                                UpdatedBy = item1.UpdatedBy,
                                UpdatedDate = item1.UpdatedDate
                            };
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
        public List<ContentCommentModel> GetMobileCommentByPlayerId(string playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var decryptResult = _encryptionService.DecryptPlayerId(playerId);
            if (decryptResult.DecryptedPlayerId == null)
            {
                errorResponseModel.StatusCode = decryptResult.StatusCode;
                errorResponseModel.Message = decryptResult.Message;
                return new List<ContentCommentModel>();
            }

            long decryptplayerId = decryptResult.DecryptedPlayerId.Value;

            var contentEntity = (from comment in context.Comments
                                 join content in context.ContentDetails on comment.ContentId equals content.ContentId
                                 join user in context.UserMasters on comment.UserId equals user.UserId
                                 join contenttype in context.ContentTypes on comment.ContentTypeId equals contenttype.ContentTypeId
                                 join player in context.PlayerDetails on content.PlayerId equals player.PlayerId
                                 where content.PlayerId == decryptplayerId && comment.IsDeleted == false && comment.IsPublic == true
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
            return contentEntity.ToList();           
        }

        /// <summary>
        /// Method is used to add view flag
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string AddMobileViewCount(ContentModelView model, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            string message = "";
            var decryptResult = _encryptionService.DecryptPlayerId(model.PlayerId);
            if (decryptResult.DecryptedPlayerId == null)
            {
                errorResponseModel.StatusCode = decryptResult.StatusCode;
                errorResponseModel.Message = decryptResult.Message;
                return null;
            }

            long decryptplayerId = decryptResult.DecryptedPlayerId.Value;

            if (model.ContentViewId == 0)
            {
                ContentView content = new ContentView();
                content.PlayerId = decryptplayerId;
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
            errorResponseModel = new ErrorResponseModel();
            string message = "";
            var decryptResult = _encryptionService.DecryptPlayerId(model.PlayerId);
            if (decryptResult.DecryptedPlayerId == null)
            {
                errorResponseModel.StatusCode = decryptResult.StatusCode;
                errorResponseModel.Message = decryptResult.Message;
                return null;
            }

            long decryptplayerId = decryptResult.DecryptedPlayerId.Value;

            var contentEntity = context.ContentFlags.FirstOrDefault(x => x.PlayerId == decryptplayerId && x.ContentId == model.ContentId && x.UserId == model.UserId && x.IsDeleted == false);
            if (contentEntity == null)
            {
                ContentFlag contentFlag = new ContentFlag();
                contentFlag.PlayerId = decryptplayerId;
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
                var contentFlagEntity = context.ContentFlags.FirstOrDefault(x => x.PlayerId == decryptplayerId && x.ContentId == model.ContentId && x.UserId == model.UserId);
                if (contentFlagEntity != null)
                {
                    contentFlagEntity.PlayerId = decryptplayerId;
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
        public PlayerMobileDataModel GetMobileSplashScreenByplayerId(string playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var decryptResult = _encryptionService.DecryptPlayerId(playerId);
            if (decryptResult.DecryptedPlayerId == null)
            {
                errorResponseModel.StatusCode = decryptResult.StatusCode;
                errorResponseModel.Message = decryptResult.Message;
                return new PlayerMobileDataModel();
            }

            long decryptplayerId = decryptResult.DecryptedPlayerId.Value;

            var playerSplashEntity = (from content in context.PlayerData
                                      where content.PlayerId == decryptplayerId && content.IsDeleted == false && content.FileCategoryTypeId == FileCategoryTypeHelper.SplashScreenTypeId
                                      select new PlayerMobileDataModel
                                      {
                                        PlayerDataId=content.PlayerDataId,
                                        PlayerId =playerId,
                                        FileName =content.FileName,
                                        FilePath =_configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(content.FilePath) ? content.FilePath : content.FilePath)
                                      }).FirstOrDefault();
            if (playerSplashEntity == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return new PlayerMobileDataModel();
            }
            return playerSplashEntity;
        }


        /// <summary>
        /// Method is used to get logo image by playerid
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public PlayerMobileDataModel GetMobileLogoImageByplayerId(string playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var decryptResult = _encryptionService.DecryptPlayerId(playerId);
            if (decryptResult.DecryptedPlayerId == null)
            {
                errorResponseModel.StatusCode = decryptResult.StatusCode;
                errorResponseModel.Message = decryptResult.Message;
                return new PlayerMobileDataModel();
            }

            long decryptplayerId = decryptResult.DecryptedPlayerId.Value;

            var playerLogoEntity = (from content in context.PlayerData
                                    where content.PlayerId == decryptplayerId && content.IsDeleted == false && content.FileCategoryTypeId == FileCategoryTypeHelper.LogoTypeId
                                    select new PlayerMobileDataModel
                                    {
                                       PlayerDataId =content.PlayerDataId,
                                       PlayerId =playerId,
                                       FileName =content.FileName,
                                       FilePath= _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(content.FilePath) ? content.FilePath : content.FilePath)
                                    }).FirstOrDefault();
            if (playerLogoEntity == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return new PlayerMobileDataModel();
            }
            return playerLogoEntity;
        }




        /// <summary>
        /// Method is used to get slide image by playerid
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<PlayerMobileDataModel> GetMobileSlideImageByplayerId(string playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var decryptResult = _encryptionService.DecryptPlayerId(playerId);
            if (decryptResult.DecryptedPlayerId == null)
            {
                errorResponseModel.StatusCode = decryptResult.StatusCode;
                errorResponseModel.Message = decryptResult.Message;
                return new List<PlayerMobileDataModel>();
            }

            long decryptplayerId = decryptResult.DecryptedPlayerId.Value;

            var playerSlideImageEntity = (from content in context.PlayerData
                                          where content.PlayerId == decryptplayerId && content.IsDeleted == false && content.FileCategoryTypeId == FileCategoryTypeHelper.SlideImageTypeId
                                          select new PlayerMobileDataModel
                                          {
                                             PlayerDataId = content.PlayerDataId,
                                             PlayerId = playerId,
                                             FileName= content.FileName,
                                             FilePath = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(content.FilePath) ? content.FilePath : content.FilePath)
                                          }).ToList();

            if (playerSlideImageEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return new List<PlayerMobileDataModel>();
            }
            return playerSlideImageEntity.ToList();          
        }



        /// <summary>
        /// Method is used to get the liked data by playerId and userId
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PlayerMobileLikeFavouriteData GetMobileLikeVideoArticle(string playerId, long userId, ref ErrorResponseModel errorResponseModel)
        {
            bool userIsLoggedIn = context.UserMasters.Where(z => z.UserId == userId && z.IsDeleted == false)
                                 .Select(z => z.IsLogin).FirstOrDefault() ?? false;
            errorResponseModel = new ErrorResponseModel();
            var decryptResult = _encryptionService.DecryptPlayerId(playerId);
            if (decryptResult.DecryptedPlayerId == null)
            {
                errorResponseModel.StatusCode = decryptResult.StatusCode;
                errorResponseModel.Message = decryptResult.Message;
                return new PlayerMobileLikeFavouriteData();
            }

            long decryptplayerId = decryptResult.DecryptedPlayerId.Value;

            PlayerMobileLikeFavouriteData playerMobileLikeFavouriteData = new PlayerMobileLikeFavouriteData();
            var likedVideoEntity = (from flag in context.ContentFlags
                                    join player in context.PlayerDetails on flag.PlayerId equals player.PlayerId
                                    join content in context.ContentDetails on flag.ContentId equals content.ContentId
                                    join type in context.ContentTypes on flag.ContentTypeId equals type.ContentTypeId
                                    where flag.IsDeleted == false && flag.MostLiked == true && flag.ContentTypeId == ContentTypeHelper.VideoContentTypeId && flag.PlayerId == decryptplayerId && content.Approved == true && flag.UserId == userId
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
                                            adv.ProductionFlag
                                        }).ToList();
                var contentVideo = context.ContentDetails.Include(x => x.Category).FirstOrDefault(x => x.ContentId == item.ContentId && x.IsDeleted == false);
                var imgmodel = new VideoImageModel();
                if (contentVideo.ContentFileName != null)
                {
                    var content1 = new ContentMobileModel();
                    var hostname1 = GetHostName(contentVideo.ProductionFlag);

                    imgmodel.url =hostname1. TrimEnd('/') + (String.IsNullOrEmpty(contentVideo.ContentFilePath) ? contentVideo.ContentFilePath : contentVideo.ContentFilePath);
                    imgmodel.Type = String.IsNullOrEmpty(contentVideo.ContentFilePath) ? "video" : "image";
                    imgmodel.FileName = (imgmodel.url);

                    content1.ContentId = contentVideo.ContentId;
                    content1.FileName = contentVideo.ContentFileName;
                    content1.FilePath = hostname1 + contentVideo.ContentFilePath;
                    content1.Title = contentVideo.Title;
                    content1.Description = contentVideo.Description;
                    content1.Position = Helper.FirstHalfContentPostion;
                    content1.CategoryName = contentVideo.Category.Name;
                    content1.Thumbnail = hostname1 + contentVideo.Thumbnail1; 
                    contentList.Add(content1);
                }
                if (contentVideo.ContentFileName1 != null)
                {
                    var content2 = new ContentMobileModel();
                    var hostname12 = GetHostName(contentVideo.ProductionFlag);

                    var imgmodel1 = new VideoImageModel();
                    imgmodel1.url = hostname12.TrimEnd('/') + (String.IsNullOrEmpty(contentVideo.ContentFilePath1) ? contentVideo.ContentFilePath1 : contentVideo.ContentFilePath1);
                    imgmodel1.Type = String.IsNullOrEmpty(contentVideo.ContentFilePath1) ? "video" : "image";
                    imgmodel1.FileName = (imgmodel1.url);

                    content2.ContentId = contentVideo.ContentId;
                    content2.FileName = contentVideo.ContentFileName1;
                    content2.FilePath = hostname12 + contentVideo.ContentFilePath1;
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
                    var hostname123 = GetHostName(advcontent.ProductionFlag);

                    imgmodel2.url = hostname123.TrimEnd('/') + (String.IsNullOrEmpty(advcontent.AdvertiseFilePath) ? advcontent.AdvertiseFilePath : advcontent.AdvertiseFilePath);
                    imgmodel2.Type = String.IsNullOrEmpty(advcontent.AdvertiseFilePath) ? "video" : "image";
                    imgmodel2.FileName = (imgmodel2.url);
                    model.AdvertiseContentId = advcontent.AdvertiseContentId;
                    model.FileName = advcontent.AdvertiseFileName;
                    model.FilePath = hostname123 + advcontent.AdvertiseFilePath;
                    model.Title = advcontent.Title;
                    model.Thumbnail = imgmodel2.url;
                    model.ContentId = advcontent.ContentId;
                    model.Position = advcontent.Position;
                    contentList.Add(model);
                }
                var hostname = GetHostName(contentVideo.ProductionFlag);

                topModel.contentMobileModels = contentList.OrderBy(x => x.Position).ToList();
                topModel.ContentId = contentVideo.ContentId;
                topModel.Title = contentVideo.Title;
                topModel.Description = contentVideo.Description;
                topModel.Thumbnail =hostname + contentVideo.Thumbnail1;
                topModel.CategoryId = contentVideo.CategoryId;
                topModel.ViewNo = context.ContentViews.Where(x => x.ContentId == item.ContentId && x.Trending == true).Select(x => x.Trending).Count();
                topModel.ContentTypeId = contentVideo.ContentTypeId;
                topModel.LikedNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.MostLiked == true && x.IsDeleted == false).Select(x => x.MostLiked).Count();
                topModel.FavouriteNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.Favourite == true && x.IsDeleted == false).Select(x => x.Favourite).Count();
                var publicEntity = context.Comments.Where(x => x.ContentId == item.ContentId && x.IsPublic == true && x.IsDeleted == false).Select(x => x.CommentId).ToList();

                if (userIsLoggedIn == true)
                {
                    var commentList = context.Comments.Where(x => x.ContentId == item.ContentId && x.UserId == userId && x.IsDeleted == false && x.IsPublic == false).Select(x => x.CommentId).ToList();
                    var commentData = context.Comments.Where(x => x.ContentId == item.ContentId && x.UserId == userId && x.IsDeleted == false).Select(x => x.CommentId).ToList();
                    var replyData = context.Comments.Where(x => commentData.Contains((long)x.ParentCommentId) && x.IsPublic == false && x.IsDeleted == false).ToList();
                    topModel.CommentCount = commentList.Count() + replyData.Count() + publicEntity.Count();
                }
                else
                {
                    topModel.CommentCount = publicEntity.Count();
                }

                // topModel.CommentCount = context.Comments.Where(x => x.ContentId == item.ContentId).Select(x => x.CommentId).Count();
                topModel.Liked = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.MostLiked == true && x.UserId == userId && x.IsDeleted == false).Select(x => x.MostLiked).Distinct().Count() >= 1 ? true : false;
                topModel.Favourite = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.Favourite == true && x.UserId == userId && x.IsDeleted == false).Select(x => x.Favourite).Distinct().Count() >= 1 ? true : false;
                topModel.CreatedDate = contentVideo.CreatedDate;
                topList.Add(topModel);
            }

            var newtoplist = topList.OrderByDescending(z => z.CreatedDate).ToList();
            var groupedContent = newtoplist.GroupBy(c => c.CategoryId)
                   .Select(g => new
                   {
                       Category = g.Key,
                       Contents = g.ToList()
                   }).ToList();

            var indexedContent = new List<MobileContentData>();

            foreach (var group in groupedContent)
            {
                int index = 0;
                foreach (var content in group.Contents)
                {
                    content.AutoIndex = index;
                    indexedContent.Add(content);
                    index++;
                }
            }

            playerMobileLikeFavouriteData.videoContentData = indexedContent;

            var likedArticleEntity = (from flag in context.ContentFlags
                                      join player in context.PlayerDetails on flag.PlayerId equals player.PlayerId
                                      join content in context.ContentDetails on flag.ContentId equals content.ContentId
                                      join type in context.ContentTypes on flag.ContentTypeId equals type.ContentTypeId
                                      where flag.IsDeleted == false && flag.MostLiked == true && flag.ContentTypeId == ContentTypeHelper.ArticleContentTypeId && flag.PlayerId == decryptplayerId && content.Approved == true && flag.UserId == userId
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
                    var hostname1 = GetHostName(contentArticle.ProductionFlag);

                    var imgmodel2 = new VideoImageModel();
                    imgmodel2.url = hostname1.TrimEnd('/') + (String.IsNullOrEmpty(contentArticle.ContentFilePath) ? contentArticle.ContentFilePath : contentArticle.ContentFilePath);
                    imgmodel2.Type = String.IsNullOrEmpty(contentArticle.ContentFilePath) ? "video" : "image";
                    imgmodel2.FileName = (imgmodel2.url);

                    articleModel.ContentId = contentArticle.ContentId;
                    articleModel.Title = contentArticle.Title;
                    articleModel.Thumbnail = hostname1 + contentArticle.Thumbnail1;
                    articleModel.Description = contentArticle.Description;
                    articleModel.CategoryId = contentArticle.CategoryId;
                    articleModel.CategoryName = contentArticle.Category.Name;
                    articleModel.FileName = contentArticle.ContentFileName;
                    articleModel.FilePath = imgmodel2.FileName;
                    articleModel.ViewNo = context.ContentViews.Where(x => x.ContentId == item.ContentId && x.Trending == true).Select(x => x.Trending).Count();
                    articleModel.ContentTypeId = contentArticle.ContentTypeId;
                    articleModel.LikedNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.MostLiked == true && x.IsDeleted == false).Select(x => x.MostLiked).Count();
                    articleModel.FavouriteNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.Favourite == true && x.IsDeleted == false).Select(x => x.Favourite).Count();
                    var publicEntity = context.Comments.Where(x => x.ContentId == item.ContentId && x.IsPublic == true && x.IsDeleted == false).Select(x => x.CommentId).ToList();

                    if (userIsLoggedIn == true)
                    {
                        var commentList = context.Comments.Where(x => x.ContentId == item.ContentId && x.UserId == userId && x.IsDeleted == false && x.IsPublic == false).Select(x => x.CommentId).ToList();
                        var commentData = context.Comments.Where(x => x.ContentId == item.ContentId && x.UserId == userId && x.IsDeleted == false).Select(x => x.CommentId).ToList();
                        var replyData = context.Comments.Where(x => commentData.Contains((long)x.ParentCommentId) && x.IsPublic == false && x.IsDeleted == false).ToList();
                        articleModel.CommentCount = commentList.Count() + replyData.Count() + publicEntity.Count();
                    }
                    else
                    {
                        articleModel.CommentCount = publicEntity.Count();
                    }

                    //articleModel.CommentCount = context.Comments.Where(x => x.ContentId == item.ContentId).Select(x => x.CommentId).Count();
                    articleModel.Liked = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.MostLiked == true && x.UserId == userId && x.IsDeleted == false).Select(x => x.MostLiked).Distinct().Count() >= 1 ? true : false;
                    articleModel.Favourite = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.Favourite == true && x.UserId == userId && x.IsDeleted == false).Select(x => x.Favourite).Distinct().Count() >= 1 ? true : false;
                    articleModel.CreatedDate = contentArticle.CreatedDate;
                    categoryArticleList.Add(articleModel);
                }
            }
            var newtoplist1 = categoryArticleList.OrderByDescending(z => z.CreatedDate).ToList();
            var groupedContent1 = newtoplist1.GroupBy(c => c.CategoryId)
                   .Select(g => new
                   {
                       Category = g.Key,
                       Contents = g.ToList()
                   }).ToList();

            var indexedContent1 = new List<CategoryArticleModel>();

            foreach (var group in groupedContent1)
            {
                int index = 0;
                foreach (var content in group.Contents)
                {
                    content.CategoryAutoIndex = index;
                    indexedContent1.Add(content);
                    index++;
                }
            }
            playerMobileLikeFavouriteData.articleContentData = indexedContent1;
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
        public PlayerMobileLikeFavouriteData GetMobileFavouriteVideoArticle(string playerId, long userId, ref ErrorResponseModel errorResponseModel)
        {
            bool userIsLoggedIn = context.UserMasters.Where(z => z.UserId == userId && z.IsDeleted == false)
                                 .Select(z => z.IsLogin).FirstOrDefault() ?? false;

            errorResponseModel = new ErrorResponseModel();
            var decryptResult = _encryptionService.DecryptPlayerId(playerId);
            if (decryptResult.DecryptedPlayerId == null)
            {
                errorResponseModel.StatusCode = decryptResult.StatusCode;
                errorResponseModel.Message = decryptResult.Message;
                return new PlayerMobileLikeFavouriteData();
            }

            long decryptplayerId = decryptResult.DecryptedPlayerId.Value;

            PlayerMobileLikeFavouriteData playerMobileLikeFavouriteData = new PlayerMobileLikeFavouriteData();
            var likedVideoEntity = (from flag in context.ContentFlags
                                    join player in context.PlayerDetails on flag.PlayerId equals player.PlayerId
                                    join content in context.ContentDetails on flag.ContentId equals content.ContentId
                                    join type in context.ContentTypes on flag.ContentTypeId equals type.ContentTypeId
                                    where flag.IsDeleted == false && flag.Favourite == true && flag.ContentTypeId == ContentTypeHelper.VideoContentTypeId && flag.PlayerId == decryptplayerId && content.Approved == true && flag.UserId == userId
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
                                            adv.ProductionFlag
                                        }).ToList();
                var contentVideo = context.ContentDetails.Include(x => x.Category).FirstOrDefault(x => x.ContentId == item.ContentId && x.IsDeleted == false);
                var imgmodel = new VideoImageModel();
                if (contentVideo.ContentFileName != null)
                {
                    var content1 = new ContentMobileModel();
                    var hostname1 = GetHostName(contentVideo.ProductionFlag);

                    imgmodel.url = hostname1.TrimEnd('/') + (String.IsNullOrEmpty(contentVideo.ContentFilePath) ? contentVideo.ContentFilePath : contentVideo.ContentFilePath);
                    imgmodel.Type = String.IsNullOrEmpty(contentVideo.ContentFilePath) ? "video" : "image";
                    imgmodel.FileName = (imgmodel.url);

                    content1.ContentId = contentVideo.ContentId;
                    content1.FileName = contentVideo.ContentFileName;
                    content1.FilePath = hostname1 + contentVideo.ContentFilePath;
                    content1.Title = contentVideo.Title;
                    content1.Description = contentVideo.Description;
                    content1.Position = Helper.FirstHalfContentPostion;
                    content1.CategoryName = contentVideo.Category.Name;
                    content1.Thumbnail = hostname1 + contentVideo.Thumbnail1;
                    contentList.Add(content1);
                }
                if (contentVideo.ContentFileName1 != null)
                {
                    var content2 = new ContentMobileModel();
                    var imgmodel1 = new VideoImageModel();
                    var hostname12 = GetHostName(contentVideo.ProductionFlag);

                    imgmodel1.url = hostname12.TrimEnd('/') + (String.IsNullOrEmpty(contentVideo.ContentFilePath1) ? contentVideo.ContentFilePath1 : contentVideo.ContentFilePath1);
                    imgmodel1.Type = String.IsNullOrEmpty(contentVideo.ContentFilePath1) ? "video" : "image";
                    imgmodel1.FileName = (imgmodel1.url);

                    content2.ContentId = contentVideo.ContentId;
                    content2.FileName = contentVideo.ContentFileName1;
                    content2.FilePath = hostname12 + contentVideo.ContentFilePath1;
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
                    var hostname123 = GetHostName(advcontent.ProductionFlag);

                    imgmodel2.url = hostname123.TrimEnd('/') + (String.IsNullOrEmpty(advcontent.AdvertiseFilePath) ? advcontent.AdvertiseFilePath : advcontent.AdvertiseFilePath);
                    imgmodel2.Type = String.IsNullOrEmpty(advcontent.AdvertiseFilePath) ? "video" : "image";
                    imgmodel2.FileName = (imgmodel2.url);
                    model.AdvertiseContentId = advcontent.AdvertiseContentId;
                    model.FileName = advcontent.AdvertiseFileName;
                    model.FilePath = hostname123 + advcontent.AdvertiseFilePath;
                    model.Title = advcontent.Title;
                    model.Thumbnail = imgmodel2.url;
                    model.ContentId = advcontent.ContentId;
                    model.Position = advcontent.Position;
                    contentList.Add(model);
                }
                var hostname = GetHostName(contentVideo.ProductionFlag);

                topModel.contentMobileModels = contentList.OrderBy(x => x.Position).ToList();
                topModel.ContentId = contentVideo.ContentId;
                topModel.Title = contentVideo.Title;
                topModel.Description = contentVideo.Description;
                topModel.Thumbnail = hostname + contentVideo.Thumbnail1;
                topModel.CategoryId = contentVideo.CategoryId;
                topModel.ViewNo = context.ContentViews.Where(x => x.ContentId == item.ContentId && x.Trending == true).Select(x => x.Trending).Count();
                topModel.ContentTypeId = contentVideo.ContentTypeId;
                topModel.LikedNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.MostLiked == true && x.IsDeleted == false).Select(x => x.MostLiked).Count();
                topModel.FavouriteNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.Favourite == true && x.IsDeleted == false).Select(x => x.Favourite).Count();
                var publicEntity = context.Comments.Where(x => x.ContentId == item.ContentId && x.IsPublic == true && x.IsDeleted == false).Select(x => x.CommentId).ToList();

                if (userIsLoggedIn == true)
                {
                    var commentList = context.Comments.Where(x => x.ContentId == item.ContentId && x.UserId == userId && x.IsDeleted == false && x.IsPublic == false).Select(x => x.CommentId).ToList();
                    var commentData = context.Comments.Where(x => x.ContentId == item.ContentId && x.UserId == userId && x.IsDeleted == false).Select(x => x.CommentId).ToList();
                    var replyData = context.Comments.Where(x => commentData.Contains((long)x.ParentCommentId) && x.IsPublic == false && x.IsDeleted == false).ToList();
                    topModel.CommentCount = commentList.Count() + replyData.Count() + publicEntity.Count();
                }
                else
                {
                    topModel.CommentCount = publicEntity.Count();
                }

                //topModel.CommentCount = context.Comments.Where(x => x.ContentId == item.ContentId).Select(x => x.CommentId).Count();
                topModel.Liked = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.MostLiked == true && x.UserId == userId).Select(x => x.MostLiked).Distinct().Count() >= 1 ? true : false;
                topModel.Favourite = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.Favourite == true && x.UserId == userId).Select(x => x.Favourite).Distinct().Count() >= 1 ? true : false;
                topModel.CreatedDate = contentVideo.CreatedDate;
                topList.Add(topModel);
            }
            var newtoplist = topList.OrderByDescending(z => z.CreatedDate).ToList();
            var groupedContent = newtoplist.GroupBy(c => c.CategoryId)
                   .Select(g => new
                   {
                       Category = g.Key,
                       Contents = g.ToList()
                   }).ToList();

            var indexedContent = new List<MobileContentData>();

            foreach (var group in groupedContent)
            {
                int index = 0;
                foreach (var content in group.Contents)
                {
                    content.AutoIndex = index;
                    indexedContent.Add(content);
                    index++;
                }
            }

            playerMobileLikeFavouriteData.videoContentData = indexedContent;

            var likedArticleEntity = (from flag in context.ContentFlags
                                      join player in context.PlayerDetails on flag.PlayerId equals player.PlayerId
                                      join content in context.ContentDetails on flag.ContentId equals content.ContentId
                                      join type in context.ContentTypes on flag.ContentTypeId equals type.ContentTypeId
                                      where flag.IsDeleted == false && flag.Favourite == true && flag.ContentTypeId == ContentTypeHelper.ArticleContentTypeId && flag.PlayerId == decryptplayerId && content.Approved == true && flag.UserId == userId
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
                    var hostname1 = GetHostName(contentArticle.ProductionFlag);

                    var imgmodel2 = new VideoImageModel();
                    imgmodel2.url =hostname1.TrimEnd('/') + (String.IsNullOrEmpty(contentArticle.ContentFilePath) ? contentArticle.ContentFilePath : contentArticle.ContentFilePath);
                    imgmodel2.Type = String.IsNullOrEmpty(contentArticle.ContentFilePath) ? "video" : "image";
                    imgmodel2.FileName = (imgmodel2.url);
                    articleModel.ContentId = contentArticle.ContentId;
                    articleModel.Title = contentArticle.Title;
                    articleModel.Description = contentArticle.Description;
                    articleModel.CategoryName = contentArticle.Category.Name;
                    articleModel.FileName = contentArticle.ContentFileName;
                    articleModel.FilePath = imgmodel2.url;
                    articleModel.CategoryId = contentArticle.CategoryId;
                    articleModel.Thumbnail = hostname1 + contentArticle.Thumbnail1;
                    articleModel.ViewNo = context.ContentViews.Where(x => x.ContentId == item.ContentId && x.Trending == true).Select(x => x.Trending).Count();
                    articleModel.ContentTypeId = contentArticle.ContentTypeId;
                    articleModel.LikedNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.MostLiked == true && x.IsDeleted == false).Select(x => x.MostLiked).Count();
                    articleModel.FavouriteNo = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.Favourite == true && x.IsDeleted == false).Select(x => x.Favourite).Count();
                    var publicEntity = context.Comments.Where(x => x.ContentId == item.ContentId && x.IsPublic == true && x.IsDeleted == false).Select(x => x.CommentId).ToList();

                    if (userIsLoggedIn == true)
                    {
                        var commentList = context.Comments.Where(x => x.ContentId == item.ContentId && x.UserId == userId && x.IsDeleted == false && x.IsPublic == false).Select(x => x.CommentId).ToList();
                        var commentData = context.Comments.Where(x => x.ContentId == item.ContentId && x.UserId == userId && x.IsDeleted == false).Select(x => x.CommentId).ToList();
                        var replyData = context.Comments.Where(x => commentData.Contains((long)x.ParentCommentId) && x.IsPublic == false && x.IsDeleted == false).ToList();
                        articleModel.CommentCount = commentList.Count() + replyData.Count() + publicEntity.Count();
                    }
                    else
                    {
                        articleModel.CommentCount = publicEntity.Count();
                    }

                    // articleModel.CommentCount = context.Comments.Where(x => x.ContentId == item.ContentId).Select(x => x.CommentId).Count();
                    articleModel.Liked = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.MostLiked == true && x.UserId == userId).Select(x => x.MostLiked).Distinct().Count() >= 1 ? true : false;
                    articleModel.Favourite = context.ContentFlags.Where(x => x.ContentId == item.ContentId && x.Favourite == true && x.UserId == userId).Select(x => x.Favourite).Distinct().Count() >= 1 ? true : false;
                    articleModel.CreatedDate = contentArticle.CreatedDate;
                    categoryArticleList.Add(articleModel);
                }
            }
            var newtoplist1 = categoryArticleList.OrderByDescending(z => z.CreatedDate).ToList();
            var groupedContent1 = newtoplist1.GroupBy(c => c.CategoryId)
                   .Select(g => new
                   {
                       Category = g.Key,
                       Contents = g.ToList()
                   }).ToList();

            var indexedContent1 = new List<CategoryArticleModel>();

            foreach (var group in groupedContent1)
            {
                int index = 0;
                foreach (var content in group.Contents)
                {
                    content.CategoryAutoIndex = index;
                    indexedContent1.Add(content);
                    index++;
                }
            }
            playerMobileLikeFavouriteData.articleContentData = indexedContent1;
            if (likedVideoEntity.Count == 0 && likedArticleEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            return playerMobileLikeFavouriteData;
        }



        /// <summary>
        /// Method is used to GetMobileCommentCount by contentId,userid
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
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

            if (PubliccommentEntity.Count == 0 && PrivatecommentEntity.Count == 0)
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


        /// <summary>
        /// Method is used to get the GetAllCategoryList
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<ExploreData> GetAllCategoryList(string playerId, long userId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var trendingCategoryId = 1;
            var decryptResult = _encryptionService.DecryptPlayerId(playerId);
            if (decryptResult.DecryptedPlayerId == null)
            {
                errorResponseModel.StatusCode = decryptResult.StatusCode;
                errorResponseModel.Message = decryptResult.Message;
                return new List<ExploreData>();
            }

            long decryptplayerId = decryptResult.DecryptedPlayerId.Value;

            var contentEntityList = (from content in context.ContentDetails
                                     join category in context.Categories on content.CategoryId equals category.CategoryId
                                     join contenttype in context.ContentTypes on content.ContentTypeId equals contenttype.ContentTypeId
                                     where content.PlayerId == decryptplayerId && content.IsDeleted == false
                                     && content.ContentTypeId != 2 orderby contenttype.ContentTypeId 
                                     select new ExploreData
                                     {
                                         Id = category.CategoryId == trendingCategoryId ? 1 : 0,
                                         CategoryName = category.Name,
                                         CategoryId = content.CategoryId,
                                         ContentTypeId = contenttype.ContentTypeId
                                     }).ToList();

            var data = contentEntityList.DistinctBy(x => x.CategoryId).ToList();
            var processedCategoryContentTypePairs = new HashSet<(int CategoryId, int ContentTypeId)>();
            var finalList = new List<ExploreData>();

            foreach (var entity in data)
            {
                if (!processedCategoryContentTypePairs.Contains(((int CategoryId, int ContentTypeId))(entity.CategoryId, entity.ContentTypeId)))
                {
                    processedCategoryContentTypePairs.Add(((int CategoryId, int ContentTypeId))(entity.CategoryId, entity.ContentTypeId));
                    finalList.Add(entity);
                }
            }

            var nextStateId = 2;
            foreach (var item in finalList)
            {
                if (item.Id == 0)
                {
                    item.Id = nextStateId++;
                }
            }

            finalList.Insert(0, new ExploreData { Id = 1, CategoryName = "Trending", CategoryId = 0, ContentTypeId = 1 });
            finalList.Add(new ExploreData { Id = nextStateId++, CategoryName = "Articles", CategoryId = null, ContentTypeId = 2 });
            finalList.Add(new ExploreData { Id = nextStateId++, CategoryName = "Listing", CategoryId = null, ContentTypeId = null });
            finalList.Add(new ExploreData { Id = nextStateId++, CategoryName = "Social Brand", CategoryId = null, ContentTypeId = null });
            return finalList;
        }    
    }
}
