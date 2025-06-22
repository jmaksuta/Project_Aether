using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration; // Required for IConfiguration
using Project_Aether_Backend.Data; // Replace with your actual project's namespace for Data
using System.IO;

namespace Project_Aether_Backend.Migrations // Use the namespace of your migrations project
{
    // This class is used by the EF Core tools (Add-Migration, Update-Database)
    // to create an instance of your DbContext.
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // IMPORTANT: Adjust this path to reliably find your appsettings.json
            // This example assumes appsettings.json is in the root of your main web project
            // which is a sibling directory to your migrations project.
            //var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Project Aether Backend"); // Adjust if your solution structure is different

            //// Fallback for when running from the startup project itself (less common for your setup)
            //if (!Directory.Exists(basePath) || !File.Exists(Path.Combine(basePath, "appsettings.json")))
            //{
            //    // If the above path doesn't work (e.g., running from solution root or directly from startup project's bin)
            //    // try the current directory (which might be the startup project's bin when EF tools are invoked)
            //    basePath = Directory.GetCurrentDirectory();
            //}

            // Build configuration to get the connection string
            //IConfigurationRoot configuration = new ConfigurationBuilder()
            //  .SetBasePath(basePath)
            //  .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //  // Add appsettings.Development.json or other environment-specific files if needed
            //  .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            //  .Build();


            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            //--------------------------------
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("DefaultConnection connection string not found in appsettings.json.");
            }

            builder.UseSqlServer(connectionString,
                sqlOptions => sqlOptions.MigrationsAssembly(typeof(ApplicationDbContextFactory).Assembly.FullName));
            // ^ Make sure to specify the migrations assembly here as well for design-time operations
            // This ensures the factory correctly tells EF Core where to find the migrations
            return new ApplicationDbContext(builder.Options);
        }
    }
}