

using BotMaster.Database.Migrations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Loader;

namespace BotMaster.Database
{


    public abstract class DatabaseContext : DbContext, IAutoMigrationContext
    {
        protected DatabaseContext()
        {
        }

        protected DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public ModelBuilder CreateBuilder()
        {
            var dependencies = Database.GetService<ModelDependencies>();
            var setBuilder = Database.GetService<IConventionSetBuilder>();
            var modelConfigurationBuilder =
                new ModelConfigurationBuilder(setBuilder.CreateConventionSet());
            
            return modelConfigurationBuilder.CreateModelBuilder(dependencies);
        }

        public IModel FinalizeModel(IModel model)
        {
            var initializer = Database.GetService<IModelRuntimeInitializer>();
            return initializer.Initialize(model);
        }

        public bool FindLastMigration([MaybeNullWhen(false)] out Migration migration, [MaybeNullWhen(false)] out string id)
        {
            migration = null;
            id = Database.GetAppliedMigrations().MaxBy(id => id);

            if (id is null)
                return false;

            var assembly = Database.GetService<IMigrationsAssembly>();

            var migrationType = assembly.Migrations[id];
            migration = (Migration)Activator.CreateInstance(migrationType)!;

            return true;
        }

        public IReadOnlyList<MigrationOperation> GenerateDiff(IModel? source, IModel? target)
        {
            var sourceModel = source?.GetRelationalModel();
            var targetModel = target?.GetRelationalModel();

            var differ = Database.GetService<IMigrationsModelDiffer>();
            return differ.GetDifferences(sourceModel, targetModel);
        }

        //public DbSet<User> Users => Set<User>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            foreach (var type in AssemblyLoadContext
                                        .Default
                                        .Assemblies
                                        .Where(x => x.FullName.Contains(nameof(BotMaster)))
                                        .SelectMany(x => x.GetTypes())
                                        .Where(type => !type.IsAbstract && !type.IsInterface && typeof(Entity).IsAssignableFrom(type)))
            {
                if (modelBuilder.Model.FindEntityType(type) is null)
                    _ = modelBuilder.Model.AddEntityType(type);
            }

            base.OnModelCreating(modelBuilder);
        }

        public void Migrate()
        {

            var assembly = Database.GetService<IMigrationsAssembly>();
            Console.WriteLine(assembly);
            Database.Migrate();
        }
    }
}
