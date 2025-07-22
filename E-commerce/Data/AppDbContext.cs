using E_commerce.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace E_commerce.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser,IdentityRole,string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { 

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }


    }
}
