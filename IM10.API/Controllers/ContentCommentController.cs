using IM10.API.Hubs;
using IM10.BAL.Implementaion;
using IM10.BAL.Interface;
using IM10.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Twilio.TwiML.Messaging;

namespace IM10.API.Controllers
{
    /// <summary>
    /// APIs for contentcomment entity 
    /// </summary>


    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class ContentCommentController : BaseAPIController
    {
        IContentCommentService contentCommentService;

        /// <summary>
        /// Used to initialize controller and inject advcontentdetails service
        /// </summary>
        /// <param name="_advContentservice"></param>

        public ContentCommentController(IContentCommentService _contentCommentService)
        {
           contentCommentService = _contentCommentService;
        }


        /// <summary>
        /// Get contentcomment by commentId
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetContentCommentById/{commentId}")]
        [ProducesResponseType(typeof(ContentCommentModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetContentCommentById(long commentId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {

                var commentModel = contentCommentService.GetContentCommentById(commentId,ref errorResponseModel);

                if (commentModel != null)
                {
                    return Ok(commentModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        /// <summary>
        /// add contentcomment
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost("AddContentCommentReply")]
        [Authorize]
        [ProducesResponseType(typeof(ContentCommentModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> AddContentCommentReply(ContentCommentModel model)
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = ((System.Security.Claims.ClaimsIdentity)User.Identity).FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
                model.CreatedBy = Convert.ToInt32(userId);
                model.UserId = Convert.ToInt32(userId);
            }
            ErrorResponseModel errorResponseModel = null;
            try
            {

                CommentNotificationModel commentModel =await contentCommentService.AddContentCommentReply(model);

                if (commentModel != null)
                {
                    return Ok(commentModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



        /// <summary>
        /// Get contentcomment by playerId
        /// </summary>
        /// <param name="">playerId</param>
        /// <returns></returns>
        [HttpGet("GetContentCommentByPlayerId/{playerId}")]
        [ProducesResponseType(typeof(ContentCommentModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetContentCommentByPlayerId(long playerId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {

                var commentModel = contentCommentService.GetContentCommentByPlayerId(playerId, ref errorResponseModel);

                if (commentModel != null)
                {
                    return Ok(commentModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



        /// <summary>
        /// Delete an Comment Reply
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [HttpDelete("DeleteCommentReply")]
        [ProducesResponseType(typeof(ContentCommentModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult DeleteCommentReply(long commentId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var Model = contentCommentService.DeleteCommentReply(commentId, ref errorResponseModel);

                if (Model != null)
                {
                    return Ok(Model);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Get contentcomment reply by commentId
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [HttpGet("GetContentCommentReplyByCommentId/{commentId}")]
        [ProducesResponseType(typeof(ContentCommentModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetContentCommentReplyByCommentId(long commentId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {

                var commentModel = contentCommentService.GetContentCommentReplyByCommentId(commentId, ref errorResponseModel);

                if (commentModel != null)
                {
                    return Ok(commentModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
