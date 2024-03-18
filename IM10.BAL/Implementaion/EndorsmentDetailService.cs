using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Implementaion
{
    /// <summary>
    /// This is implementation for the EndorsmentDetailService operations 
    /// </summary>
    /// 
    public class EndorsmentDetailService : IEndorsmentDetailService
    {
        IM10DbContext context;
        private readonly IUserAuditLogService _userAuditLogService;

        /// <summary>
        /// Creating constructor and injection dbContext
        /// </summary>
        /// <param name="_context"></param>
        public EndorsmentDetailService(IM10DbContext _context, IUserAuditLogService userAuditLogService)
        {
            context = _context;
            _userAuditLogService = userAuditLogService;
        }


        /// <summary>
        /// Method is used to add/edit EndorsmentDetail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string AddEndorsmentDetail(EndorsmentDetailModel model, ref ErrorResponseModel errorResponseModel)
        {
            string message = "";
            if (model.EndorsmentId == 0)
            {
                EndorsmentDetail type = new EndorsmentDetail();
                type.EndorsmentId = model.EndorsmentId;
                type.PlayerId = model.PlayerId;
                type.ListingId= model.ListingId;
                type.EndorsmentType = model.EndorsmentType;
                type.StartDate = model.StartDate;
                type.EndDate = model.EndDate;
                type.FinalPrice = model.FinalPrice;
                type.Notes = model.Notes;
                type.CreatedDate = DateTime.Now;
                type.CreatedBy = model.CreatedBy;
                type.UpdatedDate = DateTime.Now;
                type.IsDeleted = false;
                context.EndorsmentDetails.Add(type);
                context.SaveChanges();
                message = GlobalConstants.EndorsmentDetaisSaveMessage;
                var userAuditLog = new UserAuditLogModel();
                userAuditLog.Action = " Add Endorsment Details";
                userAuditLog.Description = " Endorsment Details Added";
                userAuditLog.UserId = (int)model.CreatedBy;
                userAuditLog.CreatedBy = model.CreatedBy;
                userAuditLog.CreatedDate = DateTime.Now;
                _userAuditLogService.AddUserAuditLog(userAuditLog);
            }
            else
            {
                var typeEntity = context.EndorsmentDetails.FirstOrDefault(x => x.EndorsmentId == model.EndorsmentId);
                if (typeEntity != null)
                {
                    typeEntity.EndorsmentId = model.EndorsmentId;
                    typeEntity.PlayerId = model.PlayerId;
                    typeEntity.ListingId = model.ListingId;
                    typeEntity.EndorsmentType = model.EndorsmentType;
                    typeEntity.StartDate = model.StartDate;
                    typeEntity.EndDate = model.EndDate;
                    typeEntity.FinalPrice = model.FinalPrice;
                    typeEntity.Notes = model.Notes;
                    typeEntity.UpdatedBy = model.UpdatedBy;
                    typeEntity.UpdatedDate = DateTime.Now;
                    typeEntity.IsDeleted = false;
                    context.EndorsmentDetails.Update(typeEntity);
                    context.SaveChanges();
                    message = GlobalConstants.EndorsmentDetailsUpdateMessage;
                    var userAuditLog = new UserAuditLogModel();
                    userAuditLog.Action = " Update Endorsment Details";
                    userAuditLog.Description = "Endorsment Details Update";
                    userAuditLog.UserId = (int)model.CreatedBy;
                    userAuditLog.UpdatedBy = model.UpdatedBy;
                    userAuditLog.UpdatedDate = DateTime.Now;
                    _userAuditLogService.AddUserAuditLog(userAuditLog);
                }
            }
            return message;
        }


        /// <summary>
        /// Method is used to delete EndorsmentDetail
        /// </summary>
        /// <param name="endorsmentId"></param>
        /// <returns></returns>
        public string DeleteEndorsmentDetail(long endorsmentId, ref ErrorResponseModel errorResponseModel)
        {
            string Message = "";
            var endorsmentEntity = context.EndorsmentDetails.FirstOrDefault(x => x.EndorsmentId == endorsmentId);
            if (endorsmentEntity != null)
            {
                endorsmentEntity.IsDeleted = true;
                context.SaveChanges();
                Message = GlobalConstants.EndorsmentDetailsDeleteMessage;

            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Delete Endorsment Details";
            userAuditLog.Description = "Endorsment Details Deleted";
            userAuditLog.UserId = (int)endorsmentEntity.CreatedBy;
            userAuditLog.UpdatedBy = endorsmentEntity.UpdatedBy;
            userAuditLog.UpdatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return "{\"message\": \"" + Message + "\"}";
        }

        /// <summary>
        /// Method is used to get EndorsmentDetail by endorsmentid
        /// </summary>
        /// <param name="endorsmentId"></param>
        /// <returns></returns>
        public EndorsmentDetailModel GetEndorsmentDetailById(long endorsmentId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel= new ErrorResponseModel();
            var endorsmentEntity= (from detail in context.EndorsmentDetails
                                   join
                                   list in context.ListingDetails on detail.ListingId equals list.ListingId
                                   where detail.EndorsmentId==endorsmentId && detail.IsDeleted == false
                                   select new
                                   {
                                       detail.EndorsmentId,
                                       detail.ListingId,
                                       detail.PlayerId,
                                       detail.EndorsmentType,
                                       detail.StartDate,
                                       detail.EndDate,
                                       detail.FinalPrice,
                                       detail.Notes,
                                       detail.CreatedDate,
                                       detail.CreatedBy,
                                       detail.UpdatedDate,
                                       detail.UpdatedBy,
                                       list.CompanyName
                                   }).FirstOrDefault();

            if(endorsmentEntity==null )
            {

            }
            return new EndorsmentDetailModel
            {
                EndorsmentId=endorsmentEntity.EndorsmentId,
                PlayerId=endorsmentEntity.PlayerId,
                ListingId=endorsmentEntity.ListingId,
                EndorsmentType=endorsmentEntity.EndorsmentType,
                StartDate=endorsmentEntity.StartDate,
                EndDate=endorsmentEntity.EndDate,
                FinalPrice=endorsmentEntity.FinalPrice,
                Notes=endorsmentEntity.Notes,
                CreatedBy=endorsmentEntity.CreatedBy,
                CreatedDate=endorsmentEntity.CreatedDate,
                UpdatedBy=endorsmentEntity.UpdatedBy,
                UpdatedDate=endorsmentEntity.UpdatedDate,
                CompanyName=endorsmentEntity.CompanyName,
            };
        }


        /// <summary>
        /// Method is used to get EndorsmentDetail by playerid
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public List<EndorsmentDetailModel> GetEndorsmentDetailPlayerId(long playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var listEntity = new List<EndorsmentDetailModel>();
            var endorsmentEntity = (from detail in context.EndorsmentDetails
                                     join
                                     list in context.ListingDetails on detail.ListingId equals list.ListingId
                                     where detail.PlayerId == playerId && detail.IsDeleted == false
                                    orderby detail.UpdatedDate descending
                                    select new EndorsmentDetailModel
                                     {
                                        EndorsmentId = detail.EndorsmentId,
                                        ListingId = detail.ListingId,
                                        PlayerId = detail.PlayerId, 
                                        EndorsmentType = detail.EndorsmentType,
                                        StartDate = detail.StartDate,
                                        EndDate = detail.EndDate,
                                        FinalPrice = detail.FinalPrice,
                                        Notes = detail.Notes,
                                        CompanyName = list.CompanyName
                                     }).ToList();
            if (endorsmentEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            return endorsmentEntity.ToList();
        }
    }
}
