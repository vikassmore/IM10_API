using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Implementaion
{
    /// <summary>
    /// This is implementation for the CampaignSocialMediaDetail operations 
    /// </summary>
    public class CampaignSocialMediaDetailService : ICampaignSocialMediaDetailService
    {
        IM10DbContext context;
        private ConfigurationModel _configuration;
        private readonly IUserAuditLogService _userAuditLogService;
        private readonly IErrorAuditLogService _logService;
        /// <summary>
        /// Creating constructor and injection dbContext
        /// </summary>
        /// <param name="_context"></param>
        public CampaignSocialMediaDetailService(IErrorAuditLogService errorAuditLogService,IM10DbContext _context, IOptions<ConfigurationModel> hostName, IUserAuditLogService userAuditLogService)
        {
            context = _context;
            _configuration = hostName.Value;
            _userAuditLogService = userAuditLogService;
            _logService = errorAuditLogService;
        }

        /// <summary>
        /// Method is used to add/edit CampaignSocialMediaDetail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string AddCampaignSocialMediaDetail(CampaignSocialMediaDetailModel model, ref ErrorResponseModel errorResponseModel)
        {
            string message = "";
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    if (model.CampaignId == 0)
                    {
                        CampaignDetail detail = new CampaignDetail();
                        detail.CampaignId = model.CampaignId;
                        detail.SocialMediaViews = model.SocialMediaViews;
                        detail.ScreenShotFileName = model.ScreenShotFileName;
                        detail.ScreenShotFilePath = model.ScreenShotFilePath;
                        detail.MarketingCampaignId = model.MarketingCampaignId;
                        detail.CreatedDate = DateTime.Now;
                        detail.CreatedBy = model.CreatedBy;
                        detail.UpdatedDate = DateTime.Now;
                        detail.IsDeleted = false;
                        context.CampaignDetails.Add(detail);
                        context.SaveChanges();
                        transaction.Commit();
                        message = GlobalConstants.CampaignSocialMediaDetailsSaveMessage;
                        var userAuditLog1 = new UserAuditLogModel();
                        userAuditLog1.Action = " Add Campaign Social Media Details";
                        userAuditLog1.Description = "Campaign Social Media Added Successfully";
                        userAuditLog1.UserId = model.CreatedBy != null ? (int)model.CreatedBy : model.UpdatedBy != null ? (int)model.UpdatedBy : 0;
                        userAuditLog1.CreatedBy = model.CreatedBy != null ? (int)model.CreatedBy : model.UpdatedBy != null ? (int)model.UpdatedBy : 0;
                        userAuditLog1.CreatedDate = DateTime.Now;
                       
                    }
                    else
                    {
                        var campaignEntity = context.CampaignDetails.FirstOrDefault(x => x.CampaignId == model.CampaignId);
                        if (campaignEntity != null)
                        {
                            campaignEntity.CampaignId = model.CampaignId;
                            campaignEntity.SocialMediaViews = model.SocialMediaViews;
                            campaignEntity.ScreenShotFileName = model.ScreenShotFileName;
                            campaignEntity.ScreenShotFilePath = model.ScreenShotFilePath;
                            campaignEntity.MarketingCampaignId = model.MarketingCampaignId;
                            campaignEntity.UpdatedDate = DateTime.Now;
                            campaignEntity.UpdatedBy = model.UpdatedBy;
                            campaignEntity.IsDeleted = false;
                            context.CampaignDetails.Update(campaignEntity);
                            context.SaveChanges();
                            transaction.Commit ();
                            message = GlobalConstants.CampaignSocialMediaDetailsUpdateMessage;
                        }
                    }
                    var userAuditLog = new UserAuditLogModel();
                    userAuditLog.Action = " Update Campaign Social Media Details";
                    userAuditLog.Description = "Campaign Social Media Updated Successfully";
                    userAuditLog.UserId = model.CreatedBy != null ? (int)model.CreatedBy : model.UpdatedBy != null ? (int)model.UpdatedBy : 0;
                    userAuditLog.CreatedBy = model.CreatedBy != null ? (int)model.CreatedBy : model.UpdatedBy != null ? (int)model.UpdatedBy : 0;
                    userAuditLog.CreatedDate = DateTime.Now;
                    userAuditLog.UpdatedBy = model.CreatedBy != null ? (int)model.CreatedBy : model.UpdatedBy != null ? (int)model.UpdatedBy : 0;
                    userAuditLog.UpdatedDate = DateTime.Now;
                    _userAuditLogService.AddUserAuditLog(userAuditLog);
                }
                catch(Exception ex) 
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
                        StackTrace = ex.StackTrace,
                        AdditionalInformation = ex.Message,
                        CreatedDate = DateTime.Now,
                        LogSource = "Add/EditCampaignSocialMediaDetail",
                        UserId = userIdForLog,
                        LogMessage = "Exception occurred in Add/EditCampaignSocialMediaDetail method"
                    });
                    return null;
                }
            }
            return message;
        }


        


        /// <summary>
        /// Method is used to get CampaignSocialMediaDetail by marketingcampaignId
        /// </summary>
        /// <param name="marketingcampaignId"></param>
        /// <returns></returns>
        public List< CampaignSocialMediaDetailModel> GetCampaignSocialMediaDetailById(long marketingcampaignId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            using (var transaction = context.Database.BeginTransaction())
            {
                var campaignList = new List<CampaignSocialMediaDetailModel>();
                try
                {
                    var campaignEntity = (from camp in context.CampaignDetails join
                                         details in context.MarketingCampaigns on
                                         camp.MarketingCampaignId equals details.MarketingCampaignId
                                          where camp.MarketingCampaignId == marketingcampaignId && camp.IsDeleted == false
                                          orderby camp.UpdatedDate descending
                                          select new
                                          {
                                              camp.CampaignId,
                                              camp.SocialMediaViews,
                                              camp.ScreenShotFileName,
                                              camp.ScreenShotFilePath,
                                              camp.MarketingCampaignId,
                                              details.Title
                                          }).ToList();
                    if (campaignEntity.Count == 0)
                    {
                        errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                        errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                    }
                    campaignEntity.ForEach(item =>
                    {
                        var imgmodel = new VideoImageModel();
                        imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(item.ScreenShotFilePath) ? item.ScreenShotFilePath : item.ScreenShotFilePath);
                        imgmodel.Type = String.IsNullOrEmpty(item.ScreenShotFilePath) ? "video" : "image";
                        // imgModel.thumbnail = _configuration.HostName.TrimEnd('/') + "/thumbnail/" + imgModel.url
                        imgmodel.FileName = (imgmodel.url);

                        campaignList.Add(new CampaignSocialMediaDetailModel
                        {
                            CampaignId = item.CampaignId,
                            SocialMediaViews = item.SocialMediaViews,
                            ScreenShotFileName = item.ScreenShotFileName,
                            ScreenShotFilePath = imgmodel.FileName,
                            MarketingCampaignId = item.CampaignId,
                            CampaignTitle = item.Title,
                        });
                    });
                    transaction.Commit();
                    return campaignList;
                }
                catch (Exception ex)
                {
                    int? userIdForLog = null;
                    CampaignSocialMediaDetailModel? campaigndetailEntity = campaignList.FirstOrDefault();
                    if (campaigndetailEntity.UpdatedBy != 0)
                    {
                        userIdForLog = campaigndetailEntity.UpdatedBy;
                    }
                    else
                    {
                        userIdForLog = campaigndetailEntity.CreatedBy;
                    }

                    transaction.Rollback();
                    var errorMessage = _logService.SaveErrorLogs(new LogEntry
                    {
                        LogType = "Error",
                        StackTrace = ex.StackTrace,
                        AdditionalInformation = ex.Message,
                        CreatedDate = DateTime.Now,
                        LogSource = "GetCampaignSocialMediaDetail",
                        UserId = userIdForLog,
                        LogMessage = "Exception occurred in GetCampaignSocialMediaDetail method"
                    });
                    return null;
                }
            }
        }
    }
}
