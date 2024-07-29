using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class FrankfurterService
{
    private readonly HttpClient _httpClient;

    public FrankfurterService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ExchangeRateResponse> GetExchangeRatesAsync(string baseCurrency, string targetCurrency)
    {
        var response = await _httpClient.GetAsync($"https://api.frankfurter.app/latest?from={baseCurrency}&to={targetCurrency}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<ExchangeRateResponse>(content) ?? new ExchangeRateResponse();
    }

    public async Task<ExchangeRateResponse> GetHistoricalRateAsync(string date)
    {
        string newFormat = DateTime.ParseExact(date, "dd'-'MM'-'yyyy", CultureInfo.InvariantCulture).ToString("yyyy'-'MM'-'dd");
        Console.WriteLine("Today is " + newFormat);
        var response = await _httpClient.GetAsync($"https://api.frankfurter.app/{newFormat}");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<ExchangeRateResponse>(apiResponse);
        return data;
    }

    public async Task<ExchangeRateResponse> GetHistoricalRatesAsync(string startDate, string endDate)
    {
        string startDateFormat = DateTime.ParseExact(startDate, "dd'-'MM'-'yyyy", CultureInfo.InvariantCulture).ToString("yyyy'-'MM'-'dd");
        string endDateFormat = DateTime.ParseExact(endDate, "dd'-'MM'-'yyyy", CultureInfo.InvariantCulture).ToString("yyyy'-'MM'-'dd");
        var response = await _httpClient.GetAsync($"https://api.frankfurter.app/{startDateFormat}..{endDateFormat}");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<ExchangeRateResponse>(apiResponse);
        return data;
    }
}

public class ExchangeRateResponse
{
    public string Base { get; set; } = null!;
    public Dictionary<string, decimal> Rates { get; set; } = null!;
    public DateTime Date { get; set; }
}
