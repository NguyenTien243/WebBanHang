using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.Models
{
    public class WebBanHangAPIDBContext : DbContext
    {
        public DbSet<LoaiSanPham> LoaiSanPhams { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<ChiTietHD> ChiTietHDs { get; set; }
        public DbSet<TrangThaiGiaoHang> TrangThaiGiaoHangs { get; set; }
        public DbSet<VaiTro> VaiTros { get; set; }
        public DbSet<GioHang> GioHangs { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlServer(@"Data Source=.;Initial Catalog=WebBanHang;Integrated Security=True");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ChiTietHD>().HasKey(ct => new { ct.HoaDonId, ct.SanPhamId });
            modelBuilder.Entity<GioHang>().HasKey(gh => new { gh.NguoiDungId, gh.SanPhamId });

        }
    }
}
