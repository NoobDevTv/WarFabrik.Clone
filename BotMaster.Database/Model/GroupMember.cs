namespace BotMaster.Database.Model
{
    public class GroupMember : IdEntity<int>
    {
        public virtual User User { get; set; }
        public virtual Group Group { get; set; }
    }
}
