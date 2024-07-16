using Azure;
using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.SqlServer.Server;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

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
            this._configuration = hostName.Value;
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
            errorResponseModel = new ErrorResponseModel();
            var errorList = new List<ErrorAuditLogModel>();
            var errorEntity = (from user in context12.UserMasters
                               join log in context12.LogInformations
                               on (int)user.UserId equals log.UserId
                               where user.IsDeleted == false
                               orderby log.LogId descending

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
                               }).ToList();
            if (errorEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            errorEntity.ForEach(item =>
            {
                errorList.Add(new ErrorAuditLogModel
                {
                    LogId = item.LogId,
                    LogType = item.LogType,
                    StackTrace = item.StackTrace,
                    AdditionalInformation = item.AdditionalInformation,
                    CreatedDate = DateTime.Now,
                    LogSource = item.LogSource,
                    UserId = item.UserId,
                    LogMessage = item.LogMessage,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    FullName = item.FirstName + " " + item.LastName,
                    EmailId = item.EmailId,
                });
            });
            return errorList;
        }


        /// <summary>
        /// Method is used to restore all ErrorAuditLog
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public string ErrorAuditLogRestore(long logId, ref ErrorResponseModel errorResponseModel)
        {
            string message = "";
            var dataEntity = context12.LogInformations.FirstOrDefault(x => x.LogId == logId);

            if (dataEntity != null)
            {
                message = GlobalConstants.ErrorAuditLogDownloadSuccessfully;
            }
            return message;
        }


        /// <summary>
        /// Method is used to generate text file for ErrorAuditLog
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private string WriteErrorListToFile(ErrorAuditLogModel error)
        {
            var folderName = Path.Combine("Resources", "ErrorAuditLogFile");
            string fileName = $"error_audit_log_{error.LogId}.txt";
            string pathtosave = "/ErrorAuditLogFile/" + fileName;
            var hostName = _configuration.HostName;
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), folderName + "/" + fileName);
            string fileUrl = $"{hostName}{pathtosave}";

            try
            {
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
                Console.WriteLine("An error occurred while writing the error audit log file: " + ex.Message);
                return ex.Message;
            }
        }


        /// <summary>
        /// Method is used to get all ErrorAuditLog by logId
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public string GetErrorAuditLogById(long logId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var errorEntity = (from user in context12.UserMasters
                               join log in context12.LogInformations
                               on (int)user.UserId equals log.UserId
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
                CreatedDate = DateTime.Now,
                LogSource = errorEntity.LogSource,
                UserId = errorEntity.UserId,
                LogMessage = errorEntity.LogMessage,
                FirstName = errorEntity.FirstName,
                LastName = errorEntity.LastName,
                FullName = errorEntity.FirstName + " " + errorEntity.LastName,
                EmailId = errorEntity.EmailId,
            };
            string downloadFile = WriteErrorListToFile(error);
            return downloadFile;
        }




        /// <summary>
        /// Method is used to save all ErrorAuditLog
        /// </summary>
        /// <param name="">logEntry</param>
        /// <returns></returns>
        public string SaveErrorLogs(LogEntry logEntry)
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

        /// <summary>
        /// Method is used to delete ErrorAuditLog
        /// </summary>
        /// <param name="">logEntry</param>
        /// <returns></returns>
        public string DeleteErrorLogs(long logId)
        {
           ErrorResponseModel errorResponseModel = new ErrorResponseModel();
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
        
    }
}
