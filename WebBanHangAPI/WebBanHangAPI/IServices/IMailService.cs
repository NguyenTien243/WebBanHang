using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanHangAPI.ViewModels;

namespace WebBanHangAPI.IServices
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
