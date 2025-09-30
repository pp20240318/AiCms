using MyCms.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MyCms.Api.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(CmsDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Always ensure critical tables exist
        try
        {
            await context.Database.ExecuteSqlRawAsync(@"
                CREATE TABLE IF NOT EXISTS Contacts (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    Phone TEXT NULL,
                    Company TEXT NULL,
                    Subject TEXT NOT NULL,
                    Message TEXT NOT NULL,
                    Status INTEGER NOT NULL DEFAULT 0,
                    Reply TEXT NULL,
                    RepliedAt TEXT NULL,
                    RepliedById INTEGER NULL,
                    IpAddress TEXT NULL,
                    UserAgent TEXT NULL,
                    CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    UpdatedAt TEXT NULL,
                    IsDeleted INTEGER NOT NULL DEFAULT 0
                )");

            await context.Database.ExecuteSqlRawAsync(@"
                CREATE TABLE IF NOT EXISTS Pages (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Slug TEXT NOT NULL,
                    Content TEXT NOT NULL,
                    Excerpt TEXT NULL,
                    MetaTitle TEXT NULL,
                    MetaDescription TEXT NULL,
                    MetaKeywords TEXT NULL,
                    FeaturedImage TEXT NULL,
                    Status INTEGER NOT NULL DEFAULT 1,
                    SortOrder INTEGER NOT NULL DEFAULT 0,
                    Template TEXT NULL,
                    CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    UpdatedAt TEXT NULL,
                    CreatedById INTEGER NOT NULL,
                    UpdatedById INTEGER NULL,
                    IsDeleted INTEGER NOT NULL DEFAULT 0
                )");

            await context.Database.ExecuteSqlRawAsync(@"
                CREATE TABLE IF NOT EXISTS WebsiteConfigs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    [Key] TEXT NOT NULL UNIQUE,
                    Value TEXT NULL,
                    Description TEXT NULL,
                    [Group] TEXT NOT NULL,
                    DataType TEXT NOT NULL,
                    IsPublic INTEGER NOT NULL DEFAULT 1,
                    SortOrder INTEGER NOT NULL DEFAULT 0,
                    CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    UpdatedAt TEXT NULL,
                    IsDeleted INTEGER NOT NULL DEFAULT 0
                )");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating tables: {ex.Message}");
        }

        // Check if any users exist
        if (context.Users.Any())
            return; // Database has been seeded
        
        // Create default roles
        var adminRole = new Role
        {
            Name = "Admin",
            Description = "系统管理员",
            IsSystem = true
        };
        
        var userRole = new Role
        {
            Name = "User",
            Description = "普通用户",
            IsSystem = true
        };
        
        await context.Roles.AddRangeAsync(adminRole, userRole);
        await context.SaveChangesAsync();
        
        // Create default permissions
        var permissions = new List<Permission>
        {
            // 仪表盘权限
            new() { Code = "dashboard.view", Name = "查看仪表盘", Description = "查看系统概览和统计数据", Module = "仪表盘" },
            
            // 用户管理权限
            new() { Code = "user.view", Name = "查看用户", Description = "查看用户列表和详情", Module = "用户管理" },
            new() { Code = "user.create", Name = "创建用户", Description = "创建新用户", Module = "用户管理" },
            new() { Code = "user.edit", Name = "编辑用户", Description = "编辑用户信息", Module = "用户管理" },
            new() { Code = "user.delete", Name = "删除用户", Description = "删除用户", Module = "用户管理" },
            new() { Code = "user.manage", Name = "管理用户", Description = "完全管理用户", Module = "用户管理" },
            
            // 角色管理权限
            new() { Code = "role.view", Name = "查看角色", Description = "查看角色列表和详情", Module = "角色管理" },
            new() { Code = "role.create", Name = "创建角色", Description = "创建新角色", Module = "角色管理" },
            new() { Code = "role.edit", Name = "编辑角色", Description = "编辑角色信息", Module = "角色管理" },
            new() { Code = "role.delete", Name = "删除角色", Description = "删除角色", Module = "角色管理" },
            new() { Code = "role.manage", Name = "管理角色", Description = "完全管理角色", Module = "角色管理" },
            
            // 会员管理权限
            new() { Code = "member.view", Name = "查看会员", Description = "查看会员列表和详情", Module = "会员管理" },
            new() { Code = "member.create", Name = "创建会员", Description = "创建新会员", Module = "会员管理" },
            new() { Code = "member.edit", Name = "编辑会员", Description = "编辑会员信息", Module = "会员管理" },
            new() { Code = "member.delete", Name = "删除会员", Description = "删除会员", Module = "会员管理" },
            new() { Code = "member.manage", Name = "管理会员", Description = "完全管理会员", Module = "会员管理" },
            
            // 菜单管理权限
            new() { Code = "menu.view", Name = "查看菜单", Description = "查看菜单列表和详情", Module = "菜单管理" },
            new() { Code = "menu.create", Name = "创建菜单", Description = "创建新菜单", Module = "菜单管理" },
            new() { Code = "menu.edit", Name = "编辑菜单", Description = "编辑菜单信息", Module = "菜单管理" },
            new() { Code = "menu.delete", Name = "删除菜单", Description = "删除菜单", Module = "菜单管理" },
            
            // 文章管理权限
            new() { Code = "article.view", Name = "查看文章", Description = "查看文章列表和详情", Module = "文章管理" },
            new() { Code = "article.create", Name = "创建文章", Description = "创建新文章", Module = "文章管理" },
            new() { Code = "article.edit", Name = "编辑文章", Description = "编辑文章信息", Module = "文章管理" },
            new() { Code = "article.delete", Name = "删除文章", Description = "删除文章", Module = "文章管理" },
            
            // 产品管理权限
            new() { Code = "product.view", Name = "查看产品", Description = "查看产品列表和详情", Module = "产品管理" },
            new() { Code = "product.create", Name = "创建产品", Description = "创建新产品", Module = "产品管理" },
            new() { Code = "product.edit", Name = "编辑产品", Description = "编辑产品信息", Module = "产品管理" },
            new() { Code = "product.delete", Name = "删除产品", Description = "删除产品", Module = "产品管理" },
            
            // 轮播图管理权限
            new() { Code = "banner.view", Name = "查看轮播图", Description = "查看轮播图列表和详情", Module = "轮播图管理" },
            new() { Code = "banner.create", Name = "创建轮播图", Description = "创建新轮播图", Module = "轮播图管理" },
            new() { Code = "banner.edit", Name = "编辑轮播图", Description = "编辑轮播图信息", Module = "轮播图管理" },
            new() { Code = "banner.delete", Name = "删除轮播图", Description = "删除轮播图", Module = "轮播图管理" },
            
            // 页面管理权限
            new() { Code = "page.view", Name = "查看页面", Description = "查看页面列表和详情", Module = "页面管理" },
            new() { Code = "page.create", Name = "创建页面", Description = "创建新页面", Module = "页面管理" },
            new() { Code = "page.edit", Name = "编辑页面", Description = "编辑页面信息", Module = "页面管理" },
            new() { Code = "page.delete", Name = "删除页面", Description = "删除页面", Module = "页面管理" },
            
            // SEO设置权限
            new() { Code = "seo.view", Name = "查看SEO设置", Description = "查看SEO设置", Module = "SEO管理" },
            new() { Code = "seo.edit", Name = "编辑SEO设置", Description = "编辑SEO设置", Module = "SEO管理" },
            
            // 联系我们权限
            new() { Code = "contact.view", Name = "查看联系消息", Description = "查看联系消息列表", Module = "联系管理" },
            new() { Code = "contact.edit", Name = "处理联系消息", Description = "回复和处理联系消息", Module = "联系管理" },
            
            // 系统管理权限
            new() { Code = "system.manage", Name = "系统管理", Description = "系统设置和管理", Module = "系统管理" }
        };
        
        await context.Permissions.AddRangeAsync(permissions);
        await context.SaveChangesAsync();
        
        // Assign all permissions to admin role
        var adminPermissions = permissions.Select(p => new RolePermission
        {
            RoleId = adminRole.Id,
            PermissionId = p.Id
        });
        
        await context.RolePermissions.AddRangeAsync(adminPermissions);
        
        // Create default admin user
        var adminUser = new User
        {
            Username = "admin",
            Email = "admin@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            RealName = "系统管理员",
            IsActive = true
        };
        
        await context.Users.AddAsync(adminUser);
        await context.SaveChangesAsync();
        
        // Assign admin role to admin user
        var userRoleAssignment = new UserRole
        {
            UserId = adminUser.Id,
            RoleId = adminRole.Id
        };
        
        await context.UserRoles.AddAsync(userRoleAssignment);
        
        // Create default menus
        var menus = new List<Menu>
        {
            new() { Name = "系统管理", Path = "/system", Icon = "el-icon-setting", SortOrder = 1, Permission = "system.manage" },
            new() { Name = "用户管理", Path = "/system/users", Icon = "el-icon-user", ParentId = null, SortOrder = 2, Permission = "user.view" },
            new() { Name = "角色管理", Path = "/system/roles", Icon = "el-icon-s-custom", ParentId = null, SortOrder = 3, Permission = "role.view" },
            new() { Name = "菜单管理", Path = "/system/menus", Icon = "el-icon-menu", ParentId = null, SortOrder = 4, Permission = "menu.view" },
            new() { Name = "内容管理", Path = "/content", Icon = "el-icon-document", SortOrder = 5 },
            new() { Name = "文章管理", Path = "/content/articles", Icon = "el-icon-edit-outline", ParentId = null, SortOrder = 6, Permission = "article.view" },
            new() { Name = "文章分类", Path = "/content/article-categories", Icon = "el-icon-folder", ParentId = null, SortOrder = 7, Permission = "article.view" },
            new() { Name = "产品管理", Path = "/content/products", Icon = "el-icon-goods", ParentId = null, SortOrder = 8, Permission = "product.view" },
            new() { Name = "产品分类", Path = "/content/product-categories", Icon = "el-icon-folder-opened", ParentId = null, SortOrder = 9, Permission = "product.view" },
            new() { Name = "轮播图管理", Path = "/content/banners", Icon = "el-icon-picture", ParentId = null, SortOrder = 10, Permission = "system.manage" }
        };
        
        await context.Menus.AddRangeAsync(menus);

        // 添加默认网站配置
        await InitializeWebsiteConfigsAsync(context);

        await context.SaveChangesAsync();
    }

    private static async Task InitializeWebsiteConfigsAsync(CmsDbContext context)
    {
        if (context.WebsiteConfigs.Any())
            return;

        var defaultConfigs = new List<WebsiteConfig>
        {
            new() { Key = WebsiteConfigKeys.SiteName, Value = "My CMS Website", Description = "网站名称", Group = "basic", DataType = "string", IsPublic = true, SortOrder = 1 },
            new() { Key = WebsiteConfigKeys.SiteDescription, Value = "A modern CMS website", Description = "网站描述", Group = "basic", DataType = "string", IsPublic = true, SortOrder = 2 },
            new() { Key = WebsiteConfigKeys.SiteKeywords, Value = "cms,website,management", Description = "网站关键词", Group = "seo", DataType = "string", IsPublic = true, SortOrder = 1 },
            new() { Key = WebsiteConfigKeys.ContactEmail, Value = "contact@example.com", Description = "联系邮箱", Group = "contact", DataType = "email", IsPublic = true, SortOrder = 1 },
            new() { Key = WebsiteConfigKeys.ContactPhone, Value = "", Description = "联系电话", Group = "contact", DataType = "string", IsPublic = true, SortOrder = 2 },
            new() { Key = WebsiteConfigKeys.ContactAddress, Value = "", Description = "联系地址", Group = "contact", DataType = "string", IsPublic = true, SortOrder = 3 },
            new() { Key = WebsiteConfigKeys.EnableContact, Value = "true", Description = "启用联系我们功能", Group = "features", DataType = "boolean", IsPublic = false, SortOrder = 1 },
            new() { Key = WebsiteConfigKeys.EnableComments, Value = "false", Description = "启用评论功能", Group = "features", DataType = "boolean", IsPublic = false, SortOrder = 2 }
        };

        await context.WebsiteConfigs.AddRangeAsync(defaultConfigs);
    }
}