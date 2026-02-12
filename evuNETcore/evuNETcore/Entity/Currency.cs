using System.ComponentModel.DataAnnotations;

namespace evuNETcore.Entity
{
    public class Currency
    {
        public int Id { get; set; }

        [MaxLength(3)]
        public required string Symbol { get; set; } // Ejemplo: USD, EUR (Será nuestra clave lógica)

        public required string Name { get; set; } // Nombre completo: United States Dollar

        // Propiedades de navegación para EF Core
        public ICollection<ExchangeRate>? RatesFrom { get; set; }
        public ICollection<ExchangeRate>? RatesTo { get; set; }
    }
}