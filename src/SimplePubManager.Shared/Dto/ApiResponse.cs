namespace SimplePubManager.Shared.Dto
{
    /// <summary>
    /// Standard API response envelope for all endpoints.
    /// </summary>
    /// <typeparam name="T">The type of data contained in the response</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// The response data.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Error information if the request failed.
        /// </summary>
        public ApiError? Error { get; set; }
    }

    /// <summary>
    /// Standard API error information.
    /// </summary>
    public class ApiError
    {
        /// <summary>
        /// Error code identifier.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Human-readable error message.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Optional detailed error information.
        /// </summary>
        public Dictionary<string, object>? Details { get; set; }
    }

    /// <summary>
    /// Paginated response container.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection</typeparam>
    public class PaginatedResponse<T>
    {
        /// <summary>
        /// The items in this page.
        /// </summary>
        public IEnumerable<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// Total number of items across all pages.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Current page number (1-based).
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Number of items per page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of pages.
        /// </summary>
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    }
}
