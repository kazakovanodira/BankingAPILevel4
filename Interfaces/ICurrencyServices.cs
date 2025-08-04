using BankingAPILevel4.Models.Requests;

namespace BankingAPILevel4.Interfaces;

public interface ICurrencyServices
{
    Task<Dictionary<string, decimal>?> FetchExchangeRates(CurrencyRequest currencyRequest);
}