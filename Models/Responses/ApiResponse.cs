namespace banking_api_repo.Models.Responses;

public class ApiResponse<T>
{
    public T? Result { get; set; }
    public string? ErrorMessage { get; set; }
    public int HttpStatusCode { get; set; }
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);
}