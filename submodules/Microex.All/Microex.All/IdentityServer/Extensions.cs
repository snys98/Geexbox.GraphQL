using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microex.All.IdentityServer.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MoreLinq;

namespace Microex.All.IdentityServer
{
    public static class Extensions
    {
        public static void EnsureIdentityServerSeedData(this IdentityServerDbContext context,
            IEnumerable<Client> clients, IEnumerable<IdentityResource> identityResources,
            IEnumerable<ApiResource> apiResources,
            IEnumerable<Role> roleConfig)
        {
            if (!context.Clients.Any())
            {
                foreach (var client in clients)
                {
                    context.Clients.Add(client.ToEntity());
                }
            }

            if (!context.IdentityResources.Any())
            {

                foreach (var resource in identityResources)
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in apiResources)
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
            }

            if (!context.Roles.Any())
            {
                context.Roles.AddRange(roleConfig);
            }
            context.SaveChanges();
        }
    }
}
