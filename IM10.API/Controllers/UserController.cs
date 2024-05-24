using IM10.BAL.Interface;
using IM10.Common;
using IM10.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net;

namespace IM10.API.Controllers
{
    /// <summary>
    /// APIs for User entity 
    /// </summary>
    
    [Route("api/user")]
    [ApiController]

    public class UserController :BaseAPIController
    {
        IUserService _userService;

        /// <summary>
        /// Used to initialize controller and inject user service
        /// </summary>
        /// <param name="userService"></param>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// To get user by User ID 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("GetUserById/{userId}")]
        [Authorize]
        [ProducesResponseType(typeof(UserModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetUserById(long userId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (userId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var userModel = _userService.GetUserById(userId, ref errorResponseModel);

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
        /// Get all user
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetAllUser")]
        [Authorize]
        [ProducesResponseType(typeof(UserModel1), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllUser()
        {
            ErrorResponseModel errorResponseModel = null; 
            try
            {

                var userModel = _userService.GetAllUser(ref errorResponseModel);

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
        /// method for Get other user
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetOtherUser")]
        [Authorize]
        [ProducesResponseType(typeof(UserModel1), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetOtherUser()
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {

                var userModel = _userService.GetOtherUser(ref errorResponseModel);

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
        /// Create an user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("AddUser")]
        [Authorize(Roles = "Super Admin")]
        [ProducesResponseType(typeof(UserModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult AddUser(UserModel model)
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = ((System.Security.Claims.ClaimsIdentity)User.Identity).FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
                model.CreatedBy = Convert.ToInt32(userId);
            }

            if (model == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid request, please verify details");
            }
            else
            {
                try
                {
                    ErrorResponseModel errorResponseModel = null;
                    var userModel = _userService.AddUser(model, ref errorResponseModel);
                    return userModel != null && userModel != "Email already exists" ? Ok(userModel) : Conflict(userModel);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }

            }


        }



        /// <summary>
        /// Update an user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("EditUser")]
        [Authorize(Roles = "Super Admin")]
        [ProducesResponseType(typeof(UserModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult EditUser(UserModel users)
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = ((System.Security.Claims.ClaimsIdentity)User.Identity).FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
                users.UpdatedBy = Convert.ToInt32(userId);
            }

            if (users == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid request, please verify details");
            }

            ErrorResponseModel errorResponseModel = null;
            try
            {

                var UserModel = _userService.EditUser(users, ref errorResponseModel);

                return UserModel != null && UserModel != "Email Id Already Exists" ? Ok(UserModel) : Conflict(UserModel);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }


        /// <summary>
        /// Delete an user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete("DeleteUser")]
        [Authorize(Roles = "Super Admin")]
        [ProducesResponseType(typeof(UserDeleteModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult DeleteUser(long userId)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            try
            {
                var UserModel = _userService.DeleteUser(userId, ref errorResponseModel);

                if (UserModel != null)
                {
                    return Ok(UserModel);
                }
                else if (errorResponseModel.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound(errorResponseModel.Message);
                }
                else
                {
                    return ReturnErrorResponse(errorResponseModel);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }


        /// <summary>
        /// forget password
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("ForgetPassword")]
        public IActionResult ForgetPassword(string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request, please verify details");
            }

            try
            {
                var errorMessage = new ErrorResponseModel();
                var userModel = _userService.ForgetPassword(email, ref errorMessage);
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
        /// Change password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("ChangePassword")]
        [Authorize]
        [ProducesResponseType(typeof(changepasswordModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult ChangePassword(changepasswordModel users)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var UserModel = _userService.ChangePassword(users, ref errorResponseModel);

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



        [HttpPost("MobileRegister")]
        public IActionResult AddMobileUser(MobileUserRegisterModel model)
        {
            var errorMessage = new ErrorResponseModel();
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = ((System.Security.Claims.ClaimsIdentity)User.Identity).FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
                model.CreatedBy = Convert.ToInt32(userId);
            }
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(GlobalConstants.InvalidRequest);
            }
            try
            {
                string userModel = _userService.AddMobileUser(model, ref errorMessage);
                if (userModel != null)
                {
                    return Ok(userModel);
                }
                return ReturnErrorResponse(errorMessage);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, GlobalConstants.Status500Message);
            }
        }

        /// <summary>
        /// activate an user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPatch("ActivateUser")]
        public IActionResult ActivateUser(ActiveUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GlobalConstants.InvalidRequest);
            }
            try
            {
                var errorMessage = new ErrorResponseModel();
                var userModel = _userService.ActivateUser(model, ref errorMessage);
                if (userModel != null)
                {
                    var jsonContent = JsonConvert.SerializeObject(userModel);
                    return Ok(jsonContent);
                }
                return ReturnErrorResponse(errorMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, GlobalConstants.Status500Message);
            }
        }
    }
}
