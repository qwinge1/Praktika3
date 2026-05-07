using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroControl.API.Models
{
    [Table("ПроизводственныеЗаказы")]
    public class ProductionOrder
    {
        [Key]
        public int ID { get; set; }
        public string НомерЗаказа { get; set; } = string.Empty;
        public int ПродуктID { get; set; }
        public int РецептID { get; set; }
        public int ТехКартаID { get; set; }
        public decimal ПланКоличество_кг { get; set; }
        public string? Статус { get; set; }
        public DateTime? ПланДатаСтарта { get; set; }
    }
}
