using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

using System.Diagnostics.CodeAnalysis;

namespace BotMaster.Database.Migrations;

public interface IAutoMigrationContext
{
    IModel FinalizeModel(IModel model);
    IReadOnlyList<MigrationOperation> GenerateDiff(IModel? source, IModel? target);
    ModelBuilder CreateBuilder();
    bool FindLastMigration([MaybeNullWhen(false)] out Migration migration, [MaybeNullWhen(false)] out string id);
}