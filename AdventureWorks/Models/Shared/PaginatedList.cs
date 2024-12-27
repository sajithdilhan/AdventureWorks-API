namespace AdventureWorks.Models.Shared;

public class PaginatedList<T>
{
    public List<T> Data { get; set; }
    public MetaData MetaData { get; set; }
}

public record MetaData(int Total, int PageIndex, int PageSize, int PageCount);