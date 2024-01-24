using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;

namespace IM10.BAL.Interface
{
    /// <summary>
    /// Interface used for emailsender related operations
    /// </summary>
    public interface IEmailSenderService
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendEmailAsync(string email, string subject, string message, Dictionary<string, MemoryStream> attachments);
        Task SendSmsAsync2(string phonenumber, string subject, string message);
        Task Execute(string email, string subject, string message);
        Task SendSmsAsync(string phonenumber, string userName, string otp);

        Task<MessageResource> SendTwilioSmsAsync(string phonenumber, string userName, string otp);
    }
}
