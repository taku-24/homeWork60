using Microsoft.AspNetCore.Identity;

namespace WebApplication7.Models;

public class User : IdentityUser<int>
{
    public DateTime DateOfBirth { get; set; } = DateTime.UtcNow;
    
}
