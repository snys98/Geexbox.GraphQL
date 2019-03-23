using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.TypedOriginalValues;
using IdentityServer4.Extensions;
using Microex.All.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microex.All.EntityFramework
{
    public class IntegratedDbContext<TUser> : IdentityDbContext<TUser, IdentityRole, string> where TUser : IdentityUser
    {
        public IntegratedDbContext(DbContextOptions options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        //public Task<int> SaveChangesAsync([CallerMemberName]string callMemberName = null, CancellationToken cancellationToken = new CancellationToken())
        //{
        //    if (callMemberName != null && callMemberName.Contains("SaveChangesAsync"))
        //    {
        //        return base.SaveChangesAsync(cancellationToken);
        //    }
        //    throw new NotSupportedException("请勿直接使用dbcontext修改数据,使用UnitOfWork来进行数据修改操作");
        //}
        public int UowSaveChanges(bool acceptAllChangesOnSuccess = true, CancellationToken cancellationToken = new CancellationToken(), [CallerMemberName]string callMemberName = null)
        {
            //if (callMemberName != nameof(UnitOfWork<IntegratedDbContext<TUser>, TUser>.SaveChangesAsync))
            //{
            //    throw new NotSupportedException("请勿直接使用dbcontext修改数据,使用UnitOfWork来进行数据修改操作");
            //}
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Metadata.ClrType == null || entry.Metadata.ClrType.BaseType == null || !entry.Metadata.ClrType.IsSubclassOf(typeof(EntityBase)))
                {
                    continue;
                }
                switch (entry.State)
                {
                    case EntityState.Deleted:
                        //级联软删除
                        foreach (var navigationEntry in entry.Navigations
                            .Where(n => !n.Metadata.IsDependentToPrincipal()))
                        {
                            if (navigationEntry is CollectionEntry collectionEntry)
                            {
                                foreach (var dependentEntry in collectionEntry.CurrentValue)
                                {
                                    HandleDependent(Entry(dependentEntry));
                                }
                            }
                            else
                            {
                                var dependentEntry = navigationEntry.CurrentValue;
                                if (dependentEntry != null)
                                {
                                    HandleDependent(Entry(dependentEntry));
                                }
                            }
                        }

                        if (entry.Entity is ISoftDelete)
                        {
                            entry.State = EntityState.Modified;
                            entry.CurrentValues[nameof(ISoftDelete.IsDeleted)] = true;
                        }
                        break;
                }
            }
            var saved = false;
            var result = 0;
            while (!saved)
            {
                try
                {
                    // Attempt to save changes to the database
                    result = base.SaveChanges(acceptAllChangesOnSuccess);
                    saved = true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        var proposedValues = entry.CurrentValues;
                        var databaseValues = entry.GetDatabaseValues();

                        if (databaseValues == null)
                        {
                            throw;
                        }
                        foreach (var property in proposedValues.Properties)
                        {
                            var proposedValue = proposedValues[property];
                            var databaseValue = databaseValues[property];

                            // 无条件接受新值
                            proposedValues[property] = proposedValue ?? databaseValue;
                        }
                        entry.OriginalValues.SetValues(databaseValues);
                    }
                    result = base.SaveChanges(acceptAllChangesOnSuccess);
                }
            }
            return result;
        }
        private void HandleDependent(EntityEntry entry)
        {
            if (entry.Metadata.ClrType == null || entry.Metadata.ClrType.BaseType == null || !entry.Metadata.ClrType.IsSubclassOf(typeof(EntityBase)))
            {
                entry.State = EntityState.Deleted;
            }
        }
        //public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        //{
        //    throw new NotSupportedException("请勿直接使用dbcontext修改数据,使用UnitOfWork来进行数据修改操作");
        //    //return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        //}

        //public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        //{
        //    throw new NotSupportedException("请勿直接使用dbcontext修改数据,使用UnitOfWork来进行数据修改操作");
        //    //return base.SaveChangesAsync(cancellationToken);
        //}

        //public override int SaveChanges(bool acceptAllChangesOnSuccess)
        //{
        //    throw new NotSupportedException("请勿直接使用dbcontext修改数据,使用UnitOfWork来进行数据修改操作");
        //    //return base.SaveChanges(acceptAllChangesOnSuccess);
        //}

        //public override int SaveChanges()
        //{
        //    throw new NotSupportedException("请勿直接使用dbcontext修改数据,使用UnitOfWork来进行数据修改操作");
        //    //return base.SaveChanges();
        //}
    }
}
