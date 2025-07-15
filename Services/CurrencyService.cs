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
            var endpoint = new Uri("https://api.freecurrencyapi.com/v1/latest?apikey=fca_live_bC3MRgGRkcSUkEpNW7dQJ8bNqboiMe2f33CL3F4V");
            var jsonString = await client.GetStringAsync(endpoint);
            var currencyApiResponse = JsonSerializer.Deserialize<ConvertedBalances>(jsonString);

            return currencyApiResponse?.convertedBalances ?? new Dictionary<string, decimal>();
        }
    }
}