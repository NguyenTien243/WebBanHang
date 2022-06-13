using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.ViewModels
{
    public class ThemMaGiamNgDung
    {
        [Required]
        public string? MaGiamGiaId { get; set; }
    }
}
