using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebBanHangAPI.Models;

namespace WebBanHangAPI.IServices
{
    public class JwtAuthenticationManager : IJwtAuthenticationManager
    {

        private readonly IDictionary<string, string> users = new Dictionary<string, string>
        {
            { "test1","password1"}, {"test2","password2"}
        };
        private readonly string key;
      
        public JwtAuthenticationManager(string key)
        {
            this.key = key;
           
        }
        // dia chi, full name, sodienthoai,email
        //https://www.youtube.com/watch?v=vWkPdurauaA
        public string Authenticate(NguoiDung nguoidung,string vaitro)
        {
                //if (!users.Any(u => u.Key == username && u.Value == password))
                //{
                //    return null;
                //}
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("nguoiDungId", nguoidung.NguoiDungId),
                    new Claim("tenNguoiDung", nguoidung.tenNguoiDung),
                    new Claim("diaChi", nguoidung.diaChi),
                    new Claim("sDT", nguoidung.sDT),
                    new Claim("email", nguoidung.email),
                    new Claim("vaiTro", vaitro),
                }),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token); 
        }

        public NguoiDung GetInFo(string token)
        {
            throw new NotImplementedException();
        }
    }
}
