using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanHangAPI.Models;
using System.IdentityModel.Tokens.Jwt;


namespace WebBanHangAPI.IServices
{
    public interface IJwtAuthenticationManager
    {
        //decode https://developer.okta.com/blog/2019/06/26/decode-jwt-in-csharp-for-authorization
        string Authenticate(NguoiDung username, string vaitro);
        JwtSecurityToken GetInFo(string token);
    }
}
