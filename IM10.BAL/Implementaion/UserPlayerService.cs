using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace IM10.BAL.Implementaion
{
    /// <summary>
    /// This is implementation for the userplayer operations 
    /// </summary>
    public class UserPlayerService : IUserPlayerService
    {
        IM10DbContext _context;
        private ConfigurationModel _configuration;
        private readonly IUserAuditLogService _userAuditLogService;
        /// <summary>
        /// Creating constructor and injection dbContext
        /// </summary>
        /// <param name="_context"></param>
        public UserPlayerService(IM10DbContext context, IOptions<ConfigurationModel> hostName, IUserAuditLogService userAuditLogService)
        {
            _context = context;
            this._configuration = hostName.Value;
            _userAuditLogService = userAuditLogService;
        }

        /// <summary>
        /// Method to get userplayer by userplayerId
        /// </summary>
        /// <param name="userplayerId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public UserPlayerModel GetUserPlayerById(long userplayerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var userplayerEntity = (from userplayer in _context.UserPlayerMappings
                                    join
                                    user in _context.UserMasters on userplayer.UserId equals user.UserId
                                    join player in _context.PlayerDetails on userplayer.PlayerId equals player.PlayerId
                                    where userplayer.UserPlayerId == userplayerId && userplayer.IsDeleted == false
                                    && player.IsDeleted==false
                                    select new
                                    {
                                        userplayer.UserPlayerId,
                                        userplayer.UserId,
                                        userplayer.PlayerId,
                                        userplayer.CreatedDate,
                                        userplayer.CreatedBy,
                                        userplayer.UpdatedDate,
                                        userplayer.UpdatedBy,
                                        user.EmailId,
                                        player.FirstName,
                                        player.LastName,
                                    }).FirstOrDefault();
                                      
            if (userplayerEntity==null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;

            }

            return new UserPlayerModel
            {
                UserPlayerId = userplayerEntity.UserPlayerId,
                UserId = userplayerEntity.UserId,
                FullName =userplayerEntity.FirstName + " " + userplayerEntity.LastName,
                EmailId= userplayerEntity.EmailId,
                PlayerId = userplayerEntity.PlayerId,
                CreatedDate = DateTime.Now,
                CreatedBy = userplayerEntity.CreatedBy,
                UpdatedDate = DateTime.Now,
                UpdatedBy = userplayerEntity.UpdatedBy,
                FirstName = userplayerEntity.FirstName,
                LastName = userplayerEntity.LastName,
            };
           
        }

        /// <summary>
        /// Method to get all userplayer 
        /// </summary>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<UserPlayerModel> GetAllUserPlayer(ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var userplyerentityList = (from userplayer in _context.UserPlayerMappings
                                       join
                                       user in _context.UserMasters on userplayer.UserId equals user.UserId
                                       join player in _context.PlayerDetails on userplayer.PlayerId equals player.PlayerId
                                       join role in _context.Roles on user.RoleId equals role.RoleId 
                                       where userplayer.IsDeleted == false && role.IsDeleted == false
                                       && player.IsDeleted == false
                                       orderby userplayer.UserId , userplayer.UpdatedDate descending
                                       select new UserPlayerModel
                                       {
                                         UserPlayerId=  userplayer.UserPlayerId,
                                         UserId=  userplayer.UserId,
                                         PlayerId = userplayer.PlayerId,
                                         RoleId=user.RoleId,
                                         RoleName=role.Name,
                                         UserFirstName=user.FirstName,
                                         UserLastName=user.LastName,
                                         UserFullName=user.FirstName + " " + user.LastName,
                                         FullName=player.FirstName + " " + player.LastName,
                                         FirstName = player.FirstName,
                                         LastName= player.LastName,
                                       }).ToList();

            if (userplyerentityList.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;             
            }
            return userplyerentityList.ToList();
        }


        /// <summary>
        /// Method to save userplayer
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public string AddUserPlayer( UserPlayerModel1 model, ref ErrorResponseModel errorResponseModel)
        {
            string message = "";
            var userplayerEntity = new UserPlayerMapping();
            var userplayerIds = model.UserPlayerIds.Split(',');
            var uncheckedplayerIds=model.UncheckedPlayerIds.Split(',');

            if (model.UserPlayerId == 0)
            {
                var userRolId = (from user in _context.UserMasters where user.UserId == model.UserId select user.RoleId).FirstOrDefault();
                int roleid = Convert.ToInt32(userRolId);
                var roleEntity = (from userplayer in _context.UserPlayerMappings
                                  join user in _context.UserMasters on userplayer.UserId equals user.UserId
                                  where user.RoleId == roleid && userplayer.IsDeleted == false
                                  select userplayer).ToList();

                var playerIds=(from player in roleEntity select player.PlayerId).ToList();

                    foreach (var item in userplayerIds)
                    {
                        var playerEntity = _context.UserPlayerMappings.Where(x => x.UserId == model.UserId && x.PlayerId == Convert.ToInt32(item) && x.IsDeleted == false).ToList();
                        if (!playerEntity.Any())
                        {
                            if (!playerIds.Contains(Convert.ToInt32(item)))
                            {
                                userplayerEntity.UserPlayerId = model.UserPlayerId;
                                userplayerEntity.UserId = model.UserId;
                                userplayerEntity.PlayerId = Convert.ToInt32(item);
                                userplayerEntity.CreatedDate = DateTime.Now;
                                userplayerEntity.CreatedBy = model.CreatedBy;
                                userplayerEntity.UpdatedDate= DateTime.Now;
                                userplayerEntity.IsDeleted = false;
                                _context.UserPlayerMappings.Add(userplayerEntity);
                                _context.SaveChanges();
                                message = GlobalConstants.UserPlayerSavedSuccessfully;
                            }

                            
                        }
                    }
                    if (uncheckedplayerIds.Length > 1)
                    {
                        foreach (var item in uncheckedplayerIds)
                        {
                            var playerEntity = _context.UserPlayerMappings.Where(x => x.UserId == model.UserId && x.PlayerId == Convert.ToInt32(item) && x.IsDeleted == false).FirstOrDefault();
                            if (playerEntity != null)
                            {
                                playerEntity.UpdatedDate = DateTime.Now;
                                playerEntity.UpdatedBy = model.UpdatedBy;
                                playerEntity.IsDeleted = true;
                                _context.SaveChanges();
                                message = GlobalConstants.UserPlayerUpdateMessage;
                            }
                        }
                    }               
            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Add User Player Mapping Details";
            userAuditLog.Description = "User Player Mapping Details Added";
            userAuditLog.UserId = (int)model.CreatedBy;
            userAuditLog.CreatedBy = model.CreatedBy;
            userAuditLog.CreatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return message;
        }

        public string EditUserPlayer(UserPlayerMappingModel playerModel, ref ErrorResponseModel errorResponseModel)
        {
            var message = "";
            var userplayerEntity = new UserPlayerMapping();
            var uncheckedplayerIds = playerModel.UncheckedPlayerIds.Split(',');


            if (playerModel.UserId != 0 && !string.IsNullOrEmpty(playerModel.UncheckedPlayerIds))
            {
                foreach (var item in uncheckedplayerIds)
                {
                    var playerId = Convert.ToInt32(item);
                    var playerEntity = _context.UserPlayerMappings.FirstOrDefault(x => x.UserId == playerModel.UserId && x.PlayerId == playerId && x.IsDeleted == false);

                    if (playerEntity != null)
                    {
                        playerEntity.UpdatedBy = playerModel.UpdatedBy;
                        playerEntity.UpdatedDate = DateTime.Now;
                        playerEntity.IsDeleted = true;
                        _context.SaveChanges();
                        message = GlobalConstants.UserPlayerUpdateMessage;
                    }
                    else
                    {
                        message = GlobalConstants.AlreadySelectedplayer;
                    }
                }
            }
            else
            {
                message = GlobalConstants.AlreadyExists;
            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Update User Player Mapping Details";
            userAuditLog.Description = "User Player Mapping Details Update";
            userAuditLog.UserId = (int)playerModel.UpdatedBy;
            userAuditLog.UpdatedBy = playerModel.UpdatedBy;
            userAuditLog.UpdatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return message;
        }




        /// <summary>
        /// Method to delete userplayer
        /// </summary>
        /// <param name="userplayerId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public string DeleteUserPlayer(long userplayerId, ref ErrorResponseModel errorResponseModel)
        {
            string Message = "";
            var userplayerEntity = _context.UserPlayerMappings.FirstOrDefault(x => x.UserPlayerId == userplayerId);
            if (userplayerEntity != null)
            {
                if (userplayerEntity.IsDeleted == true)
                {
                    errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                    errorResponseModel.Message = "User player already deleted.";
                    return null;
                }
                userplayerEntity.IsDeleted = true;
                _context.SaveChanges();
                Message = GlobalConstants.UserPlayerDeleteMessage;
            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Delete User Player Mapping Details";
            userAuditLog.Description = "User Player Mapping Details deleted";
            userAuditLog.UserId = (int)userplayerEntity.CreatedBy;
            userAuditLog.UpdatedBy = userplayerEntity.UpdatedBy;
            userAuditLog.UpdatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return "{\"message\": \"" + Message + "\"}";
        }        


         /// <summary>
        /// Method to get player by userId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<SportUserPlayerModel> GetPlayerByUserId(long userId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var userList=new List<SportUserPlayerModel>();
            var userplayerEntityList = (from userplayer in _context.UserPlayerMappings
                                    join
                                    user in _context.UserMasters on userplayer.UserId equals user.UserId
                                    join player in _context.PlayerDetails on userplayer.PlayerId equals player.PlayerId
                                    join sport in _context.SportMasters on player.SportId equals sport.SportId
                                    where userplayer.UserId== userId  && userplayer.IsDeleted == false
                                    && player.IsDeleted == false
                                    orderby userplayer.UpdatedDate descending
                                    select new
                                    {
                                        userplayer.UserPlayerId,
                                        userplayer.UserId,
                                        userplayer.PlayerId,
                                        user.EmailId,
                                        player.FirstName,
                                        player.LastName,
                                        player.ProfileImageFileName,
                                        player.ProfileImageFilePath,
                                        player.SportId,
                                        sport.SportName,
                                        player.Dob
                                    }).ToList();

            if (userplayerEntityList.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;
            }

            userplayerEntityList.ForEach(item=>
                {
                    var imgmodel = new VideoImageModel();
                    imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(item.ProfileImageFilePath) ? item.ProfileImageFilePath : item.ProfileImageFilePath);
                    imgmodel.Type = String.IsNullOrEmpty(item.ProfileImageFilePath) ? "image" : "image";
                    // imgModel.thumbnail = _configuration.HostName.TrimEnd('/') + "/thumbnail/" + imgModel.url
                    imgmodel.FileName = imgmodel.url;
                    userList.Add(new SportUserPlayerModel
                    {
                        UserPlayerId = item.UserPlayerId,
                        UserId = item.UserId,
                        PlayerId = item.PlayerId,
                        EmailId = item.EmailId,
                        FullName =item.FirstName + " " + item.LastName,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        ProfileImageFileName=item.ProfileImageFileName,
                        ProfileImageFilePath=imgmodel.FileName,
                        SportId = item.SportId,
                        SportName = item.SportName,
                        Dob=item.Dob,
                    });
            });
            return userList;
        }

        public UserPlayerModel2 GetuserplayerByUserplayerId(long userplayerId, ref ErrorResponseModel errorResponseModel)
        {
            string message = "";
            var list = new List<UserPlayerModel2>();
            var userEntity=_context.UserPlayerMappings.Where(x=>x.UserPlayerId== userplayerId && x.IsDeleted==false).FirstOrDefault();
            var playerEntity = _context.UserPlayerMappings.Include(x=>x.Player).Where(x => x.UserId == userEntity.UserId && x.IsDeleted==false).ToList();

            if (userEntity == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;
            }

            UserPlayerModel2 userPlayerModel = new UserPlayerModel2();
            List<long> playerList= new List<long>();
            List<string> playerList1 = new List<string>(); 

            userPlayerModel.UserId = userEntity.UserId;
            playerEntity.ForEach(item =>
            {
                playerList.Add(item.PlayerId);
                playerList1.Add(item.Player.FirstName + " " + item.Player.LastName);
            });
            userPlayerModel.lstPlayers = playerList;
            userPlayerModel.lstPlayer = playerList1;
            return userPlayerModel;
        }

        public List<UserPlayerModel3> GetAllUserPlayerdetailsbycommaseparate(ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var userplayerList = new List<UserPlayerModel3>();

            var userplyerentityList = (from userplayer in _context.UserPlayerMappings
                                       join
                                       user in _context.UserMasters on userplayer.UserId equals user.UserId
                                       join player in _context.PlayerDetails on userplayer.PlayerId equals player.PlayerId
                                       where userplayer.IsDeleted == false && player.IsDeleted==false
                                       select new
                                       {
                                           userplayer.UserPlayerId,
                                           userplayer.UserId,
                                           userplayer.PlayerId,                                          
                                           user.EmailId,
                                           player.FirstName,
                                           player.LastName,
                                       }).ToList();
            if (userplyerentityList.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            var groupedUserPlayerEntities = userplyerentityList.GroupBy(x => x.UserId);

            foreach (var group in groupedUserPlayerEntities)
            {
                var playerIds = string.Join(" , ", group.Select(x => x.PlayerId));
                var fullNames = string.Join(" , ", group.Select(x => $"{x.FirstName} {x.LastName}"));

                userplayerList.Add(new UserPlayerModel3
                {
                    UserPlayerId = group.First().UserPlayerId,
                    UserId = group.Key,
                    PlayerIds = playerIds,
                    EmailId = group.First().EmailId,
                    FullName = fullNames,
                });
            }
            return userplayerList;
        }

        public List<UserPlayerModel> GetPlayerByRoleId(long roleId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var userList = new List<UserPlayerModel>();
            var result1 = from pd in _context.PlayerDetails
                         where !_context.UserPlayerMappings
                                .Join(_context.UserMasters,
                                      upm => upm.UserId,
                                      um => um.UserId,
                                      (upm, um) => new
                                      { upm.PlayerId, um.RoleId, upm.IsDeleted })
                                .Any(joined => joined.RoleId == roleId && joined.IsDeleted == false &&
                                               joined.PlayerId == pd.PlayerId)
                                           && pd.IsDeleted == false
                                     select pd;
            var result = result1.ToList();

            if (result.Count== 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;
            }

            result.ForEach(item =>
            {
                var imgmodel = new VideoImageModel();
                imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(item.ProfileImageFilePath) ? item.ProfileImageFilePath : item.ProfileImageFilePath);
                imgmodel.Type = String.IsNullOrEmpty(item.ProfileImageFilePath) ? "image" : "image";
                imgmodel.FileName = imgmodel.url;

                userList.Add(new UserPlayerModel
                {
                    PlayerId = item.PlayerId,
                    FullName = item.FirstName + " " + item.LastName,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    ProfileImageFileName = item.ProfileImageFileName,
                    ProfileImageFilePath = imgmodel.FileName,
                });
            });
            return userList;
        }
    }
      
}
