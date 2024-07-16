using IM10.BAL.Implementaion;
using IM10.BAL.Interface;
using IM10.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using System.Collections.Generic;

namespace IM10.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : BaseAPIController
    {
        INotificationService service;
        private readonly IErrorAuditLogService _logService;

        public NotificationController(IErrorAuditLogService logService,INotificationService _service)
        {
            service = _service;
            _logService = logService;
        }


        [HttpPost("SendNotification")]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> SendNotification(string DeviceId, long playerId, long contentId, string title, string description, int contentTypeId, string thumbnail, int categoryId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var detailModel =await service.SendNotification(DeviceId,playerId,contentId,title,description,contentTypeId, thumbnail,categoryId);

                if (detailModel != null)
                {
                    return Ok(detailModel);
                }              
                else
                {
                    return ReturnErrorResponse(errorResponseModel);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message + ex.InnerException + ex.StackTrace + ex.Source);
            }
        }



        [HttpPost("SendCommentNotification")]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> SendCommentNotification(string DeviceId, long contentId, long commentId, string message, int contentTypeId, int categoryId, bool IsPublic)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var detailModel =await service.SendCommentNotification(DeviceId, contentId, commentId, message, contentTypeId, categoryId,true);

                if (detailModel != null)
                {
                    return Ok(detailModel);
                }
                else
                {
                    return ReturnErrorResponse(errorResponseModel);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message + ex.InnerException + ex.StackTrace + ex.Source);
            }
        }


        [HttpGet("GetAccessToken11")]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetAccessToken11()
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var detailModel = await service.GetAccessToken11();

                if (detailModel != null)
                {
                    return Ok(detailModel);
                }
                else
                {
                    return ReturnErrorResponse(errorResponseModel);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message + ex.InnerException + ex.StackTrace + ex.Source);
            }
        }


    }
}
