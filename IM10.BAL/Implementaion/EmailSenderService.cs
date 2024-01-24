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
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
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
        private ConfigurationModel _configuration;

        private SMSSettingModel _sMSSettingModel;
        /// <summary>
        /// Creating constructor and injection dbContext
        /// </summary>
        /// <param name="emailSettings"></param>
        public EmailSenderService(IOptions<EmailSettings> emailSettings, IOptions<ConfigurationModel> hostName, IOptions<SMSSettingModel> sMSSettingModel)
        {
            _emailSettings = emailSettings.Value;
            this._configuration = hostName.Value;
            _sMSSettingModel = sMSSettingModel.Value;
        }

        public EmailSettings _emailSettings { get; }
        public EmailSettings _emailSettings1 { get; }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            SendEmail(email, subject, message);
            return Task.FromResult(0);
        }

        private void SendEmail(string email, string subject, string message)
        {
            throw new NotImplementedException();
        }

        public async Task SendEmailAsync(string email, string subject, string message, Dictionary<string, MemoryStream> attachments)
        {
            await SendEmail(email, subject, message, attachments);
        }

        private Task SendEmail(string email, string subject, string message, Dictionary<string, MemoryStream> attachments)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Send SMS on mobile number
        /// </summary>
        /// <param name="phonenumber"></param>
        /// <param name="userName"></param>
        /// <param name="otp"></param>
        /// <returns></returns>
        public Task SendSmsAsync(string phonenumber, string userName, string otp)
        {
            using (var web = new WebClient())
            {
                try
                {
                    web.Proxy = null;
                    // MeshBA's sms gateway
                    string url = "http://103.233.79.217/api/mt/SendSMS?user=MeshBA&password=Mahesh@123&senderid=MESHBA&channel=Trans&DCS=0&flashsms=0&number="
                        + phonenumber +
                        "&text= Dear User, "
                        + otp +
                        " is the OTP for your login at Akshaya Agri app. In case you have not requested this, please contact us. - MeshBA&route=8&peid=1701159146303386050&dlttemplateid=1707165114455193951";
                   // string url = "http://bulksmsindia.mobi/sendurlcomma.aspx?user=20094643&pwd=serum@2020&senderid=SIIPLH&mobileno=9637871701&msgtext=SMSBell-Rx:%20You%20have%20received%20a%20SMS,%20from%20device:%20BLD%2012%20Warehouse%20Recorder%20with%20Sub:%20BLD%2012%20Warehouse%20Recorder.%20TF%20Cold%20Room-6%20(-24%20to%20-34%20%C2%B0C)%20Temperature%20High.-SERUMH&smstype=13&pe_id=1701163878251642932&template_id=1707164093652897825";
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    string result = web.DownloadString(url);
                }
                catch (Exception ex)
                {
                    //Catch and show the exception if needed. Donot supress. :)  

                }
            }
            return Task.FromResult(0);
        }




        public Task SendSmsAsync2(string phonenumber, string userName, string otp)
        {
            using (var web = new WebClient())
            {
                try
                {
                    string url = "http://103.233.79.217/api/mt/SendSMS?user=MeshBA&password=Mahesh@123&senderid=MESHBA&channel=Trans&DCS=0&flashsms=0&number=" +
                    phonenumber +
                    "&text=Dear User ," + otp + " " +
                    "is the OTP for your registration at IM10 app " +
                    //"In case you have not requested this, please contact us. -" +
                    "MeshBA&route=8&peid=1701159146303386050&dlttemplateid=1707165114455193951";

                    //MeshBA's sms gateway
                    //string url = "http://103.233.79.217/api/mt/SendSMS?user=MeshBA&password=Mahesh@123&senderid=MESHBA&channel=Trans&DCS=0&flashsms=0&number=" +
                    //    phonenumber +
                    //    "&text= " +
                    //    "SMSBell-Rx: You have received a SMS, from device: Akshaya Agri with Sub: The OTP for your login is " +
                    //    otp +
                    //    " MeshBA&route=8&peid=1701159146303386050&dlttemplateid=1707162090147685530";


                    string result = web.DownloadString(url);

                }
                catch (Exception ex)
                {
                    //Catch and show the exception if needed. Donot supress. :)  

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
                                Console.WriteLine(ex.Message);

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
        public async Task Execute(string email, string subject, string message, Dictionary<string, MemoryStream> attachments)
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
                try
                {
                    mail.CC.Add(new MailAddress(_emailSettings.CcEmail));
                }
                catch { }
                mail.Subject = subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;
                foreach (var item in attachments)
                {
                    System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);
                    mail.Attachments.Add(new Attachment(item.Value, item.Key, ct.MediaType));
                }
                using (SmtpClient smtp = new SmtpClient(_emailSettings.PrimaryDomain, _emailSettings.PrimaryPort))
                {
                    smtp.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }

            }
            catch (Exception ex)
            {
                //do something here
            }
        }

        private void LogError(Exception ex)
        {
            string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += string.Format("Message: {0}", ex.Message);
            message += Environment.NewLine;
            message += string.Format("StackTrace: {0}", ex.StackTrace);
            message += Environment.NewLine;
            message += string.Format("Source: {0}", ex.Source);
            message += Environment.NewLine;
            message += string.Format("TargetSite: {0}", ex.TargetSite.ToString());
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;

            var currentFolder = Directory.GetCurrentDirectory();

            using (StreamWriter writer = new StreamWriter(currentFolder, true))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }

        public async Task<MessageResource> SendTwilioSmsAsync(string phonenumber, string userName, string otp)
        {
            try
            {
                TwilioClient.Init(_sMSSettingModel.AccountSID, _sMSSettingModel.AuthToken);

                var result = await MessageResource.CreateAsync(
                    body: "Dear User, " + otp + " is the OTP for your login at IM10 App. In case you have not requested this, please contact us. - MeshBA",
                    from: new PhoneNumber(_sMSSettingModel.PhoneNumber),
                    to: new PhoneNumber("+91" + phonenumber)
                    );
                return result;
            }
            catch(Exception ex)
            {
                return null;
            }

        }

    }
}
