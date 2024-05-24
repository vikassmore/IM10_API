using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Implementaion
{
    /// <summary>
    /// This is implementation for the AdvContentDetail operations 
    /// </summary>
    public class AdvContentDetailService : IAdvContentDetailService
    {
        IM10DbContext context;
        private ConfigurationModel _configuration;
        private readonly IUserAuditLogService _userAuditLogService;


        /// <summary>
        /// Creating constructor and injection dbContext
        /// </summary>
        /// <param name="_context"></param>
        public AdvContentDetailService(IM10DbContext _context ,IOptions<ConfigurationModel> hostName, IUserAuditLogService userAuditLogService)
        {
             context= _context;
            _configuration= hostName.Value;
            _userAuditLogService= userAuditLogService;
        }


        /// <summary>
        /// Method to add AdvContentDetail
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public string AddAdvContentDetail(AdvContentDetailsModel1 model, ref ErrorResponseModel errorResponseModel)
        {
            string message = "";
            if (model.AdvertiseContentId == 0)
            {
                var existingPlayerEntity = context.PlayerDetails.FirstOrDefault(x => x.PlayerId == model.PlayerId && x.IsDeleted == false);
                if (existingPlayerEntity != null)
                {
                    AdvContentDetail contentDetail = new AdvContentDetail();
                    contentDetail.AdvertiseContentId = model.AdvertiseContentId;
                    contentDetail.Title = model.Title;
                    contentDetail.NationId = model.NationId;
                    contentDetail.StateId = model.StateId;
                    contentDetail.CityId = model.CityId;
                    contentDetail.PlayerId = model.PlayerId;
                    contentDetail.AdvertiseFileName = model.AdvertiseFileName;
                    contentDetail.AdvertiseFilePath = model.AdvertiseFilePath;
                    contentDetail.IsGlobal = model.IsGlobal;
                    contentDetail.ContentTypeId = model.ContentTypeId;
                    contentDetail.IsFree = model.IsFree;
                    contentDetail.StartDate = model.StartDate;
                    contentDetail.EndDate = model.EndDate;
                    contentDetail.Approved = null;
                    contentDetail.CreatedBy = model.CreatedBy;
                    contentDetail.CreatedDate = DateTime.Now;
                    contentDetail.FinalPrice = model.FinalPrice;
                    contentDetail.IsDeleted = false;
                    context.AdvContentDetails.Add(contentDetail);
                    context.SaveChanges();
                    message = GlobalConstants.AdvContentDetailSaveMessage;
                }
                else
                {
                    errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                    message = "Player Id does not exist";
                }
            }
            else
            {
                var advEntity = context.AdvContentDetails.FirstOrDefault(x => x.AdvertiseContentId == model.AdvertiseContentId);
                if (advEntity != null)
                {
                    var existingPlayerEntity = context.PlayerDetails.FirstOrDefault(x => x.PlayerId == model.PlayerId && x.IsDeleted == false);
                    if (existingPlayerEntity != null)
                    {
                        advEntity.Title = model.Title;
                        advEntity.NationId = model.NationId;
                        advEntity.StateId = model.StateId;
                        advEntity.CityId = model.CityId;
                        advEntity.PlayerId = model.PlayerId;
                        if (model.AdvertiseFileName != null)
                        {
                            advEntity.AdvertiseFileName = model.AdvertiseFileName;
                        }
                        if (model.AdvertiseFilePath != null)
                        {
                            advEntity.AdvertiseFilePath = model.AdvertiseFilePath;
                        }
                        //advEntity.AdvertiseFileName = model.AdvertiseFileName;
                        //advEntity.AdvertiseFilePath = model.AdvertiseFilePath;
                        advEntity.IsGlobal = model.IsGlobal;
                        advEntity.ContentTypeId = model.ContentTypeId;
                        advEntity.IsFree = model.IsFree;
                        advEntity.StartDate = model.StartDate;
                        advEntity.EndDate = model.EndDate;
                        advEntity.Approved = null;
                        advEntity.UpdatedBy = model.UpdatedBy;
                        advEntity.UpdatedDate = DateTime.Now;
                        advEntity.FinalPrice = model.FinalPrice;
                        advEntity.IsDeleted = false;
                        context.AdvContentDetails.Update(advEntity);
                        context.SaveChanges();
                        message = GlobalConstants.AdvContentDetailUpdateMessage;
                    }
                    else
                    {
                        errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                        message = "Player Id does not exist";
                    }
                }
            }

            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Add Advertise Content Details";
            userAuditLog.Description = "Advertise Content Details Added";
            userAuditLog.UserId = model.CreatedBy != null ? (int)model.CreatedBy : model.UpdatedBy != null ? (int)model.UpdatedBy : 0;
            userAuditLog.CreatedBy = model.CreatedBy != null ? (int)model.CreatedBy : model.UpdatedBy != null ? (int)model.UpdatedBy : 0;
            userAuditLog.CreatedDate = DateTime.Now;
            userAuditLog.UpdatedBy = model.CreatedBy != null ? (int)model.CreatedBy : model.UpdatedBy != null ? (int)model.UpdatedBy : 0;
            userAuditLog.UpdatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return message;
        }


        /// <summary>
        /// Method to approve advcontentdetail 
        /// </summary>
        /// <param name="advertisecontentId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public string ApproveAdvContentDetail(long advertisecontentId, ref ErrorResponseModel errorResponseModel)
        {
            string Message = "";
            var userplayerEntity = context.AdvContentDetails.FirstOrDefault(x => x.AdvertiseContentId == advertisecontentId);
            if (userplayerEntity != null)
            {
                userplayerEntity.Approved = true;
                context.SaveChanges();
                Message = GlobalConstants.ApprovedSuccessfully;
            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Approve Advertise Content Details";
            userAuditLog.Description = "Advertise Content Details Approved";
            userAuditLog.UserId = (int)userplayerEntity.CreatedBy;
            userAuditLog.CreatedBy = userplayerEntity.CreatedBy;
            userAuditLog.CreatedDate= DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return Message;
        }

        /// <summary>
        /// Method to delete advcontentdetail 
        /// </summary>
        /// <param name="AdvertiseContentId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public string DeleteAdvContentDetail(long AdvertiseContentId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            string Message = "";
            var advtcontentEntity = context.AdvContentDetails.FirstOrDefault(x => x.AdvertiseContentId == AdvertiseContentId);
            if (advtcontentEntity != null)
            {
                if (advtcontentEntity.Approved == true)
                {
                    errorResponseModel.StatusCode = HttpStatusCode.BadRequest;
                    Message = "Cannot delete approved advtcontent";
                    return Message;
                }
                if (advtcontentEntity.IsDeleted == true)
                {
                    errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                    errorResponseModel.Message = "Advt content details already deleted.";
                    return null;
                }
                advtcontentEntity.IsDeleted = true;
                context.SaveChanges();
                Message = GlobalConstants.AdvContentDetailDeleteMessage;

            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Delete Advertise Content Details";
            userAuditLog.Description = "Advertise Content Details Deleted";
            userAuditLog.UserId = (int)advtcontentEntity.CreatedBy;
            userAuditLog.CreatedDate = DateTime.Now;
            userAuditLog.CreatedBy = advtcontentEntity.CreatedBy;
            userAuditLog.UpdatedBy = advtcontentEntity.CreatedBy;
            userAuditLog.UpdatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return "{\"message\": \"" + Message + "\"}";
        }

        public string DeniedAdvContentDetail(AdvContentComment model, ref ErrorResponseModel errorResponseModel)
        {
            string Message = "";
            var contentupdateEntity = context.AdvContentDetails.FirstOrDefault(x => x.AdvertiseContentId == model.AdvertiseContentId);
            if (contentupdateEntity != null)
            {
                contentupdateEntity.Approved = false;
                contentupdateEntity.Comment = model.Comment;
                context.SaveChanges();
                Message = GlobalConstants.DeniedSuccessfully;
            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Denie Advertise Content Details";
            userAuditLog.Description = "Advertise Content Details Denied";
            userAuditLog.UserId = (int)contentupdateEntity.CreatedBy;
            userAuditLog.UpdatedBy = contentupdateEntity.UpdatedBy;
            userAuditLog.UpdatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return Message;
        }

        /// <summary>
        /// Method is used to get advtcontentdetail by playerId
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>

        public List<AdvContentDetailsModel> GetAdvContentByPlayerId(long playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var advList = new List<AdvContentDetailsModel>();
            var advEntityList = (from adv in context.AdvContentDetails                                
                                 join content in context.ContentTypes on adv.ContentTypeId equals content.ContentTypeId
                                 where adv.PlayerId==playerId && adv.IsDeleted == false
                                 orderby (adv.UpdatedDate == null ? adv.CreatedDate : adv.UpdatedDate) descending
                                 select new AdvContentDetailsModel
                                 {
                                     AdvertiseContentId = adv.AdvertiseContentId,
                                     Title = adv.Title,
                                     PlayerId = adv.PlayerId,
                                     IsGlobal = adv.IsGlobal,
                                     AdvertiseFileName = adv.AdvertiseFileName,
                                     AdvertiseFilePath = adv.AdvertiseFilePath,
                                     ContentTypeId = adv.ContentTypeId,
                                     ContentName = content.ContentName,
                                     StartDate = adv.StartDate,
                                     EndDate = adv.EndDate,
                                     Comment = adv.Comment,
                                     Approved= adv.Approved,
                                     FinalPrice = adv.FinalPrice,
                                     IsFree = adv.IsFree                                
                                 }).ToList();
            if (advEntityList.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;
            }
            advEntityList.ForEach(item =>
            {
                var imgmodel = new VideoImageModel();
                imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(item.AdvertiseFilePath) ? item.AdvertiseFilePath : item.AdvertiseFilePath);
                imgmodel.Type = String.IsNullOrEmpty(item.AdvertiseFilePath) ? "video" : "image";
                // imgModel.thumbnail = _configuration.HostName.TrimEnd('/') + "/thumbnail/" + imgModel.url
                imgmodel.FileName = (imgmodel.url);
                advList.Add(new AdvContentDetailsModel
                {
                    AdvertiseContentId = item.AdvertiseContentId,
                    Title = item.Title,
                    PlayerId = item.PlayerId,                  
                    IsGlobal = item.IsGlobal,
                    AdvertiseFileName = item.AdvertiseFileName,
                    AdvertiseFilePath = imgmodel.FileName,
                    ContentTypeId = item.ContentTypeId,
                    ContentName = item.ContentName,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    Comment= item.Comment,
                    Approved = item.Approved,
                    FinalPrice = item.FinalPrice,
                    IsFree = item.IsFree,                  
                });
            });
            return advList;
        }


        /// <summary>
        /// Method is used to get approvedadvtcontentdetail by playerId
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>

        public List<AdvContentDetailsModel> GetApprovedAdvContentByPlayerId(long playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var advEntityList = (from adv in context.AdvContentDetails
                                 join content in context.ContentTypes on adv.ContentTypeId equals content.ContentTypeId
                                 where adv.PlayerId == playerId && adv.IsDeleted == false &&
                                 adv.Approved==true
                                 orderby adv.UpdatedDate descending
                                 select new AdvContentDetailsModel
                                 {
                                     AdvertiseContentId = adv.AdvertiseContentId,
                                     Title = adv.Title,
                                     PlayerId = adv.PlayerId,                                                                    
                                 }).ToList();
            if (advEntityList.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;
            }            
            return advEntityList.ToList();
        }

        /// <summary>
        /// Method to get advcontentdetail by advertisecontentId
        /// </summary>
        /// <param name="AdvertiseContentId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public AdvContentDetailsModel GetAdvContentDetailById(long AdvertiseContentId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var advEntity = (from adv in context.AdvContentDetails join
                                 country in context.Countries on adv.NationId equals country.CountryId
                                 join state in context.States on adv.StateId equals state.StateId
                                 join city in context.Cities on adv.CityId equals city.CityId
                                 join content in context.ContentTypes on adv.ContentTypeId equals content.ContentTypeId
                                 where adv.IsDeleted == false && adv.AdvertiseContentId==AdvertiseContentId
                                 select new AdvContentDetailsModel
                                 {
                                     AdvertiseContentId = adv.AdvertiseContentId,
                                     Title = adv.Title,
                                     PlayerId = adv.PlayerId,
                                     NationId = adv.NationId,
                                     NationName = country.Name,
                                     StateId = adv.StateId,
                                     StateName = state.Name,
                                     CityId = adv.CityId,
                                     CityName = city.Name,
                                     IsGlobal = adv.IsGlobal,
                                     AdvertiseFileName = adv.AdvertiseFileName,
                                     AdvertiseFilePath = adv.AdvertiseFilePath,
                                     ContentTypeId = adv.ContentTypeId,
                                     ContentName = content.ContentName,
                                     StartDate = adv.StartDate,
                                     EndDate = adv.EndDate,
                                     Approved=adv.Approved,
                                     FinalPrice = adv.FinalPrice,
                                     IsFree = adv.IsFree,
                                     CreatedBy = adv.CreatedBy,
                                     CreatedDate = adv.CreatedDate,
                                     UpdatedBy = adv.UpdatedBy,
                                     UpdatedDate = adv.UpdatedDate,

                                 }).FirstOrDefault();
            if (advEntity == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;
            }

            var imgmodel = new VideoImageModel();
            imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(advEntity.AdvertiseFilePath) ? advEntity.AdvertiseFilePath : advEntity.AdvertiseFilePath);
            imgmodel.Type = String.IsNullOrEmpty(advEntity.AdvertiseFilePath) ? "video" : "image";
            // imgModel.thumbnail = _configuration.HostName.TrimEnd('/') + "/thumbnail/" + imgModel.url
            imgmodel.FileName = (imgmodel.url);
            return new AdvContentDetailsModel
            {
                AdvertiseContentId = advEntity.AdvertiseContentId,
                Title = advEntity.Title,
                PlayerId = advEntity.PlayerId,
                NationId = advEntity.NationId,
                NationName = advEntity.NationName,
                StateId = advEntity.StateId,
                StateName = advEntity.StateName,
                CityId = advEntity.CityId,
                CityName = advEntity.CityName,
                IsGlobal = advEntity.IsGlobal,
                AdvertiseFileName = advEntity.AdvertiseFileName,
                AdvertiseFilePath = imgmodel.FileName,
                ContentTypeId = advEntity.ContentTypeId,
                ContentName = advEntity.ContentName,
                StartDate = advEntity.StartDate,
                EndDate = advEntity.EndDate,
                Approved = advEntity.Approved,
                FinalPrice = advEntity.FinalPrice,
                IsFree = advEntity.IsFree,
                CreatedBy = advEntity.CreatedBy,
                CreatedDate = advEntity.CreatedDate,
                UpdatedBy = advEntity.UpdatedBy,
                UpdatedDate = advEntity.UpdatedDate,
            };          
        }       
    }
}
