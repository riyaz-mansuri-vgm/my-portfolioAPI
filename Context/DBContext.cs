using Microsoft.EntityFrameworkCore;
using MyPartFolioAPI.Modules.LoginUser.Models;
using MyPartFolioAPI.Modules.Users.Models;

namespace MyPartFolioAPI.Context;
public class DBContext : DbContext
{
    public DBContext(DbContextOptions options) : base(options) { }

    public DbSet<Users> users { get; set; }
    public DbSet<LoginUsers> LoginUsers { get; set; }
}
