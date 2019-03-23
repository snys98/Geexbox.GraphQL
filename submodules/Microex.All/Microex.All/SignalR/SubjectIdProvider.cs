using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace Microex.All.SignalR
{
    public class SubjectIdProvider: IUserIdProvider
    {
        #region Implementation of IUserIdProvider

        /// <inheritdoc />
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User.Identity.GetSubjectId();
        }

        #endregion
    }
}
