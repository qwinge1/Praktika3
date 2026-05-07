using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroControl.API.Models
{
    [Table("ВыполнениеШаговПартии")]
    public class BatchStepExecution
    {
        [Key]
        public int ID { get; set; }
        public int ПартияПроизводстваID { get; set; }
        [ForeignKey("ПартияПроизводстваID")]
        public ProductionBatch? Партия { get; set; }
        public int ШагТехКартыID { get; set; }
        public DateTime? ВремяСтарта { get; set; }
        public DateTime? ВремяОкончания { get; set; }
        public decimal? ФактТемпература { get; set; }
        public int? ФактДлительностьМинут { get; set; }
        public decimal? ФактДавление { get; set; }
        public bool Отклонение { get; set; }
        public string? КомментарийОператора { get; set; }
        public int? ОператорID { get; set; }
    }
}
