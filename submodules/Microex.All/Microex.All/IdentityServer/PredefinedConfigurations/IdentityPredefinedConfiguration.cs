using System;
using System.Collections.Generic;
using System.Text;
using Microex.All.IdentityServer.Identity;

namespace Microex.All.IdentityServer.PredefinedConfigurations
{
    public static class IdentityPredefinedConfiguration
    {
        public const string AdministrationPolicy = "Administrator";

        public const string AdministrationRoleName = "Administrator";
        public static List<Role> Roles => new List<Role>()
        {
            new Role()
            {
                Name = AdministrationRoleName,
            }
        };

    }
}
