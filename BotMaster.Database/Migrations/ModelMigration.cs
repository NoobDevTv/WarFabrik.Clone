using Microsoft.EntityFrameworkCore;
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

    public static void SetUpgradeOperations(this MigrationBuilder migrationBuilder, Migration migration, DatabaseContext context)
    {
        var migrationType = migration.GetType();
        var targetBuilder = context.CreateBuilder();

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

        var target = context.FinalizeModel((IModel) targetBuilder.Model);
        IModel? source = null;

        if (context.FindLastMigration(out var lastMigration, out var lastMigrationId))
        {
            var sourceBuilder = context.CreateBuilder();

            if (lastMigration is IAutoMigrationTypeProvider lastTypeProvider)
            {
                sourceBuilder.BuildVersion(lastTypeProvider);
            }
            else
            {
                sourceBuilder.BuildVersion(lastMigrationId);
            }

            source = context.FinalizeModel((IModel)sourceBuilder.Model);
        }

        var diff = context.GenerateDiff(source, target);
        migrationBuilder.Operations.AddRange(diff);
    }
}
