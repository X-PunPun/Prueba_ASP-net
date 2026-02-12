using evuNETcore.DTOs;

namespace evuNETcore.Services
{
    public interface IRateService
    {
        // --- CONSULTAS (READ) ---
        Task<IEnumerable<ExchangeRateDto>> GetAllAsync();
        Task<ExchangeRateDto?> GetByIdAsync(int id);
        Task<IEnumerable<ExchangeRateDto>> GetByBaseCurrencyAsync(string baseCurrency);

        // --- OPERACIONES (CREATE, UPDATE, DELETE) ---
        Task<ExchangeRateDto> CreateAsync(CreateExchangeRateDto dto); // Ya incluye fix de moneda
        Task<bool> UpdateAsync(int id, CreateExchangeRateDto dto);

        // ¡ESTE ES EL QUE FALTABA!: Actualizar por moneda base
        Task<bool> UpdateByBaseCurrencyAsync(string baseCurrency, CreateExchangeRateDto dto);

        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteByBaseCurrencyAsync(string baseCurrency);

        // --- SINCRONIZACIÓN ---
        Task SyncTimeSeriesAsync(string baseCurrency, DateTime start, DateTime end);

        // --- CÁLCULOS ---
        Task<decimal> GetAverageAsync(string baseCur, string targetCur, DateTime start, DateTime end);
        Task<(decimal Min, decimal Max)> GetMinMaxAsync(string baseCur, string targetCur, DateTime start, DateTime end);
    }
}