using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
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
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Creating constructor and injection dbContext
        /// </summary>
        /// <param name="_context"></param>
        public AdvContentMappingService(IM10DbContext _context, INotificationService notificationService,IOptions<ConfigurationModel> hostName, IUserAuditLogService userAuditLogService)
        {
            context = _context;
            _configuration = hostName.Value;
            _userAuditLogService = userAuditLogService;
            _notificationService = notificationService;
        }  

        /// <summary>
        /// Method to add advcontentmapping
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public NotificationModel AddAdvContentMapping(AdvContentMappingModel model, ref ErrorResponseModel errorResponseModel)
        {
            NotificationModel message = new NotificationModel();

            if (model.AdvContentMapId == 0)
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

                var existingcontentEntity = context.ContentDetails.FirstOrDefault(x => x.ContentId == contentEntity.ContentId);
                var thumbnailUrl = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(existingcontentEntity.ContentFilePath) ? existingcontentEntity.ContentFilePath : existingcontentEntity.ContentFilePath);

                if (existingcontentEntity != null)
                {
                    message.PlayerId = existingcontentEntity.PlayerId;
                    message.ContentId = contentEntity.ContentId;
                    message.Title = existingcontentEntity.Title;
                    message.Description = existingcontentEntity.Description;
                    message.ContentTypeId = existingcontentEntity.ContentTypeId;
                    message.Thumbnail = ThumbnailPath(thumbnailUrl);
                    message.Message = GlobalConstants.AdvContentMappingAddedSuccessfully;
                    var existing = context.Fcmnotifications.Where(x => x.PlayerId == existingcontentEntity.PlayerId).ToList();
                    foreach (var item in existing)
                    {
                        _notificationService.SendNotification(item.DeviceToken, message.PlayerId, message.ContentId, message.Title, message.Description, true,message.ContentTypeId,message.Thumbnail);
                    }
                }              
            }
            else
            {
                var contentlist = context.AdvContentMappings.FirstOrDefault(x => x.AdvContentMapId == model.AdvContentMapId);
                if (contentlist != null)
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
                    message.Message = GlobalConstants.AdvContentMappingUpdateSuccessfully;
                }
            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Add Advertise Content mapping Details";
            userAuditLog.Description = "Advertise Content mapping Details Added";
            userAuditLog.UserId = (int)model.CreatedBy;
            userAuditLog.CreatedBy = model.CreatedBy;
            userAuditLog.CreatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
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
            string Message = "";
            var contentEntity = context.AdvContentMappings.FirstOrDefault(x => x.AdvContentMapId == advcontentmapId);
            if (contentEntity != null)
            {
                contentEntity.IsDeleted = true;
                context.SaveChanges();
                Message = GlobalConstants.AdvContentMappingDeleteSuccessfully;
            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Delete Advertise Content Details";
            userAuditLog.Description = "Advertise Content Details Deleted";
            userAuditLog.UserId = (int)contentEntity.CreatedBy;
            userAuditLog.UpdatedBy = contentEntity.UpdatedBy;
            userAuditLog.UpdatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return "{\"message\": \"" + Message + "\"}";
        }

        /// <summary>
        /// Method is used to get AdvContentMapping by playerId
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public List <AdvContentMappingModel1> GetAdvContentMappingByPlayerId(long playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var advcontentmappinglist=new List<AdvContentMappingModel1>();
            var contentEntity = (from update in context.AdvContentMappings
                                 join
                                 content in context.ContentDetails
                                 on update.ContentId equals content.ContentId
                                 join advertise in context.AdvContentDetails
                                 on update.AdvertiseContentId equals advertise.AdvertiseContentId
                                 join category in context.Categories
                                 on update.CategoryId equals category.CategoryId
                                 join subcategory in context.SubCategories
                                 on update.SubCategoryId equals subcategory.SubCategoryId
                                 where content.PlayerId == playerId && advertise.IsDeleted==false &&  update.IsDeleted == false
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
                                     CreatedBy = update.CreatedBy,
                                     CreatedDate = update.CreatedDate,
                                     Position = update.Position,
                                     UpdatedDate = DateTime.Now,
                                     UpdatedBy = update.UpdatedBy,
                                 }).ToList();
            if (contentEntity == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;

            }
            contentEntity.ForEach(item =>
            {
                advcontentmappinglist.Add(new AdvContentMappingModel1
                {
                    AdvContentMapId = item.AdvContentMapId,
                    ContentId = item.ContentId,
                    ContentName = item.ContentName,
                    AdvertiseContentId = item.AdvertiseContentId,
                    AdvertiseContentName = item.AdvertiseContentName,
                    CategoryId = item.CategoryId,
                    CategoryName = item.CategoryName,
                    SubCategoryId = item.SubCategoryId,
                    SubcategoryName = item.SubcategoryName,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    Position = item.Position,
                    UpdatedDate = DateTime.Now,
                    UpdatedBy = item.UpdatedBy,
                });
            });
            return advcontentmappinglist;

        }

        public AdvContentMappingModel1 GetAdvContentMappingById(long AdvcontentmapId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var contentEntity = (from update in context.AdvContentMappings
                                 join
                                 content in context.ContentDetails
                                 on update.ContentId equals content.ContentId
                                 join advertise in context.AdvContentDetails
                                 on update.AdvertiseContentId equals advertise.AdvertiseContentId
                                 join category in context.Categories
                                 on update.CategoryId equals category.CategoryId
                                 join subcategory in context.SubCategories
                                 on update.SubCategoryId equals subcategory.SubCategoryId
                                 where update.AdvContentMapId==AdvcontentmapId && update.IsDeleted == false

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
            return new AdvContentMappingModel1
            {
                AdvContentMapId = contentEntity.AdvContentMapId,
                ContentId = contentEntity.ContentId,
                ContentName = contentEntity.ContentName,
                AdvertiseContentId = contentEntity.AdvertiseContentId,
                AdvertiseContentName = contentEntity.AdvertiseContentName,
                CategoryId = contentEntity.CategoryId,
                CategoryName = contentEntity.CategoryName,
                SubCategoryId = contentEntity.SubCategoryId,
                SubcategoryName = contentEntity.SubcategoryName,
                CreatedBy = contentEntity.CreatedBy,
                CreatedDate = contentEntity.CreatedDate,
                Position = contentEntity.Position,
                UpdatedDate = DateTime.Now,
                UpdatedBy = contentEntity.UpdatedBy,
            };

        }


        public List<AdvContentMappingModel1> GetAllAdvContentMapping(ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var contentlist = new List<AdvContentMappingModel1>();
            var contentEntity = (from update in context.AdvContentMappings
                                 join
                                 content in context.ContentDetails
                                 on update.ContentId equals content.ContentId
                                 join advertise in context.AdvContentDetails 
                                 on update.AdvertiseContentId equals advertise.AdvertiseContentId
                                 join category in context.Categories 
                                 on update.CategoryId equals category.CategoryId
                                 join subcategory in context.SubCategories
                                 on update.SubCategoryId equals subcategory.SubCategoryId
                                 where update.IsDeleted == false

                                 select new AdvContentMappingModel1
                                 {
                                     AdvContentMapId = update.AdvContentMapId,
                                     ContentId = update.ContentId,
                                     ContentName=content.Title,
                                     AdvertiseContentId = update.AdvertiseContentId,
                                     AdvertiseContentName=advertise.Title,
                                     CategoryId = update.CategoryId,
                                     CategoryName=category.Name,
                                     SubCategoryId=update.SubCategoryId,
                                     SubcategoryName=subcategory.Name,
                                     CreatedBy = update.CreatedBy,
                                     CreatedDate = update.CreatedDate,
                                     Position = update.Position,
                                     UpdatedDate = DateTime.Now,
                                     UpdatedBy = update.UpdatedBy,
                                 }).ToList();
            if (contentEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;

            }
            contentEntity.ForEach(item =>
            {
                contentlist.Add(new AdvContentMappingModel1
                {
                    AdvContentMapId = item.AdvContentMapId,
                    ContentId = item.ContentId,
                    ContentName = item.ContentName,
                    AdvertiseContentId = item.AdvertiseContentId,
                    AdvertiseContentName = item.AdvertiseContentName,
                    CategoryId = item.CategoryId,
                    CategoryName = item.CategoryName,
                    SubCategoryId = item.SubCategoryId,
                    SubcategoryName = item.SubcategoryName,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    Position = item.Position,
                    UpdatedDate = DateTime.Now,
                    UpdatedBy = item.UpdatedBy,
                });

            });
            return contentlist;
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
                    filePath = filePath.Replace(extension, ".jpeg");
                   // filePath = filePath.Replace("ContentFile/", "ContentFile/thumbnail/");
                }
            }

            return filePath;
        }

    }
}
