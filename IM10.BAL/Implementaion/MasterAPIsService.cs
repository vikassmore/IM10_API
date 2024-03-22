using IM10.BAL.Interface;
using IM10.Common;
using IM10.Entity.DataModels;
using IM10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Implementaion
{
    /// <summary>
    /// This is implementation  for the master operations 
    /// </summary>
    public class MasterAPIsService : IMasterAPIsService
    {
        IM10DbContext context;

        /// <summary>
        /// Creating constructor and injection dbContext
        /// </summary>
        /// <param name="context"></param>
        public MasterAPIsService(IM10DbContext _context)
        {
            context = _context;
        }

       

        /// <summary>
        /// Method to get all cities 
        /// </summary>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<CityModel> GetAllCity(ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel=new ErrorResponseModel();
            var mastermodellist=new List<CityModel>();
            var cityEntity=context.Cities.Where(x=>x.IsDeleted==false).ToList();
            if (cityEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            cityEntity.ForEach( item =>
            {
                mastermodellist.Add(new CityModel
                {
                    CityId=item.CityId,
                    Name=item.Name,
                    StateId=item.StateId,
                });
            });
            return mastermodellist;
        }

      

        /// <summary>
        /// Method to get all countries 
        /// </summary>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<CountryModel> GetAllCountry(ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel =new ErrorResponseModel();
            var countrymodelList=new List<CountryModel>();
            var countryEntity=context.Countries.Where(x=>x.IsDeleted== false).ToList();
            if(countryEntity.Count == 0)
            {
                errorResponseModel.StatusCode= HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            countryEntity.ForEach(item =>
            {
                countrymodelList.Add(new CountryModel
                {
                    CountryId=item.CountryId,
                    CountryCode=item.CountryCode,
                    Name=item.Name,
                });
            });
            return countrymodelList;
        }

        /// <summary>
        /// Method to get all languages 
        /// </summary>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<LanguageModel> GetAllLanguage(ref ErrorResponseModel errorResponseModel)
        {
             errorResponseModel=new ErrorResponseModel();
             var languageModelList=new List<LanguageModel>();
             var languageEntity=context.Languages.Where(x=>x.IsDeleted == false).ToList();
            if(languageEntity.Count == 0)
            {
                errorResponseModel.StatusCode=HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            languageEntity.ForEach(item =>
            {
                languageModelList.Add(new LanguageModel
                {
                    LanguageId = item.LanguageId,
                    Name = item.Name,
                });
            });
            return languageModelList;
        }

        /// <summary>
        /// Method to get all states 
        /// </summary>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<StateModel> GetAllState(ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel=new ErrorResponseModel();
            var stateModelList=new List<StateModel>();
            var stateEntity=context.States.Where(x=>x.IsDeleted==false).ToList();
            if (stateEntity.Count == 0)
            {
                errorResponseModel.StatusCode=HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            stateEntity.ForEach(item =>
            {
                stateModelList.Add(new StateModel
                {
                    StateId = item.StateId,
                    StateCode = item.StateCode,
                    Name = item.Name,
                    CountryId = item.CountryId,
                });
            });
            return stateModelList;
        }


        /// <summary>
        /// Method to get the city by cityId
        /// </summary>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<CityModel> GetByCityId(long cityId, ref ErrorResponseModel errorResponseModel)
        {
            var cityModelList = new List<CityModel>();
            errorResponseModel = new ErrorResponseModel();
            var cityEntityList = context.Cities.Where(x => x.CityId == cityId && x.IsDeleted==false).ToList();
            if (cityEntityList.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            cityEntityList.ForEach(item =>
            {
                cityModelList.Add(new CityModel
                {
                    CityId = item.CityId,
                    Name = item.Name,
                    StateId = item.StateId,
                });
            });
            return cityModelList;
        }

       

        /// <summary>
        /// Method to get the country by countryId
        /// </summary>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<CountryModel> GetByCountryId(long countryId, ref ErrorResponseModel errorResponseModel)
        {
            var countryModelList = new List<CountryModel>();
            errorResponseModel = new ErrorResponseModel();
            var countryEntityList = context.Countries.Where(x => x.CountryId == countryId && x.IsDeleted == false).ToList();
            if (countryEntityList.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            countryEntityList.ForEach(item =>
            {
                countryModelList.Add(new CountryModel
                {
                    CountryId = item.CountryId,
                    Name = item.Name,
                    CountryCode = item.CountryCode,
                });
            });
            return countryModelList;
        }

        /// <summary>
        /// Method to get the language by languageId
        /// </summary>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<LanguageModel> GetByLanguageId(long languageId, ref ErrorResponseModel errorResponseModel)
        {
            var languageModelList = new List<LanguageModel>();
            errorResponseModel = new ErrorResponseModel();
            var languageEntityList = context.Languages.Where(x => x.LanguageId == languageId && x.IsDeleted == false).ToList();
            if (languageEntityList.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            languageEntityList.ForEach(item =>
            {
                languageModelList.Add(new LanguageModel
                {
                    LanguageId = item.LanguageId,
                    Name = item.Name,
                });
            });
            return languageModelList;
        }

        /// <summary>
        /// Method to get the state by stateId
        /// </summary>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<StateModel> GetByStateId(long stateId, ref ErrorResponseModel errorResponseModel)
        {
            var stateModelList = new List<StateModel>();
            errorResponseModel = new ErrorResponseModel();
            var stateEntityList = context.States.Where(x => x.StateId == stateId && x.IsDeleted == false).ToList();
            if (stateEntityList.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            stateEntityList.ForEach(item =>
            {
                stateModelList.Add(new StateModel
                {
                    StateId = item.StateId,
                    Name = item.Name,
                    StateCode = item.StateCode,
                    CountryId = item.CountryId,
                });
            });
            return stateModelList;
        }


        /// <summary>
        /// Method to get all categories 
        /// </summary>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<CategoryModel> GetAllCategory(ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var categorymodellist = new List<CategoryModel>();
            var categoryEntity = context.Categories.Where(x => x.IsDeleted == false).ToList();
            if (categoryEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            categoryEntity.ForEach(item =>
            {
                categorymodellist.Add(new CategoryModel
                {
                    CategoryId = item.CategoryId,
                    Name = item.Name,
                    Description = item.Description,
                    SportId = item.SportId,
                });
            });
            return categorymodellist;
        }


        /// <summary>
        /// Method to get the category by categoryId
        /// </summary>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<CategoryModel> GetByCategoryId(long categoryId, ref ErrorResponseModel errorResponseModel)
        {
            var categoryModelList = new List<CategoryModel>();
            errorResponseModel = new ErrorResponseModel();
            var categoryEntityList = context.Categories.Where(x => x.CategoryId == categoryId && x.IsDeleted == false).ToList();
            if (categoryEntityList.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            categoryEntityList.ForEach(item =>
            {
                categoryModelList.Add(new CategoryModel
                {
                    CategoryId = item.CategoryId,
                    Name = item.Name,
                    Description = item.Description,
                });
            });
            return categoryModelList;
        }

        /// <summary>
        /// Method to get all subcategories 
        /// </summary>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<SubCategoryModel> GetAllSubCategory(ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var subcategorymodellist = new List<SubCategoryModel>();
            var subcategoryEntity = context.SubCategories.Where(x => x.IsDeleted == false).ToList();
            if (subcategoryEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            subcategoryEntity.ForEach(item =>
            {
                subcategorymodellist.Add(new SubCategoryModel
                {
                    SubCategoryId = item.SubCategoryId,
                    Name = item.Name,
                    CategoryId = item.CategoryId,
                    Description = item.Description,
                });
            });
            return subcategorymodellist;
        }


        /// <summary>
        /// Method to get the subcategory by subcategoryId
        /// </summary>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<SubCategoryModel> GetBySubCategoryId(long subcategoryId, ref ErrorResponseModel errorResponseModel)
        {
            var subcategoryModelList = new List<SubCategoryModel>();
            errorResponseModel = new ErrorResponseModel();
            var subcategoryEntityList = context.SubCategories.Where(x => x.SubCategoryId == subcategoryId && x.IsDeleted == false).ToList();
            if (subcategoryEntityList.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message= GlobalConstants.NotFoundMessage;
            }
            subcategoryEntityList.ForEach(item =>
            {
                subcategoryModelList.Add(new SubCategoryModel
                {
                    SubCategoryId = item.SubCategoryId,
                    Name = item.Name,
                    CategoryId = item.CategoryId
                });
            });
            return subcategoryModelList;
        }

        /// <summary>
        /// Method to get all content types 
        /// </summary>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<ContentTypeModel> GetAllContentType(ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var contentmodellist = new List<ContentTypeModel>();
            var contentEntity = context.ContentTypes.Where(x => x.IsDeleted == false).ToList();
            if (contentEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            contentEntity.ForEach(item =>
            {
                contentmodellist.Add(new ContentTypeModel
                {
                    ContentTypeId = item.ContentTypeId,
                    ContentName = item.ContentName,
                });
            });
            return contentmodellist;
        }

        /// <summary>
        /// Method to get the content type by content type id
        /// </summary>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<ContentTypeModel> GetByContentTypeId(long contenttypeId, ref ErrorResponseModel errorResponseModel)
        {
            var contentModelList = new List<ContentTypeModel>();
            errorResponseModel = new ErrorResponseModel();
            var contentEntityList = context.ContentTypes.Where(x => x.ContentTypeId == contenttypeId && x.IsDeleted == false).ToList();
            if (contentEntityList.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            contentEntityList.ForEach(item =>
            {
                contentModelList.Add(new ContentTypeModel
                {
                    ContentTypeId = item.ContentTypeId,
                    ContentName = item.ContentName,
                });
            });
            return contentModelList;
        }


        /// <summary>
        /// Get State By CountryId
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<StateModel> GetStateByCountyId(long countryId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var stateList = new List<StateModel>();
            var stateEntity = context.States.Where(x => x.CountryId == countryId && x.IsDeleted == false).ToList();
            foreach (var item in stateEntity)
            {
                var model = new StateModel();
                model.StateId = item.StateId;
                model.Name = item.Name;
                model.StateCode = item.StateCode;
                model.CountryId = item.CountryId;
                stateList.Add(model);
            }
            return stateList;
        }


        /// <summary>
        /// Get All City By StateId
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<CityModel> GetCityByStateId(long stateId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel = new ErrorResponseModel();
            var cityList = new List<CityModel>();
            var cityEntity = context.Cities.Where(x => x.StateId == stateId && x.IsDeleted == false ).ToList();
            foreach (var item in cityEntity)
            {
                var model = new CityModel();
                model.CityId = item.CityId;
                model.Name = item.Name;
                model.StateId = item.StateId;
                cityList.Add(model);
            }
            return cityList;
        }


        /// <summary>
        /// Get Subcategory By CategoryId
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<SubCategoryModel> GetSubcategoryByCategoryId(long categoryId, ref ErrorResponseModel errorResponseModel)
        {
            var subcategoryModelList = new List<SubCategoryModel>();
            errorResponseModel = new ErrorResponseModel();

            var subcategoryEntityList = context.SubCategories
                .Where(x => x.CategoryId == categoryId && x.IsDeleted == false)
                .ToList();

            if (subcategoryEntityList.Count == 0)
            {
                // If no subcategories are found, add a default "Other" to the list
                subcategoryModelList.Add(new SubCategoryModel
                {
                    SubCategoryId = 66, 
                    Name = "Other",
                    CategoryId = (int)categoryId, 
                    CreatedDate = DateTime.Now,
                });

                // Set error response accordingly if needed
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            else
            {
                // Map subcategoryEntityList to subcategoryModelList
                subcategoryEntityList.ForEach(item =>
                {
                    subcategoryModelList.Add(new SubCategoryModel
                    {
                        SubCategoryId = item.SubCategoryId,
                        Name = item.Name,
                        CategoryId = item.CategoryId,
                        CreatedDate = item.CreatedDate,
                        CreatedBy = item.CreatedBy,
                        UpdatedDate = item.UpdatedDate,
                        UpdatedBy = item.UpdatedBy,
                        IsDeleted = item.IsDeleted
                    });
                });
            }
            return subcategoryModelList;
        }


        /// <summary>
        /// Get all sports
        /// </summary>
        /// <param name="errorResponseModel"></param>
        /// <returns></returns>
        public List<SportModel> GetAllSports(ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel=new ErrorResponseModel();
            var sportEntity=context.SportMasters.ToList();
            if(sportEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            List<SportModel> sportModels = sportEntity.Select(sportEntity => new SportModel
            {
                SportId = sportEntity.SportId,
                SportName = sportEntity.SportName,
            }).ToList();

            return sportModels;
        }



        /// <summary>
        /// Method is used to get all Categories by sportId
        /// </summary>
        /// <param name="sportId"></param>
        /// <returns></returns>
        public List<CategoryModel> GetAllCategoryBySportId(long sportId, ref ErrorResponseModel errorResponseModel)
        {
            errorResponseModel=new ErrorResponseModel();
            var categoryEntity=context.Categories.Where(x=>x.SportId == sportId || x.SportId==null && x.IsDeleted==false).ToList();
            if(categoryEntity.Count == 0)
            {
                errorResponseModel.StatusCode = HttpStatusCode.NotFound;
                errorResponseModel.Message = GlobalConstants.NotFoundMessage;
            }
            List<CategoryModel> categoryModels = categoryEntity.Select(categoryEntity => new CategoryModel
            {
                CategoryId = categoryEntity.CategoryId,
                Name = categoryEntity.Name,
                Description= categoryEntity.Description,
                SportId=categoryEntity.SportId,
            }).OrderByDescending(x=>x.SportId).ToList();
            return categoryModels;
        }
    }
}
