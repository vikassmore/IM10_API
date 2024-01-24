using IM10.BAL.Implementaion;
using IM10.BAL.Interface;
using IM10.Common;
using IM10.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace IM10.API.Controllers
{
    /// <summary>
    /// APIs for UserPlayer entity 
    /// </summary>
    
    [Route("api/[controller]")]
    [ApiController]
    
    public class UserPlayerController : BaseAPIController
    {
        IUserPlayerService _userPlayerService;

        /// <summary>
        /// Used to initialize controller and inject userplayer service
        /// </summary>
        /// <param name="userPlayerService"></param>
        public UserPlayerController(IUserPlayerService userPlayerService)
        {
            _userPlayerService = userPlayerService;
        }



        /// <summary>
        /// To get userplayer by UserplayerID 
        /// </summary>
        /// <param name="userplayerId"></param>
        /// <returns></returns>
        [HttpGet("GetUserPlayerById/{userplayerId}")]
        [ProducesResponseType(typeof(UserPlayerModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetUserPlayerById(long userplayerId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (userplayerId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var userModel = _userPlayerService.GetUserPlayerById(userplayerId, ref errorResponseModel);

                if (userModel != null)
                {
                    return Ok(userModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }

        /// <summary>
        /// Get all userplayer
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetAllUserPlayer")]
        [ProducesResponseType(typeof(UserPlayerModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllUserPlayer()
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {

                var userModel = _userPlayerService.GetAllUserPlayer(ref errorResponseModel);

                if (userModel != null) 
                {
                    return Ok(userModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }



        /// <summary>
        /// Create an userplayer
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("AddUserPlayer")]
       // [Authorize]
        [ProducesResponseType(typeof(UserPlayerModel1), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult AddUserPlayer(UserPlayerModel1 model)
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
                var userModel = _userPlayerService.AddUserPlayer(model, ref errorMessage);
                if (userModel != "")
                {
                    return Ok(userModel);
                }
                return ReturnErrorResponse(errorMessage);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }

        }



        /// <summary>
        /// Update an userplayer
        /// </summary>
        /// <param name="playerModel"></param>
        /// <returns></returns>
        [HttpPut("EditUserPlayer")]
       // [Authorize]
        [ProducesResponseType(typeof(UserPlayerMappingModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult EditUserPlayer(UserPlayerMappingModel playerModel)
        {
            ErrorResponseModel errorResponseModel = null;

            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = ((System.Security.Claims.ClaimsIdentity)User.Identity).FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
                playerModel.UpdatedBy = Convert.ToInt32(userId);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(GlobalConstants.InvalidRequest);
            }
            try
            {
                var Model = _userPlayerService.EditUserPlayer(playerModel, ref errorResponseModel);

                if (Model != null)
                {
                    return Ok(Model);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }


        /// <summary>
        /// Delete an userplayer
        /// </summary>
        /// <param name="userplayerId"></param>
        /// <returns></returns>
        [HttpDelete("DeleteUserPlayer")]
        [ProducesResponseType(typeof(UserPlayerModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult DeleteUserPlayer(long userplayerId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var UserModel = _userPlayerService.DeleteUserPlayer(userplayerId, ref errorResponseModel);

                if (UserModel != null)
                {
                    return Ok(UserModel);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }




        /// <summary>
        /// To get player by UserID 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("GetPlayerByUserId/{userId}")]
        [ProducesResponseType(typeof(UserPlayerModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetPlayerByUserId(long userId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (userId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var userModel = _userPlayerService.GetPlayerByUserId(userId, ref errorResponseModel);

                if (userModel != null)
                {
                    return Ok(userModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }



        /// <summary>
        /// To get player by userplayerId 
        /// </summary>
        /// <param name="userplayerId"></param>
        /// <returns></returns>
        [HttpGet("GetuserplayerByUserplayerId/{userplayerId}")]
        [ProducesResponseType(typeof(UserPlayerModel2), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetuserplayerByUserplayerId(long userplayerId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (userplayerId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var userModel = _userPlayerService.GetuserplayerByUserplayerId(userplayerId, ref errorResponseModel);

                if (userModel != null)
                {
                    return Ok(userModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }



        /// <summary>
        /// Get all userplayerdetais
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetAllUserPlayerdetailsbycommaseparate")]
        [ProducesResponseType(typeof(UserPlayerModel3), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllUserPlayerdetailsbycommaseparate()
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {

                var userModel = _userPlayerService.GetAllUserPlayerdetailsbycommaseparate(ref errorResponseModel);

                if (userModel != null)
                {
                    return Ok(userModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }



        /// <summary>
        /// To get player by roleId 
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet("GetPlayerByRoleId/{roleId}")]
        [ProducesResponseType(typeof(UserPlayerModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetPlayerByRoleId(long roleId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (roleId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var userModel = _userPlayerService.GetPlayerByRoleId(roleId, ref errorResponseModel);

                if (userModel != null)
                {
                    return Ok(userModel);
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
