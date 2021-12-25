using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Net.Mail;
using System.Threading.Tasks;
using WebBanHangAPI.IServices;
using WebBanHangAPI.Models;
using MailKit.Net.Smtp;
using WebBanHangAPI.ViewModels;

namespace WebBanHangAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NameController : ControllerBase
    {

        private readonly IMailService _mailService;
        public NameController(WebBanHangAPIDBContext context, IMailService mailService)
        //public NameController(WebBanHangAPIDBContext context, ICustomAuthenticationManager customAuthenticationManager)
        {
            _mailService = mailService;
        //    this.customAuthenticationManager = customAuthenticationManager;

        }
        [Authorize]

        [HttpGet("name")]
        public IEnumerable<string> Get()
        {
            return new string[] { "New Jersey", "New Jorl" };
        }
        [HttpPost("sendEmailChinhThuc")]
        public async Task<IActionResult> SendMail2([FromForm] MailRequest request)
        {
            try
            {
                await _mailService.SendEmailAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet("sendmailResetPassword")]
        public IEnumerable<string> SendMailResetPassword()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Nguyen Tien", "ptshopk18@gmail.com"));
            message.To.Add(new MailboxAddress("Nguyễn Tiến", "nguyenquoctien243@gmail.com"));
            message.Subject = "test mail in asp.net core";
            message.Body = new TextPart("plain")
            {
                Text = "Xin chào Tiến" +
                "\r\n" +
                "Vui lòng nhấp vào link sau để khôi phục mật khẩu:"+ "https://www.youtube.com/watch?v=g8Be-jMjqu8"+
                "\r\n" +
                "Link có hiệu lực trong 3 phút2"

            };
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("ptshopk18@gmail.com", "TienTien2");
                client.Send(message);
                client.Disconnect(true);
            }
            return new string[] { "New Jersey", "New Jorl" };
        }
        [HttpGet("sendmail")]
        public IEnumerable<string> SendMail()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Nguyen Tien", "ptshopk18@gmail.com"));
            message.To.Add(new MailboxAddress("Nguyễn Tiến", "nguyenquoctien243@gmail.com"));
            message.Subject = "test mail in asp.net core";
            message.Body = new TextPart("plain")
            {
                Text = "hêloo word mail"
            };
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("ptshopk18@gmail.com", "TienTien2");
                client.Send(message);
                client.Disconnect(true);
            }    
            return new string[] { "New Jersey", "New Jorl" };
        }
        //[HttpPost("authenticate")]
        //public IActionResult Authenticate([FromBody] NguoiDung user)
        //{
        //    var token = customAuthenticationManager.Authenticate
        //        (user.tenDangNhap, user.matKhau);
        //    if (token == null)
        //        return Unauthorized();
        //    return Ok(token);
        //}
    }
}
