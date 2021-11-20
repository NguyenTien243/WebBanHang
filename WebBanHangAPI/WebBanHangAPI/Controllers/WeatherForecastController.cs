using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanHangAPI.Models;

namespace WebBanHangAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<LoaiSanPham> Get()
        {   
            //var _context = new WebBanHangAPIDBContext();
            //LoaiSanPham sp = new LoaiSanPham();
            //sp.QuizName = "tienvip";
            //_context.LoaiSanPhams.Add(sp);
            //_context.SaveChanges();
            return new List<LoaiSanPham>();
        }
    }
}
