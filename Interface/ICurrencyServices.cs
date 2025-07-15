namespace banking_api_repo.Interface;

public interface ICurrencyServices
{
    Task<Dictionary<string, decimal>> ConvertToCurrency();
}