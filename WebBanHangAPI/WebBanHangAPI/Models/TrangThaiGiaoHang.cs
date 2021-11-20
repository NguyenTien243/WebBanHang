using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanHangAPI.Models
{
    public class TrangThaiGiaoHang
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string TrangThaiGiaoHangId { get; set; }
        public string tenTrangThai { get; set; }
        public ICollection<HoaDon> HoaDons { get; set; }
    }
}