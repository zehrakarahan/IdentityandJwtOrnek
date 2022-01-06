

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace IdentityandJwt.Api2.Model.DbContext
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options) { }

        public DbSet<AppUser> AppUser { get; set; }

        public DbSet<Firma> Firma { get; set; }


        public DbSet<Sube> Sube { get; set; }
     

    }
}
