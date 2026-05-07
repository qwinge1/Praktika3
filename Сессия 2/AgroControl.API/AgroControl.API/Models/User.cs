using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroControl.API.Models
{
    [Table("Пользователи")]
    public class User
    {
        [Key]
        public int ID { get; set; }
        public string ИмяПользователя { get; set; } = string.Empty;
        public string ХэшПароля { get; set; } = string.Empty;
        public string ПолноеИмя { get; set; } = string.Empty;
        public string Роль { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Телефон { get; set; }
        public bool Активен { get; set; } = true;
        public DateTime? ПоследнийВход { get; set; }
        public DateTime ДатаСоздания { get; set; } = DateTime.UtcNow;
        public string? Отдел { get; set; }
    }
}