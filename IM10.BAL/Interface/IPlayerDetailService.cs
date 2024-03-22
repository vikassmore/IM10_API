using IM10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Interface
{
    /// <summary>
    /// Interface used for playerDetails related operations
    /// </summary>

    public interface IPlayerDetailService
    {
        /// <summary>
        /// Method is used to get playerDetails by id
        /// </summary>
        /// <param name="PlayerId"></param>
        /// <returns></returns>
        PlayerSportsModel GetPlayerDetailById(long playerId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get playerDetails by playerid
        /// </summary>
        /// <param name="PlayerId"></param>
        /// <returns></returns>
        PlayerDetailModel GetPlayerDetailByPlayerId(long playerId, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to get all playerDetails
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<PlayerDetailModel> GetAllPlayerDetail(ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to add playerDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string AddPlayerDetail(PlayerDetailModel model, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to Edit playerDetails
        /// </summary>
        /// <param name="playerModel"></param>
        /// <returns></returns>
        string EditPlayerDetail(PlayerDetailModel playerModel, ref ErrorResponseModel errorResponseModel);


        /// <summary>
        /// Method is used to delete playerDetails
        /// </summary>
        /// <param name="PlayerId"></param>
        /// <returns></returns>
        string DeletePlayerDetail(long PlayerId, ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to add player data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string AddPlayerData(List<PlayerDataModel> model, ref ErrorResponseModel errorResponseModel);
        /// <summary>
        /// Method is used to get all player Data
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        List<PlayerDataModel> GetAllPlayerData(ref ErrorResponseModel errorResponseModel);

        /// <summary>
        /// Method is used to delete player Data
        /// </summary>
        /// <param name="playerDataId"></param>
        /// <returns></returns>
        string DeletePlayerData(long playerDataId, ref ErrorResponseModel errorResponseModel);
    }
}
