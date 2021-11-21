using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanHangAPI.ViewModels;

namespace WebBanHangAPI.IServices
{
    public class UserService : IUserService
    {
        public Task<bool> Authenticate(LoginRequest request)
        {
            throw new NotImplementedException();
        }

        //private readonly ApplicationDbContext _context;
        //public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager);
        //public async Task<bool> Authenticate(LoginRequest request)
        //{
        //   // var user = 
        //}

        public Task<bool> Register(RegisterRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
