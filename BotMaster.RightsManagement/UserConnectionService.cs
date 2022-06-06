using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.RightsManagement;
public static class UserConnectionService
{
    public static Random random = new Random();
    public const string AllowedConnectionChars = "abcdefghijklmnopqrstvwxyzABCDEFGHIJKLMNOPQRSTVWXYZ0123456789\"§$%&/()=?{[]}\\#+*";
    public const int ConnectionStringLength = 10;

    public static string StartConnection(string userId)
    {
        var connectionCode = "";
        for (int i = 0; i < ConnectionStringLength; i++)
        {
            connectionCode += AllowedConnectionChars[random.Next(0, AllowedConnectionChars.Length)];
        }

        using var db = new UserConnectionContext();
        var plattfromUser = db.PlattformUsers.FirstOrDefault(x => x.PlattformUserId == userId);
        if (plattfromUser is null)
            return "";
        db.UserConnections.Add(new UserConnection() { PlattformUser = plattfromUser, ValidUntil = DateTime.UtcNow.AddHours(1), ConnectionCode = connectionCode });
        db.SaveChanges();
        return connectionCode;
    }

    public static void RevokeConnection(string userId, string connectionCode)
    {
        using var db = new UserConnectionContext();
        var connect = db.UserConnections.FirstOrDefault(x => x.ConnectionCode == connectionCode && x.ValidUntil > DateTime.UtcNow);
        var plattformUser = db.PlattformUsers.FirstOrDefault(x => x.PlattformUserId == userId);
        if (connect is null || plattformUser is null || connect.PlattformUser == plattformUser)
            return;
        connect.Connected = true;
        db.SaveChanges();

    }

    public static bool EndConnection(string userId, string connectionCode)
    {
        using var db = new UserConnectionContext();
        var connect = db.UserConnections.FirstOrDefault(x => x.ConnectionCode == connectionCode && x.ValidUntil > DateTime.UtcNow && x.Connected == false);
        var plattformUser = db.PlattformUsers.FirstOrDefault(x => x.PlattformUserId == userId);
        if (connect is null || plattformUser is null || connect.PlattformUser == plattformUser)
            return false;
        var otherUser = db.PlattformUsers.First(x => x.Id == plattformUser.Id);

        if (otherUser.User is not null && plattformUser.User is not null)
            return false;

        User? newUser;
        if (plattformUser.User is not null)
        {
            newUser = plattformUser.User;
        }
        else if (otherUser.User is not null)
        {
            newUser = otherUser.User;
        }
        else
        {
            newUser = db.Users.FirstOrDefault(x => x.DisplayName == connect.PlattformUser.Name);
            if (newUser is null)

                newUser = new User() { DisplayName = connect.PlattformUser.Name };
            db.Users.Add(newUser);
            db.SaveChanges();
        }

        connect.Connected = true;

        otherUser.User = newUser;
        plattformUser.User = newUser;
        db.SaveChanges();

        return connect.Connected;
    }
}
