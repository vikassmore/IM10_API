﻿using IM10.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace IM10.API.Controllers
{
    /// <summary>
    /// Perform call action required for all controllers 
    /// </summary>
    public class BaseAPIController : ControllerBase
    {
        /// <summary>
        /// This will create an error response as per status
        /// </summary>
        /// <param name="model"></param>
        /// <param name="customErrorMessage"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]

        public IActionResult ReturnErrorResponse(ErrorResponseModel model, string customErrorMessage = null)
        {
            if (model != null)
            {
                var errorMessage = string.IsNullOrEmpty(customErrorMessage) ? model.Message : customErrorMessage;
                switch (model.StatusCode)
                {
                    case HttpStatusCode.BadGateway:
                        return BadRequest(errorMessage);
                    case HttpStatusCode.ServiceUnavailable:
                        return StatusCode(StatusCodes.Status503ServiceUnavailable, "Service  not available for this user");
                    case HttpStatusCode.NotFound:
                        return StatusCode(StatusCodes.Status404NotFound, "Record Not found");
                    default:
                        return BadRequest(errorMessage);
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }

        }
    }
}
