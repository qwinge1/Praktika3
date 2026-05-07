using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroControl.API.Models
{
    [Table("Сырье")]
    public class RawMaterial
    {
        [Key]
        public int ID { get; set; }
        public string Код { get; set; } = string.Empty;
        public string Наименование { get; set; } = string.Empty;
        public string? ЕдиницаИзмерения { get; set; }
        public string? Категория { get; set; }
    }
}
