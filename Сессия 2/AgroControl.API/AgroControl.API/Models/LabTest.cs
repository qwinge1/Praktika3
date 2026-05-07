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
    }
}