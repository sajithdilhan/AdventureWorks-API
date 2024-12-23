namespace AdventureWorks.Models.Person
{
    public class PersonQuery
    {
        public string? SortBy { get; set; } = string.Empty;
        public string? SortOrder { get; set; } = "asc";
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
    }
}
