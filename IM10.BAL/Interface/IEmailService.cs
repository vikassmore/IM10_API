using IM10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.BAL.Interface
{
    /// <summary>
    /// Interface used for email related operations
    /// </summary>
    public interface IEmailService
    {
        bool SendMail(EmailSenderModel emailSenderModel);

    }
}
