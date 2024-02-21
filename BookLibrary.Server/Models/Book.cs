using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookLibrary.Server.Models;

public class Book
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    [Required] [MaxLength(50)] public string Name { get; set; }

    [Required] public ICollection<Author> Authors { get; set; }

    [Required] public string Content { get; set; }

    [Required] public decimal Price { get; set; }

    [Required] public ICollection<Genre> Genres { get; set; }
}