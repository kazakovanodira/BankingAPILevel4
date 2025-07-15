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
            var endpoint = new Uri("https://api.freecurrencyapi.com/v1/latest?apikey=fca_live_bC3MRgGRkcSUkEpNW7dQJ8bNqboiMe2f33CL3F4V?currencies=EUR,CAD");
            var jsonString = await client.GetStringAsync(endpoint);
            Console.WriteLine(jsonString);
            /*jsonString = jsonString.Remove(0, 8);
            jsonString = jsonString.Remove(jsonString.Length - 1, 1);*/
            
            
            Dictionary<string, decimal> fetchedCurrencies = JsonSerializer.Deserialize<Dictionary<string, decimal>>(jsonString) ?? 
                                               new Dictionary<string, decimal>();
            
            return fetchedCurrencies;
        }
    }
}