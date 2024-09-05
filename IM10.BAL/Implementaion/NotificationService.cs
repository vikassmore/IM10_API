using IM10.BAL.Interface;
using IM10.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;
using System.Text;
using System.Net;
using Google.Apis.Auth.OAuth2;
using System.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using System.Runtime;
using System;
using FirebaseAdmin.Messaging;
using FirebaseAdmin;

namespace IM10.BAL.Implementaion
{

    public interface INotificationService
    {
        Task<ResponseModel> SendNotification(string DeviceId, long playerId, long contentId, string title, string description, int contentTypeId, string thumbnail, int categoryId);
        Task<ResponseModel> SendCommentNotification(string DeviceId, long contentId, long commentId, string message, int contentTypeId, int categoryId, bool IsPublic);
    }

    public class NotificationService : INotificationService
    {
        public IHostingEnvironment _hostingEnvironment;
        private readonly FcmNotificationSetting _notificationSetting;
                
        public NotificationService( IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;       

        }

        public async Task<ResponseModel> SendNotification(string DeviceId, long playerId, long contentId, string title, string description, int contentTypeId, string thumbnail, int categoryId)
        {
            ResponseModel response = new ResponseModel();
            // string accessToken = await GetAccessToken();

            int success;
            try
            {
                string accessToken = await GetAccessTokenWithTimeout();
                var projectid = "";
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

                using (var clienthandler = new System.Net.Http.HttpClientHandler())
                {
                    var client = new System.Net.Http.HttpClient(clienthandler);
                    client.BaseAddress = new Uri(url);

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
                        LogErrorDetails(DeviceId, accessToken, responseString, false, json);
                       
                    }
                    else
                    {
                        response.IsSuccess = 0;
                        response.Message = "Error sending notification: " + responseString;

                        Console.WriteLine(responseString);
                        LogErrorDetails(DeviceId, accessToken, responseString, false, json);                        
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


        public async Task<ResponseModel> SendCommentNotification(string DeviceId, long contentId, long commentId, string message, int contentTypeId, int categoryId, bool IsPublic)
        {
            ResponseModel response = new ResponseModel();
            int success;
            //string accessToken = await GetAccessToken();

            try
            {
                string accessToken = await GetAccessTokenWithTimeout();

                var projectid = "";
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
                            message = message,
                            categoryId = categoryId.ToString(),
                            contentTypeId = contentTypeId.ToString(),
                            IsPublic = IsPublic.ToString()
                        },
                    }
                };
                using (var clientHandler = new System.Net.Http.HttpClientHandler())
                {
                    var client = new System.Net.Http.HttpClient(clientHandler);
                    client.BaseAddress = new Uri(url);

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
                        LogErrorDetails(DeviceId, accessToken, responseString, false, json);                        
                    }
                    else
                    {
                        response.IsSuccess = 0;
                        response.Message = "Error sending notification: " + responseString;
                        Console.WriteLine(responseString);
                        LogErrorDetails(DeviceId, accessToken, responseString, false, json);
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


        public async Task<string> GetAccessToken()
        {
            var folderName = Path.Combine("Resources", "JsonFolder");
            var credentialPath = Path.Combine(Directory.GetCurrentDirectory(), folderName + "\\firebase.json");

            if (string.IsNullOrEmpty(credentialPath))
            {               
                LogErrorDetails("", "", "Error creating credential: " + "something went wrong", true);
                throw new InvalidOperationException("Environment variable GOOGLE_APPLICATION_CREDENTIALS is not set.");
            }

            try
            {
                var bearertoken = "";
                using (var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read))
                {                                      
                    string[] scopes = new[] { "https://www.googleapis.com/auth/firebase.messaging" };
                    GoogleCredential googleCredential = GoogleCredential.FromStream(stream).CreateScoped(scopes);
                    bearertoken = await googleCredential.UnderlyingCredential.GetAccessTokenForRequestAsync();                
                    return bearertoken;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<string> GetAccessTokenWithTimeout()
        {
            try
            {
                var task = GetAccessToken();
                var timeoutTask = Task.Delay(TimeSpan.FromMinutes(5));

                var completedTask = await Task.WhenAny(task, timeoutTask);

                if (completedTask == task)
                {
                    return await task;
                }
                else
                {
                    throw new TimeoutException();
                }

            }
            catch (Exception ex)
            {
                
                throw;
            }
        }


        private void LogErrorDetails(string deviceId, string accessToken, string errorMessage, bool isException, string dataPayload = null)
        {

            string filepath = Path.Combine(_hostingEnvironment.ContentRootPath, "Resources", "ErrorAuditLogFile");

            // Ensure the directory exists
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }

            // Create a unique log file name with timestamp
            string logFileName = "log_" + DateTime.Now.ToString("dd_MMM_yyyy") + ".txt";
            string logFilePath = Path.Combine(filepath, logFileName);

            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine($"DeviceId: {deviceId}");
                    logMessage.AppendLine($"AccessToken: {accessToken}");
                    logMessage.AppendLine($"Error: {errorMessage}");
                    logMessage.AppendLine($"IsException: {isException}");
                    logMessage.AppendLine($"DataPayload: {dataPayload}");
                    Log(logMessage, writer);
                    writer.WriteLine(logMessage.ToString());
                }
                Console.WriteLine("Log written successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write to log file: {ex.Message}");
                return; // Exit the method if logging fails
            }

            try
            {
                using (StreamReader r = File.OpenText(logFilePath))
                {
                    DumpLog(r);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to read from log file: {ex.Message}");
            }
        }

        public static void Log(StringBuilder logMessage, TextWriter w)
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


    }
}

