using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroControl.API.Models
{
    [Table("ПартииСырья")]
    public class RawMaterialBatch
    {
        [Key]
        public int ID { get; set; }
        public string НомерПартии { get; set; } = string.Empty;
        public int СырьеID { get; set; }
        public string? Поставщик { get; set; }
        public DateTime? ДатаПоступления { get; set; }
        public decimal? Количество_кг { get; set; }
        public string ЛабораторныйСтатус { get; set; } = "ожидает";

        // Поле не хранится в БД – используется только в клиенте
        [NotMapped]
        public bool HasTest { get; set; }

        // Навигационное свойство – связь с таблицей Сырье
        [ForeignKey("СырьеID")]
        public virtual RawMaterial Сырье { get; set; }

        // Вычисляемое поле для отображения (не хранится в БД)
        [NotMapped]
        public string НаименованиеСырья => Сырье?.Наименование ?? "—";
    }
}