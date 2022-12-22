

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

    public abstract class MigrationDatabaseContext : DbContext, IAutoMigrationContextBuilder
    {
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

        public IReadOnlyList<MigrationOperation> GenerateDiff(IModel? source, IModel? target)
        {
            var sourceModel = source?.GetRelationalModel();
            var targetModel = target?.GetRelationalModel();

            var differ = Database.GetService<IMigrationsModelDiffer>();
            return differ.GetDifferences(sourceModel, targetModel);
        }
    }

    public abstract class DatabaseContext : DbContext, IAutoMigrationContext
    {
        protected DatabaseContext()
        {
        }

        protected DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public bool FindLastMigration(string contextName, [MaybeNullWhen(false)] out Migration migration, [MaybeNullWhen(false)] out string id)
        {
            migration = null;
            id = Database.GetAppliedMigrations().Where(x => x.Contains(contextName, StringComparison.OrdinalIgnoreCase)).MaxBy(id => id);

            if (id is null)
                return false;

            var assembly = Database.GetService<IMigrationsAssembly>();

            var migrationType = assembly.Migrations[id];
            migration = (Migration)Activator.CreateInstance(migrationType)!;

            return true;
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
        /// <summary>
        /// Migrates with a transaction
        ///     <para>
        ///         Applies any pending migrations for the context to the database. Will create the database
        ///         if it does not already exist.
        ///     </para>
        ///     <para>
        ///         Note that this API is mutually exclusive with <see cref="DatabaseFacade.EnsureCreated" />. EnsureCreated does not use migrations
        ///         to create the database and therefore the database that is created cannot be later updated using migrations.
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-migrations">Database migrations</see> for more information.
        /// </remarks>
        /// <param name="databaseFacade">The <see cref="DatabaseFacade" /> for the context.</param>
        public void Migrate()
        {
            //using var transRights = Database.BeginTransaction();
            Database.Migrate();
            //transRights.Commit();

        }
    }
}
