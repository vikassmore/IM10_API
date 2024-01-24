using IM10.BAL.Interface;
using IM10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Implementaion
{
    /// <summary>
    /// This is implementation  for the sendmail operations 
    /// </summary>
    public class EmailService 
    {
        public bool SendMail(EmailSenderModel emailSenderModel, SmtpSettingModel settingsModel)
        {
            try
            {
                var fromAddress = new MailAddress(settingsModel.from);
                var toAddress = new MailAddress(emailSenderModel.ToAddress);
                var smtp = new SmtpClient
                {
                    Host = settingsModel.host,
                    Port = settingsModel.port,
                    EnableSsl = settingsModel.enableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = settingsModel.defaultCredentials,
                    Credentials = new NetworkCredential(fromAddress.Address, settingsModel.password),

                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = emailSenderModel.Subject,
                    Body = emailSenderModel.Body,
                    IsBodyHtml = emailSenderModel.isHtml,
                })
                {
                    smtp.Send(message);
                    emailSenderModel.sentStatus = true;
                }
            }
            catch (Exception ex)
            {
                emailSenderModel.sentStatus = false;
            }
            return emailSenderModel.sentStatus;
        }
    }
}
