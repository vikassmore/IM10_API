using IM10.BAL.Interface;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace IM10.API.Controllers
{

    /// <summary>
    /// APIs for CampaignDetail entity 
    /// </summary>
    /// 
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignDetailController : BaseAPIController
    {
        ICampaignDetailService detailService;

        /// <summary>
        /// Used to initialize controller and inject CampaignDetail service
        /// </summary>
        /// <param name="_detailservice"></param>
        public CampaignDetailController(ICampaignDetailService _detailservice)
        {
            detailService = _detailservice;
        }

        /// <summary>
        /// To get CampaignDetail by marketingcampaignId 
        /// </summary>
        /// <param name="marketingcampaignId"></param>
        /// <returns></returns>
        [HttpGet("GetCampaignDetailById/{marketingcampaignId}")]
        [ProducesResponseType(typeof(CampaignDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetCampaignDetailById(long marketingcampaignId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (marketingcampaignId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var contentdetailModel = detailService.GetCampaignDetailById(marketingcampaignId, ref errorResponseModel);

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
        /// Get all CampaignDetail
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetAllCampaignDetail")]
        [ProducesResponseType(typeof(CampaignDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllCampaignDetail()
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {

                var detailModel = detailService.GetAllCampaignDetail(ref errorResponseModel);

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
        /// Create/Update an CampaignDetail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("AddCampaignDetail")]
        [Authorize]
        [ProducesResponseType(typeof(CampaignDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult AddCampaignDetail(CampaignDetailModel model)
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = ((System.Security.Claims.ClaimsIdentity)User.Identity).FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
                model.CreatedBy  = model.MarketingCampaignId==0 ?  Convert.ToInt32(userId): model.CreatedBy;
                model.UpdatedBy=model.MarketingCampaignId !=0  ? Convert.ToInt32(userId) : model.UpdatedBy;
            }
             

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request, please verify details");
            }

            try
            {
                var errorMessage = new ErrorResponseModel();
                var contentModel = detailService.AddCampaignDetail(model, ref errorMessage);

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
        /// Delete an CampaignDetail
        /// </summary>
        /// <param name="marketingcampaignId"></param>
        /// <returns></returns>
        [HttpDelete("DeleteCampaignDetail")]
        [ProducesResponseType(typeof(CampaignDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult DeleteCampaignDetail(long marketingcampaignId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var contentModel = detailService.DeleteCampaignDetail(marketingcampaignId, ref errorResponseModel);

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
        /// To get CampaignDetail by playerId 
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpGet("GetCampaignDetailByPlayerId/{playerId}")]
        [ProducesResponseType(typeof(CampaignDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetCampaignDetailByPlayerId(long playerId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (playerId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var contentdetailModel = detailService.GetCampaignDetailByPlayerId(playerId, ref errorResponseModel);

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
