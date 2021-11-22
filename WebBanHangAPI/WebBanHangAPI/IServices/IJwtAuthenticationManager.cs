using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanHangAPI.Models;

namespace WebBanHangAPI.IServices
{
    public interface IJwtAuthenticationManager
    {
        string Authenticate(NguoiDung username, string vaitro);
    }
}
