namespace Cledev.Core.Models.Queries;

public class PaginatedData<T> where T : class
{
    public int TotalPages { get; }
    public int TotalRecords { get; }

    public IList<T> Items { get; }

    public PaginatedData(IList<T> items, int totalRecords, int pageSize)
    {
        Items = items;
        TotalRecords = totalRecords;
        TotalPages = (int)Math.Ceiling(TotalRecords / (decimal)pageSize);
    }
}
