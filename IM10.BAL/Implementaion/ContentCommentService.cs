using Azure.Core;
using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Implementaion
{
    /// <summary>
    /// This is implementation for the contentcomment operations 
    /// </summary>
    public class ContentCommentService : IContentCommentService
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
        /// <param name="context_"></param>

        public ContentCommentService(IErrorAuditLogService auditLogService, IOptions<AppSettings> config, IM10DbContext context_, IOptions<ConfigurationModel> hostName, INotificationService notificationService, IUserAuditLogService userAuditLogService)
        {
             context = context_;
             this._configuration = hostName.Value;
            _userAuditLogService = userAuditLogService;
            _notificationService = notificationService;
            _logService = auditLogService;
            _config = config.Value;

        }
        private string GetHostName(bool? flag)
        {
            return (bool)flag ? _config.BunnyHostName : _configuration.HostName;
        }

        /// <summary>
        /// Method to add content comment reply
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public async Task<CommentNotificationModel> AddContentCommentReply(ContentCommentModel model)
        {
           CommentNotificationModel message = new CommentNotificationModel();
           ErrorResponseModel  errorResponseModel = new ErrorResponseModel();
           using (var transaction = context.Database.BeginTransaction())
           {
                try
                {
                    var commentEntity = context.Comments.Where(x => x.CommentId == model.CommentId).FirstOrDefault();                    
                    var commentreply = new Comment();
                    commentreply.UserId = model.UserId;
                    commentreply.ContentId = commentEntity.ContentId;
                    commentreply.ContentTypeId = commentEntity.ContentTypeId;
                    commentreply.DeviceId = commentEntity.DeviceId;
                    commentreply.Liked = false;
                    commentreply.Location = "";
                    commentreply.Shared = false;
                    commentreply.IsDeleted = false;
                    commentreply.CreatedBy = model.CreatedBy;
                    commentreply.CreatedDate = DateTime.Now;
                    commentreply.Comment1 = model.Comment1;
                    commentreply.ParentCommentId = model.CommentId;
                    commentreply.IsPublic = model.IsPublic;
                    context.Comments.Add(commentreply);
                    context.SaveChanges();
                    
                    var comment = context.Comments.FirstOrDefault(x => x.CommentId == model.CommentId);
                    if (comment != null)
                    {
                        comment.IsPublic = model.IsPublic;
                        context.Comments.Update(comment);
                        context.SaveChanges();
                    }
                     
                    var existingcontentEntity = context.ContentDetails.FirstOrDefault(x => x.ContentId == commentreply.ContentId);
                    var existingMapping = context.UserDeviceMappings.Where(x => x.UserId == commentEntity.UserId && x.IsDeleted == false).ToList();

                    message.ContentId = commentreply.ContentId;
                    message.title = commentreply.Comment1;
                    message.CommentId = commentreply.CommentId;
                    message.ContentTypeId = commentreply.ContentTypeId;
                    message.CategoryId = existingcontentEntity.CategoryId;
                    message.IsPublic = commentreply.IsPublic;
                    message.Message = GlobalConstants.ReplySaveSuccessfully;
                    
                   /* foreach (var item in existingMapping)
                    {
                        var notificationResponse = await _notificationService.SendCommentNotification(item.DeviceToken, message.ContentId, message.CommentId, message.title, message.ContentTypeId, message.CategoryId, (bool)message.IsPublic);
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
                            var userMapping = context.UserDeviceMappings.Where(x => x.DeviceToken == item.DeviceToken).ToList();
                            foreach (var mapping in userMapping)
                            {
                                if (mapping != null)
                                {
                                    mapping.IsDeleted = true;
                                    context.UserDeviceMappings.Update(mapping);
                                    context.SaveChanges();
                                }
                            }
                        }
                    
                
                    }*/
                    var userAuditLog = new UserAuditLogModel();
                    userAuditLog.Action = " Add Content Comment Reply";
                    userAuditLog.Description = "Content Comment Reply Added Successfully";
                    userAuditLog.UserId = (int)model.CreatedBy;
                    userAuditLog.CreatedBy = model.CreatedBy;
                    userAuditLog.CreatedDate = DateTime.Now;
                    _userAuditLogService.AddUserAuditLog(userAuditLog);
                    transaction.Commit();
                }
                catch (Exception ex) 
                {
                    int? userIdForLog = null;
                    if (model != null)
                    {
                        if (model.UpdatedBy != 0)
                        {
                            userIdForLog = model.UpdatedBy;
                        }
                        else
                        {
                            userIdForLog = model.CreatedBy;
                        }
                    }                   
                   transaction.Rollback();
                   var errorMessage = _logService.SaveErrorLogs(new LogEntry
                   {
                        LogType = "Error",
                       StackTrace = "Stack Trace: " + ex.StackTrace,
                       AdditionalInformation = "Inner Exception: " + ex.InnerException + "Message: " + ex.Message,
                       CreatedDate = DateTime.Now,
                        LogSource = "AddContentCommentReply",
                        UserId = userIdForLog,
                        LogMessage = "Exception occurred in AddContentCommentReply method"
                   });
                   message.Message= "Something went wrong!";
                }
            }
            return message;
        }


        /// <summary>
        /// delete comment reply
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public string DeleteCommentReply(long commentId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            string Message = "";
            try
            {
                var commentEntity = context.Comments.FirstOrDefault(x => x.CommentId == commentId);
                if (commentEntity != null)
                {
                    if (commentEntity.IsDeleted == true)
                    {
                        errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                        errorResponseModel.Message = "Comment reply already deleted.";
                        return null;
                    }
                    commentEntity.IsDeleted = true;
                    context.SaveChanges();
                    var commentReplyEntity = context.Comments.Where(x => x.ParentCommentId == commentEntity.CommentId).ToList();
                    if (commentReplyEntity.Count != 0)
                    {
                        foreach (var item in commentReplyEntity)
                        {
                            item.IsDeleted = true;
                            context.SaveChanges();
                        }
                    }
                    Message = GlobalConstants.ReplyDeleteSuccessfully;
                }
                var userAuditLog = new UserAuditLogModel();
                userAuditLog.Action = "Delete Content Comment Reply";
                userAuditLog.Description = "Content Comment Reply Deleted Successfully";
                userAuditLog.UserId = (int)commentEntity.CreatedBy;
                userAuditLog.CreatedDate = DateTime.Now;
                userAuditLog.CreatedBy = commentEntity.CreatedBy;
                userAuditLog.UpdatedBy = commentEntity.CreatedBy;
                _userAuditLogService.AddUserAuditLog(userAuditLog);
                return "{\"message\": \"" + Message + "\"}";
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Message = ex.Message;
                return Message;
            }
        }

            /// <summary>
            /// get content comment by id
            /// </summary>
            /// <param name="commentId"></param>
            /// <param name="errorResponseModel"></param>
            /// <returns></returns>
           public ContentCommentModel GetContentCommentById(long commentId, ref ErrorResponseModel errorResponseModel)
           {
                errorResponseModel = new ErrorResponseModel();
                var commentEntity = (from comment in context.Comments
                                     join user in context.UserMasters
                                     on comment.UserId equals user.UserId
                                     join content in context.ContentTypes on
                                     comment.ContentTypeId equals content.ContentTypeId
                                     where comment.IsDeleted == false && comment.CommentId == commentId
                                     && comment.IsDeleted==false
                                     select new
                                     {
                                       comment.CommentId,
                                       comment.UserId,
                                       user.FirstName,
                                       user.LastName,
                                       comment.ContentId,
                                       comment.ContentTypeId,
                                       content.ContentName,
                                       comment.Comment1,
                                       comment.IsPublic,
                                     }).FirstOrDefault();
                if (commentEntity == null)
                {
                    errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                    errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                }
                return new ContentCommentModel
                {
                   CommentId = commentEntity.CommentId,
                   UserId = commentEntity.UserId,
                   ContentId = commentEntity.ContentId,
                   ContentTypeId = commentEntity.ContentTypeId,
                   Comment1 = commentEntity.Comment1,
                   IsPublic = commentEntity.IsPublic,
                   FirstName = commentEntity.FirstName,
                   LastName = commentEntity.LastName,
                   ContentTypeName = commentEntity.ContentName,
                   FullName = commentEntity.FirstName + " " + commentEntity.LastName,
                };
           }


        /// <summary>
        /// Method is used to get contentcomment by playerId
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public List<ContentCommentModel> GetContentCommentByPlayerId(long playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            using (var transaction = context.Database.BeginTransaction())
            {
                List<ContentCommentModel>? commentEntity = null;
                try
                {
                    var commentList = new List<ContentCommentModel>();
                    commentEntity = (from comment in context.Comments
                                         join
                                         content in context.ContentDetails on comment.ContentId equals content.ContentId
                                         join user in context.UserMasters on comment.UserId equals user.UserId
                                         join contenttype in context.ContentTypes on comment.ContentTypeId equals contenttype.ContentTypeId
                                         where content.PlayerId == playerId && comment.IsDeleted == false && comment.ParentCommentId == null
                                         && content.IsDeleted == false && content.Approved == true
                                         orderby comment.CreatedDate descending
                                         select new ContentCommentModel
                                         {
                                             CommentId = comment.CommentId,
                                             ContentId = comment.ContentId,
                                             UserId = comment.UserId,
                                             MobileNo = user.MobileNo,
                                             ContentFileName = content.ContentFileName,
                                             ContentFilePath = content.ContentFilePath,
                                             DeviceId = comment.DeviceId,
                                             Liked = comment.Liked,
                                             Comment1 = comment.Comment1,
                                             IsPublic = comment.IsPublic,
                                             Title = content.Title,
                                             ContentTypeId = content.ContentTypeId,
                                             ContentTypeName = contenttype.ContentName,
                                             ProductionFlag=content.ProductionFlag
                                         }).ToList();
                    if (commentEntity.Count == 0)
                    {
                        errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                        errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                        return null;

                    }

                    commentEntity.ForEach(item =>
                    {
                        var hostname1 = GetHostName(item.ProductionFlag);
                        var imgmodel = new VideoImageModel();
                        imgmodel.url = hostname1.TrimEnd('/') + (String.IsNullOrEmpty(item.ContentFilePath) ? item.ContentFilePath : item.ContentFilePath);
                        imgmodel.Type = String.IsNullOrEmpty(item.ContentFilePath) ? "video" : "image";
                        imgmodel.FileName = (imgmodel.url);
                        commentList.Add(new ContentCommentModel
                        {
                            CommentId = item.CommentId,
                            ContentId = item.ContentId,
                            UserId = item.UserId,
                            Comment1 = item.Comment1,
                            DeviceId = item.DeviceId,
                            Liked = item.Liked,
                            Shared = item.Shared,
                            IsPublic = item.IsPublic,
                            MobileNo = item.MobileNo,
                            Title = item.Title,
                            Thumbnail1 = imgmodel.FileName,
                            ContentFileName = item.ContentFileName,
                            ContentFilePath = item.ContentFilePath,
                            ContentTypeId = item.ContentTypeId,
                            ContentTypeName = item.ContentTypeName,
                        });
                    });
                    transaction.Commit();
                    return commentList;
                }
                catch (Exception ex)
                {
                    int? userIdForLog = null;
                    ContentCommentModel advtcontentEntity = commentEntity?.FirstOrDefault();
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
                        StackTrace = "Stack Trace: " + ex.StackTrace,
                        AdditionalInformation = "Inner Exception: " + ex.InnerException + "Message: " + ex.Message,
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
        /// Method is used to get contentcomment reply by commentId
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public List<ContentCommentModel> GetContentCommentReplyByCommentId(long commentId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            using (var transaction = context.Database.BeginTransaction())
            {
                List<ContentCommentModel>? commentEntity = null;
                try
                {
                    var commentList = new List<ContentCommentModel>();
                    commentEntity = (from comment in context.Comments
                                         join content in context.ContentDetails on comment.ContentId equals content.ContentId
                                         join contenttype in context.ContentTypes on comment.ContentTypeId equals contenttype.ContentTypeId
                                         where comment.IsDeleted == false && comment.ParentCommentId == commentId
                                         orderby comment.CreatedDate descending
                                         select new ContentCommentModel
                                         {
                                             CommentId = comment.CommentId,
                                             Comment1 = comment.Comment1,
                                             ContentTypeId = content.ContentTypeId,
                                             ContentTypeName = contenttype.ContentName,
                                             IsPublic = comment.IsPublic,
                                         }).ToList();
                    if (commentEntity.Count == 0)
                    {
                        errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                        errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                        return null;

                    }
                    transaction.Commit();
                    return commentEntity.ToList();
                }
                catch (Exception ex)
                {
                    int? userIdForLog = null;
                    ContentCommentModel advtcontentEntity = commentEntity?.FirstOrDefault();
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
                        StackTrace = "Stack Trace: " + ex.StackTrace,
                        AdditionalInformation = "Inner Exception: " + ex.InnerException + "Message: " + ex.Message,
                        CreatedDate = DateTime.Now,
                        LogSource = "GetContentCommentByPlayerId",
                        UserId = userIdForLog,
                        LogMessage = "Exception occurred in GetContentCommentByPlayerId method"
                    });
                    return null;
                }
            }
        }
    }
}
