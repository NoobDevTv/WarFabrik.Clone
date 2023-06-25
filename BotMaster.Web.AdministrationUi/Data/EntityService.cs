using BotMaster.Database;

using Microsoft.EntityFrameworkCore;

namespace BotMaster.Web.AdministrationUi.Data;


public class EntityService<T, TEntity>
    where T : DatabaseContext, new()
    where TEntity : IdEntity<int>, ICloneableGeneric<TEntity>
{

    public List<TEntity> Get(params string[] includes)
    {
        List<TEntity> ret;
        {

            using var ctx = new T();
            ctx.EnableUseLazyLoading = false;
            IQueryable<TEntity> set = ctx.Set<TEntity>();

            foreach (var item in includes)
            {
                set = set.Include(item);
            }
            ret = set.ToList();


        }
        return ret;
    }

    public async Task AddForeignKey<TEntityToAdd>(TEntity entity, TEntityToAdd entityToAdd, Action<TEntity, TEntityToAdd> addFunc)
        where TEntityToAdd : Entity
    {
        using var ctx = new T();
        ctx.Attach(entity);
        ctx.Attach(entityToAdd);
        addFunc(entity, entityToAdd);
        await ctx.SaveChangesAsync();
    }

    public async Task RemoveForeignKey<TEntityToRemove>(TEntity entity, TEntityToRemove entityToRemove, Action<TEntity, TEntityToRemove> removeFunc)
        where TEntityToRemove : Entity
    {
        using var ctx = new T();
        ctx.Attach(entity);
        ctx.Attach(entityToRemove);
        removeFunc(entity, entityToRemove);
        await ctx.SaveChangesAsync();
    }

    public async Task AddOrUpdateCommand(TEntity entity)
    {
        using var ctx = new T();
        ctx.EnableUseLazyLoading = true;

        var t = ctx.Entry(entity);
        if (t.State == EntityState.Detached)
        {
            IQueryable<TEntity> set = ctx.Set<TEntity>();
            var entityExisting = set.SingleOrDefault(x => x.Id == entity.Id);
            if (entityExisting != default)
            {
                var attachedEntry = ctx.Entry(entityExisting);
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                var  c = ctx.Attach(entity);
                c.State = EntityState.Added;
            }
        }
        var res = ctx.SaveChanges();
    }

    public async Task DeleteCommand(TEntity entity)
    {
        using var ctx = new T();
        var tracking = ctx.Remove(entity);
        var res = await ctx.SaveChangesAsync();
    }

    public async Task AddOrRemove<TForeignKey>(TEntity entity, ICollection<TForeignKey> listToCompare, ICollection<TForeignKey> origValue, IEnumerable<TForeignKey> selectedEntries)
        where TForeignKey : Entity
    {
        origValue.Clear();
        foreach (var item in listToCompare)
        {
            origValue.Add(item);
        }
        foreach (var existing in listToCompare)
        {
            if (!selectedEntries.Any(x => x.Equals(existing)))
            {
                await RemoveForeignKey(entity, existing, (_, p) => origValue.Remove(p));
            }
        }
        foreach (var shouldExist in selectedEntries)
        {
            if (!listToCompare.Any(x => x.Equals(shouldExist)))
            {
                await AddForeignKey(entity, shouldExist, (_, p) => origValue.Add(p));
            }
        }
    }
}