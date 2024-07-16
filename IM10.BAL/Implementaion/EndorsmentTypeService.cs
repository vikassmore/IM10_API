using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
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
    /// This is implementation for the EndorsmentTypeService operations 
    /// </summary>
    /// 
    public class EndorsmentTypeService : IEndorsmentTypeService
    {

        IM10DbContext context;
        private readonly IUserAuditLogService _userAuditLogService;
        /// <summary>
        /// Creating constructor and injection dbContext
        /// </summary>
        /// <param name="_context"></param>
        public EndorsmentTypeService(IM10DbContext _context,IUserAuditLogService userAuditLogService)
        {
             context = _context;
            _userAuditLogService = userAuditLogService;
        }


        /// <summary>
        /// Method is used to add/edit EndorsmentType
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string AddEndorsmentType(EndorsmentTypeModel model, ref ErrorResponseModel errorResponseModel)
        {
            string message = "";
            if (model.EndorsmentTypeId == 0)
            {
                EndorsmentType type = new EndorsmentType();
                type.EndorsmentTypeId = model.EndorsmentTypeId;
                type.EndorsmentName = model.EndorsmentName;
                type.EndorsmentDescription = model.EndorsmentDescription;
                type.CreatedDate =DateTime.Now;
                type.CreatedBy = model.CreatedBy;
                type.UpdatedDate= DateTime.Now;
                type.IsDeleted = false;
                context.EndorsmentTypes.Add(type);
                context.SaveChanges();
                message = GlobalConstants.EndorsmentTypeSaveMessage;
                var userAuditLog = new UserAuditLogModel();
                userAuditLog.Action = " Add Endorsment Type";
                userAuditLog.Description = "Endorsment Type Added Successfully";
                userAuditLog.UserId = (int)model.CreatedBy;
                userAuditLog.CreatedBy = model.CreatedBy;
                userAuditLog.CreatedDate = DateTime.Now;
                _userAuditLogService.AddUserAuditLog(userAuditLog);
            }
            else
            {
                var typeEntity=context.EndorsmentTypes.FirstOrDefault(x=>x.EndorsmentTypeId==model.EndorsmentTypeId);
                if (typeEntity!=null)
                {
                    typeEntity.EndorsmentTypeId = model.EndorsmentTypeId;
                    typeEntity.EndorsmentName = model.EndorsmentName;
                    typeEntity.EndorsmentDescription = model.EndorsmentDescription;
                    typeEntity.UpdatedDate = DateTime.Now;
                    typeEntity.UpdatedBy = model.UpdatedBy;
                    typeEntity.IsDeleted = false;
                    context.EndorsmentTypes.Update(typeEntity);
                    context.SaveChanges();
                    message = GlobalConstants.EndorsmentTypeUpdateMessage;
                    var userAuditLog = new UserAuditLogModel();
                    userAuditLog.Action = " Update Endorsment Type ";
                    userAuditLog.Description = "Endorsment Type Updated Successfully";
                    userAuditLog.UserId = (int)model.UpdatedBy;
                    userAuditLog.UpdatedBy = model.UpdatedBy;
                    userAuditLog.UpdatedDate = DateTime.Now;
                    _userAuditLogService.AddUserAuditLog(userAuditLog);
                }
            }
            return message;
        }


        /// <summary>
        /// Method is used to delete EndorsmentType
        /// </summary>
        /// <param name="endorsmenttypeId"></param>
        /// <returns></returns>
        public string DeleteEndorsmentType(long endorsmenttypeId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            string Message = "";
            var endorsmentEntity = context.EndorsmentTypes.FirstOrDefault(x => x.EndorsmentTypeId == endorsmenttypeId);
            if (endorsmentEntity != null)
            {
                if (endorsmentEntity.IsDeleted == true)
                {
                    errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                    errorResponseModel.Message = "Endorsment type already deleted.";
                    return null;
                }
                endorsmentEntity.IsDeleted = true;
                context.SaveChanges();
                Message = GlobalConstants.EndorsmentTypeDeleteMessage;

            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Delete Endorsment Type";
            userAuditLog.Description = " Endorsment Type Deleted Successfully";
            userAuditLog.UserId = (int)endorsmentEntity.CreatedBy;
            userAuditLog.CreatedDate = DateTime.Now;
            userAuditLog.CreatedBy = endorsmentEntity.CreatedBy;
            userAuditLog.UpdatedBy = endorsmentEntity.CreatedBy;
            userAuditLog.UpdatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return "{\"message\": \"" + Message + "\"}";
        }


        /// <summary>
        /// Method is used to get all EndorsmentType
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public List<EndorsmentTypeModel> GetAllEndorsmentType(ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel= new ErrorResponseModel();
            var ListEntity=new List<EndorsmentTypeModel>();
            var endorsmentEntity=context.EndorsmentTypes.Where(x=>x.IsDeleted==false).ToList();
            if (endorsmentEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            endorsmentEntity.ForEach(item => 
            {
                ListEntity.Add(new EndorsmentTypeModel
                {
                    EndorsmentTypeId= item.EndorsmentTypeId,
                    EndorsmentName= item.EndorsmentName,
                    EndorsmentDescription= item.EndorsmentDescription,
                });
            });
            return ListEntity;
        }


        /// <summary>
        /// Method is used to get EndorsmentType by endorsmenttypeId
        /// </summary>
        /// <param name="endorsmenttypeId"></param>
        /// <returns></returns>
        public EndorsmentTypeModel GetEndorsmentTypeById(long endorsmenttypeId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var endorsmentEntity = context.EndorsmentTypes.Where(x => x.EndorsmentTypeId == endorsmenttypeId && x.IsDeleted==false).FirstOrDefault();
            if (endorsmentEntity == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            return new EndorsmentTypeModel
            {
                EndorsmentTypeId=endorsmentEntity.EndorsmentTypeId,
                EndorsmentName=endorsmentEntity.EndorsmentName,
                EndorsmentDescription=endorsmentEntity.EndorsmentDescription,
                CreatedBy=endorsmentEntity.CreatedBy,
                CreatedDate=endorsmentEntity.CreatedDate,
                UpdatedBy=endorsmentEntity.UpdatedBy,
                UpdatedDate = endorsmentEntity.UpdatedDate,
                IsDeleted=endorsmentEntity.IsDeleted
            };
        }
    }
}
