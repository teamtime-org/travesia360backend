using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Enums;
using TeamTime.Infrastructure.Identity;

namespace TeamTime.Infrastructure.Data;

public static class SeedData
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Seed Roles
        await SeedRolesAsync(roleManager);

        // Seed Job Titles
        await SeedJobTitlesAsync(context);

        // Seed Areas
        await SeedAreasAsync(context);

        // Seed Admin User
        await SeedAdminUserAsync(userManager, context);

        await context.SaveChangesAsync();
    }

    private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
    {
        var roles = new[]
        {
            new ApplicationRole(ApplicationRole.Names.ADMINISTRADOR, "Administrador del sistema con acceso completo"),
            new ApplicationRole(ApplicationRole.Names.COORDINADOR, "Coordinador con permisos de gestión de proyectos"),
            new ApplicationRole(ApplicationRole.Names.COLABORADOR, "Colaborador con permisos básicos")
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
            {
                await roleManager.CreateAsync(role);
            }
        }
    }

    private static async Task SeedJobTitlesAsync(ApplicationDbContext context)
    {
        if (!await context.JobTitles.AnyAsync())
        {
            var jobTitles = new[]
            {
                new JobTitle(JobTitle.Titles.DIRECTOR_AREA, "Director de Área", "Dirección", 5),
                new JobTitle(JobTitle.Titles.ARQUITECTO, "Arquitecto de Soluciones", "Tecnología", 4),
                new JobTitle(JobTitle.Titles.GERENTE_VENTAS, "Gerente de Ventas", "Ventas", 4),
                new JobTitle(JobTitle.Titles.LIDER_VENTAS, "Líder de Ventas", "Ventas", 3),
                new JobTitle(JobTitle.Titles.EJECUTIVO_VENTAS, "Ejecutivo de Ventas", "Ventas", 2),
                new JobTitle(JobTitle.Titles.GERENTE_DISENO_SOLUCIONES, "Gerente de Diseño de Soluciones", "Diseño", 4),
                new JobTitle(JobTitle.Titles.COORDINADOR_DISENO_SOLUCIONES, "Coordinador de Diseño de Soluciones", "Diseño", 3),
                new JobTitle(JobTitle.Titles.GERENTE_LICITACIONES, "Gerente de Licitaciones", "Licitaciones", 4),
                new JobTitle(JobTitle.Titles.EJECUTIVO_LICITACIONES, "Ejecutivo de Licitaciones", "Licitaciones", 2),
                new JobTitle(JobTitle.Titles.COORDINADOR_PM, "Coordinador PM", "Proyectos", 3),
                new JobTitle(JobTitle.Titles.PROJECT_MANAGER, "Project Manager", "Proyectos", 3),
                new JobTitle(JobTitle.Titles.DESARROLLADOR_SENIOR, "Desarrollador Senior", "Tecnología", 3),
                new JobTitle(JobTitle.Titles.DESARROLLADOR, "Desarrollador", "Tecnología", 2),
                new JobTitle(JobTitle.Titles.ANALISTA, "Analista", "Análisis", 2)
            };

            await context.JobTitles.AddRangeAsync(jobTitles);
        }
    }

    private static async Task SeedAreasAsync(ApplicationDbContext context)
    {
        if (!await context.Areas.AnyAsync())
        {
            var areas = new[]
            {
                new Area("Administración", "Área administrativa y financiera", "#FF6B6B"),
                new Area("Ventas", "Área comercial y de ventas", "#4ECDC4"),
                new Area("Tecnología", "Área de desarrollo y tecnología", "#45B7D1"),
                new Area("Diseño de Soluciones", "Área de arquitectura y diseño", "#96CEB4"),
                new Area("Licitaciones", "Área de procesos de licitación", "#FFEAA7"),
                new Area("Gestión de Proyectos", "Área de administración de proyectos", "#DDA0DD"),
                new Area("Recursos Humanos", "Área de gestión del talento humano", "#98D8C8")
            };

            await context.Areas.AddRangeAsync(areas);
        }
    }

    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        const string adminEmail = "admin@teamtime.com";
        const string adminPassword = "Admin123!";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            // Get the first area for admin assignment
            var adminArea = await context.Areas.FirstOrDefaultAsync(a => a.Name == "Administración");
            var directorJobTitle = await context.JobTitles.FirstOrDefaultAsync(jt => jt.Name == JobTitle.Titles.DIRECTOR_AREA);

            adminUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Administrador",
                LastName = "Sistema",
                Role = UserRole.ADMINISTRADOR,
                AreaId = adminArea?.Id,
                JobTitleId = directorJobTitle?.Id,
                EmployeeCode = "ADMIN001",
                DateOfJoining = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, ApplicationRole.Names.ADMINISTRADOR);
            }
        }
    }

    public static async Task SeedDevelopmentDataAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        // Seed additional test users for development
        if (await userManager.Users.CountAsync() <= 1) // Only admin exists
        {
            var testUsers = new[]
            {
                new { Email = "coordinador@teamtime.com", FirstName = "María", LastName = "González", Role = UserRole.COORDINADOR, EmployeeCode = "COORD001" },
                new { Email = "colaborador@teamtime.com", FirstName = "Juan", LastName = "Pérez", Role = UserRole.COLABORADOR, EmployeeCode = "COLAB001" },
                new { Email = "desarrollador@teamtime.com", FirstName = "Ana", LastName = "Martínez", Role = UserRole.COLABORADOR, EmployeeCode = "DEV001" }
            };

            var tecnologiaArea = await context.Areas.FirstOrDefaultAsync(a => a.Name == "Tecnología");
            var ventasArea = await context.Areas.FirstOrDefaultAsync(a => a.Name == "Ventas");
            var pmJobTitle = await context.JobTitles.FirstOrDefaultAsync(jt => jt.Name == JobTitle.Titles.PROJECT_MANAGER);
            var devJobTitle = await context.JobTitles.FirstOrDefaultAsync(jt => jt.Name == JobTitle.Titles.DESARROLLADOR);

            foreach (var testUser in testUsers)
            {
                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = testUser.Email,
                    Email = testUser.Email,
                    FirstName = testUser.FirstName,
                    LastName = testUser.LastName,
                    Role = testUser.Role,
                    AreaId = testUser.Role == UserRole.COORDINADOR ? ventasArea?.Id : tecnologiaArea?.Id,
                    JobTitleId = testUser.Role == UserRole.COORDINADOR ? pmJobTitle?.Id : devJobTitle?.Id,
                    EmployeeCode = testUser.EmployeeCode,
                    DateOfJoining = DateTime.UtcNow.AddDays(-Random.Shared.Next(30, 365)),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Test123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, testUser.Role.ToString());
                }
            }
        }
    }
}