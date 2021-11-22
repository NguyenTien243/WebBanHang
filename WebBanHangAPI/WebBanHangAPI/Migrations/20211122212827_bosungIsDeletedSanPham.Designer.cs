﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebBanHangAPI.Models;

namespace WebBanHangAPI.Migrations
{
    [DbContext(typeof(WebBanHangAPIDBContext))]
    [Migration("20211122212827_bosungIsDeletedSanPham")]
    partial class bosungIsDeletedSanPham
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("WebBanHangAPI.Models.ChiTietHD", b =>
                {
                    b.Property<string>("HoaDonId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("SanPhamId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("giaTien")
                        .HasColumnType("int");

                    b.Property<int>("giamGia")
                        .HasColumnType("int");

                    b.Property<int>("soLuong")
                        .HasColumnType("int");

                    b.Property<int>("tongTien")
                        .HasColumnType("int");

                    b.HasKey("HoaDonId", "SanPhamId");

                    b.HasIndex("SanPhamId");

                    b.ToTable("ChiTietHDs");
                });

            modelBuilder.Entity("WebBanHangAPI.Models.GioHang", b =>
                {
                    b.Property<string>("NguoiDungId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("SanPhamId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("soLuong")
                        .HasColumnType("int");

                    b.HasKey("NguoiDungId", "SanPhamId");

                    b.HasIndex("SanPhamId");

                    b.ToTable("GioHangs");
                });

            modelBuilder.Entity("WebBanHangAPI.Models.HoaDon", b =>
                {
                    b.Property<string>("HoaDonId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("NguoiDungId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("TrangThaiGiaoHangId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("daThanhToan")
                        .HasColumnType("bit");

                    b.Property<string>("diaChiGiaoHang")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ngayXuatDon")
                        .HasColumnType("datetime2");

                    b.Property<bool>("thanhToanOnline")
                        .HasColumnType("bit");

                    b.Property<double>("tongHoaDon")
                        .HasColumnType("float");

                    b.HasKey("HoaDonId");

                    b.HasIndex("NguoiDungId");

                    b.HasIndex("TrangThaiGiaoHangId");

                    b.ToTable("HoaDons");
                });

            modelBuilder.Entity("WebBanHangAPI.Models.LoaiSanPham", b =>
                {
                    b.Property<string>("LoaiSanPhamId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("tenLoaiSP")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LoaiSanPhamId");

                    b.ToTable("LoaiSanPhams");
                });

            modelBuilder.Entity("WebBanHangAPI.Models.NguoiDung", b =>
                {
                    b.Property<string>("NguoiDungId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("VaiTroId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("conHoatDong")
                        .HasColumnType("bit");

                    b.Property<string>("diaChi")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("gioiTinh")
                        .HasColumnType("bit");

                    b.Property<string>("matKhau")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("sDT")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("tenDangNhap")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("tenNguoiDung")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("NguoiDungId");

                    b.HasIndex("VaiTroId");

                    b.ToTable("NguoiDungs");
                });

            modelBuilder.Entity("WebBanHangAPI.Models.SanPham", b =>
                {
                    b.Property<string>("SanPhamId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoaiSanPhamId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("giaTien")
                        .HasColumnType("float");

                    b.Property<int>("giamGia")
                        .HasColumnType("int");

                    b.Property<string>("hinhAnh")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("moTa")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("soLuongConLai")
                        .HasColumnType("int");

                    b.Property<string>("tenSP")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SanPhamId");

                    b.HasIndex("LoaiSanPhamId");

                    b.ToTable("SanPhams");
                });

            modelBuilder.Entity("WebBanHangAPI.Models.TrangThaiGiaoHang", b =>
                {
                    b.Property<string>("TrangThaiGiaoHangId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("tenTrangThai")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TrangThaiGiaoHangId");

                    b.ToTable("TrangThaiGiaoHangs");
                });

            modelBuilder.Entity("WebBanHangAPI.Models.VaiTro", b =>
                {
                    b.Property<string>("VaiTroId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("tenVaiTro")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("VaiTroId");

                    b.ToTable("VaiTros");
                });

            modelBuilder.Entity("WebBanHangAPI.Models.ChiTietHD", b =>
                {
                    b.HasOne("WebBanHangAPI.Models.HoaDon", "HoaDon")
                        .WithMany("ChiTietHDs")
                        .HasForeignKey("HoaDonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebBanHangAPI.Models.SanPham", "SanPham")
                        .WithMany("ChiTietHDs")
                        .HasForeignKey("SanPhamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WebBanHangAPI.Models.GioHang", b =>
                {
                    b.HasOne("WebBanHangAPI.Models.NguoiDung", "NguoiDung")
                        .WithMany("GioHangs")
                        .HasForeignKey("NguoiDungId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebBanHangAPI.Models.SanPham", "SanPham")
                        .WithMany("GioHangs")
                        .HasForeignKey("SanPhamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WebBanHangAPI.Models.HoaDon", b =>
                {
                    b.HasOne("WebBanHangAPI.Models.NguoiDung", "NguoiDung")
                        .WithMany("HoaDonsKhachHang")
                        .HasForeignKey("NguoiDungId");

                    b.HasOne("WebBanHangAPI.Models.TrangThaiGiaoHang", "TrangThaiGiaoHang")
                        .WithMany("HoaDons")
                        .HasForeignKey("TrangThaiGiaoHangId");
                });

            modelBuilder.Entity("WebBanHangAPI.Models.NguoiDung", b =>
                {
                    b.HasOne("WebBanHangAPI.Models.VaiTro", "VaiTro")
                        .WithMany("NguoiDungs")
                        .HasForeignKey("VaiTroId");
                });

            modelBuilder.Entity("WebBanHangAPI.Models.SanPham", b =>
                {
                    b.HasOne("WebBanHangAPI.Models.LoaiSanPham", "LoaiSanPham")
                        .WithMany("SanPhams")
                        .HasForeignKey("LoaiSanPhamId");
                });
#pragma warning restore 612, 618
        }
    }
}
