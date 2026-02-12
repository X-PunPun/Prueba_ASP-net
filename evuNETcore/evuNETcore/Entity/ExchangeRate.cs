using System.ComponentModel.DataAnnotations.Schema;

namespace evuNETcore.Entity
{
    public class ExchangeRate
    {
        public int Id { get; set; }

        // Claves foráneas basadas en el símbolo (string)
        public required string BaseCurrency { get; set; }
        public required string TargetCurrency { get; set; }

        [Column(TypeName = "decimal(18, 6)")] // Precisión para valores financieros
        public decimal Rate { get; set; }

        public DateTime Date { get; set; }

        // Objetos de navegación (Relaciones)
        public Currency? BaseCurrencyObj { get; set; }
        public Currency? TargetCurrencyObj { get; set; }
    }
}