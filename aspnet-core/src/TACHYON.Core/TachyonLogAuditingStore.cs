using Abp.Auditing;
using Abp.Json;
using Atlassian.Jira;
using Castle.Core.Logging;
using System.Threading.Tasks;

namespace TACHYON
{
    /// <summary>         
    /// Implements <see cref="T:Abp.Auditing.IAuditingStore" /> to simply write audits to logs.
    /// </summary>
    public class TachyonLogAuditingStore : IAuditingStore
    {
        /// <summary>Singleton instance.</summary>
        public static TachyonLogAuditingStore Instance { get; } = new TachyonLogAuditingStore();

        public ILogger Logger { get; set; }

        public TachyonLogAuditingStore() => this.Logger = (ILogger)NullLogger.Instance;

        public Task SaveAsync(AuditInfo auditInfo)
        {
            if (auditInfo.Exception == null)
            {
                this.Logger.Info(auditInfo.ToString());
            }
            else
            {
                this.Logger.Warn(auditInfo.ToString());
                _ = CreateJiraIssue(auditInfo);
            }

            return (Task)Task.FromResult<int>(0);
        }

        public void Save(AuditInfo auditInfo)
        {
            if (auditInfo.Exception == null)
            {
                this.Logger.Info(auditInfo.ToString());
            }
            else
            {
                this.Logger.Warn(auditInfo.ToString());
                _ = CreateJiraIssue(auditInfo);
            }
        }

        private async Task CreateJiraIssue(AuditInfo auditInfo)
        {
            AuditLog auditLog = AuditLog.CreateFromAuditInfo(auditInfo);
        
            // create a connection to JIRA using the Rest client
            var jira = Jira.CreateRestClient("https://tachyonhub.atlassian.net", "ialkhateeb@trustangle.com", "cZNnMDxyJbbPdirRLQQI3A63");

            var issue = jira.CreateIssue("TAC");
            issue.Type = "Bug";
            issue.Priority = "Highest";
            issue.Summary =string.Format("TachyonAudit: {0}.{1} error!.", (object)auditLog.ServiceName, (object)auditLog.MethodName);
            issue.Description = auditLog.ToJsonString();

            await issue.SaveChangesAsync();
        }
    }
}
