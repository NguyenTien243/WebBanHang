using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.ViewModels
{
    public class RequestSendEmaiResetPassword
    {
        public string email { get; set; }
        public string url { get; set; }
    }
}
