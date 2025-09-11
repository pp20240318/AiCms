using Microsoft.EntityFrameworkCore;
using MyCms.Api.Models;

namespace MyCms.Api.Data;

public class CmsDbContext : DbContext
{
    public CmsDbContext(DbContextOptions<CmsDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<ArticleCategory> ArticleCategories { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Banner> Banners { get; set; }
    public DbSet<UploadedFile> UploadedFiles { get; set; }
    public DbSet<ScheduledTask> ScheduledTasks { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure UserRole many-to-many relationship
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });
            
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);
            
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);
        
        // Configure RolePermission many-to-many relationship
        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });
            
        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId);
            
        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId);
        
        // Configure Menu self-referencing relationship
        modelBuilder.Entity<Menu>()
            .HasOne(m => m.Parent)
            .WithMany(m => m.Children)
            .HasForeignKey(m => m.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Configure ArticleCategory self-referencing relationship
        modelBuilder.Entity<ArticleCategory>()
            .HasOne(ac => ac.Parent)
            .WithMany(ac => ac.Children)
            .HasForeignKey(ac => ac.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Configure ProductCategory self-referencing relationship
        modelBuilder.Entity<ProductCategory>()
            .HasOne(pc => pc.Parent)
            .WithMany(pc => pc.Children)
            .HasForeignKey(pc => pc.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Configure Article relationships
        modelBuilder.Entity<Article>()
            .HasOne(a => a.Category)
            .WithMany(c => c.Articles)
            .HasForeignKey(a => a.CategoryId);
            
        modelBuilder.Entity<Article>()
            .HasOne(a => a.Author)
            .WithMany()
            .HasForeignKey(a => a.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Configure Product relationships
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId);
        
        // Configure UploadedFile relationships
        modelBuilder.Entity<UploadedFile>()
            .HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // Configure unique constraints
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
            
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
            
        modelBuilder.Entity<Role>()
            .HasIndex(r => r.Name)
            .IsUnique();
            
        modelBuilder.Entity<Permission>()
            .HasIndex(p => p.Code)
            .IsUnique();
        
        base.OnModelCreating(modelBuilder);
    }
    
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }
    
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}