using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using WebApplication7.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication7.Models
{
    public class ToDoListContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ToDoTask> ToDoTasks { get; set; }

        public ToDoListContext(DbContextOptions<ToDoListContext> options) : base(options) { }
    }
}
