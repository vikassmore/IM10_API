using IM10.BAL.Interface;
using IM10.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace IM10.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class FCMNotificationController : BaseAPIController
    {
        IFCMNotificationService service;
        public FCMNotificationController(IFCMNotificationService _service)
        {
            service = _service;
        }


        /// <summary>
        /// Create an AddFCMNotificaion
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("AddFCMNotificaion")]
        [ProducesResponseType(typeof(FCMNotificationModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult AddFCMNotificaion(FCMNotificationModel model)
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = ((System.Security.Claims.ClaimsIdentity)User.Identity).FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
                model.CreatedBy = Convert.ToInt32(userId);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request, please verify details");
            }

            try
            {
                var errorMessage = new ErrorResponseModel();
                var endorsmentModel = service.AddFCMNotificaion(model);
                if (endorsmentModel != "")
                {
                    return Ok(endorsmentModel);
                }
                return ReturnErrorResponse(errorMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }
    }
}
