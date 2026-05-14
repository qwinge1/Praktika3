using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroControl.API.Models
{
    [Table("ЛабораторныеИспытания")]
    public class LabTest
    {
        [Key]
        public int ID { get; set; }
        public int? ПартияПроизводстваID { get; set; }
        public int? ПартияСырьяID { get; set; }
        public DateTime? ДатаАнализа { get; set; }
        public string? ТипОбразца { get; set; }
        public string? НаименованиеПараметра { get; set; }
        public string? ИзмеренноеЗначение { get; set; }
        public string? НормативноеЗначение { get; set; }
        public string? ЕдиницаИзмерения { get; set; }
        public string? Результат { get; set; }
        public string? Решение { get; set; }
        public string? КомментарийАналитика { get; set; }
        public int? АналитикID { get; set; }
        public DateTime? ДатаНазначения { get; set; }
        public int? ИсполнительID { get; set; }
        public string? Приоритет { get; set; }
        public string? КомментарийЛаборанта { get; set; }
        public string? Статус { get; set; }

        [ForeignKey("ИсполнительID")]
        public virtual User? Исполнитель { get; set; }

        [NotMapped]
        public string? BatchNumber { get; set; }
    }
}