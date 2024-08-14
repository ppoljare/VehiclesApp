namespace VehiclesApp.MVC
{
    public class PaginatedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public bool HasPreviousPage { get; private set; }
        public bool HasNextPage { get; private set; }

        public PaginatedList(List<T> items, int count, int currentPage, int pageSize)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            HasPreviousPage = (CurrentPage > 1);
            HasNextPage = (CurrentPage < TotalPages);

            this.AddRange(items);
        }
    }
}
