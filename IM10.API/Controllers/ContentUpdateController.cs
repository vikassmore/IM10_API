using IM10.BAL.Implementaion;
using IM10.BAL.Interface;
using IM10.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace IM10.API.Controllers
{

    /// <summary>
    /// APIs for Contentupdate entity 
    /// </summary>
    
    [Route("api/[controller]")]
    [ApiController]
    public class ContentUpdateController : BaseAPIController
    {
        IContentUpdateService contentUpdateService;

        /// <summary>
        /// Used to initialize controller and inject Contentupdate service
        /// </summary>
        /// <param name="_contentUpdateService"></param>
        public ContentUpdateController(IContentUpdateService _contentUpdateService)
        {
            contentUpdateService = _contentUpdateService;
        }

        /// <summary>
        /// To get Contentupdate by contentId 
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        [HttpGet("GetContentUpdate/{contentId}")]
        [ProducesResponseType(typeof(ContentUpdateModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetContentUpdate(long contentId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (contentId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var contentdetailModel = contentUpdateService.GetContentUpdate(contentId, ref errorResponseModel);

                if (contentdetailModel != null)
                {
                    return Ok(contentdetailModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }

        /// <summary>
        /// Get all Contentupdate
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetAllContentUpdate")]
        [ProducesResponseType(typeof(ContentUpdateModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllContentUpdate()
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {

                var detailModel = contentUpdateService.GetAllContentUpdate(ref errorResponseModel);

                if (detailModel != null)
                {
                    return Ok(detailModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }



        /// <summary>
        /// Create an Contentupdate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("AddContentUpdate")]
        [Authorize]
        [ProducesResponseType(typeof(ContentUpdateModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult AddContentUpdate(ContentUpdateModel model)
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
                var contentModel = contentUpdateService.AddContentUpdate(model, ref errorMessage);

                if (contentModel != "")
                {
                    return Ok(contentModel);
                }
                return ReturnErrorResponse(errorMessage);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }

        }   


        /// <summary>
        /// Delete an Contentupdate
        /// </summary>
        /// <param name="contentLogId"></param>
        /// <returns></returns>
        [HttpDelete("DeleteContentUpdate")]
        [ProducesResponseType(typeof(ContentUpdateModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult DeleteContentUpdate(long contentLogId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var contentModel = contentUpdateService.DeleteContentUpdate(contentLogId, ref errorResponseModel);

                if (contentModel != null)
                {
                    return Ok(contentModel);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }



        /// <summary>
        /// Get all ContentTitles
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetAllContentTitles")]
        [ProducesResponseType(typeof(ContentTitleModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllContentTitles()
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {

                var detailModel = contentUpdateService.GetAllContentTitles(ref errorResponseModel);

                if (detailModel != null)
                {
                    return Ok(detailModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }


        /// <summary>
        /// Get all Content Update for QA
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetContentUpdateforQA/{playerId}")]
        [ProducesResponseType(typeof(ContentUpdateModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetContentUpdateforQA(long playerId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {

                var detailModel = contentUpdateService.GetContentUpdateforQA(playerId, ref errorResponseModel);

                if (detailModel != null)
                {
                    return Ok(detailModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }



        /// <summary>
        /// DeniedContentupdateDetail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("DeniedContentUpdateDetail")]
        [ProducesResponseType(typeof(ContentUpdateComment), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult DeniedContentUpdateDetail(ContentUpdateComment model)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var contentupdateModel = contentUpdateService.DeniedContentUpdateDetail(model, ref errorResponseModel);

                if (contentupdateModel != null)
                {
                    return Ok(contentupdateModel);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }


        /// <summary>
        /// To get Contentupdate by playerId 
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpGet("GetContentUpdateByPlayerId/{playerId}")]
        [ProducesResponseType(typeof(ContentUpdateModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetContentUpdateByPlayerId(long playerId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (playerId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var contentdetailModel = contentUpdateService.GetContentUpdateByPlayerId(playerId, ref errorResponseModel);

                if (contentdetailModel != null)
                {
                    return Ok(contentdetailModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }

    }
}
