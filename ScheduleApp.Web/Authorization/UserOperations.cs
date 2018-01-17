using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace ScheduleApp.Web.Authorization
{
    public static class UserOperations
    {
        public static OperationAuthorizationRequirement Mail = new OperationAuthorizationRequirement { Name = Constants.MailOperationName };
    }
}
