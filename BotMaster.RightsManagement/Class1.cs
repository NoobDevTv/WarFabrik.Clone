namespace BotMaster.RightsManagement;
public class RightManager
{

}


public class Group
{
    public bool IsDefault { get; set; }
    public string Name { get; set; }
    public List<string> Users { get; set; }
}
