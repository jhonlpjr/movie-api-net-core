using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Infrastructure.Persistence.Mongo.Documents;

[BsonIgnoreExtraElements]
public class MovieDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public List<string> Genre { get; set; } = new();
    public double Rating { get; set; }
    public int Year { get; set; }
    public int Popularity { get; set; }
    public string? Description { get; set; }
}