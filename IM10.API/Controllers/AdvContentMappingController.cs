﻿using IM10.API.Hubs;
using IM10.BAL.Implementaion;
using IM10.BAL.Interface;
using IM10.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace IM10.API.Controllers
{
    /// <summary>
    /// APIs for advcontentmapping entity 
    /// </summary>


    [Route("api/[controller]")]
    [ApiController]
    public class AdvContentMappingController : BaseAPIController
    {
        IAdvContentMappingService AdvContentMapping;

        /// <summary>
        /// Used to initialize controller and inject advcontentmapping service
        /// </summary>
        /// <param name="advContentMapping"></param>
        public AdvContentMappingController(IAdvContentMappingService advContentMapping)
        {
            AdvContentMapping = advContentMapping;
        }

        /// <summary>
        /// To get advcontentmapping by AdvcontentmapId 
        /// </summary>
        /// <param name="AdvcontentmapId"></param>
        /// <returns></returns>
        [HttpGet("GetAdvContentMappingById/{AdvcontentmapId}")]
        [ProducesResponseType(typeof(AdvContentMappingModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAdvContentMappingById(long AdvcontentmapId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (AdvcontentmapId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var contentdetailModel = AdvContentMapping.GetAdvContentMappingById(AdvcontentmapId, ref errorResponseModel);

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
        /// Get all advcontentmapping
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetAllAdvContentMapping")]
        [ProducesResponseType(typeof(AdvContentMappingModel1), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllAdvContentMapping()
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {

                var detailModel = AdvContentMapping.GetAllAdvContentMapping(ref errorResponseModel);

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
        /// Create an advcontentmapping
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("AddAdvContentMapping")]
        //[Authorize]
        [ProducesResponseType(typeof(AdvContentMappingModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> AddAdvContentMapping(AdvContentMappingModel model)
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = ((System.Security.Claims.ClaimsIdentity)User.Identity).FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
                model.CreatedBy = model.AdvContentMapId == 0 ? Convert.ToInt32(userId) : model.CreatedBy;
                model.UpdatedBy = model.AdvContentMapId != 0 ? Convert.ToInt32(userId) : model.UpdatedBy;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request, please verify details");
            }

            try
            {
                var errorMessage = new ErrorResponseModel();
                NotificationModel contentModel = AdvContentMapping.AddAdvContentMapping(model, ref errorMessage);

                if (contentModel != null) 
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
        /// Delete an advcontentmapping
        /// </summary>
        /// <param name="advcontentmapId"></param>
        /// <returns></returns>
        [HttpDelete("DeleteAdvContentMapping")]
        [ProducesResponseType(typeof(AdvContentMappingModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult DeleteAdvContentMapping(long advcontentmapId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var contentModel = AdvContentMapping.DeleteAdvContentMapping(advcontentmapId, ref errorResponseModel);

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
        /// To get advcontentmapping by playerId 
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpGet("GetAdvContentMappingByPlayerId/{playerId}")]
        [ProducesResponseType(typeof(AdvContentMappingModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAdvContentMappingByPlayerId(long playerId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (playerId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var contentdetailModel = AdvContentMapping.GetAdvContentMappingByPlayerId(playerId, ref errorResponseModel);

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
