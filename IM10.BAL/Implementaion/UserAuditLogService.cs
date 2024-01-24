using IM10.BAL.Interface;
using IM10.Entity.DataModels;
using IM10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Implementaion
{
    public class UserAuditLogService : IUserAuditLogService
    {
        IM10DbContext context;

        public UserAuditLogService(IM10DbContext _context)
        {
            context = _context;
        }



        /// <summary>
        /// User Audit log add
        /// </summary>
        /// <param name="auditLog"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public void AddUserAuditLog(UserAuditLogModel auditLog)
        {
            try
            {
                var userAuditLogEntity = new UserAuditLog();
                userAuditLogEntity.Action = auditLog.Action;
                userAuditLogEntity.Description = auditLog.Description;
                userAuditLogEntity.UserId = auditLog.UserId;
                userAuditLogEntity.CreatedBy = (int?)auditLog.UserId;
                userAuditLogEntity.CreatedDate = DateTime.Now;
                context.UserAuditLogs.Add(userAuditLogEntity);
                context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
