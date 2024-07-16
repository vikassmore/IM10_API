using IM10.BAL.Interface;
using IM10.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System;
using IM10.Common;
using System.Web;
using System.Net;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Drawing.Drawing2D;
using IM10.BAL.Implementaion;
using IM10.Entity.DataModels;
using System.Threading.Tasks;

namespace IM10.API.Controllers
{

    /// <summary>
    /// APIs for User entity 
    /// </summary>

    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : BaseAPIController
    {
        IAuthService _authService;
        IConfiguration _configuration;
        public LoginController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] LoginModel model)
        {
            var authModel = new AuthModel();
            try
            {
                ErrorResponseModel errorResponseModel = null;
                if (!ModelState.IsValid)
                {
                    var errorMessage = string.Join(",", ModelState.Values.ToList());
                    return BadRequest(new { message = errorMessage });
                }
                var authData = _authService.AuthenticateUser(model.EmailId, model.Password, ref errorResponseModel);
                if (authData != null)
                {
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, authData.UserId.ToString()),
                        new Claim(ClaimTypes.Role, authData.Role),
                    };

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                    var key = Encoding.ASCII.GetBytes(GlobalConstants.AuthKey);

                    var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddDays(2),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256Signature)
                    );
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        authData.RoleId,
                        authData.Role,
                        authData.UserId,
                        authData.EmailId,
                        authData.FullName
                    });
                }


                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }


        /// <summary>
        /// Mobile login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("AuthenticateMobileUser")]
        public IActionResult AuthenticateMobileUser(MobileLoginModel model)
        {
            var authModel = new AuthModel();
            try
            {
                ErrorResponseModel errorResponseModel = null;
                if (!ModelState.IsValid)
                {
                    var errorMessage = string.Join(",", ModelState.Values.ToList());
                    return BadRequest(new { message = errorMessage });
                }
                var authData = _authService.AuthenticationForMobile(model, ref errorResponseModel);
                if (authData != null)
                {
                    return Ok(new
                    {
                        authData.UserId,
                        authData.RoleId,
                        authData.MobileNo,
                        authData.FirstName,
                        authData.LastName,
                        authData.CountryCode,
                        authData.StateId,
                        authData.CityId
                    });
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }
        /// <summary>
        /// OTP Verify
        /// </summary>
        /// <param name="otp"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("VerifyOtp")]
        public IActionResult VerifyOtp(string otp, long userId, string deviceToken)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (string.IsNullOrEmpty(otp))
                {
                    var errorMessage = string.Join(",", ModelState.Values.ToList());
                    return BadRequest(new { message = errorMessage });
                }
                var authData = _authService.OTPVerify(otp, userId, deviceToken,ref errorResponseModel);

                if (authData != null)
                {
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, authData.UserId.ToString()),
                    };

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                    var key = Encoding.ASCII.GetBytes(GlobalConstants.AuthKey);

                    var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddDays(2),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256Signature)
                    );
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        authData.RoleId,
                        authData.UserId,
                        authData.MobileNo,
                        authData.CountryCode,
                        authData.StateId,
                        authData.CityId,
                        authData.DeviceToken,
                        authData.FullName
                    });
                }
                else if(errorResponseModel != null && errorResponseModel.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return StatusCode((int)HttpStatusCode.Unauthorized, errorResponseModel.Message);
                }
                else if (errorResponseModel != null && errorResponseModel.StatusCode == HttpStatusCode.NotFound)
                {
                    return StatusCode((int)HttpStatusCode.NotFound, errorResponseModel.Message);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");

            }
        }


        /// <summary>
        /// OTP reSent
        /// </summary>
        /// <param name="mobileNo"></param>
        /// <returns></returns>
        [HttpPost("ReSentOtp")]
        public async Task<IActionResult> ReSentOtp(string mobileNo)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (!ModelState.IsValid)
                {
                    var errorMessage = string.Join(",", ModelState.Values.ToList());
                    return BadRequest(new { message = errorMessage });
                }
                var reSentOtp =await _authService.ReSentOtp(mobileNo);
                if (reSentOtp != null)
                {
                    return Ok(new
                    {
                        reSentOtp.MobileNo,
                        reSentOtp.UserId,
                        reSentOtp.Otp
                    });
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }


        /// <summary>
        /// Get Mobile User Profile
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("GetMobileUserProfile/{userId}")]
        [ProducesResponseType(typeof(UserProfileModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetMobileUserProfile(long userId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
               
                var userModel = _authService.GetMobileUserProfile(userId, ref errorResponseModel);

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
        /// method for logout user 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="deviceToken"></param>
        /// <returns></returns>
        [HttpPost("MobileLogOut/{userId}/{deviceToken}")]
        [ProducesResponseType(typeof(LogOutModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult MobileLogOut(long userId, string deviceToken)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                
                var userModel =_authService.MobileLogOut (userId,  deviceToken, ref errorResponseModel);

                if (userModel != null)
                {
                    return Ok(userModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,"Something went wrong!");
            }
        }




        /// <summary>
        /// method for check login status of  user 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("DeleteStatusOfUser")]
        [ProducesResponseType(typeof(LoginStaus), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult DeleteStatusOfUser(long userId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {               
                var userModel = _authService.DeleteStatusOfUser(userId);

                if (userModel != null)
                {
                    return Ok(userModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,ex.Message);
            }
        }



        /// <summary>
        /// This method is used to Get CountryId from ContryCode
        /// </summary>
        /// <param name="countryCode"></param>
        [HttpGet("GetCountryIdbyContryCode")]
        [ProducesResponseType(typeof(CountryModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetCountryIdbyContryCode(string countryCode)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {

                var countryModel = _authService.GetCountryIdbyContryCode(countryCode);

                if (countryModel != null)
                {
                    return Ok(countryModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }



        /// <summary>
        /// method for Delete Mobile User Account 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="deviceToken"></param>
        /// <returns></returns>
        [HttpPost("DeleteMobileUserAccount")]
        [ProducesResponseType(typeof(DeleteAccountModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult DeleteMobileUserAccount(long userId,string deviceToken)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                
                var userModel = _authService.DeleteMobileUserAccount(userId,deviceToken);

                if (userModel != null)
                {
                    return Ok(userModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,ex.Message);
            }
        }

    }
}


