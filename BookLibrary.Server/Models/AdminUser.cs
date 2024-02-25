using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookLibrary.Server.Models;

public class AdminUser
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    [Required] public string UserName { get; set; }
    [Required] public string PasswordHash { get; set; }
    [Required] public string AvatarUrl { get; set; }
    [Required] public AdminRole Roles { get; set; }
}

[Flags]
public enum AdminRole : ulong
{
    EditBooks = 1 << 0,
    DeleteBooks = 1 << 1,
    AddBooks = 1 << 2,
    EditAuthors = 1 << 3,
    DeleteAuthors = 1 << 4,
    AddAuthors = 1 << 5,
    EditGenres = 1 << 6,
    DeleteGenres = 1 << 7,
    AddGenres = 1 << 8,
    
    All = ulong.MaxValue
}