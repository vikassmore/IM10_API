using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace IM10.API.Controllers
{

    /// <summary>
    /// APIs for master entity 
    /// </summary>
    
    [Route("api/[controller]")]
    [ApiController]
    public class MasterAPIsController : BaseAPIController
    {
        IMasterAPIsService masterAPIsService;


        /// <summary>
        /// Used to initialize controller and inject master service
        /// </summary>
        /// <param name="_mastersAPIService"></param>
        public MasterAPIsController(IMasterAPIsService _masterAPIsService)
        {
            masterAPIsService = _masterAPIsService;
        }


        /// <summary>
        /// To get all states
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetAllState")]
        [ProducesResponseType(typeof(StateModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllState()
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var stateModel = masterAPIsService.GetAllState(ref errorResponseModel);

                if (stateModel != null)
                {
                    return Ok(stateModel);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }



        /// <summary>
        /// To get all countries
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetAllCountry")]
        [ProducesResponseType(typeof(CountryModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllCountry()
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var countryModel = masterAPIsService.GetAllCountry(ref errorResponseModel);

                if (countryModel != null)
                {
                    return Ok(countryModel);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }

        /// <summary>
        /// To get all cities
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetAllCity")]
        [ProducesResponseType(typeof(CityModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllCity()
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var cityModel = masterAPIsService.GetAllCity(ref errorResponseModel);

                if (cityModel != null)
                {
                    return Ok(cityModel);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }

        /// <summary>
        /// To get all language
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetAllLanguage")]
        [ProducesResponseType(typeof(LanguageModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllLanguage()
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var languageModel = masterAPIsService.GetAllLanguage(ref errorResponseModel);

                if (languageModel != null)
                {
                    return Ok(languageModel);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }



        /// <summary>
        /// To get all Category
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetAllCategory")]
        [ProducesResponseType(typeof(CategoryModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllCategory()
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var categoryModel = masterAPIsService.GetAllCategory(ref errorResponseModel);

                if (categoryModel != null)
                {
                    return Ok(categoryModel);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }


        /// <summary>
        /// To get all SubCategory
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetAllSubCategory")]
        [ProducesResponseType(typeof(SubCategoryModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllSubCategory()
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var subcategoryModel = masterAPIsService.GetAllSubCategory(ref errorResponseModel);

                if (subcategoryModel != null)
                {
                    return Ok(subcategoryModel);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }



        /// <summary>
        /// To get all ContentType
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetAllContentType")]
        [ProducesResponseType(typeof(ContentTypeModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllContentType()
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var contentModel = masterAPIsService.GetAllContentType(ref errorResponseModel);

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
        /// To get all states by id
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetByStateId/{stateId}")]
        [ProducesResponseType(typeof(StateModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetByStateId(long stateId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var stateModellist = masterAPIsService.GetByStateId(stateId,ref errorResponseModel);

                if (stateModellist != null)
                {
                    return Ok(stateModellist);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }

        /// <summary>
        /// To get all countries by id
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetByCountryId/{countryId}")]
        [ProducesResponseType(typeof(CountryModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetByCountryId(long countryId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var countryModelList = masterAPIsService.GetByCountryId(countryId,ref errorResponseModel);

                if (countryModelList != null)
                {
                    return Ok(countryModelList);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }

        /// <summary>
        /// To get all cities by id
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetByCityId/{cityId}")]
        [ProducesResponseType(typeof(CityModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetByCityId(long cityId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var cityModelList = masterAPIsService.GetByCityId(cityId,ref errorResponseModel);

                if (cityModelList != null)
                {
                    return Ok(cityModelList);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }

        /// <summary>
        /// To get all countries
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetByLanguageId/{languageId}")]
        [ProducesResponseType(typeof(LanguageModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetByLanguageId(long languageId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var languageModelList = masterAPIsService.GetByLanguageId(languageId,ref errorResponseModel);

                if (languageModelList != null)
                {
                    return Ok(languageModelList);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }


        /// <summary>
        /// To get all Category by id
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetByCategoryId/{categoryId}")]
        [ProducesResponseType(typeof(CategoryModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetByCategoryId(long categoryId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var categoryModellist = masterAPIsService.GetByCategoryId(categoryId, ref errorResponseModel);

                if (categoryModellist != null)
                {
                    return Ok(categoryModellist);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }


        /// <summary>
        /// To get all SubCategory by id
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetBySubCategoryId/{subcategoryId}")]
        [ProducesResponseType(typeof(SubCategoryModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetBySubCategoryId(long subcategoryId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var subcategoryModellist = masterAPIsService.GetBySubCategoryId(subcategoryId, ref errorResponseModel);

                if (subcategoryModellist != null)
                {
                    return Ok(subcategoryModellist);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }


        /// <summary>
        /// To get all ContentType by id
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetByContentTypeId/{contenttypeId}")]
        [ProducesResponseType(typeof(ContentTypeModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetByContentTypeId(long contenttypeId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var contentModellist = masterAPIsService.GetByContentTypeId(contenttypeId, ref errorResponseModel);

                if (contentModellist != null)
                {
                    return Ok(contentModellist);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }


        [HttpGet("GetStateByCountyId/{CountryId}")]
        [ProducesResponseType(typeof(StateModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetStateByCountyId(long CountryId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {

                var stateModel = masterAPIsService.GetStateByCountyId(CountryId, ref errorResponseModel);

                if (stateModel != null)
                {
                    return Ok(stateModel);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, GlobalConstants.Status500Message);
            }
        }



        [HttpGet("GetCityByStateId/{StateId}")]
        //[Authorize]
        [ProducesResponseType(typeof(CityModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetCityByStateId(long StateId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {

                var cityModel = masterAPIsService.GetCityByStateId(StateId, ref errorResponseModel);

                if (cityModel != null)
                {
                    return Ok(cityModel);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, GlobalConstants.Status500Message);
            }
        }


        /// <summary>
        /// To get all SubCategory by categoryid
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetSubcategoryByCategoryId/{categoryId}")]
        [ProducesResponseType(typeof(SubCategoryModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetSubcategoryByCategoryId(long categoryId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var subcategoryModellist = masterAPIsService.GetSubcategoryByCategoryId(categoryId, ref errorResponseModel);

                if (subcategoryModellist != null)
                {
                    return Ok(subcategoryModellist);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
            }
        }




        /// <summary>
        /// To get all Sports
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetAllSports")]
        [ProducesResponseType(typeof(SportModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllSports()
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var sportModel = masterAPIsService.GetAllSports(ref errorResponseModel);

                if (sportModel != null)
                {
                    return Ok(sportModel);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,ex.Message );
            }
        }


        /// <summary>
        /// Method is used to get all Categories by sportId
        /// </summary>
        /// <param name="sportId"></param>
        /// <returns></returns>
        [HttpGet("GetAllCategoryBySportId/{sportId}")]
        [ProducesResponseType(typeof(CategoryModel), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult GetAllCategoryBySportId(long sportId)
        {
            ErrorResponseModel errorResponseModel = null;
            try
            {
                var sportModel = masterAPIsService.GetAllCategoryBySportId(sportId, ref errorResponseModel);

                if (sportModel != null)
                {
                    return Ok(sportModel);
                }
                return ReturnErrorResponse(errorResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}

