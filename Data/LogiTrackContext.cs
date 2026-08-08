using LogiTrack.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LogiTrack.Data;

public class LogiTrackContext 
    : IdentityDbContext<ApplicationUser>
{
    public LogiTrackContext(
        DbContextOptions<LogiTrackContext> options)
        : base(options)
    {
    }

    public DbSet<InventoryItem> InventoryItems { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<User> Users { get; set; }
}