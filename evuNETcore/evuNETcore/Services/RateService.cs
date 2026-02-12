using AutoMapper;
using evuNETcore.Data;
using evuNETcore.DTOs;
using evuNETcore.Entity;
using Microsoft.EntityFrameworkCore;

namespace evuNETcore.Services
{
    public class RateService : IRateService
    {
        private readonly AppDbContex _context;
        private readonly FrankfurterClient _client;
        private readonly IMapper _mapper;

        public RateService(AppDbContex context, FrankfurterClient client, IMapper mapper)
        {
            _context = context;
            _client = client;
            _mapper = mapper;
        }

        // --- 1. CRUD: CREATE (Con corrección de Foreign Key) ---
        public async Task<ExchangeRateDto> CreateAsync(CreateExchangeRateDto dto)
        {
            // Verificamos si existen las monedas, si no, las creamos al vuelo.
            if (!await _context.Currencies.AnyAsync(c => c.Symbol == dto.BaseCurrency))
            {
                _context.Currencies.Add(new Currency { Symbol = dto.BaseCurrency, Name = "Creada Manual" });
                await _context.SaveChangesAsync();
            }

            if (!await _context.Currencies.AnyAsync(c => c.Symbol == dto.TargetCurrency))
            {
                _context.Currencies.Add(new Currency { Symbol = dto.TargetCurrency, Name = "Creada Manual" });
                await _context.SaveChangesAsync();
            }

            var entity = _mapper.Map<ExchangeRate>(dto);
            _context.ExchangeRates.Add(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<ExchangeRateDto>(entity);
        }

        // --- 2. CRUD: UPDATE POR ID ---
        public async Task<bool> UpdateAsync(int id, CreateExchangeRateDto dto)
        {
            var entity = await _context.ExchangeRates.FindAsync(id);
            if (entity == null) return false;

            // Actualizamos los datos
            entity.Rate = dto.Rate;
            entity.Date = dto.Date;

            await _context.SaveChangesAsync();
            return true;
        }

        // --- 3. CRUD: UPDATE POR MONEDA BASE (Requerimiento Faltante) ---
        public async Task<bool> UpdateByBaseCurrencyAsync(string baseCurrency, CreateExchangeRateDto dto)
        {
            // Buscamos TODAS las tasas que tengan esa moneda base
            var items = await _context.ExchangeRates.Where(x => x.BaseCurrency == baseCurrency).ToListAsync();

            if (!items.Any()) return false;

            foreach (var item in items)
            {
                // Actualizamos la fecha de todas (ejemplo de actualización masiva)
                item.Date = dto.Date;

                // NOTA: Actualizar el 'Rate' aquí es peligroso porque todas tendrían el mismo valor
                // aunque sean monedas destino diferentes, pero cumplimos con lo que pide el endpoint.
                // Si el usuario manda un Rate, se actualiza.
                if (dto.Rate > 0) item.Rate = dto.Rate;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // --- 4. CRUD: DELETE ---
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.ExchangeRates.FindAsync(id);
            if (entity == null) return false;
            _context.ExchangeRates.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteByBaseCurrencyAsync(string baseCurrency)
        {
            var items = await _context.ExchangeRates.Where(x => x.BaseCurrency == baseCurrency).ToListAsync();
            if (!items.Any()) return false;
            _context.ExchangeRates.RemoveRange(items);
            await _context.SaveChangesAsync();
            return true;
        }

        // --- 5. CONSULTAS ---
        public async Task<IEnumerable<ExchangeRateDto>> GetAllAsync()
        {
            var list = await _context.ExchangeRates.ToListAsync();
            return _mapper.Map<IEnumerable<ExchangeRateDto>>(list);
        }

        public async Task<ExchangeRateDto?> GetByIdAsync(int id)
        {
            var item = await _context.ExchangeRates.FindAsync(id);
            return item == null ? null : _mapper.Map<ExchangeRateDto>(item);
        }

        public async Task<IEnumerable<ExchangeRateDto>> GetByBaseCurrencyAsync(string baseCurrency)
        {
            var list = await _context.ExchangeRates.Where(x => x.BaseCurrency == baseCurrency).ToListAsync();
            return _mapper.Map<IEnumerable<ExchangeRateDto>>(list);
        }

        // --- 6. SINCRONIZACIÓN Y CÁLCULOS ---
        public async Task SyncTimeSeriesAsync(string baseCurrency, DateTime start, DateTime end)
        {
            var data = await _client.GetTimeSeriesAsync(baseCurrency, start, end);
            if (data == null || data.Rates == null) return;

            // Asegurar Base
            if (!await _context.Currencies.AnyAsync(c => c.Symbol == data.Base))
            {
                _context.Currencies.Add(new Currency { Symbol = data.Base, Name = "Importada Auto" });
                await _context.SaveChangesAsync();
            }

            foreach (var dateEntry in data.Rates)
            {
                var date = DateTime.Parse(dateEntry.Key);
                foreach (var rateEntry in dateEntry.Value)
                {
                    // Asegurar Destino
                    if (!await _context.Currencies.AnyAsync(c => c.Symbol == rateEntry.Key))
                    {
                        _context.Currencies.Add(new Currency { Symbol = rateEntry.Key, Name = "Importada Auto" });
                        await _context.SaveChangesAsync();
                    }

                    bool exists = await _context.ExchangeRates.AnyAsync(x => x.BaseCurrency == data.Base && x.TargetCurrency == rateEntry.Key && x.Date == date);
                    if (!exists)
                    {
                        _context.ExchangeRates.Add(new ExchangeRate { BaseCurrency = data.Base, TargetCurrency = rateEntry.Key, Rate = rateEntry.Value, Date = date });
                    }
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetAverageAsync(string baseCur, string targetCur, DateTime start, DateTime end)
        {
            var query = _context.ExchangeRates.Where(x => x.BaseCurrency == baseCur && x.TargetCurrency == targetCur && x.Date >= start && x.Date <= end);
            if (!await query.AnyAsync()) return 0;
            return await query.AverageAsync(x => x.Rate);
        }

        public async Task<(decimal Min, decimal Max)> GetMinMaxAsync(string baseCur, string targetCur, DateTime start, DateTime end)
        {
            var query = _context.ExchangeRates.Where(x => x.BaseCurrency == baseCur && x.TargetCurrency == targetCur && x.Date >= start && x.Date <= end);
            if (!await query.AnyAsync()) return (0, 0);
            var min = await query.MinAsync(x => x.Rate);
            var max = await query.MaxAsync(x => x.Rate);
            return (min, max);
        }
    }
}