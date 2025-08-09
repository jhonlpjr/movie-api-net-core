using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

[BsonIgnoreExtraElements]
public class Movie
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;          // nunca null en docs válidos

    public string Title { get; set; } = null!;       // requerido
    public List<string> Genre { get; set; } = new(); // lista vacía por defecto
    public double Rating { get; set; }
    public int Year { get; set; }
    public int Popularity { get; set; }

    public string? Description { get; set; }         // este sí puede ser null
}
