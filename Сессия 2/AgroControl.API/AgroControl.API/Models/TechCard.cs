using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroControl.API.Models
{
    [Table("ТехКарты")]
    public class TechCard
    {
        [Key]
        public int ID { get; set; }
        public int ПродуктID { get; set; }
        public int Версия { get; set; }
        public string? Статус { get; set; }
        public DateTime ДатаСоздания { get; set; }
        public ICollection<TechCardStep> Шаги { get; set; } = new List<TechCardStep>();
    }
}
