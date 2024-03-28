using IM10.BAL.Implementaion;
using IM10.BAL.Interface;
using IM10.Common;
using IM10.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StartUpX.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;

namespace IM10.API.Controllers
{


    /// <summary>
    /// APIs for PlayerDetail entity 
    /// </summary>

    [Route("api/[controller]")]
    [ApiController]
    public class PlayerDetailController : BaseAPIController
    {
        IPlayerDetailService playerDetailService;
        private readonly IWebHostEnvironment iwebhostingEnvironment;
        static String URLffmpeg;
        /// <summary>
        /// Used to initialize controller and inject PlayerDetail service
        /// </summary>
        /// <param name="userPlayerService"></param>
        public PlayerDetailController(IPlayerDetailService _playerDetailService, IOptions<ConfigurationModel> config, IWebHostEnvironment _iwebhostingEnvironment)
        {
            playerDetailService = _playerDetailService;
            iwebhostingEnvironment= _iwebhostingEnvironment;
            URLffmpeg = config.Value.URLffmpeg;
        }

        /// <summary>
        /// To get PlayerDetail by playerId 
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpGet("GetPlayerDetailById/{playerId}")]
        [ProducesResponseType(typeof(PlayerSportsModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetPlayerDetailById(long playerId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (playerId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var userModel = playerDetailService.GetPlayerDetailById(playerId, ref errorResponseModel);

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
        /// Get all PlayerDetail
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetAllPlayerDetail")]
        [ProducesResponseType(typeof(PlayerDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllPlayerDetail()
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {

                var playerDetailModels = playerDetailService.GetAllPlayerDetail(ref errorResponseModel);

                if (playerDetailModels != null)
                {
                    return Ok(playerDetailModels);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }



        /// <summary>
        /// Create an PlayerDetail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("AddPlayerDetail")]
        [Authorize]
        [ProducesResponseType(typeof(PlayerDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult AddPlayerDetail(IFormCollection formdata)
        {

            PlayerDetailModel model = new PlayerDetailModel();
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = ((System.Security.Claims.ClaimsIdentity)User.Identity).FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
                model.CreatedBy = Convert.ToInt32(userId);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request, please verify details");
            }

            try
            {
                var errorMessage = new ErrorResponseModel();
                {
                    model.FirstName = formdata["firstName"];
                    model.Address = formdata["address"];
                    model.LastName = formdata["lastName"];
                    model.PlayerId = Convert.ToInt32(formdata["playerId"]);
                    model.BankAcountNo = formdata["bankaccountNo"];
                    model.PancardNo = formdata["pancardNo"];
                    model.SportId = Convert.ToInt32(formdata["sportId"]);
                    model.Dob = Convert.ToDateTime(formdata["dob"]);

                    var existingFilesNotForDelete = formdata["existingFiles"].ToString().Split(",");

                    if (model.PlayerId != 0)
                    {
                        for (int i = 0; i < existingFilesNotForDelete.Count(); i++)
                        {
                            string filePath = existingFilesNotForDelete[i].ToString();
                            if (filePath == "")
                            {
                                break;
                            }
                            string[] pathArr = filePath.Split('\\');
                            string[] fileArr = pathArr.Last().Split('/');
                            string fileBaseName = fileArr.First().ToString();
                            string fileName = "/" + fileArr[3] + "/" + fileArr[4].ToString();     
                        }
                    }

                    if (Request.Form.Files.Count > 0)
                    {
                        var folder = iwebhostingEnvironment.WebRootPath;
                        var files = Request.Form.Files;

                        foreach (var file in files)
                        {
                            var folderName = Path.Combine("Resources", "PlayerDocumentDetails");
                            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                            if (file.Length > 0)
                            {
                                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                                var uniqueId = Guid.NewGuid().ToString();
                                var fileExtension = Path.GetExtension(fileName);
                                var uniqueFileName = $"{uniqueId}{fileExtension}";
                                var fullPath = Path.Combine(pathToSave, uniqueFileName);
                                var dbPath = Path.Combine(folderName, uniqueFileName);
                                using (var stream = new FileStream(fullPath, FileMode.Create))
                                {
                                    file.CopyTo(stream);
                                }
                                if (file.Name == "aadharCardFilePath")
                                {
                                    model.AadharCardFilePath = "/PlayerDocumentDetails/" + uniqueFileName;
                                    model.AadharCardFileName = uniqueFileName;
                                }
                                else if(file.Name == "panCardFilePath")
                                {
                                    model.PanCardFilePath = "/PlayerDocumentDetails/" + uniqueFileName;
                                    model.PanCardFileName = uniqueFileName;
                                }
                                else if (file.Name == "votingCardFilePath")
                                {
                                    model.VotingCardFilePath = "/PlayerDocumentDetails/" + uniqueFileName;
                                    model.VotingCardFileName = uniqueFileName;
                                }
                                else if (file.Name == "drivingLicenceFilePath")
                                {
                                    model.DrivingLicenceFilePath = "/PlayerDocumentDetails/" + uniqueFileName;
                                    model.DrivingLicenceFileName = uniqueFileName;
                                }
                                else if (file.Name == "profileImageFilePath")
                                {
                                    model.ProfileImageFilePath = "/PlayerDocumentDetails/" + uniqueFileName;
                                    model.ProfileImageFileName = uniqueFileName;
                                }
                                model.PlayerId = Convert.ToInt32(formdata["playerId"]);
                            }
                        }
                    }
                }
                string productModel = "";

                if (model.PlayerId == 0)
                    productModel = playerDetailService.AddPlayerDetail(model, ref errorMessage);
                
                if (!string.IsNullOrEmpty(productModel))
                {
                    return Ok(productModel);
                }
                return ReturnErrorResponse(errorMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, GlobalConstants.Status500Message);
            }
            
        }



        /// <summary>
        /// Update an PlayerDetail
        /// </summary>
        /// <param name="playerModel"></param>
        /// <returns></returns>
        [HttpPut("EditPlayerDetail")]
        [Authorize]
        [ProducesResponseType(typeof(PlayerDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult EditPlayerDetail(IFormCollection formdata)
        {
            PlayerDetailModel model = new PlayerDetailModel();
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = ((System.Security.Claims.ClaimsIdentity)User.Identity).FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
                model.UpdatedBy = Convert.ToInt32(userId);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request, please verify details");
            }

            try
            {
                var errorMessage = new ErrorResponseModel();
                {
                    model.FirstName = formdata["firstName"];
                    model.Address = formdata["address"];
                    model.LastName = formdata["lastName"];
                    model.FullName = formdata["fullname"];
                    model.PlayerId = Convert.ToInt32(formdata["playerId"]);
                    model.BankAcountNo = formdata["bankaccountNo"];
                    model.PancardNo = formdata["pancardNo"];
                    model.SportId = Convert.ToInt32(formdata["sportId"]);
                    model.Dob = Convert.ToDateTime(formdata["dob"]);

                    var existingFilesNotForDelete = formdata["existingFiles"].ToString().Split(",");

                    if (model.PlayerId != 0)
                    {
                        for (int i = 0; i < existingFilesNotForDelete.Count(); i++)
                        {
                            string filePath = existingFilesNotForDelete[i].ToString();
                            if (filePath == "")
                            {
                                break;
                            }
                            string[] pathArr = filePath.Split('\\');
                            string[] fileArr = pathArr.Last().Split('/');
                            string fileBaseName = fileArr.First().ToString();
                            string fileName = "/" + fileArr[3] + "/" + fileArr[4].ToString();
                        }
                    }

                    if (Request.Form.Files.Count > 0)
                    {
                        var folder = iwebhostingEnvironment.WebRootPath;
                        var files = Request.Form.Files;
                        foreach (var file in files)
                        {
                            var folderName = Path.Combine("Resources", "PlayerDocumentDetails");
                            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                            if (file.Length > 0)
                            {
                                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                                var uniqueId = Guid.NewGuid().ToString();
                                var fileExtension = Path.GetExtension(fileName);
                                var uniqueFileName = $"{uniqueId}{fileExtension}";
                                var fullPath = Path.Combine(pathToSave, uniqueFileName);
                                var dbPath = Path.Combine(folderName, uniqueFileName);
                                using (var stream = new FileStream(fullPath, FileMode.Create))
                                {
                                    file.CopyTo(stream);
                                }
                                if (file.Name == "aadharCardFilePath")
                                {
                                    model.AadharCardFilePath = "/PlayerDocumentDetails/" + uniqueFileName;
                                    model.AadharCardFileName = uniqueFileName;
                                }
                                else if (file.Name == "panCardFilePath")
                                {
                                    model.PanCardFilePath = "/PlayerDocumentDetails/" + uniqueFileName;
                                    model.PanCardFileName = uniqueFileName;
                                }
                                else if (file.Name == "votingCardFilePath")
                                {
                                    model.VotingCardFilePath = "/PlayerDocumentDetails/" + uniqueFileName;
                                    model.VotingCardFileName = uniqueFileName;
                                }
                                else if (file.Name == "drivingLicenceFilePath")
                                {
                                    model.DrivingLicenceFilePath = "/PlayerDocumentDetails/" + uniqueFileName;
                                    model.DrivingLicenceFileName = uniqueFileName;
                                }
                                else if (file.Name == "profileImageFilePath")
                                {
                                    model.ProfileImageFilePath = "/PlayerDocumentDetails/" + uniqueFileName;
                                    model.ProfileImageFileName = uniqueFileName;
                                }
                               model.PlayerId = Convert.ToInt32(formdata["playerId"]);
                            }
                        }
                    }
                }
                string productModel = "";

                if (model.PlayerId != 0)

                   productModel = playerDetailService.EditPlayerDetail(model, ref errorMessage);

                if (!string.IsNullOrEmpty(productModel))
                {
                    return Ok(productModel);
                }
                return ReturnErrorResponse(errorMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, GlobalConstants.Status500Message);
            }
        }


        /// <summary>
        /// Delete an PlayerDetail
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpDelete("DeletePlayerDetail")]
        [ProducesResponseType(typeof(PlayerDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult DeletePlayerDetail(long playerId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var Model = playerDetailService.DeletePlayerDetail(playerId, ref errorResponseModel);

                if (Model != null)
                {
                    return Ok(Model);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }

        /// <summary>
        /// Create an Player Data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("AddPlayerData")]
        [Authorize]
        [ProducesResponseType(typeof(PlayerDataModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult AddPlayerData(IFormCollection formdata)
        {
            var userId = "";
            var PlayerDataId = 0;
            List<PlayerDataModel> model = new List<PlayerDataModel>();
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                 userId = ((System.Security.Claims.ClaimsIdentity)User.Identity).FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
            }
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request, please verify details");
            }

            try
            {
                var errorMessage = new ErrorResponseModel();
                {
                    var PlayerId = Convert.ToInt32(formdata["playerId"]);
                     PlayerDataId = Convert.ToInt32(formdata["playerDataId"]);
                    var existingFilesNotForDelete = formdata["existingFiles"].ToString().Split(",");

                    if (PlayerDataId != 0)
                    {
                        for (int i = 0; i < existingFilesNotForDelete.Count(); i++)
                        {
                            string filePath = existingFilesNotForDelete[i].ToString();
                            if (filePath == "")
                            {
                                break;
                            }
                            string[] pathArr = filePath.Split('\\');
                            string[] fileArr = pathArr.Last().Split('/');
                            string fileBaseName = fileArr.First().ToString();
                            string fileName = "/" + fileArr[3] + "/" + fileArr[4].ToString();
                        }
                    }

                    if (Request.Form.Files.Count > 0)
                    {
                        var folder = iwebhostingEnvironment.WebRootPath;
                        var files = Request.Form.Files;

                        foreach (var file in files)
                        {
                            var folderName = Path.Combine("Resources", "PlayerDataDetails");
                            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                            if (file.Length > 0)
                            {
                                PlayerDataModel playerDataModel = new PlayerDataModel();
                                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                                var uniqueId = Guid.NewGuid().ToString();
                                var fileExtension = Path.GetExtension(fileName);
                                var uniqueFileName = $"{uniqueId}{fileExtension}";
                                var uniqueFileName1 = $"{uniqueId}";

                                var fullPath = Path.Combine(pathToSave, uniqueFileName);
                                var fullPathbitmap = Path.Combine(pathToSave + "\\thumbnail\\", string.Format(@"{0}.jpeg", uniqueFileName1));
                                var dbPath = Path.Combine(folderName, uniqueFileName);
                                using (var stream = new FileStream(fullPath, FileMode.Create))
                                {
                                    file.CopyTo(stream);
                                }
                                if (file.Name == "splashScreenFilePath")
                                {
                                    GetThumbnail(fullPath, fullPathbitmap);
                                    playerDataModel.FilePath = "/PlayerDataDetails/" + uniqueFileName;
                                    playerDataModel.FileName = uniqueFileName;
                                    playerDataModel.FileCategoryTypeId = FileCategoryTypeHelper.SplashScreenTypeId;
                                    playerDataModel.CreatedBy = Convert.ToInt32(userId);
                                    playerDataModel.PlayerId = PlayerId;
                                    model.Add(playerDataModel);
                                }
                                else if (file.Name == "logoFilePath")
                                {
                                    playerDataModel.FilePath = "/PlayerDataDetails/" + uniqueFileName;
                                    playerDataModel.FileName = uniqueFileName;
                                    playerDataModel.FileCategoryTypeId = FileCategoryTypeHelper.LogoTypeId;
                                    playerDataModel.CreatedBy = Convert.ToInt32(userId);
                                    playerDataModel.PlayerId = PlayerId;
                                    model.Add(playerDataModel);
                                }
                                else if (file.Name == "slideImageFilePath")
                                {
                                    playerDataModel.FilePath = "/PlayerDataDetails/" + uniqueFileName;
                                    playerDataModel.FileName = uniqueFileName;
                                    playerDataModel.FileCategoryTypeId = FileCategoryTypeHelper.SlideImageTypeId;
                                    playerDataModel.CreatedBy = Convert.ToInt32(userId);
                                    playerDataModel.PlayerId = PlayerId;
                                    model.Add(playerDataModel);
                                }
                            }
                        }
                    }
                }
                string productModel = "";

                if (PlayerDataId == 0)
                    productModel = playerDetailService.AddPlayerData(model, ref errorMessage);

                if (!string.IsNullOrEmpty(productModel))
                {
                    return Ok(productModel);
                }
                return ReturnErrorResponse(errorMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, GlobalConstants.Status500Message);
            }

        }
        public static void GetThumbnail(string video, string thumbnail)
        {
            var cmd = "-y  -i " + '"' + video + '"' + " -an -vf scale=320x240 " + '"' + thumbnail + '"';
            var startInfo = new ProcessStartInfo
            {
                FileName = URLffmpeg,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = cmd
            };

            using (var process = new Process { StartInfo = startInfo })
            {
                process.Start();
                process.WaitForExit(5000);
            }
        }
        /// <summary>
        /// Get all Player Data
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetAllPlayerData")]
        [ProducesResponseType(typeof(PlayerDataModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllPlayerData()
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {

                var playerDataModels = playerDetailService.GetAllPlayerData(ref errorResponseModel);

                if (playerDataModels != null)
                {
                    return Ok(playerDataModels);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }

        /// <summary>
        /// Delete an Player Data
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpDelete("DeletePlayerData")]
        [ProducesResponseType(typeof(PlayerDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult DeletePlayerData(long playerDataId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var Model = playerDetailService.DeletePlayerData(playerDataId, ref errorResponseModel);

                if (Model != null)
                {
                    return Ok(Model);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }





        /// <summary>
        /// Method is used to get playerDetails by playerid
        /// </summary>
        /// <param name="PlayerId"></param>
        /// <returns></returns>
        [HttpGet("GetPlayerDetailByPlayerId/{playerId}")]
        [ProducesResponseType(typeof(PlayerDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetPlayerDetailByPlayerId(long playerId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (playerId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var userModel = playerDetailService.GetPlayerDetailByPlayerId(playerId, ref errorResponseModel);

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
    }
}
