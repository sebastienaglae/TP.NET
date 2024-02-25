namespace BookLibrary.Client.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public Author[] Authors { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public Genre[] Genres { get; set; }

    public override string ToString()
    {
        return Title;
    }
}