using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Twilio.TwiML.Messaging;

namespace IM10.BAL.Implementaion
{
    /// <summary>
    /// This is implementation  for the Auth operations 
    /// </summary>
    public class AuthService : IAuthService
    {
        IM10DbContext context;
        private readonly IEmailSenderService _emailSender;
        private readonly IEncryptionService _encryptionService ;
        /// <summary>
        /// Creating constructor and injection dbContext
        /// </summary>
        /// <param name="_context"></param>
        public AuthService(IEncryptionService encryptionService, IM10DbContext _context, IEmailSenderService emailSender)
        {
            _emailSender = emailSender;
            context = _context;
            _encryptionService = encryptionService;
        }

        public AuthModel AuthenticateUser(string emailId, string password, ref ErrorResponseModel errorResponseModel)
        {
            var authModel = new AuthModel();
            errorResponseModel = new ErrorResponseModel();
            var userEntity = context.UserMasters.FirstOrDefault(x => x.EmailId == _encryptionService.GetEncryptedId(emailId.ToString()) && x.Password == _encryptionService.GetEncryptedId(password.ToString()) && x.IsActive == true && x.IsDeleted == false);

            if (userEntity == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = "User not  found. Please enter valid credentials";
                return null;
            }

            var roleEntity = context.Roles.FirstOrDefault(x => x.RoleId == userEntity.RoleId);
            //To DO: Add multiple attempt logic
            // Update role dynamic logic
            return new AuthModel
            {
                UserId = userEntity.UserId,
                Role = roleEntity.Name,
                RoleId = userEntity.RoleId,
                EmailId = userEntity.EmailId,
                FullName = userEntity.FirstName + " " + userEntity.LastName,

            };
        }
        /// <summary>
        /// Mobile User Login
        /// </summary>
        /// <param name="loginModel"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public AuthModelForMobile AuthenticationForMobile(MobileLoginModel loginModel, ref ErrorResponseModel errorResponseModel)
        {
            var authModel = new AuthModelForMobile();
            errorResponseModel = new ErrorResponseModel();

            var user = context.UserMasters.Where(x => x.MobileNo == loginModel.MobileNo && x.IsDeleted==false).FirstOrDefault();

            if (user != null)
            {
                 // User's account is active, perform logout handling
                 var existingMapping = context.UserDeviceMappings.FirstOrDefault(z => z.UserId == user.UserId && z.DeviceToken == loginModel.DeviceToken);
                  if (existingMapping != null)
                  {
                      // Update the existing mapping to mark as deleted
                      existingMapping.IsDeleted = false;
                      existingMapping.UpdatedDate = DateTime.Now;
                      context.SaveChanges();
                  }

                  // Generate OTP and send SMS for logout
                  string otp = CreateNewOTP(user.UserId);
                  if (!string.IsNullOrEmpty(user.MobileNo))
                  {
                      _emailSender.SendSmsAsync(user.MobileNo, user.FirstName + " " + user.LastName, otp);
                  }
                  authModel.UserId =user.UserId;
                  authModel.RoleId = user.RoleId;
                  authModel.MobileNo = user.MobileNo;
                  authModel.FirstName = user.FirstName;
                  authModel.LastName = user.LastName;
                  authModel.CountryCode = user.CountryCode;
                  authModel.StateId = user.StateId;
                  authModel.CityId = user.CityId;
                
            }
            else
            {
                var userEntity = new UserMaster();
                userEntity.Username = null;
                userEntity.FirstName = loginModel.FirstName;
                userEntity.LastName = loginModel.LastName;
                userEntity.EmailId = "mobileuser@gmail.com";
                userEntity.MobileNo = loginModel.MobileNo;
                userEntity.Dob = DateTime.Now;
                userEntity.Password = "123456";
                userEntity.DeviceToken = null;
                userEntity.CityId = loginModel.CityId;
                userEntity.StateId = loginModel.StateId;
                userEntity.CountryCode = loginModel.CountryCode;
                userEntity.RoleId = 12;
                userEntity.IsLogin = true;
                userEntity.AppId = 0;
                userEntity.CreatedDate = DateTime.Now;
                userEntity.IsActive = true;
                userEntity.IsDeleted = false;
                userEntity.CreatedBy = 1;
                userEntity.UpdatedBy = 1;
                context.UserMasters.Add(userEntity);
                context.SaveChanges();

                var newUserDeviceMapping = new UserDeviceMapping
                {
                    UserId = userEntity.UserId,
                    DeviceToken = loginModel.DeviceToken,
                    CreatedDate = DateTime.Now,
                    CreatedBy = (int?)userEntity.UserId,
                    UpdatedDate = DateTime.Now,
                    IsDeleted = false
                };

                context.UserDeviceMappings.Add(newUserDeviceMapping);
                context.SaveChanges();
                Random generator = new Random();
                string otp = CreateNewOTP((userEntity.UserId));

                if (!string.IsNullOrEmpty(userEntity.MobileNo))
                {
                    _emailSender.SendSmsAsync(userEntity.MobileNo, userEntity.FirstName + " " + userEntity.LastName, otp);
                }

                authModel.UserId = userEntity.UserId;
                authModel.RoleId = userEntity.RoleId;
                authModel.MobileNo = userEntity.MobileNo;
                authModel.FirstName = userEntity.FirstName;
                authModel.LastName = userEntity.LastName;
                authModel.CountryCode = userEntity.CountryCode;
                authModel.StateId = userEntity.StateId;
                authModel.CityId = userEntity.CityId;
            }
            return authModel;
        }

        public string CreateNewOTP(long userId)
        {
            string message = "";
            Random generator = new Random();
            string otp = generator.Next(0, 999999).ToString("D6");

            var otpEntity = new Otpautherization();
            if (userId == 0)
            {
                message = "error";
                return message;
            }
            else
            {
                var existingRecord = context.Otpautherizations.Where(m => m.UserId == userId).FirstOrDefault();
                if (existingRecord != null)
                {
                    existingRecord.Otp = otp;
                    existingRecord.IsActive = true;
                    existingRecord.OtpvalidDateTime = DateTime.Now;
                    context.SaveChanges();
                }
                else
                {
                    otpEntity.Otp = otp;
                    otpEntity.OtpvalidDateTime = DateTime.Now;
                    otpEntity.UserId = userId;
                    otpEntity.IsActive = true;
                    context.Otpautherizations.Add(otpEntity);
                    context.SaveChanges();
                }
                message = "record added.";
                return otp;
            }
        }


        /// <summary>
        /// OTP verify
        /// </summary>
        /// <param name="otp"></param>
        /// <param name="userId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public AuthModelForMobile OTPVerify(string otp, long userId, string deviceToken, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();

            var User = context.UserMasters.Include(x => x.UserPlayerMappings).FirstOrDefault(m => m.UserId == userId);
            if (User != null)
            {               
                var userdevicemapping = context.UserDeviceMappings.Where(z => z.UserId == User.UserId && z.DeviceToken==deviceToken).FirstOrDefault();
                
                if (userdevicemapping != null)
                {
                    // Update the existing record
                    userdevicemapping.UpdatedBy = (int?)User.UserId;
                    userdevicemapping.UpdatedDate = DateTime.Now;
                    userdevicemapping.IsDeleted = false;
                }
                else
                {
                    // Create a new UserDeviceMapping record
                    var userDeviceMapping = new UserDeviceMapping
                    {
                        UserId = User.UserId,
                        DeviceToken = deviceToken,
                        CreatedDate = DateTime.Now,
                        CreatedBy = (int?)User.UserId,
                        UpdatedDate = DateTime.Now,
                        IsDeleted = false
                    };
                    context.UserDeviceMappings.Add(userDeviceMapping);                  
                }
                var fcmModel = context.Fcmnotifications.FirstOrDefault(f => f.DeviceToken == deviceToken);
                if (fcmModel != null)
                {
                    fcmModel.UserId = User.UserId;
                    fcmModel.UpdatedBy= (int?)User.UserId;
                    fcmModel.UpdatedDate = DateTime.Now;
                }
                else
                {
                    // Create a new Fcmnotification record
                    fcmModel = new Fcmnotification
                    {
                        UserId = User.UserId,
                        CreatedDate = DateTime.Now,
                        CreatedBy = (int?)User.UserId,
                        UpdatedDate = DateTime.Now,
                    };
                    context.Fcmnotifications.Add(fcmModel);
                }
                context.SaveChanges();
                var otpEntity = context.Otpautherizations.FirstOrDefault(x => x.Otp == otp && x.UserId == User.UserId && x.IsActive == true);
                if (otpEntity != null)
                {
                    var isValidOtp = DateTime.Now.Subtract(otpEntity.OtpvalidDateTime.Value).TotalMinutes <= 2;
                    if (isValidOtp)
                    {             
                        return new AuthModelForMobile
                        {
                            UserId = userId,
                            RoleId = User.RoleId,
                            MobileNo = User.MobileNo,
                            CountryCode = User.CountryCode,
                            StateId = User.StateId,
                            CityId = User.CityId,
                            DeviceToken=deviceToken,
                            FullName=User.FirstName + " " + User.LastName,
                        };
                    }
                    else
                    {
                        errorResponseModel.StatusCode = HttpStatusCode.Unauthorized;
                        errorResponseModel.Message = "OTP Expired";
                        return null;
                    }
                }
                else
                {
                    errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                    errorResponseModel.Message = "OTP Not Found";
                    return null;
                }
            }
            else
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = "Record Not Found";
                return null;
            }
        }



        /// <summary>
        /// Get Mobile User Profile
        /// </summary>
        /// <param name="otp"></param>
        /// <param name="userId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public UserProfileModel GetMobileUserProfile(long userId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();         
            var userEntity = (from user in context.UserMasters
                              join country in context.Countries on user.CountryCode equals country.CountryCode
                              join state in context.States on user.StateId equals state.StateId
                              join city in context.Cities on user.CityId equals city.CityId
                              where user.UserId == userId && user.IsActive == true && user.IsDeleted == false
                              select new UserProfileModel
                              {
                                  UserId=userId,
                                  RoleId=user.RoleId,
                                  MobileNo=user.MobileNo,
                                  FullName=user.FirstName + " " + user.LastName,
                                  CountryCode=user.CountryCode,
                                  CountryName=country.Name,
                                  StateId=user.StateId,
                                  StateName=state.Name,
                                  CityId=user.CityId,
                                  CityName=city.Name,
                              }).FirstOrDefault();
                
            if (userEntity == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = "User Not Found";
                return null;
            }
            return userEntity;
        }


        /// <summary>
        /// method for logout user 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="deviceToken"></param>
        /// <returns></returns>
        public string MobileLogOut(long userId, string deviceToken, ref ErrorResponseModel errorResponseModel)
        {
            string message = "";
            errorResponseModel = new ErrorResponseModel();
            var existingUser = context.UserDeviceMappings.Where(x => x.UserId == userId && x.DeviceToken==deviceToken).FirstOrDefault();
            if (existingUser != null)
            {
                // Update values
                existingUser.IsDeleted = true;
                existingUser.UpdatedDate= DateTime.Now;
                existingUser.UpdatedBy = (int?)userId;
                context.SaveChanges();
                message = GlobalConstants.LogOut;
            }
            return message;
        }


        /// <summary>
        /// method for check login status of  user 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public LoginStaus DeleteStatusOfUser(long userId)
        {
            LoginStaus login = new LoginStaus();
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();

            var existinglogin = context.UserMasters.FirstOrDefault(z => z.UserId == userId);
            if (existinglogin != null)
            {
                login.UserId = userId;
                login.IsLogin = existinglogin.IsLogin;
                login.AccountDeletedStatus = existinglogin.IsActive;
            }
            return login;
        }



        /// <summary>
        /// This method is used to resend otp
        /// </summary>
        /// <param name="mobileno"></param>
        /// <param name="errorResponseModel"></param>
        public async Task<ResendOtp> ReSentOtp(string mobileNo)
        {
            var user = context.UserMasters.Where(x => x.MobileNo == mobileNo && x.IsActive == true && x.IsDeleted == false).FirstOrDefault();

            if (user != null)
            {                       
                Random generator = new Random();
                string otp = CreateNewOTP((user.UserId));

                if (!string.IsNullOrEmpty(user.MobileNo))
                {
                  await _emailSender.SendSmsAsync(user.MobileNo, user.FirstName + " " + user.LastName, otp);
                }
                return new ResendOtp
                {
                    UserId = user.UserId,
                    MobileNo = user.MobileNo,  
                    Otp=otp
                };
            }
            else
            {
                throw new ArgumentException("User not found for the provided mobile number.");
            }
        }


        /// <summary>
        /// This method is used to Get CountryId from ContryCode
        /// </summary>
        /// <param name="countryCode"></param>
        public CountryModel GetCountryIdbyContryCode(string countryCode)
        {
            var existingCountry = context.Countries.Where(z => z.CountryCode == countryCode && z.IsDeleted==false).FirstOrDefault();
            if (existingCountry != null)
            {
                CountryModel countryModel = new CountryModel();
                countryModel.CountryCode = countryCode;
                countryModel.CountryId = existingCountry.CountryId;
                countryModel.Name=existingCountry.Name;
                return countryModel;
            }
            else
            {
                throw new Exception("Country with the provided country code was not found.");
            }
        }



        /// <summary>
        /// method for Delete Mobile User Account 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="deviceToken"></param>
        /// <returns></returns>
        public string DeleteMobileUserAccount(long userId, string deviceToken)
        {
            ErrorResponseModel errorResponseModel = new ErrorResponseModel();
            string message = "";
            try
            {
                var userEntity = context.UserMasters.FirstOrDefault(z => z.UserId == userId && z.IsDeleted==false);
                if (userEntity == null)
                {
                    message = "Record Not found";
                }
                if (userEntity != null)
                {
                    var deviceEntity = context.UserDeviceMappings.FirstOrDefault(z => z.DeviceToken == deviceToken && z.UserId == userEntity.UserId);
                    if (deviceEntity != null)
                    {
                        deviceEntity.IsDeleted = true;
                        context.SaveChanges();
                    }
                    var commentUser = context.Comments.Where(z => z.UserId ==userEntity.UserId).ToList();
                    if (commentUser.Count > 0)
                    {
                        foreach (var comment in commentUser)
                        {
                            var existingreply = context.Comments.Where(z => z.ParentCommentId == comment.CommentId).ToList();
                            if (existingreply.Count > 0)
                            {
                                foreach (var commentreply in existingreply)
                                {
                                    commentreply.IsDeleted = true;
                                    context.SaveChanges();
                                }
                            }
                            comment.IsDeleted = true;
                            context.SaveChanges();
                        }
                    }
                    var likeEntity=context.ContentFlags.Where(z => z.UserId==userEntity.UserId).ToList();
                    if (likeEntity.Count > 0)
                    {
                        foreach(var like in likeEntity)
                        {
                            like.IsDeleted = true;
                            context.SaveChanges();
                        }
                    }
                    userEntity.UserId = userId;
                    userEntity.IsLogin = false;
                    userEntity.IsActive = false;
                    userEntity.IsDeleted = true;
                    context.SaveChanges();
                    message = GlobalConstants.DeleteMobileAccount;
                }
            }
            catch (Exception ex)
            {
                return "Message: " + ex.Message + Environment.NewLine + "StackTrace: " + ex.StackTrace + Environment.NewLine + "Source: " + ex.Source + Environment.NewLine +
                      (ex.InnerException != null ? "InnerException: " + ex.InnerException.Message + Environment.NewLine + "InnerException StackTrace: " + ex.InnerException.StackTrace : string.Empty);
            }
            return message;
        }
    }
}
