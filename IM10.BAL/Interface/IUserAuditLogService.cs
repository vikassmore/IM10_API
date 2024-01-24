using IM10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Interface
{
    public interface IUserAuditLogService
    {
        /// <summary>
        /// User Audit log add
        /// </summary>
        /// <param name="auditLog"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        void AddUserAuditLog(UserAuditLogModel auditLog);
    }
}
