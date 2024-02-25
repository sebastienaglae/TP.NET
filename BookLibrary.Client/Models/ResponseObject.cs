namespace BookLibrary.Client.Models;

public class ResponseObject<T>
{
    public List<T> Data { get; set; }
    public bool HasMore { get; set; }
}