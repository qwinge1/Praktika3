using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroControl.API.Models
{
    [Table("ПроизводственныеПартии")]
    public class ProductionBatch
    {
        [Key]
        public int ID { get; set; }
        public string НомерПартии { get; set; } = string.Empty;
        public int ЗаказID { get; set; }
        public DateTime? ВремяСтарта { get; set; }
        public DateTime? ВремяОкончания { get; set; }
        public string? Статус { get; set; }
        public decimal? ФактКоличество_кг { get; set; }
        public int? ТекущийШагID { get; set; }

        // Новые поля для лабораторного контроля
        public string? ЛабораторныйСтатус { get; set; }      // "ожидает", "одобрена", "заблокирована"
        public string? КомментарийРешения { get; set; }
        public string? РешениеПринял { get; set; }
        public DateTime? ДатаРешения { get; set; }
        [NotMapped]
        public bool HasTest { get; set; }

        [NotMapped]
        public DateTime? LastTestDate { get; set; }
        public ICollection<BatchStepExecution> ВыполнениеШагов { get; set; } = new List<BatchStepExecution>();
    }
}