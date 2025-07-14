using System.Text.Json;
using banking_api_repo.Interface;
using banking_api_repo.Models.Requests;

namespace banking_api_repo.Services;

public class CurrencyService : ICurrencyServices
{
    static readonly HttpClient client = new HttpClient();

    public async Task<Dictionary<string, decimal>> ConvertToCurrency()
    {
        string responseBody = await client.GetStringAsync("https://api.freecurrencyapi.com/v1/latest?apikey=fca_live_bC3MRgGRkcSUkEpNW7dQJ8bNqboiMe2f33CL3F4V");
        var convertedBalance = JsonSerializer.Deserialize<Dictionary<string, decimal>>(responseBody);
        return convertedBalance ?? new Dictionary<string, decimal>();
    }
}