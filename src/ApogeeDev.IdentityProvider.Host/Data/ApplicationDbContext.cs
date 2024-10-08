using Microsoft.EntityFrameworkCore;

namespace ApogeeDev.IdentityProvider.Host.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }
}