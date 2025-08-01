using System.Text.Json;
using banking_api_repo.Interfaces;
using banking_api_repo.Models.Requests;
using banking_api_repo.Models.Responses;
using Microsoft.Extensions.Options;

namespace banking_api_repo.Services;

public class CurrencyService : ICurrencyServices
{
    private readonly HttpClient _httpClient;
    private readonly CurrencyApiSettings _apiSettings;

    public CurrencyService(HttpClient httpClient, IOptions<CurrencyApiSettings> options)
    {
        _httpClient = httpClient;
        _apiSettings = options.Value;
    }
    
    public async Task<Dictionary<string, decimal>?> FetchExchangeRates(CurrencyRequest currencyRequest)
    {
        var requestUrl = $"?apikey={_apiSettings.ApiKey}&currencies={currencyRequest.Currency}";
        var response = await _httpClient.GetAsync(requestUrl);
        
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        
        var jsonString = await response.Content.ReadAsStringAsync();

        var currencyResponse = JsonSerializer.Deserialize<CurrencyResponse>(jsonString,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (currencyResponse?.Data is null || 
            currencyResponse.Data.Count is 0)
        {
            return null;
        }
        
        return currencyResponse.Data;
    }
}