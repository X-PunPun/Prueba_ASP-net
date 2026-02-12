namespace evuNETcore.DTOs
{
    public class CreateExchangeRateDto
    {
        // Solo pedimos lo necesario para crear el registro
        public required string BaseCurrency { get; set; }   // Ej: USD
        public required string TargetCurrency { get; set; } // Ej: EUR
        public decimal Rate { get; set; }
        public DateTime Date { get; set; }
    }
}