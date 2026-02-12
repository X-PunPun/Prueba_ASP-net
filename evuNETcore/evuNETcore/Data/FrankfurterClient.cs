using System.Text.Json;

namespace evuNETcore.Data
{
    public class FrankfurterClient
    {
        private readonly HttpClient _http;

        public FrankfurterClient(HttpClient http)
        {
            _http = http;
        }

        // Método para obtener el ÚLTIMO valor (Latest)
        public async Task<FrankfurterResponse?> GetLatestAsync(string fromSymbol = "EUR")
        {
            var response = await _http.GetAsync($"latest?from={fromSymbol}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<FrankfurterResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // NUEVO: Método para RANGO DE FECHAS (Necesario para el promedio histórico)
        public async Task<FrankfurterTimeSeriesResponse?> GetTimeSeriesAsync(string fromSymbol, DateTime start, DateTime end)
        {
            string startStr = start.ToString("yyyy-MM-dd");
            string endStr = end.ToString("yyyy-MM-dd");

            // Llamada tipo: /2024-01-01..2024-01-31?from=EUR
            var response = await _http.GetAsync($"{startStr}..{endStr}?from={fromSymbol}");

            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<FrankfurterTimeSeriesResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }

    // Clases para mapear respuestas
    public class FrankfurterResponse
    {
        public decimal Amount { get; set; }
        public string Base { get; set; }
        public string Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }

    // Respuesta compleja para series temporales
    public class FrankfurterTimeSeriesResponse
    {
        public decimal Amount { get; set; }
        public string Base { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        // Diccionario anidado: Fecha -> (Moneda -> Valor)
        public Dictionary<string, Dictionary<string, decimal>> Rates { get; set; }
    }
}