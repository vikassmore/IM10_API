using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace IM10.BAL.Implementaion
{
    /// <summary>
    /// This is implementation  for the contentUpdate operations 
    /// </summary>
    public class ContentUpdateService : IContentUpdateService
    {
        IM10DbContext context;
        private readonly IUserAuditLogService _auditLogService;
        /// <summary>
        /// Creating constructor and injection dbContext
        /// </summary>
        /// <param name="_context"></param>
        public ContentUpdateService(IM10DbContext _context, IUserAuditLogService auditLogService)
        {
            context = _context;
            _auditLogService = auditLogService;
        }


        /// <summary>
        /// Method to add contentUpdate
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public string AddContentUpdate(ContentUpdateModel model, ref ErrorResponseModel errorResponseModel)
        {
            string message = "";
           
            if (model.ContentLogId==0)
            {
                var updateEntity = context.ContentDetails.Where(x => x.ContentId == model.ContentId).FirstOrDefault();
                var contentEntity = new ContentAuditLog();
                contentEntity.ContentLogId = model.ContentLogId;
                contentEntity.ContentId=model.ContentId;
                contentEntity.Title=model.Title;
                contentEntity.ContentTitle=updateEntity.Title;
                contentEntity.Description=model.Description;
                contentEntity.CreatedBy=model.CreatedBy;
                contentEntity.CreatedDate=DateTime.Now;              
                contentEntity.ApprovedDate=DateTime.Now;
                contentEntity.Approved=null;
                contentEntity.ApprovedBy=model.ApprovedBy;
                contentEntity.IsDeleted = false;
                context.ContentAuditLogs.Add(contentEntity);
                context.SaveChanges();
                message = GlobalConstants.ContentSaveMessage;
            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Add Content Update Details";
            userAuditLog.Description = "Content Update Details Added Successfully";
            userAuditLog.UserId = (int)model.CreatedBy;
            userAuditLog.CreatedBy = model.CreatedBy;
            userAuditLog.CreatedDate = DateTime.Now;
            _auditLogService.AddUserAuditLog(userAuditLog);
            return message;

        }


        /// <summary>
        /// Method to delete contentUpdate
        /// </summary>
        /// <param name="contentLogId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public string DeleteContentUpdate(long contentLogId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            string Message = "";
            var contentEntity = context.ContentAuditLogs.FirstOrDefault(x => x.ContentLogId == contentLogId);
            if (contentEntity != null)
            {
                if (contentEntity.Approved == true)
                {
                    errorResponseModel.StatusCode = HttpStatusCode.BadRequest;
                    Message = "Cannot delete approved contentupdate";
                    return Message;
                }
                if (contentEntity.IsDeleted == true)
                {
                    errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                    errorResponseModel.Message = "Content update already deleted.";
                    return null;
                }
                contentEntity.IsDeleted = true;
                context.SaveChanges();
                Message = GlobalConstants.ContentDeleteMessage;             

            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Delete Content Update Details";
            userAuditLog.Description = " Content Update Details Deleted Successfully";
            userAuditLog.UserId = (int)contentEntity.CreatedBy;
            userAuditLog.CreatedDate = DateTime.Now;
            userAuditLog.CreatedBy = contentEntity.CreatedBy;
            userAuditLog.UpdatedBy = contentEntity.CreatedBy;
            userAuditLog.UpdatedDate = DateTime.Now;
            _auditLogService.AddUserAuditLog(userAuditLog);
            return "{\"message\": \"" + Message + "\"}";
        }

        public string DeniedContentUpdateDetail(ContentUpdateComment model, ref ErrorResponseModel errorResponseModel)
        {
            string Message = "";
            var contentupdateEntity = context.ContentAuditLogs.FirstOrDefault(x => x.ContentLogId == model.ContentLogId && x.IsDeleted==false);
            if (contentupdateEntity != null)
            {
                contentupdateEntity.Approved = false;
                contentupdateEntity.Comment = model.Comment;
                context.SaveChanges();
                Message = GlobalConstants.DeniedSuccessfully;
            }
            return Message;
        }


        /// <summary>
        /// Method to get all contentTitle 
        /// </summary>
        /// <param name=""></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<ContentTitleModel> GetAllContentTitles(ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var contentlist = new List<ContentTitleModel>();
            var contentEntity = context.ContentDetails.Where(x => x.IsDeleted == false).ToList();
            if (contentEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;

            }
            contentEntity.ForEach(item =>
            {
                contentlist.Add(new ContentTitleModel
                {
                    ContentId = item.ContentId,
                    Title = item.Title,
                });

            });
            return contentlist;

        }


        /// <summary>
        /// Method to get all contentUpdate 
        /// </summary>
        /// <param name=""></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<ContentUpdateModel> GetAllContentUpdate(ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var contentlist = new List<ContentUpdateModel>();
            var contentEntity = (from update in context.ContentAuditLogs
                                 join
                                 content in context.ContentDetails
                                 on update.ContentId equals content.ContentId
                                 where update.IsDeleted == false
                                 && content.IsDeleted == false
                                 select new ContentUpdateModel
                                 {
                                     ContentLogId = update.ContentLogId,
                                     ContentId = update.ContentId,
                                     Title = update.Title,
                                     Description = update.Description,
                                     CreatedBy = update.CreatedBy,
                                     CreatedDate = update.CreatedDate,
                                     Approved= update.Approved,
                                     ApprovedBy = update.ApprovedBy,
                                     ApprovedDate = update.ApprovedDate,
                                     ContentTitle = update.ContentTitle,

                                 }).ToList();
            if (contentEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;

            }
            return contentEntity.ToList();
        }



        /// <summary>
        /// Method to get contentUpdate by contentId
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<ContentUpdateModel> GetContentUpdate(long contentId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var contentupdateList = new List<ContentUpdateModel>();
            var contentEntity = (from update in context.ContentAuditLogs
                                 join
                                 content in context.ContentDetails 
                                 on update.ContentId equals content.ContentId
                                 where update.ContentId == contentId && update.IsDeleted == false
                                 && content.IsDeleted == false
                                 select new ContentUpdateModel
                                 {
                                     ContentLogId = update.ContentLogId,
                                     ContentId = update.ContentId,
                                     Title = update.Title,
                                     Description = update.Description,
                                     CreatedBy = update.CreatedBy,
                                     Comment=update.Comment,
                                     Approved= update.Approved,
                                     CreatedDate = update.CreatedDate,
                                     ApprovedBy = update.ApprovedBy,
                                     ApprovedDate = update.ApprovedDate,
                                     ContentTitle = update.ContentTitle
                                 }).ToList();

            if (contentEntity == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;

            }

            contentEntity.ForEach( item =>
            {
                contentupdateList.Add(new ContentUpdateModel
                {
                    ContentLogId = item.ContentLogId,
                    ContentId = item.ContentId,
                    Title = item.Title,
                    Description = item.Description,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = DateTime.Now,
                    Comment = item.Comment,
                    Approved = item.Approved,
                    ApprovedBy = item.ApprovedBy,
                    ApprovedDate = DateTime.Now,
                    ContentTitle = item.ContentTitle,
                });
            });
            return contentupdateList;
        }

        public List<ContentUpdateModel> GetContentUpdateByPlayerId(long playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel= new ErrorResponseModel();
            var contentEntity = (from details in context.ContentDetails join
                                 update in context.ContentAuditLogs on details.ContentId equals update.ContentId
                                 where details.PlayerId==playerId && update.IsDeleted==false
                                 && details.IsDeleted== false   
                                 orderby update.ContentLogId descending

                                 select new ContentUpdateModel
                                 {
                                     ContentLogId = update.ContentLogId,
                                     ContentId = update.ContentId,
                                     Title = update.Title,
                                     Description = update.Description,
                                     Comment = update.Comment,
                                     Approved = update.Approved,
                                     ContentTitle = update.ContentTitle
                                 }).ToList();
            if (contentEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;
            }
            return contentEntity.ToList();

        }

        public List< ContentUpdateModel> GetContentUpdateforQA(long playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            errorResponseModel = new ErrorResponseModel();
            var contentEntity = (from details in context.ContentDetails join
                                 update in context.ContentAuditLogs on details.ContentId equals update.ContentId
                                 where details.PlayerId == playerId && update.IsDeleted == false
                                 && details.IsDeleted == false
                                 orderby update.ContentLogId descending

                                 select new ContentUpdateModel
                                 {
                                     ContentLogId = update.ContentLogId,
                                     ContentId = update.ContentId,
                                     Title = update.Title,
                                     Description = update.Description,
                                     Comment = update.Comment,
                                     Approved = update.Approved,
                                     ContentTitle = update.ContentTitle
                                 }).ToList();
            if (contentEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;
            }
            return contentEntity.ToList();           
        }     
    }
}
