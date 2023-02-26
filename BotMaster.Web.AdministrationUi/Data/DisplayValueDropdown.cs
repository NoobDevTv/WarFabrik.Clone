namespace BotMaster.Web.AdministrationUi.Data;

public class DisplayValueDropdown<TValue>
{
    public string Display { get; set; }
    public TValue Value { get; set; }
    public DisplayValueDropdown(string display, TValue value)
    {
        Display = display;
        Value = value;
    }

}
