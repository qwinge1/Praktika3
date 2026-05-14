namespace AgroControl.API.Models
{
    public class ActiveBatchDto
    {
        public int ID { get; set; }
        public string НомерПартии { get; set; } = string.Empty;
        public string Продукт { get; set; } = string.Empty;
        public string Линия { get; set; } = string.Empty;
        public string ТекущийШаг { get; set; } = string.Empty;
        public string СтатусПартии { get; set; } = string.Empty;
        public string СтатусШага { get; set; } = string.Empty;
        public bool ЕстьПредупреждения { get; set; }
        public bool ЕстьКритическиеОтклонения { get; set; }
    }
}