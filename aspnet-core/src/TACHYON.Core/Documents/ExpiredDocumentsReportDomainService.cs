using Abp.Dependency;
using Abp.Quartz;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Authorization.Users;

namespace TACHYON.Documents
{
    public class ExpiredDocumentsReportDomainService : TACHYONDomainServiceBase
    {
        private readonly DocumentFilesManager _documentFilesManager;
        private readonly IQuartzScheduleJobManager _jobManager;
        private readonly IUserEmailer _userEmailer;

        public ExpiredDocumentsReportDomainService(DocumentFilesManager documentFilesManager, IQuartzScheduleJobManager jobManager, IUserEmailer userEmailer)
        {
            _documentFilesManager = documentFilesManager;
            _jobManager = jobManager;
            _userEmailer = userEmailer;
        }

        public async void RunJob()
        {
            //var documents =await _documentFilesManager.GetAllTenantDriverAndTruckDocumentFilesListAsync();

            //await _userEmailer.SendDocumentsExpiredInfoAsyn(documents, documents.FirstOrDefault().TenantId.Value);
            await _jobManager.ScheduleAsync<ExpiredDocumentsReportJob>(
            job =>
            {
                job.WithIdentity("MyLogJobIdentity", "MyGroup")
                    .WithDescription("A job to simply write logs.");
            },
            trigger =>
            {
                trigger.StartNow()
                       .WithIdentity("Run Infinitely every 1st day of the month", "Monthly_Day_1")
                       //.WithSchedule(CronScheduleBuilder.MonthlyOnDayAndHourAndMinute(1, 1, 0)).Build();
                       .WithSimpleSchedule(schedule =>
                       {
                           schedule.RepeatForever()
                               .WithIntervalInSeconds(240)
                               .Build();
                       }); //for test
            });
            //Console.WriteLine("tasneem ");

            // return Content("OK, scheduled!");

        }
       
    }
}
