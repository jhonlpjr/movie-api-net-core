namespace MovieApi.Contracts.Responses
{
    public sealed record ApiResponse<T>
    {
        public ApiResponse() { }

        public ApiResponse(T data, MetaResponse? meta = null)
        {
            Data = data;
            Meta = meta;
        }

        public T Data { get; init; } = default!;
        public MetaResponse? Meta { get; init; }
    }
}
