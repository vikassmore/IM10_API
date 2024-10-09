using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Interface
{
    /// <summary>
    /// Interface used for emailsender related operations
    /// </summary>
    public interface IEmailSenderService
    {
        Task Execute(string email, string subject, string message);
        Task SendSmsAsync(string phonenumber, string userName, string otp);

    }
}
