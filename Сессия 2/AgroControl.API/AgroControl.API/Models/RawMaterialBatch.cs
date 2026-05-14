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
        public string? НомерПартииПоставщика { get; set; }
        public int СырьеID { get; set; }
        public string? Поставщик { get; set; }
        public DateTime? ДатаПоступления { get; set; }
        public decimal? Количество_кг { get; set; }
        public string ЛабораторныйСтатус { get; set; } = "ожидает";
        public string? Склад { get; set; }
        public string? КомментарийРешения { get; set; }
        public string? РешениеПринял { get; set; }
        public DateTime? ДатаРешения { get; set; }

        [ForeignKey("СырьеID")]
        public virtual RawMaterial? Сырье { get; set; }

        // Вычисляемые поля – не хранятся в БД
        [NotMapped]
        public bool HasTest { get; set; }

        [NotMapped]
        public DateTime? LastTestDate { get; set; }
    }
}