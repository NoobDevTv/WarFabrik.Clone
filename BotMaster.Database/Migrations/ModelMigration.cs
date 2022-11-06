using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.Database.Migrations;

public static class ModelMigration
{
    public static void BuildVersion(this ModelBuilder modelBuilder, string version)
    {

    }

    public static void BuildVersion(this ModelBuilder modelBuilder, IAutoMigrationTypeProvider typeProvider)
    {
        //TODO: Solution required our table names are property names from the context
        foreach (var type in typeProvider.GetEntityTypes())
        {
            if (modelBuilder.Model.FindEntityType(type) is null)
                _ = modelBuilder.Model.AddEntityType(type);
        }
    }

    public static void SetUpgradeOperations(this MigrationBuilder migrationBuilder, Migration migration)
    {
        var providerContextBuilder = DatabaseFactory.DatabaseConfigurators.First().GetEmptyForMigration();

        var migrationType = migration.GetType();
        var contextAttribute = migrationType.GetCustomAttribute<DbContextAttribute>() ?? throw new ArgumentNullException();
        var currentContext = (IAutoMigrationContext)Activator.CreateInstance(contextAttribute.ContextType)!;

        var targetBuilder = providerContextBuilder.CreateBuilder();

        if (migration is IAutoMigrationTypeProvider autoTypeProvider)
        {
            targetBuilder.BuildVersion(autoTypeProvider);
        }
        else
        {
            var idAttribute = migrationType.GetCustomAttribute<MigrationAttribute>() ??
                              throw new ArgumentNullException();

            targetBuilder.BuildVersion(idAttribute.Id);
        }

        var target = providerContextBuilder.FinalizeModel((IModel) targetBuilder.Model);
        IModel? source = null;

        if (currentContext.FindLastMigration(contextAttribute.ContextType.Name, out var lastMigration, out var lastMigrationId))
        {
            var sourceBuilder = providerContextBuilder.CreateBuilder();

            if (lastMigration is IAutoMigrationTypeProvider lastTypeProvider)
            {
                sourceBuilder.BuildVersion(lastTypeProvider);
            }
            else
            {
                sourceBuilder.BuildVersion(lastMigrationId);
            }

            source = providerContextBuilder.FinalizeModel((IModel)sourceBuilder.Model);
        }

        var diff = providerContextBuilder.GenerateDiff(source, target);
        migrationBuilder.Operations.AddRange(diff);
    }
}
