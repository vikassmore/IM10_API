﻿using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
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

        /// <summary>
        /// Creating constructor and injection dbContext
        /// </summary>
        /// <param name="context_"></param>

        public ContentCommentService(IM10DbContext context_, IOptions<ConfigurationModel> hostName, INotificationService notificationService, IUserAuditLogService userAuditLogService)
        {
             context = context_;
             this._configuration = hostName.Value;
            _userAuditLogService = userAuditLogService;
            _notificationService = notificationService;
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
                var existingMapping=context.UserDeviceMappings.Where(x=>x.UserId==commentEntity.UserId && x.IsDeleted==false).ToList();
                var loggedInUsersWithDeviceTokens = (from mapping in existingMapping
                                                     join user in context.UserMasters
                                                     on mapping.UserId equals user.UserId
                                                     where user.IsLogin == true && user.IsDeleted == false
                                                     select new
                                                     {
                                                         UserId = user.UserId,
                                                         DeviceToken = mapping.DeviceToken
                                                     }).ToList();             
                message.ContentId = commentreply.ContentId;
                message.title = commentreply.Comment1;
                message.CommentId = commentreply.CommentId;
                message.ContentTypeId = commentreply.ContentTypeId;
                message.CategoryId = existingcontentEntity.CategoryId;
                message.IsPublic = commentreply.IsPublic;
                message.Message = GlobalConstants.ReplySaveSuccessfully;

                foreach(var item in loggedInUsersWithDeviceTokens)
                {               
                  var notificationResponse = await _notificationService.SendCommentNotification(item.DeviceToken, message.ContentId, message.CommentId, message.title, true, message.ContentTypeId, message.CategoryId, (bool)message.IsPublic);
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
                            context.SaveChanges() ;
                        }

                    }
                }

                var userAuditLog = new UserAuditLogModel();
                userAuditLog.Action = " Add Content Comment Reply";
                userAuditLog.Description = "Content Comment Reply Added";
                userAuditLog.UserId = (int)model.CreatedBy;
                userAuditLog.CreatedBy = model.CreatedBy;
                userAuditLog.CreatedDate = DateTime.Now;
                _userAuditLogService.AddUserAuditLog(userAuditLog);
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                message.Message = ex.Message;
                return message;
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
            string Message = "";
            var commentEntity = context.Comments.FirstOrDefault(x => x.CommentId == commentId);
            if (commentEntity != null)
            {
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
            userAuditLog.Description = "Content Comment Reply Deleted";
            userAuditLog.UserId = (int)commentEntity.CreatedBy;
            userAuditLog.UpdatedBy = commentEntity.UpdatedBy;
            userAuditLog.UpdatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return "{\"message\": \"" + Message + "\"}";
        }


        /// <summary>
        /// get all content comment
        /// </summary>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<ContentCommentModel> GetAllContentComment(ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var commentList = new List<ContentCommentModel>();
            var commentEntity = (from comment in context.Comments
                                 join user in context.UserMasters
                                 on comment.UserId equals user.UserId
                                 join content in context.ContentTypes on
                                 comment.ContentTypeId equals content.ContentTypeId
                                 where comment.IsDeleted == false
                                 select new
                                 {
                                     comment.CommentId,
                                     comment.UserId,
                                     user.EmailId,
                                     user.FirstName,
                                     user.LastName,
                                     comment.ContentId,
                                     comment.ContentTypeId,
                                     content.ContentName,
                                     comment.DeviceId,
                                     comment.Location,
                                     comment.Liked,
                                     comment.Comment1,
                                     comment.Shared,
                                     comment.IsPublic,
                                     comment.CreatedBy,
                                     comment.CreatedDate,
                                     comment.UpdatedBy,
                                     comment.UpdatedDate,
                                     comment.ParentCommentId,
                                 }).ToList();

            if (commentEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            commentEntity.ForEach(item =>
            {
                commentList.Add(new ContentCommentModel
                {
                    CommentId = item.CommentId,
                    UserId = item.UserId,
                    ContentId = item.ContentId,
                    ContentTypeId = item.ContentTypeId,
                    DeviceId = item.DeviceId,
                    Location = item.Location,
                    Liked = item.Liked,
                    Comment1 = item.Comment1,
                    Shared = item.Shared,
                    IsPublic = item.IsPublic,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = DateTime.Now,
                    UpdatedBy = item.UpdatedBy,
                    UpdatedDate = DateTime.Now,
                    ParentCommentId = item.ParentCommentId,
                    EmailId = item.EmailId,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    ContentTypeName = item.ContentName,
                    FullName = item.FirstName + " " + item.LastName,
                });
            });
            return commentList;
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
                                 select new
                                 {
                                     comment.CommentId,
                                     comment.UserId,
                                     user.EmailId,
                                     user.FirstName,
                                     user.LastName,
                                     comment.ContentId,
                                     comment.ContentTypeId,
                                     content.ContentName,
                                     comment.DeviceId,
                                     comment.Location,
                                     comment.Liked,
                                     comment.Comment1,
                                     comment.Shared,
                                     comment.IsPublic,
                                     comment.CreatedBy,
                                     comment.CreatedDate,
                                     comment.UpdatedBy,
                                     comment.UpdatedDate,
                                     comment.ParentCommentId,
                                     comment.IsDeleted
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
                DeviceId = commentEntity.DeviceId,
                Location = commentEntity.Location,
                Liked = commentEntity.Liked,
                Comment1 = commentEntity.Comment1,
                Shared = commentEntity.Shared,
                IsPublic = commentEntity.IsPublic,
                CreatedBy = commentEntity.CreatedBy,
                CreatedDate = commentEntity.CreatedDate,
                ParentCommentId = commentEntity.ParentCommentId,
                UpdatedBy = commentEntity.UpdatedBy,
                UpdatedDate = commentEntity.UpdatedDate,
                IsDeleted = commentEntity.IsDeleted,
                EmailId = commentEntity.EmailId,
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
            var commentList = new List<ContentCommentModel>();
            var commentEntity = (from comment in context.Comments
                                 join
                                 content in context.ContentDetails on comment.ContentId equals content.ContentId
                                 join user in context.UserMasters on comment.UserId equals user.UserId
                                 join contenttype in context.ContentTypes on comment.ContentTypeId equals contenttype.ContentTypeId
                                 join player in context.PlayerDetails on content.PlayerId equals player.PlayerId
                                 where content.PlayerId == playerId && comment.IsDeleted == false && comment.ParentCommentId == null && content.IsDeleted==false
                                 && content.Approved== true 
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
                                     MobileNo= user.MobileNo,
                                     ContentFileName=content.ContentFileName,
                                     ContentFilePath=content.ContentFilePath,
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
            if (commentEntity.Count==0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;

            }

            commentEntity.ForEach(item =>
            {
                var imgmodel = new VideoImageModel();
                imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(item.ContentFilePath) ? item.ContentFilePath : item.ContentFilePath);
                imgmodel.Type = String.IsNullOrEmpty(item.ContentFilePath) ? "video" : "image";
                imgmodel.thumbnail = ThumbnailPath(imgmodel.url);
                commentList.Add(new ContentCommentModel
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
                    MobileNo=item.MobileNo,
                    Thumbnail1 = imgmodel.thumbnail,
                    ContentFileName = item.ContentFileName,
                    ContentFilePath = item.ContentFilePath,
                    ContentTypeId = item.ContentTypeId,
                    ContentTypeName = item.ContentTypeName,
                    UpdatedBy = item.UpdatedBy,
                    UpdatedDate = item.UpdatedDate,
                });
            });
            return commentList;
        }

        /// <summary>
        /// Method is used to get contentcomment reply by commentId
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public List<ContentCommentModel> GetContentCommentReplyByCommentId(long commentId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var commentList = new List<ContentCommentModel>();
            var commentEntity = (from comment in context.Comments
                                 join
                                     content in context.ContentDetails on comment.ContentId equals content.ContentId
                                 join user in context.UserMasters on comment.UserId equals user.UserId
                                 join contenttype in context.ContentTypes on comment.ContentTypeId equals contenttype.ContentTypeId
                                 join player in context.PlayerDetails on content.PlayerId equals player.PlayerId
                                 where comment.IsDeleted == false && comment.ParentCommentId == commentId
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
            if (commentEntity == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;

            }

            commentEntity.ForEach(item =>
            {
                commentList.Add(new ContentCommentModel
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
            return commentList;
        }


        private string ThumbnailPath(string filePath)
        {
            byte[]? ms = null;
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

    }
}
