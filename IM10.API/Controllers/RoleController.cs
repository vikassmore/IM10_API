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
    /// APIs for role entity 
    /// </summary>

    [Route("api/role")]
    [ApiController]
    [Authorize]
    public class RoleController : BaseAPIController
    {
        IRoleService roleMasterService;

        /// <summary>
        /// Used to initialize controller and inject roles service
        /// </summary>
        /// <param name="_roleMasterService"></param>

        public RoleController(IRoleService _roleMasterService)
        {
            roleMasterService = _roleMasterService;
        }

        /// <summary>
        /// To get roles by roleId 
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet("GetRoleById/{roleId}")]
        [ProducesResponseType(typeof(RoleModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetRoleById(long roleId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (roleId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var roleModel = roleMasterService.GetRoleById(roleId, ref errorResponseModel);

                if (roleModel != null)
                {
                    return Ok(roleModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetAllRoles")]
        [ProducesResponseType(typeof(RoleModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllRoles()
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {

                var roleModel = roleMasterService.GetAllRoles(ref errorResponseModel);

                if (roleModel != null)
                
                { 
                    return Ok(roleModel); 
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
