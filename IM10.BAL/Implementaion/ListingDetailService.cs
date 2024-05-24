using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Implementaion
{
    /// <summary>
    /// This is implementation for the ListingDetail operations 
    /// </summary>
    public class ListingDetailService : IListingDetailService
    {
        IM10DbContext context;
        private ConfigurationModel _configuration;
        private readonly IUserAuditLogService _userAuditLogService;
        /// <summary>
        /// Creating constructor and injection dbContext
        /// </summary>
        /// <param name="_context"></param>
        public ListingDetailService(IM10DbContext _context, IOptions<ConfigurationModel> hostName, IUserAuditLogService userAuditLogService)
        {
            context = _context;
            this._configuration = hostName.Value;
            _userAuditLogService = userAuditLogService;
        }

        /// <summary>
        /// Method to add ListingDetail
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public string AddListingDetail(ListingDetailModel1 model, ref ErrorResponseModel errorResponseModel)
        {
            string message = "";
            if (model.ListingId == 0)
            {
                var existingPlayerEntity = context.PlayerDetails.FirstOrDefault(x => x.PlayerId == model.PlayerId && x.IsDeleted == false);
                if (existingPlayerEntity != null)
                {
                    ListingDetail detail = new ListingDetail();
                    detail.ListingId = model.ListingId;
                    detail.PlayerId = model.PlayerId;
                    detail.CompanyName = model.CompanyName;
                    detail.Description = model.Description;
                    detail.ContactPersonName = model.ContactPersonName;
                    detail.ContactPersonEmailId = model.ContactPersonEmailId;
                    detail.ContactPersonMobile = model.ContactPersonMobile;
                    detail.CompanyEmailId = model.CompanyEmailId;
                    detail.CompanyMobile = model.CompanyMobile;
                    detail.CompanyPhone = model.CompanyPhone;
                    detail.CompanyWebSite = model.CompanyWebSite;
                    detail.CompanyLogoFileName = model.CompanyLogoFileName;
                    detail.CompanyLogoFilePath = model.CompanyLogoFilePath;
                    detail.NationId = model.NationId;
                    detail.StateId = model.StateId;
                    detail.CityId = model.CityId;
                    detail.IsGlobal = model.IsGlobal;
                    detail.StartDate = model.StartDate;
                    detail.EndDate = model.EndDate;
                    detail.FinalPrice = model.FinalPrice;
                    detail.Position = model.Position;
                    detail.CreatedBy = model.CreatedBy;
                    detail.CreatedDate = DateTime.Now;
                    detail.UpdatedDate = DateTime.Now;
                    context.ListingDetails.Add(detail);
                    context.SaveChanges();
                    message = GlobalConstants.ListingDetailAddedSuccessfully;
                    var userAuditLog = new UserAuditLogModel();
                    userAuditLog.Action = " Add Listing Details";
                    userAuditLog.Description = "Listing Details Added";
                    userAuditLog.UserId = (int)model.CreatedBy;
                    userAuditLog.CreatedBy = model.CreatedBy;
                    userAuditLog.CreatedDate = DateTime.Now;
                    _userAuditLogService.AddUserAuditLog(userAuditLog);
                }
                else
                {
                    errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                    message = "Player Id does not exist";
                }
            }
            else
            {
                var listEntity = context.ListingDetails.FirstOrDefault(x => x.ListingId == model.ListingId);
                if (listEntity != null)
                {
                    var existingPlayerEntity = context.PlayerDetails.FirstOrDefault(x => x.PlayerId == model.PlayerId && x.IsDeleted == false);
                    if (existingPlayerEntity != null)
                    {
                        listEntity.ListingId = model.ListingId;
                        listEntity.PlayerId = model.PlayerId;
                        listEntity.CompanyName = model.CompanyName;
                        listEntity.Description = model.Description;
                        listEntity.ContactPersonName = model.ContactPersonName;
                        listEntity.ContactPersonEmailId = model.ContactPersonEmailId;
                        listEntity.ContactPersonMobile = model.ContactPersonMobile;
                        listEntity.CompanyEmailId = model.CompanyEmailId;
                        listEntity.CompanyMobile = model.CompanyMobile;
                        listEntity.CompanyPhone = model.CompanyPhone;
                        listEntity.CompanyWebSite = model.CompanyWebSite;
                        if (model.CompanyLogoFileName != null)
                        {
                            listEntity.CompanyLogoFileName = model.CompanyLogoFileName;
                        }
                        if (model.CompanyLogoFilePath != null)
                        {
                            listEntity.CompanyLogoFilePath = model.CompanyLogoFilePath;
                        }
                        //listEntity.CompanyLogoFileName = model.CompanyLogoFileName;
                        //listEntity.CompanyLogoFilePath = model.CompanyLogoFilePath;
                        listEntity.NationId = model.NationId;
                        listEntity.StateId = model.StateId;
                        listEntity.CityId = model.CityId;
                        listEntity.IsGlobal = model.IsGlobal;
                        listEntity.StartDate = model.StartDate;
                        listEntity.EndDate = model.EndDate;
                        listEntity.FinalPrice = model.FinalPrice;
                        listEntity.Position = model.Position;
                        listEntity.UpdatedBy = model.UpdatedBy;
                        listEntity.UpdatedDate = DateTime.Now;
                        context.ListingDetails.Update(listEntity);
                        context.SaveChanges();
                        message = GlobalConstants.ListingDetailUpdateSuccessfully;
                        var userAuditLog = new UserAuditLogModel();
                        userAuditLog.Action = " Update Listing Details";
                        userAuditLog.Description = "Listing Details Updated";
                        userAuditLog.UserId = (int)model.UpdatedBy;
                        userAuditLog.UpdatedBy = model.UpdatedBy;
                        userAuditLog.UpdatedDate = DateTime.Now;
                        _userAuditLogService.AddUserAuditLog(userAuditLog);
                    }
                    else
                    {
                        errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                        message = "Player Id does not exist";
                    }
                }
            }
            return message;
        }


        /// <summary>
        /// Method to delete ListingDetail
        /// </summary>
        /// <param name="listingId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public string DeleteListingDetail(long listingId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            string Message = "";
            var listingEntity = context.ListingDetails.FirstOrDefault(x => x.ListingId == listingId);
            if (listingEntity != null)
            {
                if (listingEntity.IsDeleted == true)
                {
                    errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                    errorResponseModel.Message = "Listing details already deleted.";
                    return null;
                }
                listingEntity.IsDeleted = true;
                context.SaveChanges();
                Message = GlobalConstants.ListingDetailDeleteSuccessfully;

            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Delete Listing Details";
            userAuditLog.Description = "Listing Details Deleted";
            userAuditLog.UserId = (int)listingEntity.CreatedBy;
            userAuditLog.CreatedDate = DateTime.Now;
            userAuditLog.CreatedBy = listingEntity.CreatedBy;
            userAuditLog.UpdatedBy = listingEntity.CreatedBy;
            userAuditLog.UpdatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return "{\"message\": \"" + Message + "\"}";
        }

        /// <summary>
        /// Method to get ListingDetail by listingId
        /// </summary>
        /// <param name="listingId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public ListingDetailModel GetListingDetailById(long listingId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var listEntity = (from list in context.ListingDetails
                              join
                              nation in context.Countries on list.NationId equals nation.CountryId
                              join state in context.States on list.StateId equals state.StateId
                              join city in context.Cities on list.CityId equals city.CityId
                              where list.IsDeleted == false && list.ListingId == listingId
                              select new ListingDetailModel
                              {
                                  ListingId = list.ListingId,
                                  PlayerId = list.PlayerId,
                                  CompanyEmailId = list.CompanyEmailId,
                                  CompanyLogoFileName = list.CompanyLogoFileName,
                                  CompanyName = list.CompanyName,
                                  Description = list.Description,
                                  CompanyLogoFilePath = list.CompanyLogoFilePath,
                                  CompanyWebSite = list.CompanyWebSite,
                                  CompanyMobile = list.CompanyMobile,
                                  CompanyPhone = list.CompanyPhone,
                                  IsGlobal = (list.IsGlobal == null) ? false : list.IsGlobal,
                                  NationId = list.NationId,
                                  NationName = nation.Name,
                                  StateId = list.StateId,
                                  StateName = state.Name,
                                  CityId = list.CityId,
                                  CityName = city.Name,
                                  StartDate = list.StartDate,
                                  EndDate = list.EndDate,
                                  FinalPrice = (list.FinalPrice == null) ? null : list.FinalPrice.ToString(),
                                  Position = list.Position,
                                  ContactPersonEmailId = list.ContactPersonEmailId,
                                  ContactPersonName = list.ContactPersonName,
                                  ContactPersonMobile = list.ContactPersonMobile,
                              }).FirstOrDefault();

            if (listEntity == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            var imgmodel = new VideoImageModel();
            imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(listEntity.CompanyLogoFilePath) ? listEntity.CompanyLogoFilePath : listEntity.CompanyLogoFilePath);
            imgmodel.Type = String.IsNullOrEmpty(listEntity.CompanyLogoFilePath) ? "video" : "image";
            // imgModel.thumbnail = _configuration.HostName.TrimEnd('/') + "/thumbnail/" + imgModel.url
            imgmodel.FileName = (imgmodel.url);
            return new ListingDetailModel
            {
                ListingId = listEntity.ListingId,
                PlayerId = listEntity.PlayerId,
                CompanyEmailId = listEntity.CompanyEmailId,
                CompanyLogoFileName = listEntity.CompanyLogoFileName,
                CompanyName = listEntity.CompanyName,
                Description = listEntity.Description,
                CompanyLogoFilePath = imgmodel.FileName,
                CompanyWebSite = listEntity.CompanyWebSite,
                CompanyMobile = listEntity.CompanyMobile,
                CompanyPhone = listEntity.CompanyPhone,
                IsGlobal = listEntity.IsGlobal,
                NationId = listEntity.NationId,
                NationName = listEntity.NationName,
                StateId = listEntity.StateId,
                StateName = listEntity.StateName,
                CityId = listEntity.CityId,
                CityName = listEntity.CityName,
                StartDate = listEntity.StartDate,
                EndDate = listEntity.EndDate,
                FinalPrice = listEntity.FinalPrice,
                Position = listEntity.Position,
                ContactPersonEmailId = listEntity.ContactPersonEmailId,
                ContactPersonName = listEntity.ContactPersonName,
                ContactPersonMobile = listEntity.ContactPersonMobile,
            };
        }


        /// <summary>
        /// Method is used to get ListingDetail by playerId
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public List<ListingDetailModel> GetListingDetailByplayerId(long playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();

            var listEntity = (from list in context.ListingDetails
                              join user in context.UserMasters on list.CreatedBy equals (int)user.UserId
                              where list.IsDeleted == false && list.PlayerId == playerId
                              orderby list.UpdatedDate descending

                              select new ListingDetailModel
                              {
                                  ListingId = list.ListingId,
                                  PlayerId = list.PlayerId,
                                  RoleId = user.RoleId,
                                  CompanyName = list.CompanyName,
                                  IsGlobal = (list.IsGlobal == null) ? false : list.IsGlobal,
                                  StartDate = list.StartDate,
                                  EndDate = list.EndDate,
                                  FinalPrice = (list.FinalPrice == null) ? null : list.FinalPrice.ToString(),
                                  Position = (list.Position == null) ? null : list.Position,
                                  ContactPersonName = list.ContactPersonName,
                                  CompanyLogoFileName= list.CompanyLogoFileName,
                                  CompanyLogoFilePath= _configuration.HostName + list.CompanyLogoFilePath,
                              }).ToList();

            if (listEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            return listEntity.ToList();           
        }
    }
}