namespace TACHYON.EntityLogs.Jobs
{
    // ItemReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    public class EntityLogJobArguments
    {
        public EntityLogJobArguments(long entityChangeId)
        {
            EntityChangeId = entityChangeId;
        }

        public long EntityChangeId { get; set; }


    }
}