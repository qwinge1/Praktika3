using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroControl.API.Models
{
    [Table("ЖурналСобытий")]
    public class EventLog
    {
        [Key]
        public int ID { get; set; }
        public int? ПартияПроизводстваID { get; set; }
        public string? ТипСобытия { get; set; }
        public DateTime ВремяСобытия { get; set; }
        public string? Описание { get; set; }
        public string Важность { get; set; } = "инфо";
        public int? СоздалID { get; set; }

        // ДОБАВИТЬ это навигационное свойство
        [ForeignKey(nameof(СоздалID))]
        public virtual User? Создал { get; set; }
    }
}