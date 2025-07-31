using banking_api_repo.Models.Requests;

namespace banking_api_repo.Interfaces;

public interface ICurrencyServices
{
    Task<Dictionary<string, decimal>?> FetchExchangeRates(CurrencyRequest currencyRequest);
}