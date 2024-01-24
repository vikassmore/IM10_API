using CorePush.Google;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using static IM10.Models.FirebaseModel;
using static IM10.Models.FirebaseModel.GoogleNotification;
using static IM10.Models.FirebaseModel.GoogleNotification1;

namespace IM10.BAL.Implementaion
{
    public interface INotificationService
    {
        Task<ResponseModel> SendNotification(string DeviceId, long playerId, long contentId, string title, string description, bool IsAndroidDevice);
        Task<ResponseModel> SendCommentNotification(string DeviceId, long contentId, long commentId, string message, bool IsAndroidDevice);

    }
    public class NotificationService : INotificationService
    {
        private readonly FcmNotificationSetting _notificationSetting;
        private readonly IM10DbContext _context;
        private readonly ConfigurationModel _configuration;

        public NotificationService(IOptions<FcmNotificationSetting> settings, IOptions<ConfigurationModel> hostName, IM10DbContext dbContext)
        {
            _notificationSetting = settings.Value;
            _configuration = hostName.Value;
            _context = dbContext;
        }

        public async Task<ResponseModel> SendNotification(string DeviceId, long playerId, long contentId, string title, string description, bool IsAndroidDevice)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                if (IsAndroidDevice == true)
                {
                    /* FCM Sender (Android Device) */
                    FcmSettings settings = new FcmSettings()
                    {
                        SenderId = _notificationSetting.SenderId,
                        ServerKey = _notificationSetting.ServerKey
                    };
                    HttpClient httpClient = new HttpClient();
                    string authorizationKey = string.Format("key={0}", settings.ServerKey);
                    string deviceToken = DeviceId;
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorizationKey);
                    httpClient.DefaultRequestHeaders.Accept
                            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    DataPayload dataPayload = new DataPayload();
                    dataPayload.PlayerId = playerId;
                    dataPayload.contentId = contentId;
                    dataPayload.Title = title;
                    dataPayload.Description = description;
                    GoogleNotification notification = new GoogleNotification();
                    notification.Data = dataPayload;
                    notification.Notification = dataPayload;

                    var fcm = new FcmSender(settings, httpClient);
                    var fcmSendResponse = await fcm.SendAsync(DeviceId, notification);

                    if (fcmSendResponse.IsSuccess())
                    {
                        response.IsSuccess = true;
                        response.Message = "Notification sent successfully";
                        notification.Data = dataPayload;
                        var pathToSave1 = Path.Combine(Directory.GetCurrentDirectory(), "Resources");
                        pathToSave1 = pathToSave1 + "\\";
                        using (StreamWriter w = File.AppendText(pathToSave1 + "log.txt"))
                        {
                            Log((response.Message).ToString(), w);
                        }
                        using (StreamReader r = File.OpenText("log.txt"))
                        {
                            DumpLog(r);
                        }
                        return response;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = fcmSendResponse.Results[0].Error;
                        var pathToSave1 = Path.Combine(Directory.GetCurrentDirectory(), "Resources");
                        pathToSave1 = pathToSave1 + "\\";
                        using (StreamWriter w = File.AppendText(pathToSave1 + "log.txt"))
                        {
                            Log((response.Message).ToString(), w);
                        }
                        using (StreamReader r = File.OpenText("log.txt"))
                        {
                            DumpLog(r);
                        }
                        return response;
                    }

                }
                else
                {
                    /* Code here for APN Sender (iOS Device) */
                    //var apn = new ApnSender(apnSettings, httpClient);
                    //await apn.SendAsync(notification, deviceToken);
                }
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error: {ex.Message}";
                var pathToSave1 = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "log.txt");
                // Log the detailed exception information
                using (StreamWriter w = File.AppendText(pathToSave1))
                {
                    Log($"Failed to send notification: {response.Message}", w);
                    // Log detailed exception information
                    Log($"Exception Type: {ex.GetType().FullName}", w);
                    Log($"Exception Message: {ex.Message}", w);
                    Log($"Stack Trace: {ex.StackTrace}", w);

                    // If it's an AggregateException, log details of inner exceptions
                    if (ex is AggregateException aggregateException)
                    {
                        foreach (var innerException in aggregateException.InnerExceptions)
                        {
                            Log($"Inner Exception Type: {innerException.GetType().FullName}", w);
                            Log($"Inner Exception Message: {innerException.Message}", w);
                            Log($"Inner Exception Stack Trace: {innerException.StackTrace}", w);
                        }
                    }
                }

                // Optionally, you can read and dump the log
                using (StreamReader r = File.OpenText(pathToSave1))
                {
                    DumpLog(r);
                }
                return response;
            }

        }
        public static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
            w.WriteLine("  :");
            w.WriteLine($"  :{logMessage}");
            w.WriteLine("-------------------------------");
        }

        public static void DumpLog(StreamReader r)
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
        }

        public async Task<ResponseModel> SendCommentNotification(string DeviceId, long contentId, long commentId, string message, bool IsAndroidDevice)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                if (IsAndroidDevice == true)
                {
                    /* FCM Sender (Android Device) */
                    FcmSettings settings = new FcmSettings()
                    {
                        SenderId = _notificationSetting.SenderId,
                        ServerKey = _notificationSetting.ServerKey
                    };
                    HttpClient httpClient = new HttpClient();
                    string authorizationKey = string.Format("key={0}", settings.ServerKey);
                    string deviceToken = DeviceId;
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorizationKey);
                    httpClient.DefaultRequestHeaders.Accept
                            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    DataPayload1 dataPayload1 = new DataPayload1();
                    //dataPayload.PlayerId = playerId;
                    dataPayload1.contentId = contentId;
                    dataPayload1.commentId = commentId;
                    dataPayload1.Title = message;
                    GoogleNotification1 notification1 = new GoogleNotification1();
                    notification1.Data1 = dataPayload1;
                    notification1.Notification1 = dataPayload1;

                    var fcm = new FcmSender(settings, httpClient);
                    var fcmSendResponse = await fcm.SendAsync(deviceToken, notification1);

                    if (fcmSendResponse.IsSuccess())
                    {
                        response.IsSuccess = true;
                        response.Message = "Notification sent successfully";
                        notification1.Data1 = dataPayload1;
                        var pathToSave1 = Path.Combine(Directory.GetCurrentDirectory(), "Resources");
                        pathToSave1 = pathToSave1 + "\\";
                        using (StreamWriter w = File.AppendText(pathToSave1 + "log.txt"))
                        {
                            Log((response.Message).ToString(), w);
                        }
                        using (StreamReader r = File.OpenText("log.txt"))
                        {
                            DumpLog(r);
                        }
                        return response;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = fcmSendResponse.Results[0].Error;
                        var pathToSave1 = Path.Combine(Directory.GetCurrentDirectory(), "Resources");
                        pathToSave1 = pathToSave1 + "\\";
                        using (StreamWriter w = File.AppendText(pathToSave1 + "log.txt"))
                        {
                            Log((response.Message).ToString(), w);
                        }
                        using (StreamReader r = File.OpenText("log.txt"))
                        {
                            DumpLog(r);
                        }
                        return response;
                    }

                }
                else
                {
                    /* Code here for APN Sender (iOS Device) */
                    //var apn = new ApnSender(apnSettings, httpClient);
                    //await apn.SendAsync(notification, deviceToken);
                }
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error: {ex.Message}";
                var pathToSave1 = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "log.txt");
                // Log the detailed exception information
                using (StreamWriter w = File.AppendText(pathToSave1))
                {
                    Log($"Failed to send notification: {response.Message}", w);
                    // Log detailed exception information
                    Log($"Exception Type: {ex.GetType().FullName}", w);
                    Log($"Exception Message: {ex.Message}", w);
                    Log($"Stack Trace: {ex.StackTrace}", w);

                    // If it's an AggregateException, log details of inner exceptions
                    if (ex is AggregateException aggregateException)
                    {
                        foreach (var innerException in aggregateException.InnerExceptions)
                        {
                            Log($"Inner Exception Type: {innerException.GetType().FullName}", w);
                            Log($"Inner Exception Message: {innerException.Message}", w);
                            Log($"Inner Exception Stack Trace: {innerException.StackTrace}", w);
                        }
                    }
                }
                // Optionally, you can read and dump the log
                using (StreamReader r = File.OpenText(pathToSave1))
                {
                    DumpLog(r);
                }
                return response;
            }
        }
    } 
}
