namespace EPSC.Utility.Pagination
{
    public class PagedResponse<T>
    {
        public MetaData MetaData { get; set; }
        public List<T> Data { get; set; }

        public PagedResponse(List<T> data, int totalItems, int pageNumber, int pageSize)
        {
            Data = data;

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            MetaData = new MetaData
            {
                TotalUnfilteredItems = totalItems,
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                HasPrevious = pageNumber > 1,
                HasNext = pageNumber < totalPages
            };
        }
    }
}
