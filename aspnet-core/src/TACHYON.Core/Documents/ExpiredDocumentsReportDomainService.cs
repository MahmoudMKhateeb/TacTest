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

                       //to set it monthly period,  1st of each month
                       .WithSchedule(CronScheduleBuilder.MonthlyOnDayAndHourAndMinute(1, 1, 0)).Build();

                       //for test 
                       //.WithSimpleSchedule(schedule =>
                       //{
                       //    schedule.RepeatForever()
                       //        .WithIntervalInSeconds(240)
                       //        .Build();
                       //});
            });

            // return Content("OK, scheduled!");

        }
       
    }
}
