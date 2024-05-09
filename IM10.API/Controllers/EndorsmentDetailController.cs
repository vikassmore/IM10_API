using IM10.BAL.Interface;
using IM10.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace IM10.API.Controllers
{
    /// <summary>
    /// APIs for EndorsmentDetails entity 
    /// </summary>
    /// 
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EndorsmentDetailController : BaseAPIController
    {
        IEndorsmentDetailService service;

        /// <summary>
        /// Used to initialize controller and inject EndorsmentDetail service
        /// </summary>
        /// <param name="_detailservice"></param>
        public EndorsmentDetailController(IEndorsmentDetailService _service)
        {
            service = _service;
        }

        /// <summary>
        /// To get EndorsmentDetail by endorsmentId 
        /// </summary>
        /// <param name="endorsmentId"></param>
        /// <returns></returns>
        [HttpGet("GetEndorsmentDetailById/{endorsmentId}")]
        [ProducesResponseType(typeof(EndorsmentDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetEndorsmentDetailById(long endorsmentId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (endorsmentId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var detailModel = service.GetEndorsmentDetailById(endorsmentId, ref errorResponseModel);

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
        /// Create/Update an EndorsmentDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("AddEndorsmentDetail")]
        [ProducesResponseType(typeof(EndorsmentDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult AddEndorsmentDetail(EndorsmentDetailModel model)
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = ((System.Security.Claims.ClaimsIdentity)User.Identity).FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
                model.CreatedBy = model.EndorsmentId == 0 ? Convert.ToInt32(userId) : model.CreatedBy;
                model.UpdatedBy = model.EndorsmentId != 0 ? Convert.ToInt32(userId) : model.UpdatedBy;
            }


            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request, please verify details");
            }

            try
            {
                var errorMessage = new ErrorResponseModel();
                var endorsmentModel = service.AddEndorsmentDetail(model, ref errorMessage);

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


        /// <summary>
        /// Delete an EndorsmentDetails
        /// </summary>
        /// <param name="endorsmentId"></param>
        /// <returns></returns>
        [HttpDelete("DeleteEndorsmentDetail")]
        [ProducesResponseType(typeof(EndorsmentDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult DeleteEndorsmentDetail(long endorsmentId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var detailModel = service.DeleteEndorsmentDetail(endorsmentId, ref errorResponseModel);

                if (detailModel != null)
                {
                    return Ok(detailModel);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }


        /// <summary>
        /// To get EndorsmentDetail by playerId 
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpGet("GetEndorsmentDetailPlayerId/{playerId}")]
        [ProducesResponseType(typeof(EndorsmentDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetEndorsmentDetailPlayerId(long playerId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (playerId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var detailModel = service.GetEndorsmentDetailPlayerId(playerId, ref errorResponseModel);

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

    }
}
