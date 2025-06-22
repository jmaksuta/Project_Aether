using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Aether_Backend.Migrations
{
    class Program
    {
        public static void Main(string[] args)
        {
            // This Main method is primarily for EF Core Design-time tools.
            // When you run 'dotnet ef migrations add' or 'dotnet ef database update',
            // EF Core will look for a design-time DbContext factory, which implicitly
            // uses this Program.cs to find your DbContext.
            Console.WriteLine("This is the Migrations project. It's primarily used by EF Core tools.");
            Console.WriteLine("Run 'dotnet ef migrations add [MigrationName]' or 'dotnet ef database update' from this project's directory.");
        }
    }
}
