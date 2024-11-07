using IM10.API.Hubs;
using IM10.BAL.Interface;
using IM10.Common;
using IM10.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace IM10.API.Controllers
{
    /// <summary>
    /// APIs for ContentDetails entity 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContentDetailController : BaseAPIController
    {
        IContentDetailService contentDetailService;
        private readonly IWebHostEnvironment iwebhostingEnvironment;
        private readonly IConfiguration _configuration;


        /// <summary>
        /// Used to initialize controller and inject contentDetails service
        /// </summary>
        /// <param name="userPlayerService"></param>
        public ContentDetailController(IContentDetailService _contentDetailService, IConfiguration configuration,IWebHostEnvironment _iwebhostingEnvironment)
        {
            contentDetailService = _contentDetailService;
            iwebhostingEnvironment = _iwebhostingEnvironment;
            _configuration = configuration;
        }


        private async Task<string> UploadFileToBunnyNetAsync(Stream fileStream, string fileName, string playerName, bool isThumbnail = false)
        {
            var apiKey = _configuration["BunnyNet:ApiKey"];
            var storageZoneName = _configuration["BunnyNet:StorageZoneName"];
            var bunnyHostName = _configuration["BunnyNet:HostName"];

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(storageZoneName) || string.IsNullOrEmpty(bunnyHostName))
            {
                throw new InvalidOperationException("BunnyNet configuration is missing.");
            }

            var directoryPath = isThumbnail ? $"{playerName}/Resources/ContentFile/thumbnail" : $"{playerName}/Resources/ContentFile";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("AccessKey", apiKey);

                using (var content = new StreamContent(fileStream))
                {
                    content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

                    var requestUri = $"https://sg.storage.bunnycdn.com/{storageZoneName}/{directoryPath}/{fileName}";
                    var response = await client.PutAsync(requestUri, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("File uploaded successfully.");
                        return $"/{directoryPath}/{fileName}";
                    }
                    else
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Failed to upload file. Status Code: {response.StatusCode}, Response: {errorResponse}");

                        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            throw new HttpRequestException("Unauthorized: Check if your API key is correct and has the necessary permissions.");
                        }
                        else
                        {
                            throw new HttpRequestException($"Failed to upload file: {response.StatusCode}");
                        }
                    }
                }
            }
        }



        /// <summary>
        /// To get contentDetails by contentId 
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        [HttpGet("GetContentDetailById/{contentId}")]
        [ProducesResponseType(typeof(ContentDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetContentDetailById(long contentId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (contentId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var contentdetailModel = contentDetailService.GetContentDetailById(contentId, ref errorResponseModel);

                if (contentdetailModel != null)
                {
                    return Ok(contentdetailModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }

        /// <summary>
        /// Get all contentDetails
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetAllContentDetail")]
        [ProducesResponseType(typeof(ContentDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllContentDetail()
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {

                var detailModel = contentDetailService.GetAllContentDetail(ref errorResponseModel);

                if (detailModel != null)
                {
                    return Ok(detailModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }



        /// <summary>
        /// Create an contentDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("AddContentDetail")]
        [Authorize(Roles = "Content Admin")]
        [DisableRequestSizeLimit]
        [ProducesResponseType(typeof(ContentDetailModel1), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> AddContentDetail(IFormCollection formdata)
        {
            ContentDetailModel1 model = new ContentDetailModel1();
            Console.WriteLine(formdata);
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
                    model.ContentId = Convert.ToInt32(formdata["contentId"]);
                    model.Title = formdata["title"];
                    model.Description = formdata["description"];
                    model.PlayerId = Convert.ToInt32(formdata["playerId"]);
                    model.CategoryId = Convert.ToInt32(formdata["categoryId"]);
                    model.SubCategoryId = Convert.ToInt32(formdata["subcategoryId"]);
                    model.ContentTypeId = Convert.ToInt32(formdata["contenttypeId"]);
                    model.LanguageId = Convert.ToInt32(formdata["languageId"]);

                    var existingFilesNotForDelete = formdata["existingFiles"].ToString().Split(",");

                    if (model.ContentId != 0)
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
                            model.ContentFileName = fileName;
                            model.ContentFilePath = formdata["imageUrl"];
                            model.ContentFileName1 = fileName;
                            model.ContentFilePath1 = formdata["imageUrl"];
                        }
                    }

                    bool productionFlag = bool.Parse(_configuration["ProductionFlag"]);
                    var playerName = contentDetailService.Getplayername(model.PlayerId);

                    if (Request.Form.Files.Count > 0)
                    {
                        foreach (var file in Request.Form.Files)
                        {
                            if (file.Length > 0)
                            {
                                var uniqueId = Guid.NewGuid().ToString();
                                var fileExtension = Path.GetExtension(file.FileName);
                                var uniqueFileName = $"{uniqueId}{fileExtension}";
                                if (productionFlag)
                                {

                                    string bunnyNetUrl = await UploadFileToBunnyNetAsync(file.OpenReadStream(), uniqueFileName, playerName, file.Name.StartsWith("thumbnail"));

                                    if (file.Name == "contentFilePath")
                                    {
                                        model.ContentFilePath = bunnyNetUrl;
                                        model.ContentFileName = uniqueFileName;
                                    }
                                    else if (file.Name == "contentFilePath_1")
                                    {
                                        model.ContentFilePath1 = bunnyNetUrl;
                                        model.ContentFileName1 = uniqueFileName;
                                    }
                                    else if (file.Name == "thumbnail1")
                                    {
                                        model.Thumbnail3 = bunnyNetUrl;
                                    }
                                }
                                else
                                {
                                    var folderName = Path.Combine("Resources", "ContentFile");
                                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                                    var fullPath = Path.Combine(pathToSave, uniqueFileName);
                                    var dbPath = Path.Combine(folderName, uniqueFileName);

                                    Directory.CreateDirectory(pathToSave);

                                    using (var stream = new FileStream(fullPath, FileMode.Create))
                                    {
                                        file.CopyTo(stream);
                                    }

                                    if (file.Name == "contentFilePath")
                                    {
                                        model.ContentFilePath = "/ContentFile/" + uniqueFileName;
                                        model.ContentFileName = uniqueFileName;
                                    }
                                    else if (file.Name == "contentFilePath_1")
                                    {
                                        model.ContentFilePath1 = "/ContentFile/" + uniqueFileName;
                                        model.ContentFileName1 = uniqueFileName;
                                    }
                                    else if (file.Name == "thumbnail1")
                                    {
                                        var thumbnailFolder = Path.Combine(pathToSave, "thumbnail");
                                        var thumbnailFullPath = Path.Combine(thumbnailFolder, uniqueFileName);

                                        Directory.CreateDirectory(thumbnailFolder);
                                        using (var thumbnailStream = new FileStream(thumbnailFullPath, FileMode.Create))
                                        {
                                            file.CopyTo(thumbnailStream);
                                        }
                                        model.Thumbnail3 = "/ContentFile/thumbnail/" + uniqueFileName;
                                    }
                                    model.ContentId = Convert.ToInt32(formdata["contentId"]);
                                }
                            }
                        }
                    }
                }
                    string productModel = "";

                if (model.ContentId == 0)
                {
                    productModel = contentDetailService.AddContentDetail(model, ref errorMessage);
                }

                if (!string.IsNullOrEmpty(productModel))
                {
                    if (productModel != "Player Id does not exist")
                    {
                        return Ok(productModel);
                    }
                    else if (productModel == "Player Id does not exist")
                    {
                        return NotFound(productModel);
                    }
                }

                return ReturnErrorResponse(errorMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, GlobalConstants.Status500Message);
            }

        }

        /// <summary>
        /// Update an contentDetails
        /// </summary>
        /// <param name="contentModel"></param>
        /// <returns></returns>
        [HttpPut("EditContentDetail")]
        [Authorize(Roles = "Content Admin")]
        [DisableRequestSizeLimit]
        [ProducesResponseType(typeof(ContentDetailModel1), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> EditContentDetail(IFormCollection formdata)
        {

            ContentDetailModel1 model = new ContentDetailModel1();
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

                    model.ContentId = Convert.ToInt32(formdata["contentId"]);
                    model.Title = formdata["title"];
                    model.Description = formdata["description"];
                    model.PlayerId = Convert.ToInt32(formdata["playerId"]);
                    model.CategoryId = Convert.ToInt32(formdata["categoryId"]);
                    model.SubCategoryId = Convert.ToInt32(formdata["subcategoryId"]);
                    model.ContentTypeId = Convert.ToInt32(formdata["contenttypeId"]);
                    model.LanguageId = Convert.ToInt32(formdata["languageId"]);

                    var existingFilesNotForDelete = formdata["existingFiles"].ToString().Split(",");

                    if (model.ContentId != 0)
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
                            model.ContentFileName = fileName;
                            model.ContentFilePath = formdata["imageUrl"];
                        }
                    }

                    bool productionFlag = bool.Parse(_configuration["ProductionFlag"]);

                    if (Request.Form.Files.Count > 0)
                    {
                        var folder = iwebhostingEnvironment.WebRootPath;
                        var files = Request.Form.Files;
                        foreach (var file in files)
                        {

                            if (file.Length > 0)
                            {
                                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                                var uniqueId = Guid.NewGuid().ToString();
                                var fileExtension = Path.GetExtension(fileName);
                                var uniqueFileName = $"{uniqueId}{fileExtension}";
                                var playerName = contentDetailService.Getplayername(model.PlayerId);

                                if (productionFlag)
                                {
                                    // Upload to Bunny.net
                                    string bunnyNetUrl = await UploadFileToBunnyNetAsync(file.OpenReadStream(), uniqueFileName, playerName, file.Name.StartsWith("thumbnail"));

                                    if (file.Name == "contentFilePath")
                                    {
                                        model.ContentFilePath = bunnyNetUrl;
                                        model.ContentFileName = uniqueFileName;
                                    }
                                    else if (file.Name == "contentFilePath_1")
                                    {
                                        model.ContentFilePath1 = bunnyNetUrl;
                                        model.ContentFileName1 = uniqueFileName;
                                    }
                                    else if (file.Name == "thumbnail1")
                                    {
                                        model.Thumbnail3 = bunnyNetUrl;
                                    }
                                }
                                else
                                {
                                    var folderName = Path.Combine("Resources", "ContentFile");
                                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                                    var fullPath = Path.Combine(pathToSave, uniqueFileName);
                                    var dbPath = Path.Combine(folderName, uniqueFileName);
                                    Directory.CreateDirectory(pathToSave);

                                    using (var stream = new FileStream(fullPath, FileMode.Create))
                                    {
                                        file.CopyTo(stream);
                                    }

                                    if (file.Name == "contentFilePath")
                                    {
                                        model.ContentFilePath = "/ContentFile/" + uniqueFileName;
                                        model.ContentFileName = uniqueFileName;
                                    }
                                    else if (file.Name == "contentFilePath_1")
                                    {
                                        model.ContentFilePath1 = "/ContentFile/" + uniqueFileName;
                                        model.ContentFileName1 = uniqueFileName;
                                    }
                                    else if (file.Name == "thumbnail1")
                                    {
                                        var thumbnailFolder = Path.Combine(pathToSave, "thumbnail");
                                        var thumbnailFullPath = Path.Combine(thumbnailFolder, uniqueFileName);

                                        Directory.CreateDirectory(thumbnailFolder);
                                        using (var thumbnailStream = new FileStream(thumbnailFullPath, FileMode.Create))
                                        {
                                            file.CopyTo(thumbnailStream);
                                        }
                                        model.Thumbnail3 = "/ContentFile/thumbnail/" + uniqueFileName;
                                    }
                                    model.ContentId = Convert.ToInt32(formdata["contentId"]);
                                }
                            }
                        }
                    }
                }
                string productModel = "";

                if (model.ContentId != 0)
                {
                    productModel = contentDetailService.EditContentDetail(model, ref errorMessage);
                }

                if (!string.IsNullOrEmpty(productModel))
                {
                    if (productModel != "Player Id does not exist")
                    {
                        return Ok(productModel);
                    }
                    else if (productModel == "Player Id does not exist")
                    {
                        return NotFound(productModel);
                    }
                }

                return ReturnErrorResponse(errorMessage);


            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, GlobalConstants.Status500Message);
            }


        }


        /// <summary>
        /// Update an contentDetails
        /// </summary>
        /// <param name="contentModel"></param>
        /// <returns></returns>
        [HttpPut("ContentDetailUpdateByContentLog")]
        [Authorize]
        [ProducesResponseType(typeof(ContentModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult ContentDetailUpdateByContentLog(ContentModel contentModel)
        {
            ErrorResponseModel errorResponseModel = null;
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = ((System.Security.Claims.ClaimsIdentity)User.Identity).FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
                contentModel.UpdatedBy = Convert.ToInt32(userId);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request, please verify details");
            }
            try
            {
                var Model = contentDetailService.ContentDetailUpdateByContentLog(contentModel, ref errorResponseModel);

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
        /// Delete an contentDetails
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        [HttpDelete("DeleteContentDetail")]
        [Authorize(Roles = "Content Admin")]
        [ProducesResponseType(typeof(ContentDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult DeleteContentDetail(long contentId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var contentModel = contentDetailService.DeleteContentDetail(contentId, ref errorResponseModel);

                if (contentModel != null && contentModel != "Cannot delete approved content")
                {
                    return Ok(contentModel);
                }
                else if(contentModel== "Cannot delete approved content")
                {
                    return BadRequest(contentModel);
                }
                else if (errorResponseModel.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound(errorResponseModel.Message);
                }
                else
                {
                    return ReturnErrorResponse(errorResponseModel);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }



        /// <summary>
        /// To get contentDetails by playerId 
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpGet("GetContentdetailByPlayerId/{playerId}")]
        [ProducesResponseType(typeof(ContentDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetContentdetailByPlayerId(long playerId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (playerId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var contentdetailModel = contentDetailService.GetContentdetailByPlayerId(playerId, ref errorResponseModel);

                if (contentdetailModel != null)
                {
                    return Ok(contentdetailModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }

        }



        /// <summary>
        /// ApproveContentDetail
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        [HttpPost("ApproveContentDetail/{contentId}")]
       // [Authorize]
        [DisableRequestSizeLimit]
        [ProducesResponseType(typeof(ContentDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public  async Task<IActionResult> ApproveContentDetail(long contentId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                NotificationModel contentModel =await contentDetailService.ApproveContentDetail(contentId);

                if (contentModel != null)
                {
                   return Ok(contentModel);                  
                }
                else if (errorResponseModel.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound(errorResponseModel.Message);
                }
                else
                {
                    return ReturnErrorResponse(errorResponseModel);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }


        /// <summary>
        /// To get approvedcontentDetails by playerId 
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpGet("GetApprovedContentdetailByPlayerId/{playerId}")]
        [ProducesResponseType(typeof(ContentDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetApprovedContentdetailByPlayerId(long playerId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (playerId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var contentdetailModel = contentDetailService.GetApprovedContentdetailByPlayerId(playerId, ref errorResponseModel);

                if (contentdetailModel != null)
                {
                    return Ok(contentdetailModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }

        }

        /// <summary>
        /// DeniedContentDetail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("DeniedContentDetail")]
        [ProducesResponseType(typeof(CommentModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult DeniedContentDetail(CommentModel model)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var contentModel = contentDetailService.DeniedContentDetail(model, ref errorResponseModel);

                if (contentModel != null)
                {
                    return Ok(contentModel);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }




        /// <summary>
        /// To get contenttitles by playerId 
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpGet("GetContentTitlesByPlayerId/{playerId}")]
        [ProducesResponseType(typeof(ContentTitleModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetContentTitlesByPlayerId(long playerId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (playerId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var contentdetailModel = contentDetailService.GetContentTitlesByPlayerId(playerId, ref errorResponseModel);

                if (contentdetailModel != null)
                {
                    return Ok(contentdetailModel);
                }

                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }


        /// <summary>
        /// To get GetApprovedContentTitles by playerId,contenttypeid
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="contenttypeId"></param>
        /// <returns></returns>
        [HttpGet("GetApprovedContentTitles/{playerId}/{contenttypeId}")]
        [ProducesResponseType(typeof(ContentTitleModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetApprovedContentTitles(long playerId, long contenttypeId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                if (playerId <= 0)
                {
                    return BadRequest("Invalid data");
                }
                var contentdetailModel = contentDetailService.GetApprovedContentTitles(playerId, contenttypeId, ref errorResponseModel);

                if (contentdetailModel != null)
                {
                    return Ok(contentdetailModel);
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
