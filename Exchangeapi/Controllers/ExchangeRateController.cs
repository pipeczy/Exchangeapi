using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Exchangeapi.Data;
using Exchangeapi.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;

namespace CurrencyExchangeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExchangeRateController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IMemoryCache _cache;

        public ExchangeRateController(ApplicationDBContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExchangeRate>>> GetExchangeRates()
        {
            return await _context.ExchangeRates.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExchangeRate>> GetExchangeRate(int id)
        {
            var exchangeRate = await _context.ExchangeRates.FindAsync(id);
            if (exchangeRate == null)
            {
                return NotFound();
            }

            return exchangeRate;
        }

        [HttpGet("currency/{baseCurrency}")]
        public async Task<ActionResult<IEnumerable<ExchangeRate>>> GetExchangeRatesByBaseCurrency(string baseCurrency)
        {
            return await _context.ExchangeRates
                .Where(er => er.BaseCurrency == baseCurrency)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<ExchangeRate>> PostExchangeRate(ExchangeRate exchangeRate)
        {
            _context.ExchangeRates.Add(exchangeRate);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExchangeRate), new { id = exchangeRate.Id }, exchangeRate);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutExchangeRate(int id, ExchangeRate exchangeRate)
        {
            if (id != exchangeRate.Id)
            {
                return BadRequest();
            }

            _context.Entry(exchangeRate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExchangeRateExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPut("currency/{baseCurrency}")]
        public async Task<IActionResult> PutExchangeRatesByBaseCurrency(string baseCurrency, List<ExchangeRate> exchangeRates)
        {
            var existingRates = await _context.ExchangeRates
                .Where(er => er.BaseCurrency == baseCurrency)
                .ToListAsync();

            _context.ExchangeRates.RemoveRange(existingRates);
            _context.ExchangeRates.AddRange(exchangeRates);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExchangeRate(int id)
        {
            var exchangeRate = await _context.ExchangeRates.FindAsync(id);
            if (exchangeRate == null)
            {
                return NotFound();
            }

            _context.ExchangeRates.Remove(exchangeRate);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("currency/{baseCurrency}")]
        public async Task<IActionResult> DeleteExchangeRatesByBaseCurrency(string baseCurrency)
        {
            var exchangeRates = await _context.ExchangeRates
                .Where(er => er.BaseCurrency == baseCurrency)
                .ToListAsync();

            _context.ExchangeRates.RemoveRange(exchangeRates);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("average")]
        public async Task<ActionResult<decimal>> GetAverageRate(string baseCurrency, string targetCurrency, DateTime start, DateTime end)
        {
            var cacheKey = $"{baseCurrency}-{targetCurrency}-{start.ToShortDateString()}-{end.ToShortDateString()}-average";
            if (!_cache.TryGetValue(cacheKey, out decimal averageRate))
            {
                var rates = await _context.ExchangeRates
                    .Where(er => er.BaseCurrency == baseCurrency && er.TargetCurrency == targetCurrency && er.Date >= start && er.Date <= end)
                    .ToListAsync();

                if (rates.Count == 0)
                {
                    return NotFound();
                }

                averageRate = rates.Average(er => er.Rate);

                _cache.Set(cacheKey, averageRate, TimeSpan.FromMinutes(5));
            }

            return averageRate;
        }

        [HttpGet("minmax")]
        public async Task<ActionResult<object>> GetMinMaxRate(string baseCurrency, string targetCurrency, DateTime start, DateTime end)
        {
            var cacheKey = $"{baseCurrency}-{targetCurrency}-{start.ToShortDateString()}-{end.ToShortDateString()}-minmax";
            if (!_cache.TryGetValue(cacheKey, out (decimal MinRate, decimal MaxRate) minMaxRates))
            {
                var rates = await _context.ExchangeRates
                    .Where(er => er.BaseCurrency == baseCurrency && er.TargetCurrency == targetCurrency && er.Date >= start && er.Date <= end)
                    .ToListAsync();

                if (rates.Count == 0)
                {
                    return NotFound();
                }

                minMaxRates.MinRate = rates.Min(er => er.Rate);
                minMaxRates.MaxRate = rates.Max(er => er.Rate);

                _cache.Set(cacheKey, minMaxRates, TimeSpan.FromMinutes(5));
            }

            return new { MinRate = minMaxRates.MinRate, MaxRate = minMaxRates.MaxRate };
        }

        private bool ExchangeRateExists(int id)
        {
            return _context.ExchangeRates.Any(e => e.Id == id);
        }
    }
}
