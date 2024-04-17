using IM10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Interface
{
    /// <summary>
    /// Interface used for Auth related operations
    /// </summary>
    public interface IAuthService
    {

        /// <summary>
        /// This method is used to validate user credentials
        /// </summary>
        /// <param name="emailId"></param>
        /// <param name="password"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        AuthModel AuthenticateUser(string emailId, string password, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// This method is used to validate user credentials
        /// </summary>
        /// <param name="emailId"></param>
        /// <param name="password"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        AuthModel AuthenticationForMobile(MobileLoginModel loginModel, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Create OTP
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        string CreateNewOTP(long userId);

        /// <summary>
        /// OTP verify
        /// </summary>
        /// <param name="otp"></param>
        /// <param name="userId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        AuthModel OTPVerify(string otp, int userId,string deviceToken, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Get Mobile User Profile
        /// </summary>
        /// <param name="otp"></param>
        /// <param name="userId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        UserProfileModel GetMobileUserProfile(int userId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// method for logout user 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="deviceToken"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        string MobileLogOut(long userId,string deviceToken, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// method for check login status of  user 
        /// </summary>
        /// <param name="Mobile"></param>
        /// <returns></returns>
        LoginStaus LoginStatusOfUser(string Mobile);


        /// <summary>
        /// This method is used to resend otp
        /// </summary>
        /// <param name="mobileno"></param>
         Task<ResendOtp> ReSentOtp(string mobileNo);



        /// <summary>
        /// This method is used to Get CountryId from ContryCode
        /// </summary>
        /// <param name="countryCode"></param>
        CountryModel GetCountryIdbyContryCode(string countryCode);

    }
}
