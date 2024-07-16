using IM10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Interface
{
    /// <summary>
    /// Interface used for FCM notification related operations
    /// </summary>
    public interface IFCMNotificationService
    {
        /// <summary>
        /// Method is used to save FCM notification token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string AddFCMNotificaion(FCMNotificationModel model);


       
    }
}
