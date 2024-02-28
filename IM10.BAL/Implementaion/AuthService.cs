using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            var userEntity = context.UserMasters.FirstOrDefault(x => x.EmailId == emailId && x.Password == EncryptionHelper.Encrypt(password.ToString()) && x.IsActive == true);

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
            var user = context.UserMasters.FirstOrDefault(x => x.MobileNo == loginModel.MobileNo && x.IsActive==true && x.IsDeleted == false);
            
            if (user != null)
            {
                var existingUser = context.UserMasters.FirstOrDefault(x => x.MobileNo == loginModel.MobileNo && x.IsLogin == false && x.IsDeleted == false);
                {
                    context.Entry(existingUser).Property(x => x.IsLogin).IsModified = true;
                    context.Entry(existingUser).Property(x => x.DeviceToken).IsModified = true;

                    // Update values
                    existingUser.IsLogin = true;
                    existingUser.DeviceToken = loginModel.DeviceToken;
                    context.SaveChanges();
                }
                var fcmexstingUser = context.Fcmnotifications.Where(x => x.DeviceToken == loginModel.DeviceToken && x.PlayerId == loginModel.PlayerId).FirstOrDefault();
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
                    //_emailSender.SendTwilioSmsAsync(user.MobileNo, user.FirstName + " " + user.LastName, otp);
                    var userComment=context.Comments.Where(x=>x.UserId== user.UserId && x.ParentCommentId==null).OrderByDescending(x => x.CreatedDate).ToList();
                    if (userComment.Count > 0)
                    {
                        var commentids = string.Join(",", userComment.Where(p => p.IsDeleted == false)
                                 .Select(p => p.CommentId.ToString()));                 
                        IEnumerable<long> ids = commentids.Split(',').Select(str => long.Parse(str));
                         context.Comments.Where(rec => ids.Contains(rec.CommentId)).ToList().ForEach(x => x.DeviceId = loginModel.DeviceToken);
                        context.SaveChanges();
                    }
                }
                return new AuthModel
                {
                    UserId = user.UserId,
                    RoleId = user.RoleId,
                    MobileNo = user.MobileNo,

                };
            }
            else
            {
                var userEntity = new UserMaster();
                userEntity.Username = null;
                userEntity.FirstName = "Mobile User";
                userEntity.LastName = "Mobile User";
                userEntity.EmailId = "mobileuser@gmail.com";
                userEntity.MobileNo = loginModel.MobileNo;
                userEntity.Dob = DateTime.Now;
                userEntity.Password = "123456";
                userEntity.DeviceToken=loginModel.DeviceToken;
                userEntity.CountryCode=loginModel.CountryCode;
                userEntity.RoleId = 12;
                userEntity.IsLogin = true;
                userEntity.AppId = (int?)loginModel.PlayerId;
                userEntity.CreatedDate = DateTime.Now;
                userEntity.IsActive = true;
                userEntity.IsDeleted = false;
                context.UserMasters.Add(userEntity);
                context.SaveChanges();
                Random generator = new Random();
                string otp = CreateNewOTP((userEntity.UserId));

                if (!string.IsNullOrEmpty(userEntity.MobileNo))
                {
                    _emailSender.SendSmsAsync(userEntity.MobileNo, userEntity.FirstName + " " + userEntity.LastName, otp);
                    //_emailSender.SendTwilioSmsAsync(userEntity.MobileNo, userEntity.FirstName + " " + userEntity.LastName, otp);
                }
                return new AuthModel
                {
                    UserId = userEntity.UserId,
                    RoleId = userEntity.RoleId,
                    MobileNo = userEntity.MobileNo,
                    DeviceToken=userEntity.DeviceToken,
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
                    existingRecord.OtpvalidDateTime = DateTime.UtcNow;
                    context.SaveChanges();
                }
                else
                {
                    otpEntity.Otp = otp;
                    otpEntity.OtpvalidDateTime = DateTime.UtcNow;
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
        public AuthModel OTPVerify(string otp, int userId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var User = context.UserMasters.Include(x => x.UserPlayerMappings).FirstOrDefault(m => m.UserId == userId && m.IsActive == true);
            if (User != null)
            {
                var otpEntity = context.Otpautherizations.FirstOrDefault(x => x.Otp == otp && x.UserId == User.UserId && x.IsActive == true);
                if (otpEntity != null)
                {
                    var isValidOtp = DateTime.UtcNow.Subtract(otpEntity.OtpvalidDateTime.Value).TotalMinutes <= 5;
                    if (isValidOtp)
                    {
                        return new AuthModel
                        {
                            UserId = User.UserId,
                            RoleId = User.RoleId,
                            MobileNo = User.MobileNo,
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
        public AuthModel GetMobileUserProfile(int userId, ref ErrorResponseModel errorResponseModel)
        {
            var authModel = new AuthModel();
            errorResponseModel = new ErrorResponseModel();
            var user = context.UserMasters.FirstOrDefault(x => x.UserId == userId && x.IsActive == true && x.IsDeleted == false);
            if (user != null)
            {
                authModel.UserId = user.UserId;
                authModel.RoleId = user.RoleId;
                authModel.MobileNo = user.MobileNo;
            }
            else
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = "User Not Found";
                return null;
            }
            return authModel;
        }


        /// <summary>
        /// method for logout user 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string MobileLogOut(long userId, ref ErrorResponseModel errorResponseModel)
        {
            var userModel = new UserMaster();
            string message = "";
            var existingUser = context.UserMasters.Where(x => x.UserId == userId ).FirstOrDefault();
            if (existingUser != null)
            {
                context.Entry(existingUser).Property(x => x.IsLogin).IsModified = true;
                context.Entry(existingUser).Property(x => x.DeviceToken).IsModified = true;

                // Update values
                existingUser.IsLogin = false;
                existingUser.DeviceToken = null;
               // context.UserMasters.Update(userModel);
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
    }
}
