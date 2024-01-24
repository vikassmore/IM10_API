using IM10.BAL.Implementaion;
using IM10.BAL.Interface;
using IM10.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


namespace IM10.API.Controllers
{

    /// <summary>
    /// APIs for ErrorAuditLog entity 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorAuditLogController : BaseAPIController
    {
         IErrorAuditLogService auditLogService;

        /// <summary>
        /// Used to initialize controller and inject ErrorAuditLog service
        /// </summary>
        /// <param name="_AuditLogService"></param>
        public ErrorAuditLogController(IErrorAuditLogService _AuditLogService)
        {
            auditLogService = _AuditLogService;
        }


        /// <summary>
        /// Get all ErrorAuditLog
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetErrorAuditLogById")]
        // [Authorize]
        [ProducesResponseType(typeof(ErrorAuditLogModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetErrorAuditLogById(long logId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {

                var filepath = auditLogService.GetErrorAuditLogById(logId,ref errorResponseModel);

                if (filepath != null)
                {
                    var responseObj1 = new { filepath }; // Create anonymous object
                    string jsonResponse1 = System.Text.Json.JsonSerializer.Serialize(responseObj1); // Convert to JSON
                    return Ok(jsonResponse1);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }




        /// <summary>
        /// Get all ErrorAuditLog
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetAllErrorAuditLog")]
       // [Authorize]
        [ProducesResponseType(typeof(ErrorAuditLogModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllErrorAuditLog()
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {

                var errorModels = auditLogService.GetAllErrorAuditLog(ref errorResponseModel);

                if (errorModels != null)
                {
                    return Ok(errorModels);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }


        /// <summary>
        /// Method is used to restore all ErrorAuditLog
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("RestoreBackup/{logId:int}")]
        public async Task<ActionResult> RestoreBackup(long logId)
        {
            ErrorResponseModel errorResponseModel = null;
            string message;
            var provider = new FileExtensionContentTypeProvider();
            string file = auditLogService.GetErrorAuditLogById(logId,ref errorResponseModel);

            if (string.IsNullOrEmpty(file))
            {
                message = "file not found.";
                var responseObj = new { message }; // Create anonymous object
                string jsonResponse = System.Text.Json.JsonSerializer.Serialize(responseObj); // Convert to JSON
                return Content(jsonResponse, "application/json"); // 
            }
            var filePath = file; // Here, you should validate the request and the existance of the file. var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            auditLogService.ErrorAuditLogRestore(logId, ref errorResponseModel);
            return File(bytes, contentType, Path.GetFileName(filePath));
        }
    }
}
