namespace Infrastructure.Options;

public class MongoOptions
{
    public string User { get; set; } = "";
    public string Password { get; set; } = "";
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 27017;
    public string Database { get; set; } = "movies_dev";
    public string AuthSource { get; set; } = "admin";
    public string? ConnectionString { get; set; } // opcional: si quieres pasarla completa
    public string ToConnectionString() =>
        !string.IsNullOrWhiteSpace(ConnectionString)
            ? ConnectionString!
            : $"mongodb://{User}:{Password}@{Host}:{Port}/{Database}?authSource={AuthSource}";
}
