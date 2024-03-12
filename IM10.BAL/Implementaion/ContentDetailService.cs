using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Options;
using Serilog;
using StartUpX.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML.Messaging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace IM10.BAL.Implementaion
{
    /// <summary>
    /// This is implementation  for the contentDetails operations 
    /// </summary>
    public class ContentDetailService : IContentDetailService
    {
        IM10DbContext context;
        private ConfigurationModel _configuration;
        private readonly IUserAuditLogService _userAuditLogService;
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Creating constructor and injection dbContext
        /// </summary>
        /// <param name="_context"></param>
        public ContentDetailService(IM10DbContext _context, INotificationService notificationService,IOptions<ConfigurationModel> hostName, IUserAuditLogService userAuditLogService)
        {
            context = _context;
            this._configuration = hostName.Value;
            _userAuditLogService = userAuditLogService;
            _notificationService = notificationService;
        }


        /// <summary>
        /// Method to get contentDetails by userId
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public ContentDetailModel GetContentDetailById(long contentId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var contentEntity = (from content in context.ContentDetails
                                 join
                                 category in context.Categories on content.CategoryId equals category.CategoryId
                                 join subcategory in context.SubCategories on content.SubCategoryId equals subcategory.SubCategoryId
                                 join player in context.PlayerDetails on content.PlayerId equals player.PlayerId
                                 join contenttype in context.ContentTypes on content.ContentTypeId equals contenttype.ContentTypeId
                                 join language in context.Languages on content.LanguageId equals language.LanguageId
                                 where content.ContentId == contentId && content.IsDeleted == false
                                 select new ContentDetailModel
                                 {
                                     ContentId = (int)content.ContentId,
                                     ContentFileName = content.ContentFileName,
                                     ContentFilePath = content.ContentFilePath,
                                     Title = content.Title,
                                     ContentFileName1 = content.ContentFileName1,
                                     ContentFilePath1 = content.ContentFilePath1,
                                     Description = content.Description,
                                     CreatedDate = content.CreatedDate,
                                     CreatedBy = content.CreatedBy,
                                     CategoryId = content.CategoryId,
                                     CategoryName = category.Name,
                                     SubCategoryId = content.SubCategoryId,
                                     SubCategoryName = subcategory.Name,
                                     PlayerId = content.PlayerId,
                                     FirstName = player.FirstName,
                                     LastName = player.LastName,
                                     ContentTypeId = content.ContentTypeId,
                                     ContentTypeName = contenttype.ContentName,
                                     LanguageId = content.LanguageId,
                                     LanguageName = language.Name,
                                     Approved = content.Approved,
                                     UpdatedDate = content.UpdatedDate,
                                     UpdatedBy = content.UpdatedBy,
                                 }).FirstOrDefault();
            if (contentEntity == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;
            }
            var imgmodel = new VideoImageModel();
            imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(contentEntity.ContentFilePath) ? contentEntity.ContentFilePath : contentEntity.ContentFilePath);
            imgmodel.Type = String.IsNullOrEmpty(contentEntity.ContentFilePath) ? "video" : "image";
            // imgModel.thumbnail = _configuration.HostName.TrimEnd('/') + "/thumbnail/" + imgModel.url
            imgmodel.thumbnail = ThumbnailPath(imgmodel.url);

            var imgmodel1 = new VideoImageModel();
            imgmodel1.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(contentEntity.ContentFilePath1) ? contentEntity.ContentFilePath1 : contentEntity.ContentFilePath1);
            imgmodel1.Type = String.IsNullOrEmpty(contentEntity.ContentFilePath1) ? "video" : "image";
            // imgModel.thumbnail = _configuration.HostName.TrimEnd('/') + "/thumbnail/" + imgModel.url
            imgmodel1.thumbnail = ThumbnailPath(imgmodel1.url);
            return new ContentDetailModel
            {

                ContentId = contentEntity.ContentId,
                ContentFileName = contentEntity.ContentFileName,
                ContentFilePath = contentEntity.ContentFilePath,
                Title = contentEntity.Title,
                ContentFileName1 = contentEntity.ContentFileName1,
                ContentFilePath1 = contentEntity.ContentFilePath1,
                Description = contentEntity.Description,
                Thumbnail_2 = imgmodel1.thumbnail,
                Thumbnail1 = imgmodel.thumbnail,//contentEntity.Thumbnail,
                CreatedDate = contentEntity.CreatedDate,
                CreatedBy = contentEntity.CreatedBy,
                CategoryId = contentEntity.CategoryId,
                CategoryName = contentEntity.CategoryName,
                SubCategoryId = contentEntity.SubCategoryId,
                SubCategoryName = contentEntity.SubCategoryName,
                PlayerId = contentEntity.PlayerId,
                FirstName = contentEntity.FirstName,
                LastName = contentEntity.LastName,
                FullName = contentEntity.FirstName + " " + contentEntity.LastName,
                ContentTypeId = contentEntity.ContentTypeId,
                ContentTypeName = contentEntity.ContentTypeName,
                LanguageId = contentEntity.LanguageId,
                LanguageName = contentEntity.LanguageName,
                Approved = contentEntity.Approved,
                UpdatedBy = contentEntity.UpdatedBy,
                UpdatedDate = contentEntity.UpdatedDate,

            };
        }


        /// <summary>
        /// Method to get all contentDetails 
        /// </summary>
        /// <param name=""></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<ContentDetailModel> GetAllContentDetail(ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var contentdetailModelList = new List<ContentDetailModel>();
            var contentdetailEntity = (from content in context.ContentDetails
                                       join category in context.Categories on content.CategoryId equals category.CategoryId
                                       join subcategory in context.SubCategories on content.SubCategoryId equals subcategory.SubCategoryId
                                       join player in context.PlayerDetails on content.PlayerId equals player.PlayerId
                                       join contenttype in context.ContentTypes on content.ContentTypeId equals contenttype.ContentTypeId
                                       join language in context.Languages on content.LanguageId equals language.LanguageId
                                       where content.IsDeleted == false
                                       select new
                                       {
                                           ContentId = (int)content.ContentId,
                                           ContentFileName = content.ContentFileName,
                                           ContentFilePath = content.ContentFilePath,
                                           Title = content.Title,
                                           ContentFileName1= content.ContentFileName1,
                                           ContentFilePath1=content.ContentFilePath1,
                                           Thumbnail1=content.Thumbnail1,
                                           Description = content.Description,
                                           Thumbnail = content.Thumbnail,
                                           CreatedDate = content.CreatedDate,
                                           CreatedBy = content.CreatedBy,
                                           CategoryId = content.CategoryId,
                                           CategoryName = category.Name,
                                           SubCategoryId = content.SubCategoryId,
                                           SubCategoryName = subcategory.Name,
                                           PlayerId = content.PlayerId,
                                           FirstName = player.FirstName,
                                           LastName = player.LastName,
                                           ContentTypeId = content.ContentTypeId,
                                           ContentTypeName = contenttype.ContentName,
                                           LanguageId = content.LanguageId,
                                           LanguageName = language.Name,
                                           Approved = content.Approved,
                                           UpdatedDate = content.UpdatedDate,
                                           UpdatedBy = content.UpdatedBy,
                                       }).ToList();
            if (contentdetailEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;

            }
            contentdetailEntity.ForEach(item =>
            {
                var imgmodel = new VideoImageModel();
                imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(item.ContentFilePath) ? item.ContentFilePath: item.ContentFilePath);
                imgmodel.Type = String.IsNullOrEmpty(item.ContentFilePath) ? "video" : "image";
                // imgModel.thumbnail = _configuration.HostName.TrimEnd('/') + "/thumbnail/" + imgModel.url
                imgmodel.thumbnail = ThumbnailPath(imgmodel.url);

                var imgmodel1 = new VideoImageModel();
                imgmodel1.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(item.ContentFilePath1) ? item.ContentFilePath1 : item.ContentFilePath1);
                imgmodel1.Type = String.IsNullOrEmpty(item.ContentFilePath1) ? "video" : "image";
                // imgModel.thumbnail = _configuration.HostName.TrimEnd('/') + "/thumbnail/" + imgModel.url
                imgmodel1.thumbnail = ThumbnailPath(imgmodel1.url);

                contentdetailModelList.Add(new ContentDetailModel
                {
                    ContentId = item.ContentId,
                    ContentFileName = item.ContentFileName,
                    ContentFilePath = item.ContentFilePath,
                    Title = item.Title,
                    Description = item.Description,
                    ContentFileName1 = item.ContentFileName1,
                    ContentFilePath1 = item.ContentFilePath1,
                    Thumbnail_2 = imgmodel1.thumbnail,
                   //Thumbnail = ThumbnailPath(item.ContentFilePath),
                    Thumbnail1 = imgmodel.thumbnail,//item.Thumbnail,
                    CategoryId = item.CategoryId,
                    CategoryName = item.CategoryName,
                    SubCategoryId = item.SubCategoryId,
                    SubCategoryName = item.SubCategoryName,
                    PlayerId = item.PlayerId,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    FullName = item.FirstName + " " + item.LastName,
                    ContentTypeId = item.ContentTypeId,
                    ContentTypeName = item.ContentTypeName,
                    LanguageId = item.LanguageId,
                    LanguageName = item.LanguageName,
                    Approved = item.Approved,
                    UpdatedDate = item.UpdatedDate,
                    UpdatedBy = item.UpdatedBy,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                }); 
            }); 
            return contentdetailModelList;
        }


        /// <summary>
        /// Method to add contentDetails
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public string AddContentDetail(ContentDetailModel1 model, ref ErrorResponseModel errorResponseModel)
        {
            string message = "";
            if (model.ContentId == 0)
            {
                var contentEntity = new ContentDetail();
                contentEntity.ContentFileName = model.ContentFileName;
                contentEntity.ContentFilePath = model.ContentFilePath;//contentfilepath;
                contentEntity.ContentFileName1 = model.ContentFileName1;
                contentEntity.ContentFilePath1 = model.ContentFilePath1;
                contentEntity.Thumbnail1= model.Thumbnail3;
                contentEntity.Title = model.Title;
                contentEntity.Thumbnail = model.Thumbnail2;
                contentEntity.Description = model.Description;
                contentEntity.CreatedBy = model.CreatedBy;
                contentEntity.CreatedDate = DateTime.Now;
                contentEntity.CategoryId = model.CategoryId;
                contentEntity.UpdatedDate = DateTime.Now;
                contentEntity.SubCategoryId = model.SubCategoryId;
                contentEntity.PlayerId = model.PlayerId;
                contentEntity.ContentTypeId = model.ContentTypeId;
                contentEntity.LanguageId = model.LanguageId;
                contentEntity.Approved = null;
                contentEntity.IsDeleted = false;
                context.ContentDetails.Add(contentEntity);
                context.SaveChanges();
                message = GlobalConstants.ContentDetailAddSuccessfully;
            }

            else
            {
                message = GlobalConstants.AlreadyExists;
            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Add Content Details";
            userAuditLog.Description = "Content Details Added";
            userAuditLog.UserId = (int)model.CreatedBy;
            userAuditLog.CreatedBy = model.CreatedBy;
            userAuditLog.CreatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return message;
        }


        /// <summary>
        /// Method to edit contentDetails
        /// </summary>
        /// <param name="contentModel"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public string EditContentDetail(ContentDetailModel1 contentModel, ref ErrorResponseModel errorResponseModel)
        {
            var message = "";
            var contentdetailEntity = context.ContentDetails.Where(x => x.ContentId == contentModel.ContentId).FirstOrDefault();
            if (contentdetailEntity != null)
            {            
                contentdetailEntity.ContentId = contentModel.ContentId;
                if (contentModel.ContentFileName != null)
                {
                    contentdetailEntity.ContentFileName = contentModel.ContentFileName;
                }
                if (contentModel.ContentFilePath != null)
                {
                    contentdetailEntity.ContentFilePath = contentModel.ContentFilePath;
                }

                if (contentModel.ContentFileName1 != null)
                {
                    contentdetailEntity.ContentFileName1 = contentModel.ContentFileName1;
                }

                if (contentModel.ContentFilePath1 != null)
                {
                    contentdetailEntity.ContentFilePath1 = contentModel.ContentFilePath1;
                }
                //contentdetailEntity.ContentFileName = contentModel.ContentFileName;
                //contentdetailEntity.ContentFilePath = contentModel.ContentFilePath ;
                contentdetailEntity.Title = contentModel.Title;
                contentdetailEntity.Thumbnail1 = contentModel.Thumbnail3;
                contentdetailEntity.Thumbnail = contentModel.Thumbnail2;
                contentdetailEntity.Description = contentModel.Description;
                contentdetailEntity.CategoryId = contentModel.CategoryId;
                contentdetailEntity.SubCategoryId = contentModel.SubCategoryId;
                contentdetailEntity.PlayerId = contentModel.PlayerId;
                contentdetailEntity.ContentTypeId = contentModel.ContentTypeId;
                contentdetailEntity.LanguageId = contentModel.LanguageId;
                contentdetailEntity.Approved = null;
                contentdetailEntity.UpdatedDate = DateTime.Now;
                contentdetailEntity.UpdatedBy = contentModel.UpdatedBy;
                contentdetailEntity.IsDeleted = false;
                context.ContentDetails.Update(contentdetailEntity);
                context.SaveChanges();
                message = GlobalConstants.ContentDetailUpdateSuccessfully;
            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = "Edit Content Details";
            userAuditLog.Description = "Content Details Updated";
            userAuditLog.UserId = (int)contentModel.UpdatedBy;
            userAuditLog.UpdatedBy = contentModel.UpdatedBy;
            userAuditLog.UpdatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return message;
        }


        /// <summary>
        /// Method to delete contentDetails
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public string DeleteContentDetail(long contentId, ref ErrorResponseModel errorResponseModel)
        {
            string Message = "";
            var userplayerEntity = context.ContentDetails.FirstOrDefault(x => x.ContentId == contentId);
            if (userplayerEntity != null)
            {
                userplayerEntity.IsDeleted = true;
                userplayerEntity.Approved=null;
                context.SaveChanges();
                Message = GlobalConstants.ContentDetailDeleteSuccessfully;
            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Delete Content Details";
            userAuditLog.Description = "Content Details Deleted";
            userAuditLog.UserId = (int)userplayerEntity.CreatedBy;
            userAuditLog.UpdatedBy = userplayerEntity.UpdatedBy;
            userAuditLog.UpdatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return "{\"message\": \"" + Message + "\"}";
        }

        public List<ContentDetailModel> GetContentdetailByPlayerId(long playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var contentList = new List<ContentDetailModel>();
            var contentEntityList = (from content in context.ContentDetails
                                     join
                                     category in context.Categories on content.CategoryId equals category.CategoryId
                                     join update in context.ContentAuditLogs on content.ContentId equals update.ContentId into updates
                                     from update in updates.DefaultIfEmpty()
                                     join subcategory in context.SubCategories on content.SubCategoryId equals subcategory.SubCategoryId
                                     join player in context.PlayerDetails on content.PlayerId equals player.PlayerId
                                     join contenttype in context.ContentTypes on content.ContentTypeId equals contenttype.ContentTypeId
                                     join language in context.Languages on content.LanguageId equals language.LanguageId
                                     where content.PlayerId == playerId && content.IsDeleted == false 

                                     orderby content.UpdatedDate descending
                                     select new ContentDetailModel
                                     {
                                         ContentId = (int)content.ContentId,
                                         ContentFileName = content.ContentFileName,
                                         ContentFilePath = content.ContentFilePath,
                                         ContentFileName1 = content.ContentFileName1,
                                         ContentFilePath1 = content.ContentFilePath1,
                                         Title = update.ContentTitle != null && update.Approved==true ? $"{content.Title} ({update.ContentTitle})" : content.Title,
                                         Description = content.Description,
                                         CreatedDate = content.CreatedDate,
                                         CreatedBy = content.CreatedBy,
                                         CategoryId = content.CategoryId,
                                         CategoryName = category.Name,
                                         SubCategoryId = content.SubCategoryId,
                                         SubCategoryName = subcategory.Name, 
                                         PlayerId = content.PlayerId,
                                         FirstName = player.FirstName,
                                         LastName = player.LastName,
                                         ContentTypeId = content.ContentTypeId,
                                         ContentTypeName = contenttype.ContentName,
                                         LanguageId = content.LanguageId,
                                         LanguageName = language.Name,
                                         Comment = content.Comment,
                                         Approved = content.Approved,
                                         UpdatedDate = content.UpdatedDate,
                                         UpdatedBy = content.UpdatedBy,
                                     }

                               ).ToList();
            if (contentEntityList == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;

            }

            contentEntityList.ForEach(item =>
            {
                var imgmodel = new VideoImageModel();
                imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(item.ContentFilePath) ? item.ContentFilePath : item.ContentFilePath);
                imgmodel.Type = String.IsNullOrEmpty(item.ContentFilePath) ? "video" : "image";
                // imgModel.thumbnail = _configuration.HostName.TrimEnd('/') + "/thumbnail/" + imgModel.url
                imgmodel.thumbnail = ThumbnailPath(imgmodel.url);

                var imgmodel1 = new VideoImageModel();
                imgmodel1.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(item.ContentFilePath1) ? item.ContentFilePath1 : item.ContentFilePath1);
                imgmodel1.Type = String.IsNullOrEmpty(item.ContentFilePath1) ? "video" : "image";
                // imgModel.thumbnail = _configuration.HostName.TrimEnd('/') + "/thumbnail/" + imgModel.url
                imgmodel1.thumbnail = ThumbnailPath(imgmodel1.url);

                contentList.Add(new ContentDetailModel
                {
                    ContentId = item.ContentId,
                    ContentFileName = item.ContentFileName,
                    ContentFilePath = item.ContentFilePath,
                    ContentFileName1= item.ContentFileName1,
                    ContentFilePath1 = item.ContentFilePath1,
                    Thumbnail_2=imgmodel1.thumbnail,
                    Title = item.Title,
                    Description = item.Description,
                    Thumbnail1 = imgmodel.thumbnail,
                    CreatedDate = item.CreatedDate,
                    CreatedBy = item.CreatedBy,
                    CategoryId = item.CategoryId,
                    CategoryName = item.CategoryName,
                    SubCategoryId = item.SubCategoryId,
                    SubCategoryName = item.SubCategoryName,
                    PlayerId = item.PlayerId,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    FullName = item.FirstName + " " + item.LastName,
                    ContentTypeId = item.ContentTypeId,
                    ContentTypeName = item.ContentTypeName,
                    LanguageId = item.LanguageId,
                    LanguageName = item.LanguageName,
                    Comment= item.Comment,
                    Approved = item.Approved,
                    UpdatedBy = item.UpdatedBy,
                    UpdatedDate = item.UpdatedDate,
                });
            });
            return contentList;
        }

        /// <summary>
        /// Method is used to get approvedcontentdetails by id
        /// </summary>
        /// <param name="PlayerId"></param>
        /// <returns></returns>
        public List<ContentDetailModel> GetApprovedContentdetailByPlayerId(long playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var contentList = new List<ContentDetailModel>(); 
            var contentEntityList = (from content in context.ContentDetails
                                     join
                                     category in context.Categories on content.CategoryId equals category.CategoryId
                                     join subcategory in context.SubCategories on content.SubCategoryId equals subcategory.SubCategoryId
                                     join player in context.PlayerDetails on content.PlayerId equals player.PlayerId
                                     join contenttype in context.ContentTypes on content.ContentTypeId equals contenttype.ContentTypeId
                                     join language in context.Languages on content.LanguageId equals language.LanguageId
                                     where content.PlayerId == playerId && content.IsDeleted == false &&
                                     content.Approved==true && content.ContentTypeId == ContentTypeHelper.VideoContentTypeId
                                     orderby content.UpdatedDate descending
                                     select new ContentDetailModel
                                     {
                                         ContentId = (int)content.ContentId,
                                         ContentFileName = content.ContentFileName,
                                         ContentFilePath = content.ContentFilePath,
                                         ContentFileName1 = content.ContentFileName1,
                                         ContentFilePath1 = content.ContentFilePath1,
                                         Title = content.Title,
                                         Description = content.Description,
                                         CreatedDate = content.CreatedDate,
                                         CreatedBy = content.CreatedBy,
                                         CategoryId = content.CategoryId,
                                         CategoryName = category.Name,
                                         SubCategoryId = content.SubCategoryId,
                                         SubCategoryName = subcategory.Name,
                                         PlayerId = content.PlayerId,
                                         FirstName = player.FirstName,
                                         LastName = player.LastName,
                                         ContentTypeId = content.ContentTypeId,
                                         ContentTypeName = contenttype.ContentName,
                                         LanguageId = content.LanguageId,
                                         LanguageName = language.Name,
                                         Comment = content.Comment,
                                         Approved = content.Approved,
                                         UpdatedDate = content.UpdatedDate,
                                         UpdatedBy = content.UpdatedBy,
                                     }

                               ).ToList();
            if (contentEntityList == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;

            }

            contentEntityList.ForEach(item =>
            {
                var imgmodel = new VideoImageModel();
                imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(item.ContentFilePath) ? item.ContentFilePath : item.ContentFilePath);
                imgmodel.Type = String.IsNullOrEmpty(item.ContentFilePath) ? "video" : "image";
                // imgModel.thumbnail = _configuration.HostName.TrimEnd('/') + "/thumbnail/" + imgModel.url
                imgmodel.thumbnail = ThumbnailPath(imgmodel.url);

                var imgmodel1 = new VideoImageModel();
                imgmodel1.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(item.ContentFilePath1) ? item.ContentFilePath1 : item.ContentFilePath1);
                imgmodel1.Type = String.IsNullOrEmpty(item.ContentFilePath1) ? "video" : "image";
                // imgModel.thumbnail = _configuration.HostName.TrimEnd('/') + "/thumbnail/" + imgModel.url
                imgmodel1.thumbnail = ThumbnailPath(imgmodel1.url);

                contentList.Add(new ContentDetailModel
                {
                    ContentId = item.ContentId,
                    ContentFileName = item.ContentFileName,
                    ContentFilePath = item.ContentFilePath,
                    ContentFileName1 = item.ContentFileName1,
                    ContentFilePath1= item.ContentFilePath1,
                    Thumbnail_2=imgmodel1.thumbnail,
                    Title = item.Title,
                    Description = item.Description,
                    Thumbnail1 = imgmodel.thumbnail,
                    CreatedDate = item.CreatedDate,
                    CreatedBy = item.CreatedBy,
                    CategoryId = item.CategoryId,
                    CategoryName = item.CategoryName,
                    SubCategoryId = item.SubCategoryId,
                    SubCategoryName = item.SubCategoryName,
                    PlayerId = item.PlayerId,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    FullName = item.FirstName + " " + item.LastName,
                    ContentTypeId = item.ContentTypeId,
                    ContentTypeName = item.ContentTypeName,
                    LanguageId = item.LanguageId,
                    LanguageName = item.LanguageName,
                    Comment = item.Comment,
                    Approved = item.Approved,
                    UpdatedBy = item.UpdatedBy,
                    UpdatedDate = item.UpdatedDate,
                });
            });
            return contentList;
        }


        /// <summary>
        /// Method to approve contentdetail 
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public async Task<NotificationModel> ApproveContentDetail(long contentId)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            NotificationModel message = new NotificationModel();
            try
            {
                var contentEntity = context.ContentDetails.FirstOrDefault(x => x.ContentId == contentId && x.IsDeleted==false);

                if (contentEntity != null)
                {
                    contentEntity.Approved = true;
                    context.SaveChanges();
                    message.PlayerId = contentEntity.PlayerId;
                    message.Title = contentEntity.Title;
                    message.Description = contentEntity.Description;
                    message.ContentId = contentId;
                    message.CategoryId = contentEntity.CategoryId;
                    message.ContentTypeId = contentEntity.ContentTypeId;
                    message.Message = GlobalConstants.ApprovedSuccessfully;
                    message.Thumbnail = ThumbnailPath(_configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(contentEntity.ContentFilePath) ? contentEntity.ContentFilePath : contentEntity.ContentFilePath));

                    var existing = context.Fcmnotifications.Where(x => x.PlayerId == contentEntity.PlayerId && x.IsDeleted==false).ToList();

                    
                        foreach (var item in existing)
                        {
                          var notificationResponse=  await _notificationService.SendNotification(item.DeviceToken, message.PlayerId, message.ContentId, message.Title, message.Description, true, message.ContentTypeId, message.Thumbnail, message.CategoryId);
                            if (notificationResponse.IsSuccess == 0)
                            {
                                var fcmNotification = context.Fcmnotifications.FirstOrDefault(x => x.DeviceToken == item.DeviceToken);
                                if (fcmNotification != null)
                                {
                                    fcmNotification.IsDeleted = true;
                                    context.Fcmnotifications.Update(fcmNotification);
                                    context.SaveChanges();
                                }

                                // Set IsDeleted to true for the device token in UserDeviceMapping table
                                var userMapping = context.UserDeviceMappings.FirstOrDefault(x => x.DeviceToken == item.DeviceToken);
                                if (userMapping != null)
                                {
                                    userMapping.IsDeleted = true;
                                    context.UserDeviceMappings.Update(userMapping);
                                    context.SaveChanges();
                                }

                            }
                        }
                    
                    var userAuditLog = new UserAuditLogModel();
                    userAuditLog.Action = " Approve Content Details";
                    userAuditLog.Description = "Content Details Approved";
                    userAuditLog.UserId = (int)contentEntity.CreatedBy;
                    userAuditLog.UpdatedBy = contentEntity.UpdatedBy;
                    userAuditLog.UpdatedDate = DateTime.Now;
                    _userAuditLogService.AddUserAuditLog(userAuditLog);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, log it, or take appropriate action
                Console.WriteLine($"An error Message : {ex.Message}");
                message.Message= ex.Message;
                return message;
            }

            return message;
        }

        public string ContentDetailUpdateByContentLog(ContentModel model1, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel= new ErrorResponseModel();
            var logEntity = context.ContentAuditLogs.Where(x=>x.ContentLogId==model1.ContentLogId).FirstOrDefault();
            string message = "";
            if (model1.Approved==true)
            {
                var contentEntity = context.ContentDetails.Where(x => x.ContentId == model1.ContentId).FirstOrDefault();
                if (contentEntity == null)
                {
                    errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                    errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                    return null;
                }
                else
                {
                    contentEntity.Title = model1.Title;
                    contentEntity.Description = model1.Description;
                    logEntity.Approved = model1.Approved;
                    contentEntity.UpdatedDate = DateTime.Now;
                    contentEntity.UpdatedBy = model1.UpdatedBy;
                    context.ContentDetails.Update(contentEntity);
                    context.ContentAuditLogs.Update(logEntity);
                    context.SaveChanges();
                    message = "Content approved";
                }
            }

            else
            {
                message= "Content not approved";
            }
            return message;

        }

        private string ThumbnailPath(string filePath)
        {
            byte[]? ms =null;
            string extension = "";
            if (!String.IsNullOrWhiteSpace(filePath))
            {
                extension = Path.GetExtension(filePath);
                if (!String.IsNullOrWhiteSpace(extension))
                {
                    // filePath = filePath.Replace(extension, "*");
                    //filePath = filePath.Replace("/Resources/ContentFile/");
                     //ms = System.IO.File.ReadAllBytes(filePath);

                    //var string1 = filePath;
                    //filePath = string1.Replace(extension, ".jpeg");
                }
            }

            return filePath;
        }

        public string DeniedContentDetail(CommentModel model, ref ErrorResponseModel errorResponseModel)
        {
            string Message = "";
            var contentEntity = context.ContentDetails.FirstOrDefault(x => x.ContentId == model.ContentId);
            if (contentEntity != null)
            {
                contentEntity.Approved = false;
                contentEntity.Comment = model.Comment;
                context.SaveChanges();
                Message = GlobalConstants.DeniedSuccessfully;
            }
            return Message;  
        }
    }
}
