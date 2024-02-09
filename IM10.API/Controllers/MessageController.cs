using FirebaseAdmin.Messaging;
using IM10.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;

namespace IM10.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SendMessageAsync([FromBody] MessageRequest request)
        {
            try
            {
                var message = new Message()

                {
                    Notification = new Notification
                    {
                        Title = request.Title,
                        Body = request.Body,
                    },
                    Data = new Dictionary<string, string>()
                    {
                        ["FirstName"] = "John",
                        ["LastName"] = "Doe"
                    },
                    Token = request.DeviceToken
                };
                
                var messaging = FirebaseMessaging.DefaultInstance;
                var result = await messaging.SendAsync(message);

                if (!string.IsNullOrEmpty(result))
                {
                    // Message was sent successfully
                    return Ok("Message sent successfully!");
                }
                else
                {
                    // There was an error sending the message
                    throw new Exception("Error sending the message.");
                }
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,ex.Message);
            }
        }
    }
}
