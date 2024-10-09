using IM10.BAL.Interface;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using IM10.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace IM10.BAL.Implementaion
{
    /// <summary>
    /// This is implementation  for the Email operations 
    /// </summary>
    public class EmailSenderService : IEmailSenderService
    {
        /// <summary>
        /// Creating constructor and injection dbContext
        /// </summary>
        /// <param name="emailSettings"></param>
        public EmailSenderService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public EmailSettings _emailSettings { get; }
        
        /// <summary>
        /// Send SMS on mobile number
        /// <param name="phonenumber"></param>
        /// <param name="userName"></param>
        /// <param name="otp"></param>
        /// </summary>
        
        public Task SendSmsAsync(string phonenumber, string userName, string otp)
        {
            using (var web = new WebClient())
            {
                try
                {
                    web.Proxy = null;
                    // MeshBA's sms gateway                   
                    string url = "https://sms6.rmlconnect.net:8443/bulksms/messagesubmit?username=SerumIndia&password=y92E-%5BlP&dlr=1&destination="
                                    + phonenumber
                                    + "&source=MESHBA&message=Dear User, "
                                    + otp + " "
                                    + "is the OTP for your login. In case you have not requested this, please contact us. - MeshBA&entityid=1701159146303386050&tempid=1707171283812216615";
                    
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    string result = web.DownloadString(url);
                    Console.WriteLine(result);                 
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return Task.FromResult(0);
        }


        public async Task Execute(string email, string subject, string message)
        {
            try
            {
                string toEmail = string.IsNullOrEmpty(email)
                                 ? _emailSettings.ToEmail
                                 : email;
                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.UsernameEmail, "IM10")
                };
                mail.To.Add(new MailAddress(toEmail));
                mail.Subject = subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                try
                {
                    using (SmtpClient smtp = new SmtpClient(_emailSettings.PrimaryDomain, _emailSettings.PrimaryPort))
                    {
                        try
                        {
                            smtp.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
                            smtp.EnableSsl = _emailSettings.EnableSsl;
                            smtp.UseDefaultCredentials = _emailSettings.UseDefaultCredentials;
                            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                            await smtp.SendMailAsync(mail);
                        }
                        catch (SmtpException ex)
                        {
                            try
                            {
                                using (StreamWriter w = File.AppendText("log.txt"))
                                {
                                    Log(ex.Message, w);
                                    Log(ex.InnerException.Message, w);
                                }
                                using (StreamReader r = File.OpenText("log.txt"))
                                {
                                    DumpLog(r);
                                }
                            }
                            catch (Exception ex1)
                            {
                                throw;
                            }
                            await Task.FromResult(ex.Message);
                        }
                    }
                }
                catch (SmtpException ex)
                {
                    await Task.FromResult(ex.Message);
                }
            }
            catch (SmtpException ex)
            {
                await Task.FromResult(ex.Message);
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

    }
}
