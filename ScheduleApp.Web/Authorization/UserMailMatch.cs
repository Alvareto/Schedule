using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace ScheduleApp.Web.Authorization
{
    public class UserMailMatch : AuthorizationHandler<OperationAuthorizationRequirement, IEnumerable<ScheduleApp.Model.User>>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
            IEnumerable<ScheduleApp.Model.User> resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.FromResult(0);
            }

            // If we're not asking for CRUD permission, return.
            if (resource.Any(s => String.Equals(context.User.Identity.Name, s.Email) && s.IsActive.GetValueOrDefault(false)))
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }
    }
}
