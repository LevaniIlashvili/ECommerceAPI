namespace ECommerceAPI.DTOs
{
    public class PaginationResultDTO<T>
    {
        public IEnumerable<T> Products { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
