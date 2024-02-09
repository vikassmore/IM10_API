using IM10.API.Hubs;
using IM10.BAL.Implementaion;
using IM10.BAL.Interface;
using IM10.Common;
using IM10.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using StartUpX.Common;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace IM10.API.Controllers
{
    /// <summary>
    /// APIs for ContentDetails entity 
    /// </summary>

    [Route("api/[controller]")]
    [ApiController]
    public class ContentDetailController : BaseAPIController
    {
        IContentDetailService contentDetailService;
        private readonly IWebHostEnvironment iwebhostingEnvironment;

        static String URLffmpeg;
        private ConfigurationModel _configurationModel;
        private readonly IHubContext<NotificationsHubService> _hubContext;

        /// <summary>
        /// Used to initialize controller and inject contentDetails service
        /// </summary>
        /// <param name="userPlayerService"></param>
        public ContentDetailController(IContentDetailService _contentDetailService, IOptions<ConfigurationModel> config, IWebHostEnvironment _iwebhostingEnvironment, IHubContext<NotificationsHubService> hubContext)
        {
            contentDetailService = _contentDetailService;
            URLffmpeg = config.Value.URLffmpeg;
            iwebhostingEnvironment = _iwebhostingEnvironment;
            _hubContext= hubContext;
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
        [Authorize]
        [DisableRequestSizeLimit]
        [ProducesResponseType(typeof(ContentDetailModel1), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult AddContentDetail(IFormCollection formdata)
        {
            ContentDetailModel1 model = new ContentDetailModel1();
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

                    if (Request.Form.Files.Count > 0)
                    {
                        var folder = iwebhostingEnvironment.WebRootPath;
                        var files = Request.Form.Files;
                        foreach (var file in files)
                        {
                            var folderName = Path.Combine("Resources", "ContentFile");
                            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                            if (file.Length > 0)
                            {
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
                                if (file.Name == "contentFilePath")
                                {
                                    GetThumbnail(fullPath, fullPathbitmap);
                                    model.ContentFilePath = "/ContentFile/" + uniqueFileName;
                                    model.ContentFileName = uniqueFileName;
                                }
                                else if (file.Name == "contentFilePath_1")
                                {
                                    GetThumbnail(fullPath, fullPathbitmap);
                                    model.ContentFilePath1 = "/ContentFile/" + uniqueFileName;
                                    model.ContentFileName1 = uniqueFileName;

                                }
                                model.ContentId = Convert.ToInt32(formdata["contentId"]);

                            }
                        }
                    }
                }
                string productModel = "";

                if (model.ContentId == 0)
                    productModel = contentDetailService.AddContentDetail(model, ref errorMessage);
                else
                    productModel = contentDetailService.EditContentDetail(model, ref errorMessage);

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
        /// Update an contentDetails
        /// </summary>
        /// <param name="contentModel"></param>
        /// <returns></returns>
        [HttpPut("EditContentDetail")]
        [Authorize]
        [DisableRequestSizeLimit]
        [ProducesResponseType(typeof(ContentDetailModel1), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult EditContentDetail(IFormCollection formdata)
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

                    if (Request.Form.Files.Count > 0)
                    {
                        var folder = iwebhostingEnvironment.WebRootPath;
                        var files = Request.Form.Files;
                        foreach (var file in files)
                        {
                            var folderName = Path.Combine("Resources", "ContentFile");
                            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                            if (file.Length > 0)
                            {
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
                                if (file.Name == "contentFilePath")
                                {
                                    GetThumbnail(fullPath, fullPathbitmap);
                                    model.ContentFilePath = "/ContentFile/" + uniqueFileName;
                                    model.ContentFileName = uniqueFileName;
                                    model.Thumbnail2 = fullPathbitmap;

                                }
                                else if (file.Name == "contentFilePath_1")
                                {
                                    GetThumbnail(fullPath, fullPathbitmap);
                                    model.ContentFilePath1 = "/ContentFile/" + uniqueFileName;
                                    model.ContentFileName1 = uniqueFileName;
                                    model.Thumbnail3 = fullPathbitmap;

                                }
                                model.ContentId = Convert.ToInt32(formdata["contentId"]);
                            }
                        }
                    }
                }
                string productModel = "";

                if (model.ContentId == 0)
                    productModel = contentDetailService.AddContentDetail(model, ref errorMessage);
                else
                    productModel = contentDetailService.EditContentDetail(model, ref errorMessage);

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
        /// Update an contentDetails
        /// </summary>
        /// <param name="contentModel"></param>
        /// <returns></returns>
        [HttpPut("ContentDetailUpdateByContentLog")]
        [Authorize]
        [ProducesResponseType(typeof(ContentModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
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
        [ProducesResponseType(typeof(ContentDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult DeleteContentDetail(long contentId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var contentModel = contentDetailService.DeleteContentDetail(contentId, ref errorResponseModel);

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
        /// To get contentDetails by playerId 
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpGet("GetContentdetailByPlayerId/{playerId}")]
        [ProducesResponseType(typeof(ContentDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
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
        [HttpPost("ApproveContentDetail")]
       // [Authorize]
        [DisableRequestSizeLimit]
        [ProducesResponseType(typeof(ContentDetailModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public  async Task<IActionResult> ApproveContentDetail(long contentId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                NotificationModel contentModel =await contentDetailService.ApproveContentDetail(contentId);

                if (contentModel != null)
                {
                    if (contentModel.ContentTypeId == ContentTypeHelper.ArticleContentTypeId)
                    {
                      return Ok(contentModel);
                    }                   
                } 
                return ReturnErrorResponse(errorResponseModel);               
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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


    }
}
