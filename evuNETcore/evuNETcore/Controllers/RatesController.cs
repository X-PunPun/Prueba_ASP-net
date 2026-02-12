using evuNETcore.DTOs;
using evuNETcore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace evuNETcore.Controllers
{
    [Route("api/rates")]
    [ApiController]
    [Authorize] // Todo requiere Token JWT
    public class RatesController : ControllerBase
    {
        private readonly IRateService _service;

        public RatesController(IRateService service)
        {
            _service = service;
        }

        // -------------------------------------------------------------------
        // 1. ENDPOINTS CRUD BÁSICOS
        // -------------------------------------------------------------------

        // GET /rates: Devuelve la lista de tasas de cambio almacenadas.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        // GET /rates/{id}: Devuelve una tasa de cambio específica por Id.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        // POST /rates: Crea nuevas tasas de cambio.
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExchangeRateDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        // PUT /rates/{id}: Actualiza una tasa de cambio existente por Id.
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateExchangeRateDto dto)
        {
            var success = await _service.UpdateAsync(id, dto);
            return success ? Ok(new { message = "Actualizado correctamente" }) : NotFound();
        }

        // DELETE /rates/{id}: Elimina una tasa de cambio por Id.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            return success ? Ok(new { message = "Eliminado correctamente" }) : NotFound();
        }

        // -------------------------------------------------------------------
        // 2. ENDPOINTS CRUD POR MONEDA (Currency)
        // -------------------------------------------------------------------

        // GET /rates/currency/{baseCurrency}: Devuelve las tasas por moneda base.
        [HttpGet("currency/{baseCurrency}")]
        public async Task<IActionResult> GetByCurrency(string baseCurrency)
        {
            var list = await _service.GetByBaseCurrencyAsync(baseCurrency);
            return Ok(list);
        }

        // PUT /rates/currency/{baseCurrency}: Actualiza tasas por moneda base.
        [HttpPut("currency/{baseCurrency}")]
        public async Task<IActionResult> UpdateByCurrency(string baseCurrency, [FromBody] CreateExchangeRateDto dto)
        {
            var success = await _service.UpdateByBaseCurrencyAsync(baseCurrency, dto);
            return success ? Ok(new { message = $"Tasas para {baseCurrency} actualizadas" }) : NotFound("No se encontraron tasas para esa moneda");
        }

        // DELETE /rates/currency/{baseCurrency}: Elimina tasas por moneda base.
        [HttpDelete("currency/{baseCurrency}")]
        public async Task<IActionResult> DeleteByCurrency(string baseCurrency)
        {
            var success = await _service.DeleteByBaseCurrencyAsync(baseCurrency);
            return success ? Ok(new { message = $"Tasas para {baseCurrency} eliminadas" }) : NotFound("No se encontraron tasas para esa moneda");
        }

        // -------------------------------------------------------------------
        // 3. TRANSFORMACIÓN DE DATOS (Cálculos)
        // -------------------------------------------------------------------

        // GET /rates/average
        [HttpGet("average")]
        [AllowAnonymous] // Opcional: Permitir sin token para facilitar pruebas
        public async Task<IActionResult> GetAverage(
            [FromQuery] string baseCur,
            [FromQuery] string targetCur,
            [FromQuery] DateTime start,
            [FromQuery] DateTime end)
        {
            var avg = await _service.GetAverageAsync(baseCur, targetCur, start, end);
            return Ok(new
            {
                MonedaBase = baseCur,
                MonedaDestino = targetCur,
                Periodo = $"{start:d} - {end:d}",
                Promedio = avg
            });
        }

        // GET /rates/minmax
        [HttpGet("minmax")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMinMax(
            [FromQuery] string baseCur,
            [FromQuery] string targetCur,
            [FromQuery] DateTime start,
            [FromQuery] DateTime end)
        {
            var (min, max) = await _service.GetMinMaxAsync(baseCur, targetCur, start, end);
            return Ok(new
            {
                MonedaBase = baseCur,
                MonedaDestino = targetCur,
                Minimo = min,
                Maximo = max
            });
        }

        // -------------------------------------------------------------------
        // 4. EXTRA: Sincronización (Necesario para poblar datos)
        // -------------------------------------------------------------------
        [HttpPost("sync-history")]
        [AllowAnonymous]
        public async Task<IActionResult> SyncHistory(
            [FromQuery] string baseCurrency,
            [FromQuery] DateTime start,
            [FromQuery] DateTime end)
        {
            await _service.SyncTimeSeriesAsync(baseCurrency, start, end);
            return Ok("Sincronización histórica completada.");
        }
    }
}