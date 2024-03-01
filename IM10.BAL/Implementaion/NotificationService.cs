using CorePush.Google;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IM10.BAL.Implementaion
{
    public interface INotificationService
    {
        Task<ResponseModel> SendNotification(string DeviceId, long playerId, long contentId, string title, string description, bool IsAndroidDevice, int contentTypeId,string thumbnail,int categoryId);
        Task<ResponseModel> SendCommentNotification(string DeviceId, long contentId, long commentId, string message, bool IsAndroidDevice,int contentTypeId, int categoryId,bool IsPublic);

    }
    public class NotificationService : INotificationService
    {
        private readonly FcmNotificationSetting _notificationSetting;
        private readonly IM10DbContext _context;
        private readonly ConfigurationModel _configuration;
        private static object fileLock = new object();


        public NotificationService(IOptions<FcmNotificationSetting> settings, IOptions<ConfigurationModel> hostName, IM10DbContext dbContext)
        {
            _notificationSetting = settings.Value;
            _configuration = hostName.Value;
            _context = dbContext;
        }

        private string GenerateCollapseKey()
        {
            // Use current timestamp to generate a unique collapse_key
            return DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        }
        public async Task<ResponseModel> SendNotification (string DeviceId, long playerId, long contentId, string title, string description, bool IsAndroidDevice, int contentTypeId, string thumbnail, int categoryId)
        {
            ResponseModel response = new ResponseModel();
            int success;
            try
            {
                string SenderId = _notificationSetting.SenderId;
                string ServerKey = _notificationSetting.ServerKey;
                string deviceId = DeviceId;
                string collapseKey = GenerateCollapseKey();
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Proxy = null;
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = deviceId,
                    collapse_key = collapseKey,
                    data = new
                    {
                        playerId = playerId.ToString(),
                        contentId = contentId.ToString(),
                        title = title,
                        description = description,
                        categoryId = categoryId,
                        contentTypeId = contentTypeId.ToString(),
                        thumbnail = thumbnail,
                        IsAndroidDevice = IsAndroidDevice,
                        message = "Approved Successfully"                        
                    },
                    notification = new
                    {
                        body = $"{description}",
                        title = $"{title}",
                        sound = "Enabled",                       
                        icon = "ic_launcher"                       
                    }
                };
                
                var json = System.Text.Json.JsonSerializer.Serialize(data);
                Console.WriteLine(json);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", ServerKey));
                tRequest.Headers.Add(string.Format("Sender: id={0}", SenderId));
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                string str = sResponseFromServer;
                                Console.WriteLine(str);
                                dynamic json1 = JsonConvert.DeserializeObject(str);
                                success = json1.success;
                                response.IsSuccess = success;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                response.IsSuccess= 0;
                response.Message= ex.Message;
                return response;
            }
            return response;
        }


        
        public async Task<ResponseModel> SendCommentNotification(string DeviceId, long contentId, long commentId, string message, bool IsAndroidDevice, int contentTypeId, int categoryId,bool IsPublic)
        {
            ResponseModel response = new ResponseModel();
            int success;
            try
            {
                string SenderId = _notificationSetting.SenderId;
                string ServerKey = _notificationSetting.ServerKey;
                string deviceId = DeviceId;
                string collapseKey = GenerateCollapseKey();
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Proxy = null;
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = deviceId,
                    collapse_key = collapseKey,
                    data = new
                    {
                        contentId = contentId,
                        commentId = commentId,
                        message = message,
                        categoryId = categoryId,
                        isAndroidDevice = IsAndroidDevice,
                        contentTypeId = contentTypeId,
                        IsPublic = IsPublic
                    },
                    notification = new
                    {
                        body = $"{message}",
                        title = "New Comment Arrived!",
                        sound = "Enabled",
                        icon = "ic_launcher"                       
                    }                                
            };
                Console.WriteLine(data);
                var json = System.Text.Json.JsonSerializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", ServerKey));
                tRequest.Headers.Add(string.Format("Sender: id={0}", SenderId));
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                string str = sResponseFromServer;
                                Console.WriteLine(str);
                                dynamic json1 = JsonConvert.DeserializeObject(str);
                                success = json1.success;
                                response.IsSuccess = success;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                response.IsSuccess = 0;
                response.Message = ex.Message;
                return response;
            }
            Console.WriteLine(response);
            return response;
            
        }
    } 
}
