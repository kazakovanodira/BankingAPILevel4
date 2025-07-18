using banking_api_repo.Models.Requests;

namespace banking_api_repo.Interface;

public interface ICurrencyServices
{
    Task<Dictionary<string, decimal>?> FetchCurrencyApi(CurrencyRequest currencyRequest);
}