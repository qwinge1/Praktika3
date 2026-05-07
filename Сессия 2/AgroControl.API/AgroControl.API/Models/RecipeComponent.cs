using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroControl.API.Models
{
    [Table("СоставРецептуры")]
    public class RecipeComponent
    {
        [Key]
        public int ID { get; set; }
        public int РецептID { get; set; }
        [ForeignKey("РецептID")]
        public Recipe? Рецептура { get; set; }
        public int СырьеID { get; set; }
        [ForeignKey("СырьеID")]
        public RawMaterial? Сырье { get; set; }
        public decimal Процент { get; set; }
        public int ПорядокЗагрузки { get; set; }
        public decimal? ДопускОт { get; set; }
        public decimal? ДопускДо { get; set; }
    }
}
