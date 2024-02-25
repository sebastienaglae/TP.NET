using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BookLibrary.Server.Dtos;
using BookLibrary.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using AuthenticationService = BookLibrary.Server.Services.AuthenticationService;

namespace BookLibrary.Server.Controllers;

[AllowAnonymous]
public class AuthenticationController(AuthenticationService authenticationService) : Controller
{
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto model)
    {
        if (!ModelState.IsValid) return View();
        
        var user = await authenticationService.AuthenticateAsync(model.Username, model.Password);
        if (user is null)
        {
            ModelState.AddModelError("Password", "Invalid username or password");
            ViewData["Username"] = model.Username;
            return View();
        }
        
        ClaimsIdentity identity = new(new[]
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(AuthenticationService.ClaimAvatar, user.AvatarUrl)
        }, ClaimsIdentity.DefaultNameClaimType);

        foreach (AdminRole role in Enum.GetValues<AdminRole>())
        {
            if ((user.Roles & role) == role)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role.ToString()));
            }
        }
        
        await HttpContext.SignInAsync(new ClaimsPrincipal(identity));
        
        // redirect to needed page
        return RedirectToAction("Index", "Home");
    }
    
    public async Task<IActionResult> Logout()
    {
        if (User.Identity?.IsAuthenticated == true)
            await HttpContext.SignOutAsync();
        
        return RedirectToAction("Index", "Home");
    }
}