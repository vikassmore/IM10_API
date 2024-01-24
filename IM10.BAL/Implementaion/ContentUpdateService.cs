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
                var contentEntity = new ContentAuditLog();
                contentEntity.ContentLogId = model.ContentLogId;
                contentEntity.ContentId=model.ContentId;
                contentEntity.Title=model.Title;
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
            userAuditLog.Description = "Content Update Details Added";
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
            string Message = "";
            var contentEntity = context.ContentAuditLogs.FirstOrDefault(x => x.ContentLogId == contentLogId);
            if (contentEntity != null)
            {
                contentEntity.IsDeleted = true;
                context.SaveChanges();
                Message = GlobalConstants.ContentDeleteMessage;             

            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Delete Content Update Details";
            userAuditLog.Description = " Content Update Details Deleted";
            userAuditLog.UserId = (int)contentEntity.CreatedBy;
            userAuditLog.UpdatedBy = contentEntity.CreatedBy;
            userAuditLog.UpdatedDate = DateTime.Now;
            _auditLogService.AddUserAuditLog(userAuditLog);
            return "{\"message\": \"" + Message + "\"}";
        }

        public string DeniedContentUpdateDetail(ContentUpdateComment model, ref ErrorResponseModel errorResponseModel)
        {
            string Message = "";
            var contentupdateEntity = context.ContentAuditLogs.FirstOrDefault(x => x.ContentLogId == model.ContentLogId);
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
                                     ContentTitle = content.Title,

                                 }).ToList();
            if (contentEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;

            }
            contentEntity.ForEach(item =>
            {
                contentlist.Add(new ContentUpdateModel
                {
                    ContentLogId=item.ContentLogId,
                    ContentId=item.ContentId,
                    Title=item.Title,
                    Description=item.Description,
                    CreatedBy=item.CreatedBy,
                    Approved=item.Approved,
                    CreatedDate=DateTime.Now,
                    ApprovedBy=item.ApprovedBy,
                    ApprovedDate=DateTime.Now,      
                    ContentTitle=item.ContentTitle
                });

            });
            return contentlist;
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
                                     ContentTitle = content.Title
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
            var contentupldateList=new List<ContentUpdateModel>();
            var contentEntity = (from details in context.ContentDetails join
                                 update in context.ContentAuditLogs on details.ContentId equals update.ContentId
                                 where details.PlayerId==playerId && update.IsDeleted==false
                                 orderby update.ContentLogId descending

                                 select new ContentUpdateModel
                                 {

                                     ContentLogId = update.ContentLogId,
                                     ContentId = update.ContentId,
                                     Title = update.Title,
                                     Description = update.Description,
                                     CreatedBy = update.CreatedBy,
                                     Comment = update.Comment,
                                     Approved = update.Approved,
                                     CreatedDate = update.CreatedDate,
                                     ApprovedBy = update.ApprovedBy,
                                     ApprovedDate = update.ApprovedDate,
                                     ContentTitle = details.Title
                                 }).ToList();
            if (contentEntity == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;
            }

            contentEntity.ForEach(item =>
            {
                contentupldateList.Add(new ContentUpdateModel
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
            return contentupldateList;

        }

        public List< ContentUpdateModel> GetContentUpdateforQA(long playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var list=new List<ContentUpdateModel>();
            ContentUpdateModel contentUpdateModel = new ContentUpdateModel();
            //var contentEntity = (from update in context.ContentAuditLogs
            //                     join
            //                     content in context.ContentDetails
            //                     on update.ContentId equals content.ContentId
            //                     where content.PlayerId == playerId && update.IsDeleted == false
            //                     orderby update.ContentLogId descending

            //var groupedLogs =(from content in context.ContentAuditLogs join details in context.ContentDetails
            //                  on content.ContentId equals details.ContentId select new ContentUpdateModel { })
            //                 .GroupBy(log => log.ContentId)
            //                 .ToList();

            //var contentEntity =  groupedLogs 
            //    .Select(groupedLogs => groupedLogs.OrderByDescending(log => log.ContentLogId).FirstOrDefault())
            //    .ToList();          
             var joinedData = context.ContentAuditLogs
                            .Join(
                                context.ContentDetails,
                                log => log.ContentId,
                                detail => detail.ContentId,
                                (log, detail) => new { Log = log, Detail = detail }).OrderByDescending(x=>x.Log.ContentLogId).ToList();

             var groupedData = joinedData
                 .Where(data => !data.Log.IsDeleted) // Filter out deleted content
                .GroupBy(data => data.Log.ContentId).ToList();

            var contentEntity = groupedData
                .Select(group => new ContentUpdateModel
                {
                    ContentId = group.Key,
                    ContentTitle = group.First().Detail.Title,
                    ContentLogId=group.First().Log.ContentLogId,
                    Title=group.First().Log.Title,
                    Description=group.First().Log.Description,
                    Comment=group.First().Log.Comment,
                    Approved=group.First().Log.Approved,
                    CreatedBy=group.First().Log.CreatedBy,
                    CreatedDate=group.First().Log.CreatedDate,
                    ApprovedBy=group.First().Log.ApprovedBy,
                    ApprovedDate=group.First().Log.ApprovedDate,
                    IsDeleted=group.First().Log.IsDeleted,
                }).OrderByDescending(x=>x.ContentLogId).ToList();


            if (contentEntity == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            contentEntity.ForEach(item =>
            {
                list.Add(new ContentUpdateModel
                {
                    ContentLogId = item.ContentLogId,
                    ContentId = item.ContentId,
                    Title = item.Title,
                    Description = item.Description,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = DateTime.Now,
                    Approved = item.Approved,
                    ApprovedBy = item.ApprovedBy,
                    ApprovedDate = DateTime.Now,
                    ContentTitle = item.ContentTitle,
                    Comment = item.Comment,
                });
            });
            return list;
            
        }

       
    }
}
