using BotMaster.Core.Extensibility;

using Microsoft.EntityFrameworkCore;

using NLog;

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

        public static ILogger logger = LogManager.GetCurrentClassLogger();
       
        public static void Initialize(string source)
        {
            if (DatabaseConfigurators.Count >= 1)
                return;
            logger.Debug("Start initialization of database factory, source: "+ source);

            var dbAss2 = AssemblyLoadContext.GetLoadContext(typeof(IDatabaseConfigurator).Assembly);
            dbAss2.Resolving += Default_Resolving;
            logger.Debug("Got load context dbAss2");
            var fullName = new FileInfo(source).FullName;
            logger.Debug($"got full name \"{fullName}\" for source");
            var databasePlugin = dbAss2.LoadFromAssemblyPath(fullName);
            logger.Debug("created plugin assembly");

            //Get All IDatabaseConfigurator
            DatabaseConfigurators
                .AddRange(databasePlugin
                .GetTypes()
                .Where(x => x.IsAssignableTo(typeof(IDatabaseConfigurator)) && x.GetConstructor(Array.Empty<Type>()) != null)
                .Select(x => (IDatabaseConfigurator)Activator.CreateInstance(x)));
            logger.Debug("DatabaseConfigurators created");
        }

        private static Assembly Default_Resolving(AssemblyLoadContext context, AssemblyName name)
        {
            if (name.Name.EndsWith("resources"))
                return null;

            var existing = context.Assemblies.FirstOrDefault(x => x.FullName == name.FullName);
            if (existing is not null)
                return existing;
            string assemblyPath = new FileInfo($"{name.Name}.dll").FullName;
            if (assemblyPath is not null)
                return context.LoadFromAssemblyPath(assemblyPath);
            return null;
        }
    }
}
