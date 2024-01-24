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
    /// APIs for EndorsmentType entity 
    /// </summary>
    /// 
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EndorsmentTypeController : BaseAPIController
    {
        IEndorsmentTypeService service;

        /// <summary>
        /// Used to initialize controller and inject EndorsmentType service
        /// </summary>
        /// <param name="_detailservice"></param>
        public EndorsmentTypeController(IEndorsmentTypeService _service)
        {
            service = _service;
        }


        /// <summary>
        /// To get EndorsmentType by endorsmenttypeId 
        /// </summary>
        /// <param name="endorsmenttypeId"></param>
        /// <returns></returns>
        [HttpGet("GetEndorsmentTypeById/{endorsmenttypeId}")]
        [ProducesResponseType(typeof(EndorsmentTypeModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetEndorsmentTypeById(long endorsmenttypeId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (endorsmenttypeId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var typeModel = service.GetEndorsmentTypeById(endorsmenttypeId, ref errorResponseModel);

                if (typeModel != null)
                {
                    return Ok(typeModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }

        /// <summary>
        /// Get all EndorsmentType
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetAllEndorsmentType")]
        [ProducesResponseType(typeof(EndorsmentTypeModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllEndorsmentType()
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {

                var endorsmentModel = service.GetAllEndorsmentType(ref errorResponseModel);

                if (endorsmentModel != null)
                {
                    return Ok(endorsmentModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }



        /// <summary>
        /// Create/Update an EndorsmentType
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("AddEndorsmentType")]
        [ProducesResponseType(typeof(EndorsmentTypeModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult AddEndorsmentType(EndorsmentTypeModel model)
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = ((System.Security.Claims.ClaimsIdentity)User.Identity).FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
                model.CreatedBy = model.EndorsmentTypeId == 0 ? Convert.ToInt32(userId) : model.CreatedBy;
                model.UpdatedBy = model.EndorsmentTypeId != 0 ? Convert.ToInt32(userId) : model.UpdatedBy;
            }


            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request, please verify details");
            }

            try
            {
                var errorMessage = new ErrorResponseModel();
                var endorsmentModel = service.AddEndorsmentType(model, ref errorMessage);

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
        /// Delete an EndorsmentType
        /// </summary>
        /// <param name="endorsmenttypeId"></param>
        /// <returns></returns>
        [HttpDelete("DeleteEndorsmentType")]
        [ProducesResponseType(typeof(EndorsmentTypeModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult DeleteEndorsmentType(long endorsmenttypeId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var typeModel = service.DeleteEndorsmentType(endorsmenttypeId, ref errorResponseModel);

                if (typeModel != null)
                {
                    return Ok(typeModel);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }
    }
}
