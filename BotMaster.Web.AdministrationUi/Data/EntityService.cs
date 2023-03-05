using BotMaster.Commandos;
using BotMaster.Database;
using BotMaster.RightsManagement;

using Microsoft.EntityFrameworkCore;

using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Xml.Schema;

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
        //var tracking = ctx.Update(entity);
        //ctx.ChangeTracker.TrackGraph(entity, (a) =>
        //{
        //    if (a.Entry.IsKeySet)
        //    {
        //        IQueryable<TEntity> set = ctx.Set<TEntity>();
        //        var entityExisting = set.SingleOrDefault(x => x.Id == entity.Id);
        //        var attachedEntry = ctx.Entry(entityExisting);
        //        attachedEntry.CurrentValues.SetValues(entity);
        //        a.Entry.State = attachedEntry.State;
        //    }
        //    else
        //    {
        //        a.Entry.State = EntityState.Added;
        //    }
        //});
        var t = ctx.Entry<TEntity>(entity);
        if (t.State == EntityState.Detached)
        {
            IQueryable<TEntity> set = ctx.Set<TEntity>();
            var entityExisting = set.SingleOrDefault(x => x.Id == entity.Id);
            var attachedEntry = ctx.Entry(entityExisting);
            attachedEntry.CurrentValues.SetValues(entity);
        }
        var res = ctx.SaveChanges();
    }

    public async Task DeleteCommand(TEntity entity)
    {
        using var ctx = new T();
        var tracking = ctx.Remove(entity);
        var res = await ctx.SaveChangesAsync();
    }
}