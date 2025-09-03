namespace MovieApi.Contracts.Responses
{
    public class MetaResponse
    {
       public int limit { get; set; } = default!;
       public string? query;
       public string? genre;
    }
}
