using System.Text.Json;
using banking_api_repo.Interface;
using banking_api_repo.Models.Responses;

namespace banking_api_repo.Services;

public class CurrencyService : ICurrencyServices
{
    public async Task<Dictionary<string, decimal>> ConvertToCurrency()
    {
        using (HttpClient client = new HttpClient())
        {
            var endpoint = new Uri("https://api.freecurrencyapi.com/v1/latest?apikey=fca_live_bC3MRgGRkcSUkEpNW7dQJ8bNqboiMe2f33CL3F4V&currencies=EUR,CAD");
            var jsonString = await client.GetStringAsync(endpoint);
            
            CurrencyResponse currencyResponse = JsonSerializer.Deserialize<CurrencyResponse>(jsonString,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new NullReferenceException();
            
            return currencyResponse.Data;
        }
    }
}