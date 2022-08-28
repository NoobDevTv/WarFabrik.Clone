using Microsoft.EntityFrameworkCore;

using System.Reflection;
using System.Runtime.Loader;

namespace BotMaster.Database
{
    public interface IDatabaseConfigurator
    {
        void OnConfiguring(DbContextOptionsBuilder optionsBuilder, string connectionString);
    }

    public static class DatabaseFactory
    {
        public static List<IDatabaseConfigurator> DatabaseConfigurators { get; } = new();

       
        public static void Initialize(string source)
        {
            if (DatabaseConfigurators.Count > 1)
                return;

            var databasePlugin = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(source)));

            //Get All IDatabaseConfigurator
            DatabaseConfigurators
                .AddRange(databasePlugin
                .GetTypes()
                .Where(x => x.IsAssignableTo(typeof(IDatabaseConfigurator)) && x.GetConstructor(Array.Empty<Type>()) != null)
                .Select(x => (IDatabaseConfigurator)Activator.CreateInstance(x)));
        }
    }
}
