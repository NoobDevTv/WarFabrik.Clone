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
            ;
        }

        //var ent = ret.FirstOrDefault();
        //if (ent is User)
        //{
        //    //AddOrUpdateCommand(System.Text.Json.JsonSerializer.Deserialize<TEntity>(System.Text.Json.JsonSerializer.Serialize(ent))).GetAwaiter().GetResult();
        //    AddOrUpdateCommand(ent).GetAwaiter().GetResult();
        //}
        return ret;
    }

    public async Task AddOrUpdateCommand(TEntity entity)
    {
        using var ctx = new T();
        ctx.EnableUseLazyLoading = true;
        var tracking = ctx.Update(entity);
        //var t = ctx.Entry<TEntity>(entity);
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
        //if (t.State == EntityState.Detached)
        //{
        //    IQueryable<TEntity> set = ctx.Set<TEntity>();
        //    var entityExisting = set.SingleOrDefault(x => x.Id == entity.Id);
        //    var attachedEntry = ctx.Entry(entityExisting);
        //    attachedEntry.CurrentValues.SetValues(entity);
        //}
        var res = ctx.SaveChanges();
    }

    public async Task DeleteCommand(TEntity entity)
    {
        using var ctx = new T();
        var tracking = ctx.Remove(entity);
        var res = await ctx.SaveChangesAsync();
    }
}