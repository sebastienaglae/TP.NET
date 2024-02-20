using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.Server.Controllers.Api;

[ApiController]
[Produces("application/json")]
public abstract class ApiController : ControllerBase
{
}