using BotMaster.Database;

namespace BotMaster.RightsManagement;
public static class EntityRightExtension
{
    public static bool HasRight(this User user, string right)
        => user.Rights.Any(x => x.Name == right)
            || user.Groups.Any(x => x.HasRight(right));

    public static bool HasRight(this Group group, string right) => group.Rights?.Any(x => x.Name == right) ?? false;

    public static bool HasRight(this PlattformUser user, string right)
        => user.Rights.Any(x => x.Name == right)
            || (user.User?.Rights.Any(x => x.Name == right) ?? false)
            || user.Groups.Any(x => x.HasRight(right));

    public static void AddRight(this User user, DatabaseContext context, string rightName)
        => user.Rights.Add(GetRight(context, rightName, true));

    public static void AddRight(this PlattformUser user, DatabaseContext context, string rightName)
        => user.Rights.Add(GetRight(context, rightName, true));

    public static void AddRight(this Group group, DatabaseContext context, string rightName)
        => group.Rights.Add(GetRight(context, rightName, true));

    public static void RemoveRight(this User user, DatabaseContext context, string rightName)
    {
        var right = GetRight(context, rightName);
        if (right is null)
            return;

        user.Rights.Remove(right);
    }

    public static void RemoveRight(this PlattformUser user, DatabaseContext context, string rightName)
    {
        var right = GetRight(context, rightName);
        if (right is null)
            return;

        user.Rights.Remove(right);
    }

    public static void RemoveRight(this Group group, DatabaseContext context, string rightName)
    {
        var right = GetRight(context, rightName);
        if (right is null)
            return;

        group.Rights.Remove(right);
    }

    private static Right GetRight(DatabaseContext context, string rightName, bool createIfNotExisting = false)
    {
        var right = context.Set<Right>().FirstOrDefault(x => x.Name == rightName);
        if (right is null && createIfNotExisting)
        {
            right = context.Set<Right>().Add(new Right { Name = rightName }).Entity;
        }
        return right;
    }
}
