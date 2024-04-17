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
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        /// <summary>
        /// OTP Verify
        /// </summary>
        /// <param name="otp"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("VerifyOtp")]
        public IActionResult VerifyOtp(string otp, int userId, string deviceToken)
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
                        authData.Role,
                        authData.UserId,
                        authData.MobileNo,
                        authData.CountryCode,
                        authData.StateId,
                        authData.CityId,
                        authData.DeviceToken
                    });
                }
                else if(errorResponseModel.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return Ok(errorResponseModel.Message);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

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
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetMobileUserProfile(int userId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (userId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var userModel = _authService.GetMobileUserProfile(userId, ref errorResponseModel);

                if (userModel != null)
                {
                    return Ok(userModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult MobileLogOut(long userId, string deviceToken)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (userId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var userModel =_authService.MobileLogOut (userId,  deviceToken, ref errorResponseModel);

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
        /// Get Mobile User Profile
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("LoginStatusOfUser")]
        [ProducesResponseType(typeof(LoginStaus), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult LoginStatusOfUser(string Mobile)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                
                var userModel = _authService.LoginStatusOfUser(Mobile);

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
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}


