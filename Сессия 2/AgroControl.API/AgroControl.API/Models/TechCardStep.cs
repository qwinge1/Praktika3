using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroControl.API.Models
{
    [Table("ШагиТехКарты")]
    public class TechCardStep
    {
        [Key]
        public int ID { get; set; }
        public int ТехКартаID { get; set; }
        [ForeignKey("ТехКартаID")]
        public TechCard? ТехКарта { get; set; }
        public int НомерШага { get; set; }
        public string НаименованиеШага { get; set; } = string.Empty;
        public decimal? ПланТемпература { get; set; }
        public int? ПланДлительностьМинут { get; set; }
        public decimal? ПланДавление { get; set; }
        public bool Обязательный { get; set; }
        public string? Инструкция { get; set; }
    }
}
