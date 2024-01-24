using IM10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Interface
{

    /// <summary>
    /// Interface used for master related operations
    /// </summary>
    public interface IMasterAPIsService
    {
        /// <summary>
        /// Method is used to get country by id
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
      List<CountryModel> GetByCountryId(long countryId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get all countries
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<CountryModel> GetAllCountry(ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to get state by id
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
       List<StateModel> GetByStateId(long stateId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get all states
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<StateModel> GetAllState(ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to get city by id
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        List<CityModel> GetByCityId(long cityId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get all cities
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<CityModel> GetAllCity(ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get language by id
        /// </summary>
        /// <param name="languageId"></param>
        /// <returns></returns>
        List<LanguageModel> GetByLanguageId(long languageId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get all languages
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<LanguageModel> GetAllLanguage(ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to get category by id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        List<CategoryModel> GetByCategoryId(long categoryId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get all Categories
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<CategoryModel> GetAllCategory(ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to get subcategory by id
        /// </summary>
        /// <param name="subcategoryId"></param>
        /// <returns></returns>
        List<SubCategoryModel> GetBySubCategoryId(long subcategoryId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get subcategory by categoryId
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        List<SubCategoryModel> GetSubcategoryByCategoryId(long categoryId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get all subcategory
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<SubCategoryModel> GetAllSubCategory(ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to get contenttype by id
        /// </summary>
        /// <param name="contenttypeId"></param>
        /// <returns></returns>
        List<ContentTypeModel> GetByContentTypeId(long contenttypeId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get all content type
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<ContentTypeModel> GetAllContentType(ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method to get the state  by countryId
        /// </summary>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        List<StateModel> GetStateByCountyId(long countryId, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method to get the City  by stateid
        /// </summary>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        List<CityModel> GetCityByStateId(long stateId, ref ErrorResponseModel errorResponseModel);

    }
}
