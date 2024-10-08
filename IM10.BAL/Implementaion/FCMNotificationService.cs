﻿using IM10.BAL.Interface;
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
    public class FCMNotificationService:IFCMNotificationService
    {
        IM10DbContext context;
        private readonly INotificationService _notificationService;
        private readonly IUserAuditLogService _userAuditLogService;
        private readonly IEncryptionService _encryptionService;

        /// <summary>
        /// Creating constructor and injection dbContext
        /// </summary>
        /// <param name="_context"></param>
        public FCMNotificationService(IM10DbContext _context, IEncryptionService encryptionService, INotificationService notificationService, IUserAuditLogService userAuditLogService)
        {
            context = _context;
            _notificationService = notificationService;
            _userAuditLogService = userAuditLogService;
            _encryptionService= encryptionService;
        }


        /// <summary>
        /// Method to save FCM notifiction token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string AddFCMNotificaion(FCMNotificationModel model)
        {
            string message = "";
            var decryptResult = _encryptionService.DecryptPlayerId(model.PlayerId);
            if (decryptResult.DecryptedPlayerId == null)
            {
                return message;
            }
            long decryptplayerId = decryptResult.DecryptedPlayerId.Value;

            if (model.FcmnotificationId == 0)
            {
                Fcmnotification fcmnotification = new Fcmnotification();
                fcmnotification.FcmnotificationId = model.FcmnotificationId;
                fcmnotification.DeviceToken= model.DeviceToken;
                fcmnotification.PlayerId =decryptplayerId;
                fcmnotification.UserId= model.UserId;
                fcmnotification.CreatedDate=DateTime.Now;
                fcmnotification.CreatedBy= (int?)model.UserId;
                fcmnotification.UpdatedDate=DateTime.Now;
                fcmnotification.IsDeleted = false;
                context.Fcmnotifications.Add(fcmnotification);
                context.SaveChanges();
                message = GlobalConstants.SaveToken;
            }
            return message;
        }
    }
}
