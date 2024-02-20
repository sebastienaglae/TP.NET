using BookLibrary.Server.Database;
using BookLibrary.Server.ViewModels;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using BookLibrary.Server.Dtos;
using BookLibrary.Server.Models;

namespace BookLibrary.Server;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Rajouter autant de ligne ici que vous avez de mapping Model <-> DTO
        // https://docs.automapper.org/en/latest/
        CreateMap<Book, BookDto>();
    }
}