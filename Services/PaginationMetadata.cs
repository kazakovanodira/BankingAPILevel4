namespace banking_api_repo.Services;

public class PaginationMetadata
{
    public int TotalItemCount { get; set; }
    public int TotalpageCount { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }

    public PaginationMetadata(int totalItemCount, int pageSize, int currentPage)
    {
        TotalItemCount = totalItemCount;
        PageSize = pageSize;
        CurrentPage = currentPage;
        TotalpageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
    }
}