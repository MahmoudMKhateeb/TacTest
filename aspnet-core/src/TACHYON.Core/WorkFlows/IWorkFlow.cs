using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.WorkFlows
{
    public interface IWorkFlow<TArgs, TEnum> : IDomainService where TEnum : Enum
    {

        List<WorkFlow<TArgs, TEnum>> Flows { get; set; }

        List<WorkflowTransaction<TArgs, TEnum>> GetTransactions(int workFlowVersion);

        List<WorkflowTransaction<TArgs, TEnum>> GetAvailableTransactions(int workFlowVersion, TEnum statusesEnum);
    }
}