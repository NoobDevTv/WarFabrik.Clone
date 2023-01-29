using BotMaster.RightsManagement;


namespace BotMaster.Web.AdministrationUi.Data;



public class WeatherForecastService
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public WeatherForecast[] GetForecastAsync(DateOnly startDate)
    {
        using var sd = new UserConnectionContext();


        return sd.Users.Select(index => new WeatherForecast
        {
            Id = index.Id,
            UserName= index.DisplayName
        }).ToArray();
    }
    public UserConnection[] GetConnections()
    {
        using var sd = new UserConnectionContext();


        return sd.UserConnections.ToArray();
    }
}
