using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.Extensions.Options;
using System.ComponentModel.Design;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace IM10.BAL.Implementaion
{
    /// <summary>
    /// This is implementation  for the user operations 
    /// </summary>
    public class UserService : IUserService
    {
        IM10DbContext context;
        private readonly IEmailSenderService _emailSender;
        private ConfigurationModel _url;
        private readonly IAuthService _authService;
        private readonly IUserAuditLogService _userAuditLogService;
        private readonly IEncryptionService _encryptionservice;
        /// <summary>
        /// Creating constructor and injection dbContext
        /// </summary>
        /// <param name="_context"></param>
        public UserService(IM10DbContext _context, IEncryptionService encryptionService, IEmailSenderService emailSender, IOptions<ConfigurationModel> hostName, IAuthService authService, IUserAuditLogService userAuditLogService)
        {
            context = _context;
            _emailSender = emailSender;
            this._url = hostName.Value;
            _authService = authService;
            _userAuditLogService = userAuditLogService;
            _encryptionservice = encryptionService;
        }

        /// <summary>
        /// Method to get user by userId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public UserInformation GetUserById(long userId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var userEntity = (from user in context.UserMasters
                              join
                              role in context.Roles on user.RoleId equals role.RoleId
                              where user.IsDeleted == false && user.UserId == userId
                              orderby user.CreatedDate descending, user.UpdatedDate descending

                              select new UserInformation
                              {
                                 UserId= user.UserId,   
                                 FirstName= user.FirstName,
                                 LastName= user.LastName,
                                 EmailId= _encryptionservice.GetDecryotedId(user.EmailId.ToString()),
                                 MobileNo = _encryptionservice.GetDecryotedId(user.MobileNo.ToString()),
                                 Dob= user.Dob,
                                 RoleId=   user.RoleId,                                
                                 IsActive = user.IsActive,
                                 Password = _encryptionservice.GetDecryotedId(user.Password.ToString()),
                                 Name = role.Name,                                 
                                 FullName = user.FirstName + " " + user.LastName,
                              }).FirstOrDefault();

            if (userEntity == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;

            }
            return userEntity;
        }


        /// <summary>
        /// Method to get all the User
        /// </summary>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<UserModel1> GetAllUser(ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var userEntityList = (from user in context.UserMasters
                                  join
                                  role in context.Roles on user.RoleId equals role.RoleId
                                  where user.IsDeleted == false && role.RoleId != 1 && role.RoleId != 12
                                  && role.IsDeleted==false
                                  let latestDate = user.UpdatedDate.HasValue ? user.UpdatedDate.Value : user.CreatedDate
                                  orderby latestDate descending
                                  select new UserModel1
                                  {
                                     UserId= user.UserId,
                                     FirstName=user.FirstName,
                                     LastName=user.LastName,
                                     EmailId= _encryptionservice.GetDecryotedId( user.EmailId.ToString()),
                                     MobileNo=_encryptionservice.GetDecryotedId(user.MobileNo.ToString()),
                                     RoleId=user.RoleId,
                                     Name=  role.Name,
                                     FullName=user.FirstName + " " + user.LastName,
                                  }).ToList();
            if (userEntityList.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;

            }          
            return userEntityList;
        }

        /// <summary>
        /// Method for saving new User
        /// </summary>
        /// <param name="user"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public string AddUser(UserModel user, ref ErrorResponseModel errorResponseModel)
        {            
            string message = "";
            var userentity = context.UserMasters.Where(x => (x.EmailId == _encryptionservice.GetEncryptedId(user.EmailId.ToString())) && x.IsDeleted == false).FirstOrDefault();
            if (userentity == null)
            {
                var userEntity = new UserMaster();
                userEntity.UserId = user.UserId;
                userEntity.FirstName = user.FirstName;
                userEntity.LastName = user.LastName;
                userEntity.EmailId = _encryptionservice.GetEncryptedId(user.EmailId.ToString());
                userEntity.MobileNo = _encryptionservice.GetEncryptedId(user.MobileNo.ToString());
                userEntity.Dob = user.Dob;
                userEntity.Password = _encryptionservice.GetEncryptedId(user.Password.ToString());
                userEntity.RoleId = user.RoleId;
                userEntity.CityId = user.CityId;
                userEntity.AppId = user.AppId;
                userEntity.CreatedDate = DateTime.Now;
                userEntity.CreatedBy = user.CreatedBy;
               // userEntity.UpdatedDate = DateTime.Now;
                userEntity.IsDeleted = false;
                userEntity.IsActive = true;
                context.UserMasters.Add(userEntity);
                context.SaveChanges();
                message = GlobalConstants.UserSaveMessage;
            }
            else
            {
                if (userentity.EmailId == _encryptionservice.GetEncryptedId(user.EmailId.ToString()))
                {
                    message = "Email already exists";
                }
            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Add User Details";
            userAuditLog.Description = "User details Added Successfully";
            userAuditLog.UserId = (int)user.CreatedBy;
            userAuditLog.CreatedBy = user.CreatedBy;
            userAuditLog.CreatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return message;
        }

        /// <summary>
        /// Method to update user
        /// </summary>
        /// <param name="users"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public string EditUser(UserModel users, ref ErrorResponseModel errorResponseModel)
        {
            var message = "";
            var userEntity = context.UserMasters.Where(x => (x.UserId == users.UserId) && x.IsDeleted == false).FirstOrDefault();
            if (userEntity != null)
            {
                if (userEntity.EmailId != users.EmailId)
                {
                    var userEntity2 = context.UserMasters.Where(x => x.EmailId == _encryptionservice.GetEncryptedId(users.EmailId.ToString()) && x.IsDeleted == false).FirstOrDefault();
                    if (userEntity2 != null)
                    {
                        message = "Email Id Already Exists";
                        return message;
                    }
                }
                userEntity.FirstName = users.FirstName;
                userEntity.LastName = users.LastName;
                userEntity.EmailId = _encryptionservice.GetEncryptedId(users.EmailId.ToString());
                userEntity.MobileNo = _encryptionservice.GetEncryptedId(users.MobileNo.ToString());
                userEntity.Dob = users.Dob;
                userEntity.Password = _encryptionservice.GetEncryptedId(users.Password.ToString());
                userEntity.RoleId = users.RoleId;
                userEntity.CityId = users.CityId;
                userEntity.AppId = users.AppId;
                userEntity.UpdatedDate = DateTime.Now;
                userEntity.UpdatedBy = users.UpdatedBy;
                userEntity.IsDeleted = false;
                userEntity.IsActive = true;
                context.UserMasters.Update(userEntity);
                context.SaveChanges();
                message = GlobalConstants.UserUpdateMessage;
            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Update User";
            userAuditLog.Description = "User Updated Successfully";
            userAuditLog.UserId = (int)users.UpdatedBy;
            userAuditLog.UpdatedBy = users.UpdatedBy;
            userAuditLog.UpdatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return message;
        }


        /// <summary>
        /// Method is used for delete User.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public string DeleteUser(long userId, ref ErrorResponseModel errorResponseModel)
        {
            string Message = "";
            errorResponseModel = new ErrorResponseModel();
            var userEntity = context.UserMasters.FirstOrDefault(x => x.UserId == userId);
            
            if (userEntity != null)
            {
                if (userEntity.IsDeleted == true)
                {
                    errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                    errorResponseModel.Message = "User already deleted.";
                    return null;
                }
                userEntity.IsDeleted = true;
                context.SaveChanges();
                Message = GlobalConstants.UserDeleteMessage;
            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Delete User";
            userAuditLog.Description = "User Deleted Successfully";
            userAuditLog.UserId = (int)userEntity.CreatedBy;
            userAuditLog.CreatedDate = DateTime.Now;
            userAuditLog.CreatedBy = userEntity.CreatedBy;
            userAuditLog.UpdatedBy = userEntity.CreatedBy;
            userAuditLog.UpdatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return "{\"message\": \"" + Message + "\"}";
        }


        /// <summary>
        /// Method is used for forget password.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public string ForgetPassword(string email, ref ErrorResponseModel errorResponseModel)
        {
            string message = "";
            var userEntity = context.UserMasters.Where(x => x.EmailId == _encryptionservice.GetEncryptedId(email.ToString()) && x.IsDeleted==false).FirstOrDefault();

            if (userEntity == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                message = GlobalConstants.EmailNotFound;
            }
            else
            {
                userEntity.Password = _encryptionservice.GetDecryotedId(userEntity.Password.ToString());
                try
                {
                    //string subject = "Forgot password link sent on your email. Please check.";

                    StringBuilder strBody = new StringBuilder();
                    strBody.Append("<body>");
                    strBody.Append("Hello  " + userEntity.FirstName);
                    strBody.Append("<P>Your password for IM10 portal is - </P>");
                    strBody.Append("</body>" + userEntity.Password);
                    var emailModel = new EmailModel();
                    emailModel.ToAddress = email;
                    emailModel.Body = strBody.ToString();
                    emailModel.isHtml = true;
                    emailModel.Subject = GlobalConstants.ForgotPassword;
                    if (!string.IsNullOrEmpty(emailModel.ToAddress))
                    {
                        _emailSender.Execute(emailModel.ToAddress, emailModel.Subject, emailModel.Body);
                    }
                    message = GlobalConstants.ForgotPasswordMessage;
                }
                catch (Exception ex)
                {
                   return ex.Message;
                }

            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = "Forget Password";
            userAuditLog.Description = "Forget Password Successfully";
            userAuditLog.UserId = (int)userEntity.CreatedBy;
            userAuditLog.UpdatedBy = userEntity.UpdatedBy;
            userAuditLog.UpdatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return message;
        }


        /// <summary>
        /// Method to changepassword
        /// </summary>
        /// <param name="userModel"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public string ChangePassword(changepasswordModel userModel, ref ErrorResponseModel errorResponseModel)
        {
            string message = "";
            var userEntity = context.UserMasters.Where(x => x.UserId == userModel.UserId && x.IsDeleted == false).FirstOrDefault();
            userEntity.Password = _encryptionservice.GetEncryptedId(userEntity.Password.ToString());
            if (userEntity != null)
            {
                userEntity.Password = _encryptionservice.GetEncryptedId(userModel.Password.ToString());
                context.UserMasters.Update(userEntity);
                context.SaveChanges();
                message = GlobalConstants.ChangePassword;
            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = "Change Passord";
            userAuditLog.Description = "Password Changed Successfully";
            userAuditLog.UserId = (int)userEntity.CreatedBy;
            userAuditLog.UpdatedBy = userEntity.UpdatedBy;
            userAuditLog.UpdatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return message;
        }


        /// <summary>
        /// Method to get other users
        /// </summary>
        /// <param name=""></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<UserModel1> GetOtherUser(ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var userEntityList = (from user in context.UserMasters
                                  join
                                  role in context.Roles on user.RoleId equals role.RoleId
                                  where user.IsDeleted == false && role.RoleId != 1 && role.RoleId != 12
                                  && role.RoleId != 10 && role.IsDeleted == false
                                  select new UserModel1
                                  {
                                     UserId= user.UserId,
                                     FirstName= user.FirstName,
                                     LastName = user.LastName,
                                     EmailId= _encryptionservice.GetDecryotedId(user.EmailId.ToString()),
                                     MobileNo= _encryptionservice.GetDecryotedId(user.MobileNo.ToString()),
                                     RoleId = user.RoleId,
                                     Name= role.Name,
                                     FullName=user.FirstName + " " + user.LastName,
                                  }).ToList();
            if (userEntityList.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;

            }
            return userEntityList.ToList();
        }


        /// <summary>
        /// Method is used to add mobile user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string AddMobileUser(MobileUserRegisterModel model, ref ErrorResponseModel errorResponseModel)
        {
            var message = string.Empty;
            long userid = 0;
            var existingUser = context.UserMasters.Where(x => x.MobileNo == model.MobileNo && x.IsDeleted == false).FirstOrDefault();
            if (existingUser != null)
            {
                message = GlobalConstants.ExistingUserMessage;
                userid = existingUser.UserId;
            }
            else
            {
                var userEntity = new UserMaster();
                userEntity.Username = null;
                userEntity.FirstName = "Mobile User";
                userEntity.LastName = "Mobile User";
                userEntity.EmailId = "mobileuser@gmail.com";
                userEntity.MobileNo = model.MobileNo;
                userEntity.Dob = DateTime.Now;
                userEntity.Password = "123456";
                userEntity.RoleId = 12;
                userEntity.CreatedBy = model.CreatedBy;
                userEntity.CreatedDate = DateTime.Now;
                userEntity.UpdatedDate=DateTime.Now;
                userEntity.IsActive = true;
                userEntity.IsDeleted = false;
                context.UserMasters.Add(userEntity);
                context.SaveChanges();
                Random generator = new Random();
                string otp = _authService.CreateNewOTP((userEntity.UserId));
              
                if (!string.IsNullOrEmpty(userEntity.MobileNo))
                {
                    _emailSender.SendSmsAsync(userEntity.MobileNo,userEntity.FirstName + " " + userEntity.LastName,otp);
                }
                message = GlobalConstants.MobileUserSaveMessage;
                userid = userEntity.UserId;
            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Add Mobile User";
            userAuditLog.Description = "Mobile User Added Successfully";
            userAuditLog.UserId = (int)model.CreatedBy;
            userAuditLog.CreatedBy = model.CreatedBy;
            userAuditLog.CreatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return "{\"message\": \"" + message + "\",\"UserId\": \"" + userid + "\"}";

        }

        public string ActivateUser(ActiveUserModel model, ref ErrorResponseModel errorResponseModel)
        {
            string message = "";
            var appuserEntity = context.UserMasters.FirstOrDefault(x=>x.UserId == model.UserId && x.IsDeleted == false);
            var otpEntity = context.Otpautherizations.FirstOrDefault(x => x.UserId == model.UserId && x.Otp == model.Otp);

            if (appuserEntity == null || otpEntity==null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            else
            {

                appuserEntity.IsActive = true;
                appuserEntity.UpdatedDate = DateTime.Now;
                appuserEntity.UpdatedBy = model.UpdatedBy;
                context.UserMasters.Update(appuserEntity);
                context.SaveChanges();
                message = GlobalConstants.UserActivateSuccessfully;
            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Activate Mobile User";
            userAuditLog.Description = "Mobile User Activated Successfully";
            userAuditLog.UserId = (int)appuserEntity.CreatedBy;
            userAuditLog.UpdatedBy = appuserEntity.UpdatedBy;
            userAuditLog.UpdatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return message;
        }
    }
}
