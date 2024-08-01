using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace IM10.BAL.Implementaion
{
    /// <summary>
    /// This is implementation for the ErrorAuditLog operations 
    /// </summary>
    public class ErrorAuditLogService : IErrorAuditLogService
    {
        IM10DbContext context12;
        private ConfigurationModel _configuration;
        private readonly ILogger<ErrorAuditLogService> _logger;

        /// <summary>
        /// Creating constructor and injection dbContext
        /// </summary>
        /// <param name="_context"></param>
        public ErrorAuditLogService(ILogger<ErrorAuditLogService> logger, IM10DbContext _context1, IOptions<ConfigurationModel> hostName)
        {
            context12 = _context1;
            _configuration = hostName.Value;
            _logger = logger;
        }


        /// <summary>
        /// Method to get all ErrorAuditLog
        /// </summary>
        /// <param name=""></param>
        /// <param name="errorResponseModel"></param>  
        /// <returns></returns>
        public List<ErrorAuditLogModel> GetAllErrorAuditLog(ref ErrorResponseModel errorResponseModel)
        {
            try
            {
                errorResponseModel = new ErrorResponseModel();
                var errorList = new List<ErrorAuditLogModel>();
                var errorEntity = (from user in context12.UserMasters
                                   join log in context12.LogInformations on (int)user.UserId equals log.UserId
                                   where user.IsDeleted == false
                                   orderby log.LogId descending
                                   select new ErrorAuditLogModel
                                   {
                                       LogId = log.LogId,
                                       UserId = log.UserId,
                                       LogType = log.LogType,
                                       StackTrace = log.StackTrace,
                                       AdditionalInformation = log.AdditionalInformation,
                                       LogSource = log.LogSource,
                                       CreatedDate = log.CreatedDate,
                                       LogMessage = log.LogMessage,
                                       FirstName = user.FirstName,
                                       LastName = user.LastName,
                                       EmailId = user.EmailId,
                                       FullName=user.FirstName + " " + user.LastName
                                   }).ToList();
                if (errorEntity.Count == 0)
                {
                    errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                    errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                }
                return errorEntity.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all error audit logs.");
                errorResponseModel.StatusCode = HttpStatusCode.InternalServerError;
                errorResponseModel.Message = ex.InnerException + ex.StackTrace + ex.Message;
                return new List<ErrorAuditLogModel>();
            }
        }


        /// <summary>
        /// Method is used to restore all ErrorAuditLog
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public string ErrorAuditLogRestore(long logId, ref ErrorResponseModel errorResponseModel)
        {
            try
            {
                string message = "";
                var dataEntity = context12.LogInformations.FirstOrDefault(x => x.LogId == logId);

                if (dataEntity != null)
                {
                    message = GlobalConstants.ErrorAuditLogDownloadSuccessfully;
                }
                return message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all error audit logs.");
                errorResponseModel.StatusCode = HttpStatusCode.InternalServerError;
                errorResponseModel.Message = ex.InnerException + ex.StackTrace + ex.Message;
                return null;
            }
        }


        /// <summary>
        /// Method is used to generate text file for ErrorAuditLog
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private string WriteErrorListToFile(ErrorAuditLogModel error)
        {
            try
            {
                var folderName = Path.Combine("Resources", "ErrorAuditLogFile");
                string fileName = $"error_audit_log_{error.LogId}.txt";
                string pathtosave = "/ErrorAuditLogFile/" + fileName;
                var hostName = _configuration.HostName;
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), folderName + "/" + fileName);
                string fileUrl = $"{hostName}{pathtosave}";

                using (StreamWriter writer = File.CreateText(filePath))
                {
                    writer.WriteLine($"Log ID: {error.LogId}");
                    writer.WriteLine($"Log Type: {error.LogType}");
                    writer.WriteLine($"Created Date: {error.CreatedDate}\n");
                    writer.WriteLine($"User ID: {error.UserId}");
                    writer.WriteLine($"Email ID: {error.EmailId}");
                    writer.WriteLine($"Full Name: {error.FullName}\n");
                    writer.WriteLine($"Log Source: {error.LogSource}");
                    writer.WriteLine($"Error: {error.LogMessage}");
                    writer.WriteLine($"Description: {error.AdditionalInformation}");
                    writer.WriteLine($"Stack Trace: {error.StackTrace}");
                    writer.Close();
                    byte[] fileBytes = File.ReadAllBytes(filePath);

                    // Create the response message with file contents
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new ByteArrayContent(fileBytes);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };
                }
                return fileUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException + ex.StackTrace + ex.Message, $"Error writing audit log to file for Log ID {error.LogId}.");
                return null;
            }
        }


        /// <summary>
        /// Method is used to get all ErrorAuditLog by logId
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public string GetErrorAuditLogById(long logId, ref ErrorResponseModel errorResponseModel)
        {
            try
            {
                errorResponseModel = new ErrorResponseModel();
                var errorEntity = (from user in context12.UserMasters
                                   join log in context12.LogInformations on (int)user.UserId equals log.UserId
                                   where log.LogId == logId
                                   select new
                                   {
                                       log.LogId,
                                       log.UserId,
                                       log.LogType,
                                       log.StackTrace,
                                       log.AdditionalInformation,
                                       log.LogSource,
                                       log.CreatedDate,
                                       log.LogMessage,
                                       user.FirstName,
                                       user.LastName,
                                       user.EmailId
                                   }).FirstOrDefault();

                if (errorEntity == null)
                {
                    errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                    errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                    return null;
                }

                var error = new ErrorAuditLogModel
                {
                    LogId = errorEntity.LogId,
                    LogType = errorEntity.LogType,
                    StackTrace = errorEntity.StackTrace,
                    AdditionalInformation = errorEntity.AdditionalInformation,
                    CreatedDate = errorEntity.CreatedDate,
                    LogSource = errorEntity.LogSource,
                    UserId = errorEntity.UserId,
                    LogMessage = errorEntity.LogMessage,
                    FirstName = errorEntity.FirstName,
                    LastName = errorEntity.LastName,
                    FullName = errorEntity.FirstName + " " + errorEntity.LastName,
                    EmailId = errorEntity.EmailId,
                };
                Console.WriteLine(error);
                string downloadFile = WriteErrorListToFile(error);
                return downloadFile;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"Error fetching audit log by ID {logId}.");
                errorResponseModel.StatusCode = HttpStatusCode.InternalServerError;
                errorResponseModel.Message =ex.InnerException+ ex.StackTrace + ex.Message;
                return null;
            }
        }


        /// <summary>
        /// Method is used to save all ErrorAuditLog
        /// </summary>
        /// <param name="">logEntry</param>
        /// <returns></returns>
        public string SaveErrorLogs(LogEntry logEntry)
        {
            try
            {
                using (var newcontext = new IM10DbContext())
                {
                    var log = new LogInformation
                    {
                        LogType = logEntry.LogType,
                        StackTrace = logEntry.StackTrace,
                        AdditionalInformation = logEntry.AdditionalInformation,
                        CreatedDate = logEntry.CreatedDate,
                        LogSource = logEntry.LogSource,
                        UserId = logEntry.UserId,
                        LogMessage = logEntry.LogMessage,
                    };
                    newcontext.LogInformations.Add(log);
                    newcontext.SaveChanges();
                }
                return "Error log saved successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving audit log to the database.");
                return null;
            }
        }

        /// <summary>
        /// Method is used to delete ErrorAuditLog
        /// </summary>
        /// <param name="">logEntry</param>
        /// <returns></returns>
        public string DeleteErrorLogs(long logId)
        {
            try
            {
                string Message = "";
                var logEntity = context12.LogInformations.FirstOrDefault(x => x.LogId == logId);
                if (logEntity != null)
                {
                    context12.LogInformations.Remove(logEntity);
                    context12.SaveChanges();
                    Message = GlobalConstants.ErrorLogDeleteSuccessfully;
                }
                return "{\"message\": \"" + Message + "\"}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException + ex.StackTrace + ex.Message, $"Error deleting audit log with ID {logId}.");
                return "{\"message\": \"An error occurred while deleting the log.\"}";
            }
        }        
    }
}
