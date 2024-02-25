using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BookLibrary.Server.Database;
using BookLibrary.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLibrary.Server.Services;

public class AuthenticationService
{
    public const string AdminRole = "Admin";
    public const string UserRole = "User";
    public const string ClaimAvatar = "Avatar";
    
    private static ReadOnlySpan<byte> Salt => "q1trmvVsNbRmnt3w1ChohKDKRBrbGCLX"u8; // debug
    
    private readonly LibraryDbContext _dbContext;
    private readonly ILogger<AuthenticationService> _logger;
    
    public AuthenticationService(LibraryDbContext dbContext, ILogger<AuthenticationService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<AdminUser> AuthenticateAsync(string username, string password)
    {
        var user = await _dbContext.AdminUsers.FirstOrDefaultAsync(x => x.UserName == username);
        if (user is null)
        {
            _logger.LogDebug("User {Username} not found", username);
            return null;
        }
        
        if (user.PasswordHash != HashPassword(password))
        {
            _logger.LogDebug("User {Username} provided wrong password", username);
            return null;
        }
        
        return user;
    }
    
    internal static string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length > 100)
            throw new ArgumentException("Password length must be between 1 and 100 characters");
        
        Span<byte> hash = stackalloc byte[32];
        HMACSHA256.HashData(Salt, MemoryMarshal.Cast<char, byte>(password), hash);
        return Convert.ToBase64String(hash);
    }
}