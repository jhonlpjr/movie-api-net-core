// Infrastructure/Options/RedisOptions.cs
namespace Infrastructure.Options;

public class RedisOptions
{
    // host:port (soportamos cluster simple vía coma si quieres varios nodos)
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 6379;

    // Opcionales
    public string? Password { get; set; }       // si tu redis tiene auth
    public bool Ssl { get; set; } = false;      // ssl=True/False
    public bool AbortConnect { get; set; } = false; // recomendado false en contenedores
    public int? DefaultDatabase { get; set; }   // null usa el default (0)
    public string InstanceName { get; set; } = "movieapi:"; // prefijo de claves

    // Devuelve el connection string estilo StackExchange.Redis
    public string ToConfigurationString()
    {
        var parts = new List<string> { $"{Host}:{Port}" };
        if (!string.IsNullOrWhiteSpace(Password)) parts.Add($"password={Password}");
        parts.Add($"ssl={(Ssl ? "True" : "False")}");
        parts.Add($"abortConnect={(AbortConnect ? "True" : "False")}");
        if (DefaultDatabase.HasValue) parts.Add($"defaultDatabase={DefaultDatabase.Value}");
        return string.Join(",", parts);
    }
}
