using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AgroControl.API.Models
{
    [Table("Продукция")]
    public class Product
    {
        [Key]
        public int ID { get; set; }
        public string Код { get; set; } = string.Empty;
        public string Наименование { get; set; } = string.Empty;
        public string? Тип { get; set; }
        public string? ФормаВыпуска { get; set; }
        public string? Статус { get; set; }
    }
}