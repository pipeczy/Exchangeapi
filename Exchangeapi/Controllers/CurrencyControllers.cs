using Microsoft.AspNetCore.Mvc;
using Exchangeapi.Data;
using Exchangeapi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace CurrencyExchangeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CurrencyController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly FrankfurterService _frankfurterService;

        public CurrencyController(ApplicationDBContext context, FrankfurterService frankfurterService)
        {
            _context = context;
            _frankfurterService = frankfurterService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Currency>>> GetCurrencies()
        {
            return await _context.Currencies.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Currency>> GetCurrency(int id)
        {
            var currency = await _context.Currencies.FindAsync(id);
            if (currency == null)
            {
                return NotFound();
            }

            return currency;
        }

        [HttpPost]
        public async Task<ActionResult<Currency>> PostCurrency(Currency currency)
        {
            _context.Currencies.Add(currency);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCurrency), new { id = currency.Id }, currency);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCurrency(int id, Currency currency)
        {
            if (!int.TryParse(currency.Id, out int currencyId))
            {
                return BadRequest();
            }

            _context.Entry(currency).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CurrencyExists(id))
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurrency(int id)
        {
            var currency = await _context.Currencies.FindAsync(id);
            if (currency == null)
            {
                return NotFound();
            }

            _context.Currencies.Remove(currency);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CurrencyExists(int id)
        {
            return _context.Currencies.Any(e => e.Id == id.ToString());
        }

        [HttpGet("fetch-rates")]
        public async Task<IActionResult> FetchAndStoreExchangeRates([FromQuery] string baseCurrency, [FromQuery] string targetCurrency)
        {
     
            var exchangeRateResponse = await _frankfurterService.GetExchangeRatesAsync(baseCurrency, targetCurrency);

          
            await EnsureCurrencyExists(baseCurrency);
            await EnsureCurrencyExists(targetCurrency);

            foreach (var rate in exchangeRateResponse.Rates)
            {
                var exchangeRate = new ExchangeRate
                {
                    BaseCurrency = baseCurrency,
                    TargetCurrency = rate.Key,
                    Rate = rate.Value,
                    Date = exchangeRateResponse.Date
                };
                _context.ExchangeRates.Add(exchangeRate);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        private async Task EnsureCurrencyExists(string currencySymbol)
        {
            var currency = await _context.Currencies
                .FirstOrDefaultAsync(c => c.Symbol == currencySymbol);

            if (currency == null)
            {
                currency = new Currency
                {
                    Symbol = currencySymbol,
                    Name = currencySymbol 
                };

                _context.Currencies.Add(currency);
                await _context.SaveChangesAsync();
            }
        }
    }
}
