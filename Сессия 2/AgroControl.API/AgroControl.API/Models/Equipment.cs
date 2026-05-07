using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroControl.API.Models
{
    [Table("Оборудование")]
    public class Equipment
    {
        [Key]
        public int ID { get; set; }
        public string Наименование { get; set; } = string.Empty;
        public string? НомерЛинии { get; set; }
        public string? Тип { get; set; }
    }
}
