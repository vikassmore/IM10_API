using IM10.Entity.DataModels;
using IM10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Interface
{
    /// <summary>
    /// Interface used for ErrorAuditLog related operations
    /// </summary>

    public interface IErrorAuditLogService
    {
        /// <summary>
        /// Method is used to get all ErrorAuditLog
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<ErrorAuditLogModel> GetAllErrorAuditLog(ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get all ErrorAuditLog by id
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        string GetErrorAuditLogById(long logId,  ref ErrorResponseModel errorResponseModel);



        /// <summary>
        /// Method is used to restore all ErrorAuditLog
        /// </summary>
        /// <param name="">logId</param>
        /// <returns></returns>
        string ErrorAuditLogRestore(long logId,ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to save all ErrorAuditLog
        /// </summary>
        /// <param name="">logEntry</param>
        /// <returns></returns>
        string SaveErrorLogs(LogEntry logEntry);


        /// <summary>
        /// Method is used to delete  ErrorAuditLog
        /// </summary>
        /// <param name="">logEntry</param>
        /// <returns></returns>
        string DeleteErrorLogs(long logId);
    }
}
