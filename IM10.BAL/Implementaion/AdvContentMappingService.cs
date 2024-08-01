using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
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
    /// This is implementation  for the Advcontentmapping operations 
    /// </summary>
    public class AdvContentMappingService : IAdvContentMappingService
    {

        IM10DbContext context;
        private ConfigurationModel _configuration;
        private readonly IUserAuditLogService _userAuditLogService;
        private readonly IErrorAuditLogService _logService;

        /// <summary>
        /// Creating constructor and injection dbContext
        /// </summary>
        /// <param name="_context"></param>
        public AdvContentMappingService(IErrorAuditLogService auditLogService,IM10DbContext _context,IOptions<ConfigurationModel> hostName, IUserAuditLogService userAuditLogService)
        {
            context = _context;
            _configuration = hostName.Value;
            _userAuditLogService = userAuditLogService;
            _logService = auditLogService;
        }  

        /// <summary>
        /// Method to add advcontentmapping
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public string AddAdvContentMapping(AdvContentMappingModel model)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            string message = "";
            using (var transaction = context.Database.BeginTransaction())
            {              
                try
                {
                    if (model.AdvContentMapId == 0)
                    { 
                        var existingMapping = context.AdvContentMappings.FirstOrDefault(x => x.ContentId == model.ContentId && x.AdvertiseContentId == model.AdvertiseContentId && x.IsDeleted==false && x.Position==model.Position);

                        if (existingMapping != null)
                        {
                            message = "Advertise Ads On already exist.";
                        }
                        else
                        {
                            AdvContentMapping contentEntity = new AdvContentMapping();
                            contentEntity.AdvContentMapId = model.AdvContentMapId;
                            contentEntity.ContentId = model.ContentId;
                            contentEntity.AdvertiseContentId = model.AdvertiseContentId;
                            contentEntity.CategoryId = model.CategoryId;
                            contentEntity.SubCategoryId = model.SubCategoryId;
                            contentEntity.Position = model.Position;
                            contentEntity.CreatedBy = model.CreatedBy;
                            contentEntity.CreatedDate = DateTime.Now;
                            contentEntity.UpdatedDate = DateTime.Now;
                            contentEntity.IsDeleted = false;
                            context.AdvContentMappings.Add(contentEntity);
                            context.SaveChanges();
                            transaction.Commit();
                            message = GlobalConstants.AdvContentMappingAddedSuccessfully;
                            var AuditLog = new UserAuditLogModel();
                            AuditLog.Action = " Add Advertise Content mapping";
                            AuditLog.Description = "Advertise Content mapping Added Successfully";
                            AuditLog.UserId = model.CreatedBy != null ? (int)model.CreatedBy : model.UpdatedBy != null ? (int)model.UpdatedBy : 0;
                            AuditLog.CreatedBy = model.CreatedBy != null ? (int)model.CreatedBy : model.UpdatedBy != null ? (int)model.UpdatedBy : 0;
                            AuditLog.CreatedDate = DateTime.Now;                     
                            _userAuditLogService.AddUserAuditLog(AuditLog);
                        }
                    }
                    else
                    {
                        var contentlist = context.AdvContentMappings.FirstOrDefault(x => x.AdvContentMapId == model.AdvContentMapId);
                        if (contentlist != null)
                        {
                            var existingMapping = context.AdvContentMappings.FirstOrDefault(x => x.ContentId == model.ContentId && x.AdvertiseContentId == model.AdvertiseContentId && x.IsDeleted == false && x.Position == model.Position);

                            if (existingMapping != null)
                            {
                                message = "Advertise Ads On already exist.";
                            }
                            else
                            {
                                contentlist.AdvContentMapId = model.AdvContentMapId;
                                contentlist.ContentId = model.ContentId;
                                contentlist.AdvertiseContentId = model.AdvertiseContentId;
                                contentlist.CategoryId = model.CategoryId;
                                contentlist.SubCategoryId = model.SubCategoryId;
                                contentlist.Position = model.Position;
                                contentlist.UpdatedBy = model.UpdatedBy;
                                contentlist.UpdatedDate = DateTime.Now;
                                contentlist.IsDeleted = false;
                                context.AdvContentMappings.Update(contentlist);
                                context.SaveChanges();
                                transaction.Commit();
                                message = GlobalConstants.AdvContentMappingUpdateSuccessfully;
                            }
                        }
                    }
                    var userAuditLog = new UserAuditLogModel();
                    userAuditLog.Action = " Add Advertise Content mapping ";
                    userAuditLog.Description = "Advertise Content mapping  updated Successfully";
                    userAuditLog.UserId = model.CreatedBy != null ? (int)model.CreatedBy : model.UpdatedBy != null ? (int)model.UpdatedBy : 0;                   
                    userAuditLog.UpdatedBy = model.CreatedBy != null ? (int)model.CreatedBy : model.UpdatedBy != null ? (int)model.UpdatedBy : 0;
                    userAuditLog.UpdatedDate = DateTime.Now;
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
                        StackTrace = "Stack Trace: " + ex.StackTrace,
                        AdditionalInformation = "Inner Exception: " + ex.InnerException + "Message: " + ex.Message,
                        CreatedDate = DateTime.Now,
                        LogSource = "Add/EditAdvContentMapping",
                        UserId = userIdForLog,
                        LogMessage = "Exception occurred in AddAdvContentMapping method"
                    });
                    return "Something went wrong!";
                }
            }
            return message;
        }


        /// <summary>
        /// Method to delete advcontentmapping
        /// </summary>
        /// <param name="advcontentmapId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public string DeleteAdvContentMapping(long advcontentmapId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            string Message = "";
            using (var transaction = context.Database.BeginTransaction())
            {
                var advtcontentEntity = context.AdvContentMappings.FirstOrDefault(x => x.AdvContentMapId == advcontentmapId);
                try
                {                 
                    if (advtcontentEntity == null)
                    {
                        errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                        errorResponseModel.Message = "Adv content mapping not found.";
                        return null;
                    }
                    if (advtcontentEntity != null)
                    {
                        if (advtcontentEntity.IsDeleted == true)
                        {
                            errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                            errorResponseModel.Message = "Adv content mapping already deleted.";
                            return null;
                        }
                        advtcontentEntity.IsDeleted = true;
                        context.SaveChanges();
                        transaction.Commit();
                        Message = GlobalConstants.AdvContentMappingDeleteSuccessfully;
                    }
                    var userAuditLog = new UserAuditLogModel();
                    userAuditLog.Action = " Delete Advertise mapping Details";
                    userAuditLog.Description = "Advertise Content mapping Deleted";
                    userAuditLog.UserId = (int)advtcontentEntity.CreatedBy;
                    userAuditLog.CreatedDate = DateTime.Now;
                    userAuditLog.CreatedBy = advtcontentEntity.CreatedBy;
                    userAuditLog.UpdatedBy = advtcontentEntity.CreatedBy;
                    userAuditLog.UpdatedDate = DateTime.Now;
                    _userAuditLogService.AddUserAuditLog(userAuditLog);
                }
                catch (Exception ex)
                {
                    int? userIdForLog =null;
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
                        LogSource = "DeleteAdvContentMapping",
                        UserId = userIdForLog,
                        LogMessage = "Exception occurred in DeleteAdvContentMapping method"
                    });
                    return "Something went wrong!";
                }
                return "{\"message\": \"" + Message + "\"}";
            }
        }

        /// <summary>
        /// Method is used to get AdvContentMapping by playerId
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public List <AdvContentMappingModel1> GetAdvContentMappingByPlayerId(long playerId, ref ErrorResponseModel errorResponseModel)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                List<AdvContentMappingModel1>? contentEntity = null;
                try
                {
                    errorResponseModel = new ErrorResponseModel();
                    contentEntity = (from update in context.AdvContentMappings
                                         join
                                         content in context.ContentDetails on update.ContentId equals content.ContentId
                                         join advertise in context.AdvContentDetails on update.AdvertiseContentId equals advertise.AdvertiseContentId
                                         join category in context.Categories on update.CategoryId equals category.CategoryId
                                         join subcategory in context.SubCategories on update.SubCategoryId equals subcategory.SubCategoryId
                                         where content.PlayerId == playerId && advertise.IsDeleted == false && update.IsDeleted == false
                                         && content.Approved == true && content.IsDeleted == false
                                         orderby update.UpdatedDate descending
                                         select new AdvContentMappingModel1
                                         {
                                             AdvContentMapId = update.AdvContentMapId,
                                             ContentId = update.ContentId,
                                             ContentName = content.Title,
                                             AdvertiseContentId = update.AdvertiseContentId,
                                             AdvertiseContentName = advertise.Title,
                                             CategoryId = update.CategoryId,
                                             CategoryName = category.Name,
                                             SubCategoryId = update.SubCategoryId,
                                             SubcategoryName = subcategory.Name,
                                             Position = update.Position,
                                         }).ToList();
                    if (contentEntity.Count == 0)
                    {
                        errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                        errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                        return null;
                    }
                    transaction.Commit();
                    return contentEntity.ToList();
                }
                catch (Exception ex)
                {
                    int? userIdForLog = null;
                    AdvContentMappingModel1 advtcontentEntity = contentEntity?.FirstOrDefault();
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
                        LogSource = "GetAdvContentMappingByPlayerId",
                        UserId = userIdForLog,
                        LogMessage = "Exception occurred in GetAdvContentMappingByPlayerId method"
                    });
                    return null;
                }
            }
        }


        /// <summary>
        /// Method is used to get AdvContentMapping by AdvcontentmapId
        /// </summary>
        /// <param name="AdvcontentmapId"></param>
        /// <returns></returns>
        public AdvContentMappingModel1 GetAdvContentMappingById(long AdvcontentmapId, ref ErrorResponseModel errorResponseModel)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                AdvContentMappingModel1? contentEntity = null;
                try
                {
                    errorResponseModel = new ErrorResponseModel();
                    contentEntity = (from update in context.AdvContentMappings join
                                         content in context.ContentDetails on update.ContentId equals content.ContentId
                                         join advertise in context.AdvContentDetails on update.AdvertiseContentId equals advertise.AdvertiseContentId
                                         join category in context.Categories on update.CategoryId equals category.CategoryId
                                         join subcategory in context.SubCategories on update.SubCategoryId equals subcategory.SubCategoryId
                                         where update.AdvContentMapId == AdvcontentmapId && update.IsDeleted == false
                                         select new AdvContentMappingModel1
                                         {
                                             AdvContentMapId = update.AdvContentMapId,
                                             ContentId = update.ContentId,
                                             ContentName = content.Title,
                                             AdvertiseContentId = update.AdvertiseContentId,
                                             AdvertiseContentName = advertise.Title,
                                             CategoryId = update.CategoryId,
                                             CategoryName = category.Name,
                                             SubCategoryId = update.SubCategoryId,
                                             SubcategoryName = subcategory.Name,
                                             CreatedBy = update.CreatedBy,
                                             CreatedDate = update.CreatedDate,
                                             Position = update.Position,
                                             UpdatedDate = DateTime.Now,
                                             UpdatedBy = update.UpdatedBy,
                                         }).FirstOrDefault();
                    if (contentEntity == null)
                    {
                        errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                        errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                        return null;
                    }
                    transaction.Commit();
                    return contentEntity;                   
                }
                catch (Exception ex)
                {
                    int? userIdForLog = null;
                    AdvContentMappingModel1? advtcontentEntity = contentEntity;
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
                        LogSource = "GetAdvContentMappingById",
                        UserId = userIdForLog,
                        LogMessage = "Exception occurred in GetAdvContentMappingById method"
                    });
                    return null;
                }
            }
        }
    }
}
