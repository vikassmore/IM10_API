using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Implementaion
{
    /// <summary>
    /// This is implementation for the CampaignDetail operations 
    /// </summary>
    public class CampaignDetailService : ICampaignDetailService
    {

        IM10DbContext context;
        private readonly IUserAuditLogService _auditLogService;
        private readonly IErrorAuditLogService _logService;
        /// <summary>
        /// Creating constructor and injection dbContext
        /// </summary>
        /// <param name="_context"></param>
        public CampaignDetailService(IErrorAuditLogService errorAuditLogService, IM10DbContext _context, IUserAuditLogService auditLogService)
        {
            context = _context;
            _auditLogService = auditLogService;
            _logService = errorAuditLogService;
        }

        /// <summary>
        /// Method is used to add/edit CampaignDetail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string AddCampaignDetail(CampaignDetailModel model, ref ErrorResponseModel errorResponseModel)
        {
            string message = "";
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    if (model.MarketingCampaignId == 0)
                    {
                        var existingPlayerEntity = context.PlayerDetails.FirstOrDefault(x => x.PlayerId == model.PlayerId && x.IsDeleted == false);
                        if (existingPlayerEntity != null)
                        {
                            var campaignEntity = new MarketingCampaign();
                            campaignEntity.MarketingCampaignId = model.MarketingCampaignId;
                            campaignEntity.Title = model.Title;
                            campaignEntity.Description = model.Description;
                            campaignEntity.PlayerId = model.PlayerId;
                            campaignEntity.ContentId = model.ContentId;
                            campaignEntity.ContentTypeId = model.ContentTypeId;
                            campaignEntity.CreatedDate = DateTime.Now;
                            campaignEntity.CreatedBy = model.CreatedBy;
                            campaignEntity.UpdatedDate = DateTime.Now;
                            campaignEntity.IsDeleted = false;
                            context.MarketingCampaigns.Add(campaignEntity);
                            context.SaveChanges();
                            transaction.Commit();
                            message = GlobalConstants.CampaignDetailsSaveMessage;
                            var AuditLog = new UserAuditLogModel();
                            AuditLog.Action = " Add Campaign Details";
                            AuditLog.Description = "Campaign Details Added Successfully";
                            AuditLog.UserId = model.CreatedBy != null ? (int)model.CreatedBy : model.UpdatedBy != null ? (int)model.UpdatedBy : 0;
                            AuditLog.CreatedBy = model.CreatedBy != null ? (int)model.CreatedBy : model.UpdatedBy != null ? (int)model.UpdatedBy : 0;
                            AuditLog.CreatedDate = DateTime.Now;
                            _auditLogService.AddUserAuditLog(AuditLog);
                        }
                        else
                        {
                            errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                            message = "Player Id does not exist";
                        }
                    }
                    else
                    {
                        var detailEntity = context.MarketingCampaigns.FirstOrDefault(x => x.MarketingCampaignId == model.MarketingCampaignId);
                        if (detailEntity != null)
                        {
                            var existingPlayerEntity = context.PlayerDetails.FirstOrDefault(x => x.PlayerId == model.PlayerId && x.IsDeleted == false);
                            if (existingPlayerEntity != null)
                            {
                                detailEntity.MarketingCampaignId = model.MarketingCampaignId;
                                detailEntity.Title = model.Title;
                                detailEntity.Description = model.Description;
                                detailEntity.PlayerId = model.PlayerId;
                                detailEntity.ContentId = model.ContentId;
                                detailEntity.ContentTypeId = model.ContentTypeId;
                                detailEntity.UpdatedDate = DateTime.Now;
                                detailEntity.UpdatedBy = model.UpdatedBy;
                                detailEntity.IsDeleted = false;
                                context.MarketingCampaigns.Update(detailEntity);
                                context.SaveChanges();
                                transaction.Commit();
                                message = GlobalConstants.CampaignDetailsUpdateMessage;
                            }
                            else
                            {
                                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                                message = "Player Id does not exist";
                            }
                        }
                    }
                    var userAuditLog = new UserAuditLogModel();
                    userAuditLog.Action = " Update Campaign Details";
                    userAuditLog.Description = "Campaign Details Updated Successfully";
                    userAuditLog.UserId = model.CreatedBy != null ? (int)model.CreatedBy : model.UpdatedBy != null ? (int)model.UpdatedBy : 0;
                    userAuditLog.UpdatedBy = model.CreatedBy != null ? (int)model.CreatedBy : model.UpdatedBy != null ? (int)model.UpdatedBy : 0;
                    userAuditLog.UpdatedDate = DateTime.Now;
                    _auditLogService.AddUserAuditLog(userAuditLog);
                }
                catch (Exception ex)
                {
                    int? userIdForLog = null;
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
                        LogSource = "Add/Editcampaingdetails",
                        UserId = userIdForLog,
                        LogMessage = "Exception occurred in Add/Editcampaingdetails method"
                    });
                    return null;
                }
            }
            return message;

        }


        /// <summary>
        /// Method is used to delete CampaignDetail
        /// </summary>
        /// <param name="marketingcampaignId"></param>
        /// <returns></returns>
        public string DeleteCampaignDetail(long marketingcampaignId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            string Message = "";
            using (var transaction = context.Database.BeginTransaction())
            {
                var campaignEntity = context.MarketingCampaigns.FirstOrDefault(x => x.MarketingCampaignId == marketingcampaignId);
                try
                {
                    if (campaignEntity != null)
                    {
                        var socialCampaign = context.CampaignDetails.FirstOrDefault(x => x.MarketingCampaignId == campaignEntity.MarketingCampaignId);
                        if (socialCampaign != null)
                        {
                            socialCampaign.IsDeleted = true;
                            context.SaveChanges();
                        }
                        if (campaignEntity.IsDeleted == true)
                        {
                            errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                            errorResponseModel.Message = "Campaign details already deleted.";
                            return null;
                        }
                        campaignEntity.IsDeleted = true;
                        context.SaveChanges();
                        transaction.Commit();
                        Message = GlobalConstants.CampaignDetailsDeleteMessage;

                    }
                    var userAuditLog = new UserAuditLogModel();
                    userAuditLog.Action = " Delete Campaign Details";
                    userAuditLog.Description = "Campaign Details Deleted";
                    userAuditLog.UserId = (int)campaignEntity.CreatedBy;
                    userAuditLog.CreatedDate = DateTime.Now;
                    userAuditLog.CreatedBy = campaignEntity.CreatedBy;
                    userAuditLog.UpdatedBy = campaignEntity.CreatedBy;
                    userAuditLog.UpdatedDate = DateTime.Now;
                    _auditLogService.AddUserAuditLog(userAuditLog);
                }
                catch (Exception ex)
                {
                    int? userIdForLog = null;
                    if (campaignEntity.UpdatedBy != 0)
                    {
                        userIdForLog = campaignEntity.UpdatedBy;
                    }
                    else
                    {
                        userIdForLog = campaignEntity.CreatedBy;
                    }

                    transaction.Rollback();
                    var errorMessage = _logService.SaveErrorLogs(new LogEntry
                    {
                        LogType = "Error",
                        StackTrace = "Stack Trace: " + ex.StackTrace,
                        AdditionalInformation = "Inner Exception: " + ex.InnerException + "Message: " + ex.Message,
                        CreatedDate = DateTime.Now,
                        LogSource = "Deletecampaingdetails",
                        UserId = userIdForLog,
                        LogMessage = "Exception occurred in Deletecampaingdetails method"
                    });
                    return null;
                }
            }
            return "{\"message\": \"" + Message + "\"}";
        }


        /// <summary>
        /// Method is used to get all CampaignDetail
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public List<CampaignDetailModel> GetAllCampaignDetail(ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            using (var transaction = context.Database.BeginTransaction())
            {
                List<CampaignDetailModel> campaignEntity = null;
                try
                {
                    campaignEntity = (from camp in context.MarketingCampaigns
                                      join
                                      content in context.ContentDetails on camp.ContentId equals content.ContentId
                                      join type in context.ContentTypes on camp.ContentTypeId equals type.ContentTypeId
                                      where camp.IsDeleted == false
                                      select new CampaignDetailModel
                                      {
                                          MarketingCampaignId = camp.MarketingCampaignId,
                                          Title = camp.Title,
                                          Description = camp.Description,
                                          PlayerId = camp.PlayerId,
                                          ContentId = camp.ContentId,
                                          ContentTypeId = camp.ContentTypeId,
                                          CreatedDate = camp.CreatedDate,
                                          CreatedBy = camp.CreatedBy,
                                          UpdatedDate = camp.UpdatedDate,
                                          UpdatedBy = camp.UpdatedBy,
                                          ContentTitle = content.Title,
                                          ContentTypeName = type.ContentName
                                      }).ToList();
                    if (campaignEntity.Count == 0)
                    {
                        errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                        errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                        return null;
                    }
                    transaction.Commit();
                    return campaignEntity.ToList();
                }
                catch (Exception ex)
                {
                    int? userIdForLog = null;
                    CampaignDetailModel? campaigndetailEntity = campaignEntity.FirstOrDefault();
                    if (campaigndetailEntity != null)
                    {
                        if (campaigndetailEntity.UpdatedBy != 0)
                        {
                            userIdForLog = campaigndetailEntity.UpdatedBy;
                        }
                        else
                        {
                            userIdForLog = campaigndetailEntity.CreatedBy;
                        }
                    }
                    transaction.Rollback();
                    var errorMessage = _logService.SaveErrorLogs(new LogEntry
                    {
                        LogType = "Error",
                        StackTrace = "Stack Trace: " + ex.StackTrace,
                        AdditionalInformation = "Inner Exception: " + ex.InnerException + "Message: " + ex.Message,
                        CreatedDate = DateTime.Now,
                        LogSource = "getallcampaingdetails",
                        UserId = userIdForLog,
                        LogMessage = "Exception occurred in getallcampaingdetails method"
                    });
                    return null;
                }
            }
        }


        /// <summary>
        /// Method is used to get CampaignDetail by id
        /// </summary>
        /// <param name="marketingcampaignId"></param>
        /// <returns></returns>
        public CampaignDetailModel GetCampaignDetailById(long marketingcampaignId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            using (var transaction = context.Database.BeginTransaction())
            {
                CampaignDetailModel? detailEntity = null;
                try
                {
                     detailEntity = (from camp in context.MarketingCampaigns
                                        join
                                        content in context.ContentDetails on camp.ContentId equals content.ContentId
                                        join type in context.ContentTypes on camp.ContentTypeId equals type.ContentTypeId
                                        join player in context.PlayerDetails on camp.PlayerId equals player.PlayerId
                                        where camp.MarketingCampaignId == marketingcampaignId && camp.IsDeleted == false && player.IsDeleted == false
                                        select new CampaignDetailModel
                                        {
                                            MarketingCampaignId = camp.MarketingCampaignId,
                                            Title = camp.Title,
                                            Description = camp.Description,
                                            PlayerId = camp.PlayerId,
                                            ContentId = camp.ContentId,
                                            ContentTypeId = camp.ContentTypeId,
                                            CreatedDate = camp.CreatedDate,
                                            CreatedBy = camp.CreatedBy,
                                            UpdatedDate = camp.UpdatedDate,
                                            UpdatedBy = camp.UpdatedBy,
                                            ContentTitle = content.Title,
                                            ContentTypeName = type.ContentName,
                                            IsDeleted = camp.IsDeleted,
                                            FullName = player.FirstName + " " + player.LastName,
                                        }).FirstOrDefault();
                    if (detailEntity == null)
                    {
                        errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                        errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                        return null;
                    }
                    transaction.Commit();
                    return detailEntity;
                }
                catch (Exception ex)
                {
                    int? userIdForLog = null;
                    CampaignDetailModel? campaigndetailEntity = detailEntity;
                    if (campaigndetailEntity != null)
                    {
                        if (campaigndetailEntity.UpdatedBy != 0)
                        {
                            userIdForLog = campaigndetailEntity.UpdatedBy;
                        }
                        else
                        {
                            userIdForLog = campaigndetailEntity.CreatedBy;
                        }
                    }
                    transaction.Rollback();
                    var errorMessage = _logService.SaveErrorLogs(new LogEntry
                    {
                        LogType = "Error",
                        StackTrace = "Stack Trace: " + ex.StackTrace,
                        AdditionalInformation = "Inner Exception: " + ex.InnerException + "Message: " + ex.Message,
                        CreatedDate = DateTime.Now,
                        LogSource = "GetCampaignDetailById",
                        UserId = userIdForLog,
                        LogMessage = "Exception occurred in GetCampaignDetailById method"
                    });
                    return null;
                }
            }

        }


        /// <summary>
        /// Method is used to get CampaignDetail by playerId
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public List<CampaignDetailModel> GetCampaignDetailByPlayerId(long playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            using (var transaction = context.Database.BeginTransaction())
            {
                List<CampaignDetailModel> campaignEntity = null;
                try
                {
                     campaignEntity = (from camp in context.MarketingCampaigns
                                          join
                                          content in context.ContentDetails on camp.ContentId equals content.ContentId
                                          join type in context.ContentTypes on camp.ContentTypeId equals type.ContentTypeId
                                          where camp.PlayerId == playerId && camp.IsDeleted == false
                                          orderby camp.UpdatedDate descending
                                          select new CampaignDetailModel
                                          {
                                              MarketingCampaignId = camp.MarketingCampaignId,
                                              Title = camp.Title,
                                              Description = camp.Description,
                                              PlayerId = camp.PlayerId,
                                              ContentId = camp.ContentId,
                                              ContentTypeId = camp.ContentTypeId,
                                              ContentTitle = content.Title,
                                              ContentTypeName = type.ContentName
                                          }).ToList();
                    if (campaignEntity.Count == 0)
                    {
                        errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                        errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                        return null;
                    }
                    transaction.Commit();
                    return campaignEntity.ToList();
                }
                catch (Exception ex)
                {
                    int? userIdForLog = null;
                    CampaignDetailModel? campaigndetailEntity = campaignEntity.FirstOrDefault();
                    if (campaigndetailEntity != null)
                    {
                        if (campaigndetailEntity.UpdatedBy != 0)
                        {
                            userIdForLog = campaigndetailEntity.UpdatedBy;
                        }
                        else
                        {
                            userIdForLog = campaigndetailEntity.CreatedBy;
                        }
                    }
                    transaction.Rollback();
                    var errorMessage = _logService.SaveErrorLogs(new LogEntry
                    {
                        LogType = "Error",
                        StackTrace = "Stack Trace: " + ex.StackTrace,
                        AdditionalInformation = "Inner Exception: " + ex.InnerException + "Message: " + ex.Message,
                        CreatedDate = DateTime.Now,
                        LogSource = "GetCampaignDetailByPlayerId",
                        UserId = userIdForLog,
                        LogMessage = "Exception occurred in GetCampaignDetailByPlayerId method"
                    });
                    return null;
                }
            }
        }
    }
}