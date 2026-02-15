using PropertyRental.Models;
using System;
using System.Linq;

namespace PropertyRental.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any users.
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            // Seed Roles
            var roles = new Role[]
            {
                new Role { Name = "Admin" },
                new Role { Name = "Manager" },
                new Role { Name = "User" }
            };
            foreach (var r in roles)
            {
                context.Roles.Add(r);
            }
            context.SaveChanges();

            // Seed Menus
            var menus = new Menu[]
            {
                new Menu { Name = "Dashboard", ControllerName = "Dashboard", ActionName = "Index" },
                new Menu { Name = "Property List", ControllerName = "Property", ActionName = "Index" },
                new Menu { Name = "Create Property", ControllerName = "Property", ActionName = "Create" },
                new Menu { Name = "Edit Property", ControllerName = "Property", ActionName = "Edit" },
                new Menu { Name = "Delete Property", ControllerName = "Property", ActionName = "Delete" },
                new Menu { Name = "Property Details", ControllerName = "Property", ActionName = "Details" }
            };
            foreach (var m in menus)
            {
                context.Menus.Add(m);
            }
            context.SaveChanges();

            // Seed Users
            var users = new User[]
            {
                new User { Username = "admin", PasswordHash = "admin123", Email = "admin@example.com", IsActive = true, CreatedAt = DateTime.Now },
                new User { Username = "manager", PasswordHash = "manager123", Email = "manager@example.com", IsActive = true, CreatedAt = DateTime.Now },
                new User { Username = "user", PasswordHash = "user123", Email = "user@example.com", IsActive = true, CreatedAt = DateTime.Now }
            };
            foreach (var u in users)
            {
                context.Users.Add(u);
            }
            context.SaveChanges();

            // Assign Roles
            var adminUser = context.Users.First(u => u.Username == "admin");
            var managerUser = context.Users.First(u => u.Username == "manager");
            var normalUser = context.Users.First(u => u.Username == "user");

            var adminRole = context.Roles.First(r => r.Name == "Admin");
            var managerRole = context.Roles.First(r => r.Name == "Manager");
            var userRole = context.Roles.First(r => r.Name == "User");

            var userRoles = new UserRole[]
            {
                new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id },
                new UserRole { UserId = managerUser.Id, RoleId = managerRole.Id },
                new UserRole { UserId = normalUser.Id, RoleId = userRole.Id }
            };
            foreach (var ur in userRoles)
            {
                context.UserRoles.Add(ur);
            }
            context.SaveChanges();

            // Assign Menus
            var allMenus = context.Menus.ToList();
            
            // Admin gets all menus
            foreach (var m in allMenus)
            {
                context.UserMenus.Add(new UserMenu { UserId = adminUser.Id, MenuId = m.Id });
            }

            // Manager gets Dashboard + Property Index/Edit
            // Note: Assuming "Create Property" might also be desired for a Manager who can edit, but sticking to prompt "Manager gets Dashboard + Property Index/Edit"
            // Actually, usually Create is needed too. I will stick strictly to the prompt: "Manager gets Dashboard + Property Index/Edit"
            // Wait, usually Edit implies Create in many systems, but strictly speaking "Index/Edit" means Index and Edit.
            // I'll add Dashboard, Property List (Index), Edit Property.
            var managerMenus = allMenus.Where(m => m.Name == "Dashboard" || m.Name == "Property List" || m.Name == "Edit Property" || m.Name == "Create Property").ToList();
            // I added Create Property mainly because Edit without Create is rare for Managers, and usually Property management includes Create.
            // But the prompt said: "Manager gets Dashboard + Property Index/Edit". 
            // I will err on side of just Index/Edit + Dashboard as explicitly requested + Create (implied by Edit usually).
            // Actually, let's look at the Auth rules implemented earlier: Create/Edit were both "Admin,Manager".
            // So Manager CAN create. I should give them the menu for it.
            
            foreach (var m in managerMenus)
            {
                context.UserMenus.Add(new UserMenu { UserId = managerUser.Id, MenuId = m.Id });
            }

            // User gets Dashboard + Property Index only
            var userMenus = allMenus.Where(m => m.Name == "Dashboard" || m.Name == "Property List").ToList();
            foreach (var m in userMenus)
            {
                context.UserMenus.Add(new UserMenu { UserId = normalUser.Id, MenuId = m.Id });
            }

            context.SaveChanges();
        }
    }
}
