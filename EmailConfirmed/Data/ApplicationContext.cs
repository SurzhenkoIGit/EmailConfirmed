using EmailConfirmed.Models;
using EmailConfirmed.Models.Client;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EmailConfirmed.Data
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) 
        {
        }
        public DbSet<ClientApp> Clients { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
    }
}
