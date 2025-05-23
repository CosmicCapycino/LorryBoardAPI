using Microsoft.EntityFrameworkCore;

namespace LorryBoardAPI.Models;

public class LorryBoardDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<Customer?> Customers { get; set; }
    
    public string DbPath { get; }
    
    public LorryBoardDbContext()
    {
        DbPath =  "lorry-board.db";
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}