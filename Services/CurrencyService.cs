using System.Text.Json;
using banking_api_repo.Interface;
using banking_api_repo.Models.Requests;

namespace RESTAPIBankingApplication.Services;

public class CurrencyService : ICurrencyServices
{
    static readonly HttpClient client = new HttpClient();

    public async Task<Dictionary<string, decimal>> ConvertToCurrency(CurrencyRequest currencyRequest)
    {
       
        using HttpResponseMessage response = await client.GetAsync("https://api.freecurrencyapi.com/v1/latest?apikey=fca_live_bC3MRgGRkcSUkEpNW7dQJ8bNqboiMe2f33CL3F4V");
        response.EnsureSuccessStatusCode();
        string responseBody = await client.GetStringAsync("https://api.freecurrencyapi.com/v1/latest?apikey=fca_live_bC3MRgGRkcSUkEpNW7dQJ8bNqboiMe2f33CL3F4V");
        var convertedBalance = JsonSerializer.Deserialize<Dictionary<string, decimal>>(responseBody);
        
        return convertedBalance;
        
        /*catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
        }*/
    }
}