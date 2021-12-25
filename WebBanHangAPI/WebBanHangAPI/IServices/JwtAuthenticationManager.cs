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
        //https://www.youtube.com/watch?v=-N6O2rtCdI8
        private readonly IList<NguoiDung> users = new List<NguoiDung>
        {
           new NguoiDung {tenDangNhap = "test1",matKhau = "password1",VaiTroId = "Admin"},
           new NguoiDung {tenDangNhap = "test2",matKhau = "password2",VaiTroId = "User"},
        };

        private readonly IDictionary<string, Tuple<string, string>> tokens = new Dictionary<string, Tuple<string, string>>();

        private readonly string key;
        public IDictionary<string, Tuple<string, string>> Tokens => tokens;
      
        public JwtAuthenticationManager(string key)
        {
            this.key = key;
           
        }

        public string Authenticate(string username, string password)
        {
            if(!users.Any(u => u.tenDangNhap == username && u.matKhau == password))
            {
                return null;
            }
            var token = Guid.NewGuid().ToString();
            tokens.Add(token, new Tuple<string, string>(username, users.First(u=>u.tenDangNhap == username && u.matKhau == password).VaiTroId));
            return null;
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
        //https://www.youtube.com/watch?v=vWkPdurauaA
        public string TokenResetPassword(NguoiDung nguoidung, string vaitro)
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
                Expires = DateTime.Now.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public JwtSecurityToken GetInFo(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken resulttoken = null;
            try
            {
                resulttoken = handler.ReadJwtToken(token.Split(" ", 2)[1]);
            }
            catch (IndexOutOfRangeException e)
            {
                throw new InvalidOperationException("Không đọc được token");
            }
         
            return resulttoken;
        }
    }
}
