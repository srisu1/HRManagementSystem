namespace HRManagementSystem.Application.DTOs.Common;

/// <summary>
///This is the generic paginated result wrapper
///It is used to return lists of items with pagination metadata
/// </summary>
/// <typeparam name="T">Type of items in the list</typeparam>



public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalRecords / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}