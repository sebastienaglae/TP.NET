using System.Collections.Generic;

namespace BookLibrary.Server.Dtos;

public class ImportBookResponse
{
    public string Name { get; set; }
    public string Description { get; set; }
    public IEnumerable<ImportBookMissingAuthor> MissingAuthors { get; set; }
    public IEnumerable<ImportBookFoundAuthor> FoundAuthors { get; set; }
}

public class ImportBookMissingAuthor
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

public class ImportBookFoundAuthor
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Id { get; set; }
}