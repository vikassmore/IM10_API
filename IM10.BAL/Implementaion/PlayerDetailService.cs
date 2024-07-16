using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using StartUpX.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Implementaion
{
    /// <summary>
    /// This is implementation for the playerDetails operations 
    /// </summary>
    public class PlayerDetailService : IPlayerDetailService
    {
        IM10DbContext context;
        private ConfigurationModel _configuration;
        private readonly IUserAuditLogService _userAuditLogService;
        /// <summary>
        /// Creating constructor and injection dbContext
        /// </summary>
        /// <param name="context_"></param>

        public PlayerDetailService(IM10DbContext context_, IOptions<ConfigurationModel> hostName, IUserAuditLogService userAuditLogService)
        {
            context = context_;
            this._configuration = hostName.Value;
            _userAuditLogService = userAuditLogService;
        }

        /// <summary>
        /// Method to get playerDetails by PlayerId
        /// </summary>
        /// <param name="PlayerId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public PlayerSportsModel GetPlayerDetailById(long playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var playerEntity = (from player in context.PlayerDetails
                                join sport in context.SportMasters on player.SportId equals sport.SportId
                                where player.PlayerId == playerId && player.IsDeleted == false
                                select new
                                {

                                    player.PlayerId,
                                    player.FirstName,
                                    player.LastName,
                                    player.SportId,
                                    player.ProfileImageFileName,
                                    player.ProfileImageFilePath,
                                    sport.SportName,
                                    player.Dob,
                                }).FirstOrDefault();

            if (playerEntity == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;
            }
            var imgmodel = new VideoImageModel();
            imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(playerEntity.ProfileImageFilePath) ? playerEntity.ProfileImageFilePath : playerEntity.ProfileImageFilePath);
            imgmodel.Type = String.IsNullOrEmpty(playerEntity.ProfileImageFilePath) ? "image" : "image";
            imgmodel.FileName = imgmodel.url;
           
            return new PlayerSportsModel
            {
                PlayerId = playerEntity.PlayerId,
                PlayerName = playerEntity.FirstName + " " + playerEntity.LastName,
                FirstName = playerEntity.FirstName,
                LastName = playerEntity.LastName,
                SportId = playerEntity.SportId,
                SportName = playerEntity.SportName,
                FullName = playerEntity.FirstName + " " + playerEntity.LastName,
                ProfileImageFileName = playerEntity.ProfileImageFileName,
                ProfileImageFilePath = imgmodel.FileName,
                Dob = playerEntity.Dob
             };
        }

        /// <summary>
        /// Method to get all playerDetails
        /// </summary>
        /// <param name=""></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<PlayerDetailModel> GetAllPlayerDetail(ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var playerlist = new List<PlayerDetailModel>();
            var playerEntity = context.PlayerDetails.Where(x => x.IsDeleted == false).OrderByDescending(x => x.UpdatedDate).ToList();
            if (playerEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;
            }
            playerEntity.ForEach(item =>
            {
                var imgmodel = new VideoImageModel();
                imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(item.ProfileImageFilePath) ? item.ProfileImageFilePath : item.ProfileImageFilePath);
                imgmodel.Type = String.IsNullOrEmpty(item.ProfileImageFilePath) ? "image" : "image";
                // imgModel.thumbnail = _configuration.HostName.TrimEnd('/') + "/thumbnail/" + imgModel.url
                imgmodel.FileName = imgmodel.url;

                var aadharimg = new VideoImageModel();
                aadharimg.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(item.AadharCardFilePath) ? item.AadharCardFilePath : item.AadharCardFilePath.Replace("\r\n", ""));
                aadharimg.Type = String.IsNullOrEmpty(item.AadharCardFilePath) ? "image" : "image";
                aadharimg.FileName = aadharimg.url;

                var panimg = new VideoImageModel();
                panimg.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(item.PanCardFilePath) ? item.PanCardFilePath : item.PanCardFilePath.Replace("\r\n", ""));
                panimg.Type = String.IsNullOrEmpty(item.PanCardFilePath) ? "image" : "image";
                panimg.FileName = panimg.url;

                var votingimg = new VideoImageModel();
                votingimg.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(item.VotingCardFilePath) ? item.VotingCardFilePath : item.VotingCardFilePath);
                votingimg.Type = String.IsNullOrEmpty(item.VotingCardFilePath) ? "image" : "image";
                votingimg.FileName = votingimg.url;

                var licenseimg = new VideoImageModel();
                licenseimg.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(item.DrivingLicenceFilePath) ? item.DrivingLicenceFilePath : item.DrivingLicenceFilePath.Replace("\r\n", ""));
                licenseimg.Type = String.IsNullOrEmpty(item.DrivingLicenceFilePath) ? "image" : "image";
                licenseimg.FileName = licenseimg.url;

                playerlist.Add(new PlayerDetailModel
                {
                    PlayerId = item.PlayerId,
                    PlayerName = item.FirstName + " " + item.LastName,
                    AadharCardFileName = item.AadharCardFileName,
                    AadharCardFilePath = aadharimg.FileName,
                    PanCardFileName = item.PanCardFileName,
                    PanCardFilePath = panimg.FileName,
                    VotingCardFileName = item.VotingCardFileName,
                    VotingCardFilePath = votingimg.FileName,
                    DrivingLicenceFileName = item.DrivingLicenceFileName,
                    DrivingLicenceFilePath = licenseimg.FileName,
                    BankAcountNo = item.BankAcountNo,
                    PancardNo = item.PancardNo,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    FullName = item.FirstName + " " + item.LastName,
                    Address = item.Address,
                    ProfileImageFileName = item.ProfileImageFileName,
                    ProfileImageFilePath = imgmodel.FileName,
                    Dob=item.Dob,
                });
            });
            return playerlist;
        }

        /// <summary>
        /// Method to add playerDetails
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public string AddPlayerDetail(PlayerDetailModel model, ref ErrorResponseModel errorResponseModel)
        {

            string message = "";
            if (model.PlayerId == 0)
            {
                var existingPlayerEntity = context.PlayerDetails.FirstOrDefault(x => x.PancardNo == model.PancardNo && x.IsDeleted == false);
                if (existingPlayerEntity == null) 
                {
                    var playerEntity = new PlayerDetail();
                    playerEntity.PlayerId = model.PlayerId;
                    playerEntity.AadharCardFilePath = model.AadharCardFilePath;
                    playerEntity.AadharCardFileName = model.AadharCardFileName;
                    playerEntity.PanCardFileName = model.PanCardFileName;
                    playerEntity.PanCardFilePath = model.PanCardFilePath;
                    playerEntity.VotingCardFileName = model.VotingCardFileName;
                    playerEntity.VotingCardFilePath = model.VotingCardFilePath;
                    playerEntity.DrivingLicenceFileName = model.DrivingLicenceFileName;
                    playerEntity.DrivingLicenceFilePath = model.DrivingLicenceFilePath;
                    playerEntity.BankAcountNo = model.BankAcountNo;
                    playerEntity.PancardNo = model.PancardNo;
                    playerEntity.FirstName = model.FirstName;
                    playerEntity.LastName = model.LastName;
                    playerEntity.SportId = model.SportId;
                    playerEntity.Address = model.Address;
                    playerEntity.Dob = model.Dob;
                    playerEntity.ProfileImageFileName = model.ProfileImageFileName;
                    playerEntity.ProfileImageFilePath = model.ProfileImageFilePath;
                    playerEntity.CreatedBy = model.CreatedBy;
                    playerEntity.CreatedDate = DateTime.Now;
                    playerEntity.UpdatedDate = DateTime.Now;
                    playerEntity.IsDeleted = false;
                    context.PlayerDetails.Add(playerEntity);
                    context.SaveChanges();
                    message = GlobalConstants.PlayerDetailAddSuccessfully;
                }
                else
                {
                    errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                    message = "player already exist with same pan card number";
                }
            }

            else
            {
                message = GlobalConstants.AlreadyExists;
            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Add Player Details";
            userAuditLog.Description = "Player Details Added Successfully";
            userAuditLog.UserId = (int)model.CreatedBy;
            userAuditLog.CreatedBy = model.CreatedBy;
            userAuditLog.CreatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return message;
        }

        /// <summary>
        /// Method to edit playerDetails 
        /// </summary>
        /// <param name="playerModel"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public string EditPlayerDetail(PlayerDetailModel playerModel, ref ErrorResponseModel errorResponseModel)
        {
            var message = "";
            var playerEntity = context.PlayerDetails.Where(x => x.PlayerId == playerModel.PlayerId).FirstOrDefault();
            if (playerEntity != null)
            {
                var existingPlayerEntity = context.PlayerDetails.FirstOrDefault(x => x.PancardNo == playerModel.PancardNo && x.IsDeleted == false);
                if (existingPlayerEntity == null)
                {
                    playerEntity.PlayerId = playerModel.PlayerId;
                    if (playerModel.AadharCardFileName != null)
                    {
                        playerEntity.AadharCardFileName = playerModel.AadharCardFileName;
                    }

                    if (playerModel.AadharCardFilePath != null)
                    {
                        playerEntity.AadharCardFilePath = playerModel.AadharCardFilePath;
                    }

                    if (playerModel.PanCardFileName != null)
                    {
                        playerEntity.PanCardFileName = playerModel.PanCardFileName;
                    }

                    if (playerModel.PanCardFilePath != null)
                    {
                        playerEntity.PanCardFilePath = playerModel.PanCardFilePath;
                    }

                    if (playerModel.VotingCardFileName != null)
                    {
                        playerEntity.VotingCardFileName = playerModel.VotingCardFileName;
                    }

                    if (playerModel.VotingCardFilePath != null)
                    {
                        playerEntity.VotingCardFilePath = playerModel.VotingCardFilePath;
                    }

                    if (playerModel.DrivingLicenceFileName != null)
                    {
                        playerEntity.DrivingLicenceFileName = playerModel.DrivingLicenceFileName;
                    }

                    if (playerModel.DrivingLicenceFilePath != null)
                    {
                        playerEntity.DrivingLicenceFilePath = playerModel.DrivingLicenceFilePath;
                    }

                    if (playerModel.ProfileImageFileName != null)
                    {
                        playerEntity.ProfileImageFileName = playerModel.ProfileImageFileName;
                    }

                    if (playerModel.ProfileImageFilePath != null)
                    {
                        playerEntity.ProfileImageFilePath = playerModel.ProfileImageFilePath;
                    }

                    playerEntity.BankAcountNo = playerModel.BankAcountNo;
                    playerEntity.PancardNo = playerModel.PancardNo;
                    playerEntity.FirstName = playerModel.FirstName;
                    playerEntity.LastName = playerModel.LastName;
                    playerEntity.SportId = playerModel.SportId;
                    playerEntity.Dob = playerModel.Dob;
                    playerEntity.Address = playerModel.Address;
                    playerEntity.UpdatedBy = playerModel.UpdatedBy;
                    playerEntity.UpdatedDate = DateTime.Now;
                    playerEntity.IsDeleted = false;
                    context.PlayerDetails.Update(playerEntity);
                    context.SaveChanges();
                    message = GlobalConstants.PlayerDetailUpdateSuccessfully;
                }
                else
                {
                    errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                    message = "player already exist with same pan card number";
                }
            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Update Player Details";
            userAuditLog.Description = "Player Details Updated Successfully";
            userAuditLog.UserId = (int)playerModel.UpdatedBy;
            userAuditLog.UpdatedBy = playerModel.UpdatedBy;
            userAuditLog.UpdatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return message;
        }

        /// <summary>
        /// Method to delete playerDetails
        /// </summary>
        /// <param name="PlayerId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public string DeletePlayerDetail(long PlayerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            string Message = "";
            var playerEntity = context.PlayerDetails.FirstOrDefault(x => x.PlayerId == PlayerId);
            if (playerEntity != null)
            {
                if (playerEntity.IsDeleted == true)
                {
                    errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                    errorResponseModel.Message = "Player already deleted.";
                    return null;
                }
                playerEntity.IsDeleted = true;
                context.SaveChanges();
                Message = GlobalConstants.PlayerDetailDeleteSuccessfully;
            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Delete Player Details";
            userAuditLog.Description = "Player Details Deleted Successfully";
            userAuditLog.UserId = (int)playerEntity.CreatedBy;
            userAuditLog.CreatedDate = DateTime.Now;
            userAuditLog.CreatedBy = playerEntity.CreatedBy;
            userAuditLog.UpdatedBy = playerEntity.CreatedBy;
            userAuditLog.UpdatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return "{\"message\": \"" + Message + "\"}";
        }

        /// <summary>
        /// Method is used to add player data
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public string AddPlayerData(List<PlayerDataModel> model, ref ErrorResponseModel errorResponseModel)
        {

            string message = "";
            foreach (var item in model)
            {
                var playerDataEntity = context.PlayerData.FirstOrDefault(x => x.PlayerDataId == item.PlayerDataId && x.PlayerId == item.PlayerId && x.FileCategoryTypeId == item.FileCategoryTypeId && x.IsDeleted == false);
                if (playerDataEntity == null)
                {
                    var playerDataEntity1 = new PlayerData();
                    playerDataEntity1.PlayerId = item.PlayerId;
                    playerDataEntity1.FileName = item.FileName;
                    playerDataEntity1.FilePath = item.FilePath;
                    playerDataEntity1.FileCategoryTypeId = item.FileCategoryTypeId;
                    playerDataEntity1.CreatedBy = item.CreatedBy;
                    playerDataEntity1.CreatedDate = DateTime.Now;
                    playerDataEntity1.IsDeleted = false;
                    context.PlayerData.Add(playerDataEntity1);
                }
                context.SaveChanges();
                message = GlobalConstants.AddPlayerDetaMessage;
                var userAuditLog = new UserAuditLogModel();
                userAuditLog.Action = " Add Player Data";
                userAuditLog.Description = "Player Data Added Successfully";
                userAuditLog.UserId = (int)item.CreatedBy;
                userAuditLog.CreatedBy = item.CreatedBy;
                userAuditLog.CreatedDate = DateTime.Now;
                _userAuditLogService.AddUserAuditLog(userAuditLog);
            }
            return message;
        }

        /// <summary>
        /// Method is used to get all player Data
        /// </summary>
        /// <param name=""></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<PlayerDataModel> GetAllPlayerData(ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var playerDatalist = new List<PlayerDataModel>();
            var playerDataEntity =(from player in context.PlayerDetails join playerdata in 
                                   context.PlayerData on player.PlayerId equals playerdata.PlayerId 
                                   where playerdata.IsDeleted == false && player.IsDeleted==false
                                   select playerdata).Include(x => x.Player)
                                   .OrderByDescending(x => x.CreatedDate).ToList();
            if (playerDataEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;
            }
            playerDataEntity.ForEach(item =>
            {
                var imgmodel = new VideoImageModel();
                if (item.FileCategoryTypeId == FileCategoryTypeHelper.SplashScreenTypeId)
                {
                    imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(item.FilePath) ? item.FilePath : item.FilePath);
                    imgmodel.Type = String.IsNullOrEmpty(item.FilePath) ? "video" : "image";
                    imgmodel.thumbnail = ThumbnailPath(imgmodel.url);
                }
                else
                {
                    imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(item.FilePath) ? item.FilePath : item.FilePath.Replace("\r\n", ""));
                    imgmodel.Type = String.IsNullOrEmpty(item.FilePath) ? "image" : "image";
                    imgmodel.thumbnail = imgmodel.url;
                }

                playerDatalist.Add(new PlayerDataModel
                {
                    PlayerDataId = item.PlayerDataId,
                    PlayerId = item.PlayerId,
                    PlayerName = item.Player.FirstName + " " + item.Player.LastName,
                    FileName = item.FileName,
                    FilePath = imgmodel.thumbnail,
                    FileCategoryTypeId = item.FileCategoryTypeId,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    UpdatedBy = item.UpdatedBy,
                    UpdatedDate = item.UpdatedDate,
                    IsDeleted = item.IsDeleted,
                });
            });
            return playerDatalist;
        }


        private string ThumbnailPath(string filePath)
        {
            byte[]? ms = null;
            string extension = "";
            if (!String.IsNullOrWhiteSpace(filePath))
            {
                extension = Path.GetExtension(filePath);
                if (!String.IsNullOrWhiteSpace(extension))
                {
                    // filePath = filePath.Replace(extension, "*");
                    //filePath = filePath.Replace("/Resources/ContentFile/");
                    //ms = System.IO.File.ReadAllBytes(filePath);

                    //var string1 = filePath;
                    //filePath = string1.Replace(extension, ".jpeg");
                }
            }

            return filePath;
        }

        /// <summary>
        /// Method to delete player Data
        /// </summary>
        /// <param name="playerDataId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public string DeletePlayerData(long playerDataId, ref ErrorResponseModel errorResponseModel)
        {
            string Message = "";
            var playerdataEntity = context.PlayerData.FirstOrDefault(x => x.PlayerDataId == playerDataId);
            if (playerdataEntity != null)
            {
                playerdataEntity.IsDeleted = true;
                context.SaveChanges();
                Message = GlobalConstants.PlayerDetailDeleteSuccessfully;
            }
            var userAuditLog = new UserAuditLogModel();
            userAuditLog.Action = " Delete Player Data";
            userAuditLog.Description = "Player Data Deleted Successfully";
            userAuditLog.UserId = (int)playerdataEntity.CreatedBy;
            userAuditLog.CreatedDate = DateTime.Now;
            userAuditLog.CreatedBy = playerdataEntity.CreatedBy;
            userAuditLog.UpdatedBy = playerdataEntity.CreatedBy;
            userAuditLog.UpdatedDate = DateTime.Now;
            _userAuditLogService.AddUserAuditLog(userAuditLog);
            return "{\"message\": \"" + Message + "\"}";
        }



        /// <summary>
        /// Method is used to get playerDetails by playerid
        /// </summary>
        /// <param name="PlayerId"></param>
        /// <returns></returns>
        public PlayerDetailModel GetPlayerDetailByPlayerId(long playerId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var playerEntity = context.PlayerDetails.FirstOrDefault(x => x.PlayerId == playerId && x.IsDeleted == false);
            if (playerEntity == null)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
                return null;

            }
            var imgmodel = new VideoImageModel();
            imgmodel.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(playerEntity.ProfileImageFilePath) ? playerEntity.ProfileImageFilePath : playerEntity.ProfileImageFilePath);
            imgmodel.Type = String.IsNullOrEmpty(playerEntity.ProfileImageFilePath) ? "image" : "image";
            imgmodel.FileName = imgmodel.url;

            var aadharimg = new VideoImageModel();
            aadharimg.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(playerEntity.AadharCardFilePath) ? playerEntity.AadharCardFilePath : playerEntity.AadharCardFilePath.Replace("\r\n", ""));
            aadharimg.Type = String.IsNullOrEmpty(playerEntity.AadharCardFilePath) ? "image" : "image";
            aadharimg.FileName = aadharimg.url;

            var panimg = new VideoImageModel();
            panimg.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(playerEntity.PanCardFilePath) ? playerEntity.PanCardFilePath : playerEntity.PanCardFilePath.Replace("\r\n", ""));
            panimg.Type = String.IsNullOrEmpty(playerEntity.PanCardFilePath) ? "image" : "image";
            panimg.FileName = panimg.url;

            var votingimg = new VideoImageModel();
            votingimg.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(playerEntity.VotingCardFilePath) ? playerEntity.VotingCardFilePath : playerEntity.VotingCardFilePath);
            votingimg.Type = String.IsNullOrEmpty(playerEntity.VotingCardFilePath) ? "image" : "image";
            votingimg.FileName = votingimg.url;

            var licenseimg = new VideoImageModel();
            licenseimg.url = _configuration.HostName.TrimEnd('/') + (String.IsNullOrEmpty(playerEntity.DrivingLicenceFilePath) ? playerEntity.DrivingLicenceFilePath : playerEntity.DrivingLicenceFilePath.Replace("\r\n", ""));
            licenseimg.Type = String.IsNullOrEmpty(playerEntity.DrivingLicenceFilePath) ? "image" : "image";
            licenseimg.FileName = licenseimg.url;

            return new PlayerDetailModel
            {
                PlayerId = playerEntity.PlayerId,
                PlayerName = playerEntity.FirstName + " " + playerEntity.LastName,
                AadharCardFileName = playerEntity.AadharCardFileName,
                AadharCardFilePath = aadharimg.FileName,
                PanCardFileName = playerEntity.PanCardFileName,
                PanCardFilePath = panimg.FileName,
                VotingCardFileName = playerEntity.VotingCardFileName,
                VotingCardFilePath = votingimg.FileName,
                DrivingLicenceFileName = playerEntity.DrivingLicenceFileName,
                DrivingLicenceFilePath = licenseimg.FileName,
                BankAcountNo = playerEntity.BankAcountNo,
                PancardNo = playerEntity.PancardNo,
                FirstName = playerEntity.FirstName,
                SportId=playerEntity.SportId,
                LastName = playerEntity.LastName,
                FullName = playerEntity.FirstName + " " + playerEntity.LastName,
                Address = playerEntity.Address,
                Dob=playerEntity.Dob,
                ProfileImageFileName = playerEntity.ProfileImageFileName,
                ProfileImageFilePath = imgmodel.FileName,
                CreatedBy = playerEntity.CreatedBy,
                CreatedDate = playerEntity.CreatedDate,
                UpdatedBy = playerEntity.UpdatedBy,
                UpdatedDate = playerEntity.UpdatedDate,
            };
        }
    } 
}
