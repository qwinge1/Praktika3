using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroControl.API.Models
{
    [Table("Рецептуры")]
    public class Recipe
    {
        [Key]
        public int ID { get; set; }
        public int ПродуктID { get; set; }
        [ForeignKey("ПродуктID")]
        public Product? Продукт { get; set; }
        public int Версия { get; set; }
        public string? Статус { get; set; }

        [Column(TypeName = "datetime")]      // ← добавлено
        public DateTime ДатаСоздания { get; set; }

        public int? СоздалID { get; set; }
        public DateTime? ДатаУтверждения { get; set; }
        public int? УтвердилID { get; set; }
        public ICollection<RecipeComponent> Состав { get; set; } = new List<RecipeComponent>();
    }
}