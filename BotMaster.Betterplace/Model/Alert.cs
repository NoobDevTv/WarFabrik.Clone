namespace BotMaster.Betterplace.Model
{
    public record struct Alert (int Id, string Name, string Message, int Amount, DateTime Created);
}
