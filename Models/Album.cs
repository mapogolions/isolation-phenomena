namespace IsoLevelsAdoNet.Models;

public class Album
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Artist { get; set; }
    public decimal Price { get; set; }

    public override string ToString()
    {
        return $"Album(Id: {Id}, Title: {Title}, Artist: {Artist}, Price: {Price})";
    }
}
