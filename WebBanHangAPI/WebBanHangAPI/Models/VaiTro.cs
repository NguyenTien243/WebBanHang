using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHangAPI.Models
{
    public class VaiTro
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string VaiTroId { get; set; }
        public string tenVaiTro { get; set; }
        public ICollection<NguoiDung> NguoiDungs { get; set; }
    }
}
