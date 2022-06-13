using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebBanHangAPI.Models;
using WebBanHangAPI.ViewModels;

namespace WebBanHangAPI.ValidationAttributes
{
    public class MaGiamGia_NgayHetHanPhaiSauNgayBatDauAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var maGiaGia = validationContext.ObjectInstance as MaGiamGiaModel;

            if (!maGiaGia.KTNgayHetHanPhaiSauNgayBatDau()) return new ValidationResult("Ngay Het Han Phai Sau Hoac Bang Ngay Bat Dau!");
            return ValidationResult.Success;
        }
    }
}
