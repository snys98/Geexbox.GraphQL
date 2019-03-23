using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microex.All.Common;
using Microex.All.EntityFramework;
using Microex.All.IdentityServer;
using Microex.All.IdentityServer.Identity;
using Microex.All.IdentityServer.PredefinedConfigurations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Microex.All.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// 用于自动迁移dbcontext
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="host"></param>
        /// <returns></returns>
        public static IWebHost EnsurePredefinedIdentityServerConfigs<TContext>(this IWebHost host, Action<TContext> seedAction = null) where TContext : IdentityServerDbContext
        {
            using (var scope = host.Services.CreateScope())
            {//只在本区间内有效
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                try
                {

                    var context = services.GetRequiredService<TContext>();
                    context.Database.Migrate();
                    context.EnsureIdentityServerSeedData(new[] { ClientPredefinedConfiguration.AdminManageClient },
                        ResourcePredefinedConfiguration.IdentityResources,
                        new ApiResource[] { },
                        IdentityPredefinedConfiguration.Roles);
                    seedAction?.Invoke(context);
                    logger.LogInformation($"AutoMigrateDbContext {typeof(TContext).Name} 执行成功");
                }
                catch (Exception e)
                {
                    logger.LogCritical(e, "数据库自动migration失败");
                    throw;
                }

            }
            return host;
        }

        /// <summary>
        /// 用于自动迁移dbcontext
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="host"></param>
        /// <returns></returns>
        public static IWebHost EnsureMigrations<TContext, TUser>(this IWebHost host, Action<IServiceProvider,UnitOfWork<TContext, TUser>> seedAction = null) where TContext : IntegratedDbContext<TUser> where TUser : IdentityUser
        {
            using (var scope = host.Services.CreateScope())
            {//只在本区间内有效
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                try
                {
                    var context = services.GetRequiredService<TContext>();
                    context.Database.Migrate();
                    var uow = services.GetRequiredService<UnitOfWork<TContext, TUser>>();
                    seedAction?.Invoke(services,uow);
                    logger.LogInformation($"AutoMigrateDbContext {typeof(TContext).Name} 执行成功");
                }
                catch (Exception e)
                {
                    logger.LogCritical(e, "数据库自动migration失败");
                    throw;
                }
            }
            return host;
        }

        /// <summary>
        /// 自动添加包含符合标准主键的表到Repository中
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddUnitOfWork<TContext, TUser>(this IServiceCollection serviceCollection,
            Action<DbContextOptionsBuilder> optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped) where TContext : IntegratedDbContext<TUser> where TUser : IdentityUser
        {
            serviceCollection.AddDbContext<TContext, TContext>(optionsAction, contextLifetime, optionsLifetime);
            //单例的uow,保证事务完整性
            serviceCollection.AddScoped<UnitOfWork<TContext, TUser>>();
            return serviceCollection;
        }

        public static IQueryable<T> PageBy<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> orderBy, int page, int pageSize, bool orderByDescending = true)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            // It is necessary sort items before it
            query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public static IEnumerable<T> PageBy<T, TKey>(this IEnumerable<T> query, Func<T, TKey> orderBy, int page, int pageSize, bool orderByDescending = true)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (orderBy == null)
            {
                return query.Skip((page) * pageSize).Take(pageSize);
            }

            // It is necessary sort items before it
            query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

            return query.Skip((page) * pageSize).Take(pageSize);
        }


        public static async Task<PagedList<TEntity>> ToPagedListAsync<TEntity, TKey>(this IQueryable<TEntity> rawData, int pageIndex, int pageSize, Expression<Func<TEntity, IComparable>> orderBy, bool orderByDescending = false) where TEntity : EntityBase
        {
            var pagedList = new PagedList<TEntity>();
            IQueryable<TEntity> query = rawData;

            pagedList.TotalCount = await query.CountAsync();

            query = query.PageBy(orderBy, pageIndex, pageSize, orderByDescending);

            List<TEntity> data = await query
                .ToListAsync();
            pagedList.Items.AddRange(data);
            pagedList.PageSize = pageSize;
            return pagedList;
        }

        public static async Task<PagedList<TEntity>> ToPagedListAsync<TEntity, TKey>(this IEnumerable<TEntity> rawData, int pageIndex, int pageSize, Func<TEntity, IComparable> orderBy, bool orderByDescending = false) where TEntity : class
        {
            var pagedList = new PagedList<TEntity>();
            IEnumerable<TEntity> query = rawData;

            pagedList.TotalCount = query.Count();

            query = query.PageBy(orderBy, pageIndex, pageSize, orderByDescending);

            List<TEntity> data = query
                .ToList();
            pagedList.Items.AddRange(data);
            pagedList.PageSize = pageSize;
            return pagedList;
        }

        public static async Task<PagedList<TEntity>> ToPagedListAsync<TEntity>(this IEnumerable<TEntity> rawData, int pageIndex = 0, int pageSize = 10, Func<TEntity, IComparable> orderBy = null, bool orderByDescending = true) where TEntity : class
        {
            return await ToPagedListAsync<TEntity, string>(rawData, pageIndex, pageSize, orderBy, orderByDescending = true);
        }

        public static async Task<PagedList<TEntity>> ToPagedListByCreateTimeAsync<TEntity>(this IEnumerable<TEntity> rawData, int pageIndex = 0, int pageSize = 10, bool orderByDescending = true) where TEntity : EntityBase
        {
            return await ToPagedListAsync<TEntity, string>(rawData, pageIndex, pageSize, entity => entity.CreateTime, orderByDescending = true);
        }

        public static void ConfigIEntity<T>(this EntityTypeBuilder<T> builder) where T : EntityBase
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreateTime).ValueGeneratedOnAdd().HasValueGenerator<DatetimeValueGenerator>().HasDefaultValueSql("GETDATE()");
            builder.HasIndex(x => x.CreateTime);
            builder.HasQueryFilter(x => EF.Functions.DateDiffYear(x.CreateTime, DateTime.Now) < 1);
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
            {
                builder.Property(typeof(bool), nameof(ISoftDelete.IsDeleted)).HasDefaultValue(false);
                builder.HasQueryFilter(x => ((ISoftDelete)x).IsDeleted == false);
            }
            builder.Property(x => x.LastModifyTime).ValueGeneratedOnAddOrUpdate().HasValueGenerator<DatetimeValueGenerator>().HasDefaultValueSql("GETDATE()");
        }

        public static IQueryable<T> IncludeAll<T>(this IQueryable<T> queryable) where T : class
        {
            var type = typeof(T);
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var isVirtual = property.GetGetMethod().IsVirtual;
                if (isVirtual && properties.FirstOrDefault(c => c.Name == property.Name + "Id") != null)
                {
                    queryable = queryable.Include(property.Name);
                }
            }
            return queryable;
        }

    }
}
