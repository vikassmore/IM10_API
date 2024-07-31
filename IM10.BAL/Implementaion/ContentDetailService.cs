using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Configuration;
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
using System.Transactions;
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
        private readonly IErrorAuditLogService _logService;
        private readonly AppSettings _config;


        /// <summary>
        /// Creating constructor and injection dbContext
        /// </summary>
        /// <param name="_context"></param>
        public ContentDetailService(IOptions<AppSettings> config, IErrorAuditLogService logService,IM10DbContext _context, INotificationService notificationService,IOptions<ConfigurationModel> hostName, IUserAuditLogService userAuditLogService)
        {
            context = _context;
            this._configuration = hostName.Value;
            _userAuditLogService = userAuditLogService;
            _notificationService = notificationService;
            _logService = logService;
            _config = config.Value;
        }


        private string GetHostName(bool? flag)
        {
            return (bool)flag ? _config.BunnyHostName : _configuration.HostName;
        }

        /// <summary>
        /// Method to get contentDetails by userId
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public ContentDetailModel GetContentDetailById(long contentId, ref ErrorResponseModel errorResponseModel)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                ContentDetailModel? contentEntity = null;
                try
                {
                    errorResponseModel = new ErrorResponseModel();
                    contentEntity = (from content in context.ContentDetails
                                         join
                                         category in context.Categories on content.CategoryId equals category.CategoryId
                                         join subcategory in context.SubCategories on content.SubCategoryId equals subcategory.SubCategoryId
                                         join player in context.PlayerDetails on content.PlayerId equals player.PlayerId
                                         join contenttype in context.ContentTypes on content.ContentTypeId equals contenttype.ContentTypeId
                                         join language in context.Languages on content.LanguageId equals language.LanguageId
                                         where content.ContentId == contentId && content.IsDeleted == false && player.IsDeleted == false
                                         select new ContentDetailModel
                                         {
                                             ContentId = (int)content.ContentId,
                                             ContentFileName = content.ContentFileName,
                                             ContentFilePath = content.ContentFilePath,
                                             Title = content.Title,
                                             Thumbnail_2 = content.Thumbnail,
                                             Thumbnail1 = content.Thumbnail1,
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
                                             ProductionFlag=content.ProductionFlag,
                                         }).FirstOrDefault();
                    if (contentEntity == null)
                    {
                        errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                        errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                        return null;
                    }
                    var imgmodel = new VideoImageModel();
                    var hostname = GetHostName(contentEntity.ProductionFlag);
                    imgmodel.url = hostname.TrimEnd('/') + (String.IsNullOrEmpty(contentEntity.ContentFilePath) ? contentEntity.ContentFilePath : contentEntity.ContentFilePath);
                    imgmodel.Type = String.IsNullOrEmpty(contentEntity.ContentFilePath) ? "video" : "image";
                    // imgModel.thumbnail = _configuration.HostName.TrimEnd('/') + "/thumbnail/" + imgModel.url
                    imgmodel.thumbnail = ThumbnailPath(imgmodel.url);

                    var imgmodel1 = new VideoImageModel();
                    imgmodel1.url = hostname.TrimEnd('/') + (String.IsNullOrEmpty(contentEntity.ContentFilePath1) ? contentEntity.ContentFilePath1 : contentEntity.ContentFilePath1);
                    imgmodel1.Type = String.IsNullOrEmpty(contentEntity.ContentFilePath1) ? "video" : "image";
                    // imgModel.thumbnail = _configuration.HostName.TrimEnd('/') + "/thumbnail/" + imgModel.url
                    imgmodel1.thumbnail = ThumbnailPath(imgmodel1.url);
                    transaction.Commit();
                    return new ContentDetailModel
                    {

                        ContentId = contentEntity.ContentId,
                        ContentFileName = contentEntity.ContentFileName,
                        ContentFilePath = imgmodel.url,
                        Title = contentEntity.Title,
                        ContentFileName1 = contentEntity.ContentFileName1,
                        ContentFilePath1 = imgmodel1.url,
                        Description = contentEntity.Description,
                        Thumbnail_2 = contentEntity.Thumbnail_2,
                        Thumbnail1 = contentEntity.Thumbnail1,//contentEntity.Thumbnail,
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
                        ProductionFlag= contentEntity.ProductionFlag,
                    };
                }
                catch (Exception ex)
                {
                    int? userIdForLog = null;
                    ContentDetailModel advtcontentEntity = contentEntity;
                    if (advtcontentEntity != null)
                    {
                        if (advtcontentEntity.UpdatedBy != 0)
                        {
                            userIdForLog = advtcontentEntity.UpdatedBy;
                        }
                        else
                        {
                            userIdForLog = advtcontentEntity.CreatedBy;
                        }
                    }
                    transaction.Rollback();
                    var errorMessage = _logService.SaveErrorLogs(new LogEntry
                    {
                        LogType = "Error",
                        StackTrace = ex.StackTrace,
                        AdditionalInformation = ex.Message,
                        CreatedDate = DateTime.Now,
                        LogSource = "GetContentCommentByPlayerId",
                        UserId = userIdForLog,
                        LogMessage = "Exception occurred in GetContentCommentByPlayerId method"
                    });
                    return null;
                }
            }
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
            using (var transaction = context.Database.BeginTransaction())
            {
                List<ContentDetailModel>? contentdetailEntity = null;
                var contentdetailModelList = new List<ContentDetailModel>();
                try
                {
                    contentdetailEntity = (from content in context.ContentDetails
                                           join category in context.Categories on content.CategoryId equals category.CategoryId
                                           join subcategory in context.SubCategories on content.SubCategoryId equals subcategory.SubCategoryId
                                           join player in context.PlayerDetails on content.PlayerId equals player.PlayerId
                                           join contenttype in context.ContentTypes on content.ContentTypeId equals contenttype.ContentTypeId
                                           join language in context.Languages on content.LanguageId equals language.LanguageId
                                           where content.IsDeleted == false && player.IsDeleted == false
                                           select new ContentDetailModel
                                           {
                                               ContentId = (int)content.ContentId,
                                               ContentFileName = content.ContentFileName,
                                               ContentFilePath = content.ContentFilePath,
                                               Title = content.Title,
                                               ContentFileName1 = content.ContentFileName1,
                                               ContentFilePath1 = content.ContentFilePath1,
                                               Thumbnail1 = content.Thumbnail1,
                                               Description = content.Description,
                                               Thumbnail_2 = content.Thumbnail,
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
                                               ProductionFlag=content.ProductionFlag
                                           }).ToList();
                    if (contentdetailEntity.Count == 0)
                    {
                        errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                        errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                        return null;
                    }
                    contentdetailEntity.ForEach(item =>
                    {
                        var hostname = GetHostName(item.ProductionFlag);
                        var imgmodel = new VideoImageModel();
                        imgmodel.url = hostname.TrimEnd('/') + (String.IsNullOrEmpty(item.ContentFilePath) ? item.ContentFilePath : item.ContentFilePath);
                        imgmodel.Type = String.IsNullOrEmpty(item.ContentFilePath) ? "video" : "image";
                        // imgModel.thumbnail = _configuration.HostName.TrimEnd('/') + "/thumbnail/" + imgModel.url
                        imgmodel.thumbnail = ThumbnailPath(imgmodel.url);

                        var imgmodel1 = new VideoImageModel();
                        imgmodel1.url = hostname.TrimEnd('/') + (String.IsNullOrEmpty(item.ContentFilePath1) ? item.ContentFilePath1 : item.ContentFilePath1);
                        imgmodel1.Type = String.IsNullOrEmpty(item.ContentFilePath1) ? "video" : "image";
                        // imgModel.thumbnail = _configuration.HostName.TrimEnd('/') + "/thumbnail/" + imgModel.url
                        imgmodel1.thumbnail = ThumbnailPath(imgmodel1.url);

                        contentdetailModelList.Add(new ContentDetailModel
                        {
                            ContentId = item.ContentId,
                            ContentFileName = item.ContentFileName,
                            ContentFilePath = imgmodel.url,
                            Title = item.Title,
                            Description = item.Description,
                            ContentFileName1 = item.ContentFileName1,
                            ContentFilePath1 = imgmodel1.url,
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
                            ProductionFlag = item.ProductionFlag,
                        });
                    });
                    transaction.Commit();
                   return contentdetailModelList;
                }
                catch (Exception ex)
                {
                    int? userIdForLog = null;
                    ContentDetailModel? advtcontentEntity = contentdetailEntity.FirstOrDefault();
                    if (advtcontentEntity != null)
                    {
                        if (advtcontentEntity.UpdatedBy != 0)
                        {
                            userIdForLog = advtcontentEntity.UpdatedBy;
                        }
                        else
                        {
                            userIdForLog = advtcontentEntity.CreatedBy;
                        }
                    }
                    transaction.Rollback();
                    var errorMessage = _logService.SaveErrorLogs(new LogEntry
                    {
                        LogType = "Error",
                        StackTrace = ex.StackTrace,
                        AdditionalInformation = ex.Message,
                        CreatedDate = DateTime.Now,
                        LogSource = "Getallcontendetail",
                        UserId = userIdForLog,
                        LogMessage = "Exception occurred in Getallcontendetail method"
                    });
                    return null;
                }
            }
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
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    if (model.ContentId == 0)
                    {
                        var existingPlayerEntity = context.PlayerDetails.FirstOrDefault(x => x.PlayerId == model.PlayerId && x.IsDeleted == false);
                        if (existingPlayerEntity != null)
                        {
                            var contentEntity = new ContentDetail();
                            contentEntity.ContentFileName = model.ContentFileName;
                            contentEntity.ContentFilePath = model.ContentFilePath;//contentfilepath;
                            contentEntity.ContentFileName1 = model.ContentFileName1;
                            contentEntity.ContentFilePath1 = model.ContentFilePath1;
                            contentEntity.Thumbnail1 = model.Thumbnail3;
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
                            contentEntity.ProductionFlag = _config.ProductionFlag;
                            context.ContentDetails.Add(contentEntity);
                            context.SaveChanges();
                            transaction.Commit();
                            message = GlobalConstants.ContentDetailAddSuccessfully;
                        }
                        else
                        {
                            errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                            message = "Player Id does not exist";
                        }
                    }

                    else
                    {
                        message = GlobalConstants.AlreadyExists;
                    }
                    var userAuditLog = new UserAuditLogModel();
                    userAuditLog.Action = " Add Content Details";
                    userAuditLog.Description = "Content Details Added Successfully";
                    userAuditLog.UserId = (int)model.CreatedBy;
                    userAuditLog.CreatedBy = model.CreatedBy;
                    userAuditLog.CreatedDate = DateTime.Now;
                    _userAuditLogService.AddUserAuditLog(userAuditLog);
                }
                catch (Exception ex)
                {
                    int? userIdForLog;
                    if (model.UpdatedBy != 0)
                    {
                        userIdForLog = model.UpdatedBy;
                    }
                    else
                    {
                        userIdForLog = model.CreatedBy;
                    }
                    transaction.Rollback();
                    var errorMessage = _logService.SaveErrorLogs(new LogEntry
                    {
                        LogType = "Error",
                        StackTrace = ex.StackTrace,
                        AdditionalInformation = ex.Message,
                        CreatedDate = DateTime.Now,
                        LogSource = "AddContentDetail",
                        UserId = userIdForLog,
                        LogMessage = "Exception occurred in AddContentDetail method"
                    });
                    return "Something went wrong!";
                }
            }
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
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {                    
                            var contentdetailEntity = context.ContentDetails.Where(x => x.ContentId == contentModel.ContentId).FirstOrDefault();
                            if (contentdetailEntity != null)
                            {
                                var existingPlayerEntity = context.PlayerDetails.FirstOrDefault(x => x.PlayerId == contentModel.PlayerId && x.IsDeleted == false);
                                if (existingPlayerEntity != null)
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
                                    if (contentModel.Thumbnail3 != null)
                                    {
                                        contentdetailEntity.Thumbnail1 = contentModel.Thumbnail3;
                                    }
                                    //contentdetailEntity.ContentFileName = contentModel.ContentFileName;
                                    //contentdetailEntity.ContentFilePath = contentModel.ContentFilePath ;
                                    contentdetailEntity.Title = contentModel.Title;
                                    //contentdetailEntity.Thumbnail1 = contentModel.Thumbnail3;
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
                                    contentdetailEntity.ProductionFlag = _config.ProductionFlag;
                                    context.ContentDetails.Update(contentdetailEntity);
                                    context.SaveChanges();
                                    transaction.Commit();
                                    message = GlobalConstants.ContentDetailUpdateSuccessfully;
                                    }
                                else
                                {
                                  errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                                  message = "Player Id does not exist";
                                }
                            }
                            var userAuditLog = new UserAuditLogModel();
                            userAuditLog.Action = "Edit Content Details";
                            userAuditLog.Description = "Content Details Updated Successfully";
                            userAuditLog.UserId = (int)contentModel.UpdatedBy;
                            userAuditLog.UpdatedBy = contentModel.UpdatedBy;
                            userAuditLog.UpdatedDate = DateTime.Now;
                            _userAuditLogService.AddUserAuditLog(userAuditLog);
                        }
                        catch (Exception ex)
                        {
                            int? userIdForLog;
                            if (contentModel.UpdatedBy != 0)
                            {
                                userIdForLog = contentModel.UpdatedBy;
                            }
                            else
                            {
                                userIdForLog = contentModel.CreatedBy;
                            }
                            transaction.Rollback();
                            var errorMessage = _logService.SaveErrorLogs(new LogEntry
                            {
                                LogType = "Error",
                                StackTrace = ex.StackTrace,
                                AdditionalInformation = ex.Message,
                                CreatedDate = DateTime.Now,
                                LogSource = "EditContentDetails",
                                UserId = userIdForLog,
                                LogMessage = "Exception occurred in EditContentDetails method"
                            });
                            return "Something went wrong!";
                        }
                    }
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
            errorResponseModel = new ErrorResponseModel();
            string Message = "";
            var contentdetailsEntity = context.ContentDetails.FirstOrDefault(x => x.ContentId == contentId);
            if (contentdetailsEntity != null)
            {
                if (contentdetailsEntity.Approved == true)
                {
                    errorResponseModel.StatusCode = HttpStatusCode.BadRequest; 
                    Message= "Cannot delete approved content";
                    return Message;
                }
                if (contentdetailsEntity.IsDeleted == true)
                {
                    errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                    errorResponseModel.Message = "Content already deleted.";
                    return null;
                }
                contentdetailsEntity.IsDeleted = true;
                contentdetailsEntity.Approved=null;
                context.SaveChanges();
                Message = GlobalConstants.ContentDetailDeleteSuccessfully;
            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Delete Content Details";
            userAuditLog.Description = "Content Details Deleted Successfully";
            userAuditLog.UserId = (int)contentdetailsEntity.CreatedBy;
            userAuditLog.CreatedDate = DateTime.Now;
            userAuditLog.CreatedBy = contentdetailsEntity.CreatedBy;
            userAuditLog.UpdatedBy = contentdetailsEntity.CreatedBy;
            userAuditLog.UpdatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return "{\"message\": \"" + Message + "\"}";
        }

        public List<ContentDetailModel> GetContentdetailByPlayerId(long playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            using (var transaction = context.Database.BeginTransaction())
            {
                List<ContentDetailModel>? contentEntityList = null;
                try
                {
                    var contentList = new List<ContentDetailModel>();
                    contentEntityList = (from content in context.ContentDetails
                                             join
                                             category in context.Categories on content.CategoryId equals category.CategoryId
                                             join update in context.ContentAuditLogs.Where(u => !u.IsDeleted) on content.ContentId equals update.ContentId into updates
                                             from update in updates.DefaultIfEmpty()
                                             join subcategory in context.SubCategories on content.SubCategoryId equals subcategory.SubCategoryId
                                             join player in context.PlayerDetails on content.PlayerId equals player.PlayerId
                                             join contenttype in context.ContentTypes on content.ContentTypeId equals contenttype.ContentTypeId
                                             join language in context.Languages on content.LanguageId equals language.LanguageId
                                             where content.PlayerId == playerId && content.IsDeleted == false && player.IsDeleted == false
                                             orderby content.UpdatedDate descending
                                             select new ContentDetailModel
                                             {
                                                 ContentId = (int)content.ContentId,
                                                 ContentFileName = content.ContentFileName,
                                                 ContentFilePath = content.ContentFilePath,
                                                 ContentFileName1 = content.ContentFileName1,
                                                 ContentFilePath1 = content.ContentFilePath1,
                                                 Title = update.ContentTitle != null && update.Approved == true ? $"{content.Title} ({update.ContentTitle})" : content.Title,
                                                 Description = content.Description,
                                                 CategoryId = content.CategoryId,
                                                 CategoryName = category.Name,
                                                 Thumbnail3 = content.Thumbnail1,
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
                                                 ProductionFlag = content.ProductionFlag,
                                             }).ToList();
                    if (contentEntityList.Count == 0)
                    {
                        errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                        errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                        return null;
                    }
                    contentEntityList.ForEach(item =>
                    {
                        var hostName = GetHostName(item.ProductionFlag);

                        var imgmodel = new VideoImageModel();
                        imgmodel.url = hostName.TrimEnd('/') + (String.IsNullOrEmpty(item.ContentFilePath) ? item.ContentFilePath : item.ContentFilePath);
                        imgmodel.Type = String.IsNullOrEmpty(item.ContentFilePath) ? "video" : "image";
                        // imgModel.thumbnail = _configuration.HostName.TrimEnd('/') + "/thumbnail/" + imgModel.url
                        imgmodel.thumbnail = ThumbnailPath(imgmodel.url);

                        var imgmodel1 = new VideoImageModel();
                        imgmodel1.url = hostName.TrimEnd('/') + (String.IsNullOrEmpty(item.ContentFilePath1) ? item.ContentFilePath1 : item.ContentFilePath1);
                        imgmodel1.Type = String.IsNullOrEmpty(item.ContentFilePath1) ? "video" : "image";
                        // imgModel.thumbnail = _configuration.HostName.TrimEnd('/') + "/thumbnail/" + imgModel.url
                        imgmodel1.thumbnail = ThumbnailPath(imgmodel1.url);

                        contentList.Add(new ContentDetailModel
                        {
                            ContentId = item.ContentId,
                            ContentFileName = item.ContentFileName,
                            ContentFilePath = item.ContentFilePath,
                            ContentFileName1 = item.ContentFileName1,
                            ContentFilePath1 = item.ContentFilePath1,
                            Thumbnail_2 = hostName + item.Thumbnail3,
                            ContentThumbnail_2 = imgmodel1.thumbnail,
                            Title = item.Title,
                            Description = item.Description,
                            ContentThumbnail1 = imgmodel.thumbnail,
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
                            ProductionFlag = item.ProductionFlag,
                        });
                    });
                    transaction.Commit();
                    return contentList;
                }
                catch (Exception ex)
                {
                    int? userIdForLog = null;
                    ContentDetailModel? advtcontentEntity = contentEntityList?.FirstOrDefault();
                    if (advtcontentEntity != null)
                    {
                        if (advtcontentEntity.UpdatedBy != 0)
                        {
                            userIdForLog = advtcontentEntity.UpdatedBy;
                        }
                        else
                        {
                            userIdForLog = advtcontentEntity.CreatedBy;
                        }
                    }
                    transaction.Rollback();
                    var errorMessage = _logService.SaveErrorLogs(new LogEntry
                    {
                        LogType = "Error",
                        StackTrace = ex.StackTrace,
                        AdditionalInformation = ex.Message,
                        CreatedDate = DateTime.Now,
                        LogSource = "GetContentdetailByPlayerId",
                        UserId = userIdForLog,
                        LogMessage = "Exception occurred in GetContentdetailByPlayerId method"
                    });
                    return null;
                }
            }
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
                                     content.Approved==true && player.IsDeleted==false
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
                                         ProductionFlag = content.ProductionFlag,
                                     }).ToList();
            if (contentEntityList.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;
            }

            contentEntityList.ForEach(item =>
            {
                var hostname= GetHostName(item.ProductionFlag);
                var imgmodel = new VideoImageModel();
                imgmodel.url = hostname.TrimEnd('/') + (String.IsNullOrEmpty(item.ContentFilePath) ? item.ContentFilePath : item.ContentFilePath);
                imgmodel.Type = String.IsNullOrEmpty(item.ContentFilePath) ? "video" : "image";
                // imgModel.thumbnail = _configuration.HostName.TrimEnd('/') + "/thumbnail/" + imgModel.url
                imgmodel.thumbnail = ThumbnailPath(imgmodel.url);

                var imgmodel1 = new VideoImageModel();
                imgmodel1.url =hostname.TrimEnd('/') + (String.IsNullOrEmpty(item.ContentFilePath1) ? item.ContentFilePath1 : item.ContentFilePath1);
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
                    ProductionFlag = item.ProductionFlag,
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
            
                var contentEntity = context.ContentDetails.FirstOrDefault(x => x.ContentId == contentId && x.IsDeleted == false);                
                try
                {
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
                        var userAuditLog = new UserAuditLogModel();
                        userAuditLog.Action = "Approve Content Details";
                        userAuditLog.Description = "Content Details Approved Successfully";
                        userAuditLog.UserId = (int)contentEntity.CreatedBy;
                        userAuditLog.UpdatedBy = contentEntity.UpdatedBy;
                        userAuditLog.UpdatedDate = DateTime.Now;
                        _userAuditLogService.AddUserAuditLog(userAuditLog);
                    }
                }
                catch (Exception ex)
                {
                    string deviceId = "";
                    int? userIdForLog = null;
                    if (contentEntity != null)
                    {
                        if (contentEntity.UpdatedBy != 0)
                        {
                            userIdForLog = contentEntity.UpdatedBy;
                        }
                        else
                        {
                            userIdForLog = contentEntity.CreatedBy;
                        }
                    }
                    var existingNotification = context.Fcmnotifications.FirstOrDefault(x => x.PlayerId == contentEntity.PlayerId && !x.IsDeleted);
                    if (existingNotification != null)
                    {
                       deviceId = existingNotification.DeviceToken;
                    }                  
                    message.Message= "Something went wrong!";
                }
            
            return message;
        }

        
        public string ContentDetailUpdateByContentLog(ContentModel model1, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel= new ErrorResponseModel();
            var logEntity = context.ContentAuditLogs.Where(x=>x.ContentLogId==model1.ContentLogId && x.IsDeleted==false).FirstOrDefault();
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
                }
            }

            return filePath;
        }

        public string DeniedContentDetail(CommentModel model, ref ErrorResponseModel errorResponseModel)
        {
            string Message = "";
            var contentEntity = context.ContentDetails.FirstOrDefault(x => x.ContentId == model.ContentId && x.IsDeleted==false);
            if (contentEntity != null)
            {
                contentEntity.Approved = false;
                contentEntity.Comment = model.Comment;
                context.SaveChanges();
                Message = GlobalConstants.DeniedSuccessfully;
            }
            return Message;  
        }


        /// <summary>
        /// To get contenttitles by playerId 
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public List<ContentTitleModel> GetContentTitlesByPlayerId(long playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var contentEntityList = (from content in context.ContentDetails
                                     join update in context.ContentAuditLogs on content.ContentId equals update.ContentId into updates
                                     from update in updates.DefaultIfEmpty()
                                     join player in context.PlayerDetails on content.PlayerId equals player.PlayerId
                                     where content.PlayerId == playerId && content.IsDeleted == false && player.IsDeleted == false
                                     && content.Approved == true 
                                     orderby content.UpdatedDate descending
                                     select new ContentTitleModel
                                     {
                                         ContentId = (int)content.ContentId,
                                         Title = update.ContentTitle != null && update.Approved == true ? $"{content.Title} ({update.ContentTitle})" : content.Title,
                                     }).ToList();
            if (contentEntityList.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;
            }
            return contentEntityList.ToList();
        }


        public List<ContentTitleModel> GetApprovedContentTitles(long playerId, long contenttypeId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var contentEntityList = (from content in context.ContentDetails
                                     join player in context.PlayerDetails on content.PlayerId equals player.PlayerId
                                     where content.PlayerId == playerId && content.IsDeleted == false &&
                                     content.ContentTypeId == contenttypeId &&
                                     content.Approved == true && player.IsDeleted == false
                                     orderby content.UpdatedDate descending
                                     select new ContentTitleModel
                                     {
                                         ContentId = (int)content.ContentId,
                                         Title = content.Title,
                                     }).ToList();
            if (contentEntityList.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;
            }
            return contentEntityList.ToList();
        }
    }
}
