namespace evuNETcore.DTOs
{
    public class ExchangeRateDto
    {
        public int Id { get; set; } // Aquí sí va el ID
        public string BaseCurrency { get; set; } = string.Empty;
        public string TargetCurrency { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public DateTime Date { get; set; }
    }
}