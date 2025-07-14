namespace banking_api_repo.Interface;

public interface ICurrencyServices
{
    public Task<Dictionary<string, decimal>> ConvertToCurrency();
}