namespace ScheduleApp.Web.Extensions
{
    public static class Constants
    {
        public static readonly string REQUEST_STATUS_NEW = "NEW";
        public static readonly string REQUEST_STATUS_ACCEPTED = "ACCEPTED";
        public static readonly string REQUEST_STATUS_REJECTED = "DECLINED";
    }

    public enum RequestType
    {
        MY_REQUESTS,
        REQUESTS_TO_ME
        //myDirect,
        //myBroadcast,
        //directToMe,
        //broadcastToMe
    }
}