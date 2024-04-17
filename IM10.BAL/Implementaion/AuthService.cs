﻿using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
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
        /// <summary>
        /// Creating constructor and injection dbContext
        /// </summary>
        /// <param name="_context"></param>
        public AuthService(IM10DbContext _context, IEmailSenderService emailSender)
        {
            _emailSender = emailSender;
            context = _context;
        }

        public AuthModel AuthenticateUser(string emailId, string password, ref ErrorResponseModel errorResponseModel)
        {
            var authModel = new AuthModel();
            errorResponseModel = new ErrorResponseModel();
            var userEntity = context.UserMasters.FirstOrDefault(x => x.EmailId == emailId && x.Password == EncryptionHelper.Encrypt(password.ToString()) && x.IsActive == true && x.IsDeleted==false);

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
        public AuthModel AuthenticationForMobile(MobileLoginModel loginModel, ref ErrorResponseModel errorResponseModel)
        {
            var authModel = new AuthModel();
            errorResponseModel = new ErrorResponseModel();
            var user = context.UserMasters.Where(x => x.MobileNo == loginModel.MobileNo  && x.IsActive==true && x.IsDeleted == false).FirstOrDefault();
            
            if (user != null)
            {
                var existingUser = context.UserMasters.Where(x => x.MobileNo == loginModel
                  .MobileNo  && x.IsDeleted == false).FirstOrDefault();
                {
                    context.Entry(existingUser).Property(x => x.FirstName).IsModified = true;
                    context.Entry(existingUser).Property(x => x.LastName).IsModified = true;
                    context.Entry(existingUser).Property(x => x.CountryCode).IsModified = true;
                    context.Entry(existingUser).Property(x => x.StateId).IsModified = true;
                    context.Entry(existingUser).Property(x => x.CityId).IsModified = true;


                    // Update values
                    existingUser.FirstName = loginModel.FirstName;
                    existingUser.LastName= loginModel.LastName;
                    existingUser.CountryCode = loginModel.CountryCode;
                    existingUser.StateId = loginModel.StateId;
                    existingUser.CityId = loginModel.CityId;
                    context.SaveChanges();
                }

                var fcmexstingUser = context.Fcmnotifications.Where(x => x.DeviceToken == loginModel.DeviceToken && x.PlayerId == loginModel.PlayerId && x.IsDeleted==false).FirstOrDefault();
                if (fcmexstingUser != null)
                {
                    fcmexstingUser.UserId = user.UserId;
                    context.SaveChanges();
                }

                Random generator = new Random();
                string otp = CreateNewOTP((user.UserId));

                if (!string.IsNullOrEmpty(user.MobileNo))
                {
                    _emailSender.SendSmsAsync(user.MobileNo, user.FirstName + " " + user.LastName, otp);                   
                }
                return new AuthModel
                {
                    UserId = user.UserId,
                    RoleId = user.RoleId,
                    MobileNo = user.MobileNo,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CountryCode = user.CountryCode,
                    StateId = user.StateId,
                    CityId = user.CityId,
                };
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
                userEntity.DeviceToken=null;
                userEntity.CityId = loginModel.CityId;
                userEntity.StateId=loginModel.StateId;
                userEntity.CountryCode=loginModel.CountryCode;
                userEntity.RoleId = 12;
                userEntity.IsLogin = true;
                userEntity.AppId = (int?)loginModel.PlayerId;
                userEntity.CreatedDate = DateTime.Now;
                userEntity.IsActive = true;
                userEntity.IsDeleted = false;
                context.UserMasters.Add(userEntity);
                context.SaveChanges();

                var newUserDeviceMapping = new UserDeviceMapping
                {
                    UserId = userEntity.UserId,
                    DeviceToken = loginModel.DeviceToken,
                    CreatedDate = DateTime.Now,
                    CreatedBy= (int?)userEntity.UserId,
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
                return new AuthModel
                {
                    UserId = userEntity.UserId,
                    RoleId = userEntity.RoleId,
                    MobileNo = userEntity.MobileNo,
                    FirstName = userEntity.FirstName,
                    LastName = userEntity.LastName,
                    CountryCode = userEntity.CountryCode,
                    StateId = userEntity.StateId,
                    CityId = userEntity.CityId,
                };
            }
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
        public AuthModel OTPVerify(string otp, int userId, string deviceToken, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var User = context.UserMasters.Include(x => x.UserPlayerMappings).FirstOrDefault(m => m.UserId == userId && m.IsActive == true);
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

                context.SaveChanges();
                var otpEntity = context.Otpautherizations.FirstOrDefault(x => x.Otp == otp && x.UserId == User.UserId && x.IsActive == true);
                if (otpEntity != null)
                {
                    var isValidOtp = DateTime.UtcNow.Subtract(otpEntity.OtpvalidDateTime.Value).TotalMinutes <= 2;
                    if (isValidOtp)
                    {
                        return new AuthModel
                        {
                            UserId = User.UserId,
                            RoleId = User.RoleId,
                            MobileNo = User.MobileNo,
                            CountryCode = User.CountryCode,
                            StateId = User.StateId,
                            CityId = User.CityId,
                            DeviceToken=deviceToken
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
        public UserProfileModel GetMobileUserProfile(int userId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var userEntity = (from user in context.UserMasters
                              join country in context.Countries on user.CountryCode equals country.CountryCode
                              join state in context.States on user.StateId equals state.StateId
                              join city in context.Cities on user.CityId equals city.CityId
                              where user.UserId == userId && user.IsActive == true && user.IsDeleted == false
                              select new UserProfileModel
                              {
                                  UserId=user.UserId,
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
            var existingUser = context.UserDeviceMappings.Where(x => x.UserId == userId && x.DeviceToken==deviceToken).FirstOrDefault();
            if (existingUser != null)
            {
                context.Entry(existingUser).Property(x => x.IsDeleted).IsModified = true;
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
        public LoginStaus LoginStatusOfUser(string Mobile)
        {
            LoginStaus login = new LoginStaus();
            var existinglogin = context.UserMasters.Where(z => z.MobileNo == Mobile && z.IsDeleted == false).FirstOrDefault();
            if (existinglogin != null)
            {
                login.IsLogin = existinglogin.IsLogin;
                login.UserId=existinglogin.UserId;
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
                    _emailSender.SendSmsAsync(user.MobileNo, user.FirstName + " " + user.LastName, otp);
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
    }
}
