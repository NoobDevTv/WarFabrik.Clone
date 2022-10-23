namespace BotMaster.Database.Migrations;

public interface IAutoMigrationTypeProvider
{
    IReadOnlyList<Type> GetEntityTypes();
}