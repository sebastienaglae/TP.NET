using System;
using BookLibrary.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookLibrary.Server.Services;

public class AuthenticateAttribute : Attribute, IAuthorizationFilter
{
    private readonly AdminRole _requiredRole;
    
    public AuthenticateAttribute(AdminRole requiredRole = default)
    {
        _requiredRole = requiredRole;
    }
    
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var isAuthenticated = context.HttpContext.User.Identity?.IsAuthenticated ?? false;
        if (!isAuthenticated)
        {
            context.Result = new RedirectToActionResult("Login", "Authentication", new
            {
                ReturnUrl = context.HttpContext.Request.Path
            });
        }
        else if (_requiredRole != default && !context.HttpContext.User.IsInRole(_requiredRole.ToString()))
        {
            context.Result = new ForbidResult();
        }
    }
}