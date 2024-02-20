using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookLibrary.Server.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookLibrary.Server.Database;

namespace BookLibrary.Server.Controllers;

public class GenreController(LibraryDbContext libraryDbContext, IMapper mapper) : Controller
{
    private readonly LibraryDbContext libraryDbContext = libraryDbContext;
    private readonly IMapper mapper = mapper;

    // A vous de faire comme BookController.List mais pour les genres !
}