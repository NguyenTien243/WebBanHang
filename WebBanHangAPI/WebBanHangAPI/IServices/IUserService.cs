using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanHangAPI.ViewModels;

namespace WebBanHangAPI.IServices
{
    public interface IUserService
    {
        Task<bool> Authenticate(LoginRequest request);
        Task<bool> Register(RegisterRequest request);
    }
}
