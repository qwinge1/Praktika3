namespace AgroControl.API.Models
{
    public class ExtruderLiveData
    {
        public int ТемператураЗоны1 { get; set; }
        public int ТемператураЗоны2 { get; set; }
        public double Давление { get; set; }
        public int СкоростьШнека { get; set; }
        public int ТекущаяМощность { get; set; }
        public string ВремяРаботы { get; set; } = string.Empty;
    }
}