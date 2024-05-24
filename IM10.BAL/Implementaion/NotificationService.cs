using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
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
        Task<ResponseModel> SendNotification(string DeviceId, long playerId, long contentId, string title, string description, int contentTypeId, string thumbnail, int categoryId);
        Task<ResponseModel> SendCommentNotification(string DeviceId, long contentId, long commentId, string message, int contentTypeId, int categoryId, bool IsPublic);

    }
    public class NotificationService : INotificationService
    {
        private readonly FcmNotificationSetting _notificationSetting;

        public NotificationService(IOptions<FcmNotificationSetting> settings)
        {
            _notificationSetting = settings.Value;
        }

       

        public async Task<ResponseModel> SendNotification(string DeviceId, long playerId, long contentId, string title, string description, int contentTypeId, string thumbnail, int categoryId)
        {
            ResponseModel response = new ResponseModel();
            int success;
            try
            {
                var projectid = "im10-f32bd";
                string accessToken = await GetAccessToken();
                var url = $"https://fcm.googleapis.com/v1/projects/{projectid}/messages:send";
                var data = new
                {
                    message = new
                    {
                        token = DeviceId,
                        data = new
                        {
                            playerId = playerId.ToString(),
                            contentId = contentId.ToString(),
                            title = title,
                            categoryId = categoryId.ToString(),
                            contentTypeId = contentTypeId.ToString(),
                            thumbnail = thumbnail,
                            message = description,
                        },
                    }
                };

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var json = JsonConvert.SerializeObject(data);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var result = await client.PostAsync(url, content);
                    var responseString = await result.Content.ReadAsStringAsync();
                    if (result.IsSuccessStatusCode)
                    {
                        dynamic jsonResponse = JsonConvert.DeserializeObject(responseString);
                        Console.WriteLine(jsonResponse + DeviceId);
                        response.IsSuccess = 1;
                        response.Message = jsonResponse.name;
                    }
                    else
                    {
                        response.IsSuccess = 0;
                        response.Message = "Error sending notification: " + responseString;
                        Console.WriteLine(responseString);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                response.IsSuccess = 0;
                response.Message = ex.Message;
            }
            return response;
        }



        public async Task<ResponseModel> SendCommentNotification(string DeviceId, long contentId, long commentId, string message, int contentTypeId, int categoryId, bool IsPublic)
        {
            ResponseModel response = new ResponseModel();
            int success;
            try
            {
                var projectid = "im10-f32bd";
                string accessToken = await GetAccessToken();
                var url = $"https://fcm.googleapis.com/v1/projects/{projectid}/messages:send";
                var data = new
                {
                    message = new
                    {
                        token = DeviceId,                       
                        data = new
                          {
                            contentId = contentId.ToString(),
                            commentId = commentId.ToString(),
                            title = "New Comment Arrived!",
                            message=message,
                            categoryId = categoryId.ToString(),
                            contentTypeId = contentTypeId.ToString(),
                            IsPublic = IsPublic.ToString()
                         },
                        /*notification = new
                        {
                            title = "New Comment Arrived!",
                            body = message
                        },*/

                    }
                };

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var json = JsonConvert.SerializeObject(data);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var result = await client.PostAsync(url, content);
                    var responseString = await result.Content.ReadAsStringAsync();
                    if (result.IsSuccessStatusCode)
                    {
                        dynamic jsonResponse = JsonConvert.DeserializeObject(responseString);
                        Console.WriteLine(responseString + json);
                        response.IsSuccess = 1;
                        response.Message = jsonResponse.name;
                    }
                    else
                    {
                        response.IsSuccess = 0;
                        response.Message = "Error sending notification: " + responseString;
                        Console.WriteLine(responseString);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                response.IsSuccess = 0;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<string> GetAccessToken()
        {
         //   string credentialPath = Path.Combine(Directory.GetCurrentDirectory());// "D:\\IM10NewRepository\\IM10.API\\firebase.json";

            string currentDirectory = Directory.GetCurrentDirectory();
            string fileName = "firebase.json";
            string credentialPath = Path.Combine(currentDirectory, fileName);


            if (string.IsNullOrEmpty(credentialPath))
            {
                throw new InvalidOperationException("Environment variable GOOGLE_APPLICATION_CREDENTIALS is not set.");
            }
            string[] scopes = { "https://www.googleapis.com/auth/firebase.messaging" };
            GoogleCredential credential;
            try
            {
                // Load the Google credential from the JSON file
                using (var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read))
                using (var reader = new StreamReader(stream))
                {
                    string jsonContent = await File.ReadAllTextAsync(credentialPath);
                    credential = GoogleCredential.FromJson(jsonContent).CreateScoped(scopes);
                }

                // Retrieve the access token
                var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
                return token;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during credential creation
                throw new InvalidOperationException("Error creating credential: " + ex.Message);
            }
        }
    }
}
