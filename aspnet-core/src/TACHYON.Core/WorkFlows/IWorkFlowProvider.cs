using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Tracking.Dto.WorkFlow;

namespace TACHYON.WorkFlows
{
    /// <summary>
    /// Base Interface for Workflow Provider 
    /// </summary>
    /// <typeparam name="TTransaction"></typeparam>
    /// <typeparam name="TArgs"></typeparam>
    /// <typeparam name="TEnum"></typeparam>
    public interface IWorkFlowProvider<TTransaction,TArgs,TEnum> : IDomainService where
        TEnum : Enum where TTransaction : IWorkflowTransaction
    {
        List<WorkFlow<TTransaction>> Flows { get; set; }
        Task Invoke(TArgs args, string action);
        List<TTransaction> GetTransactions(int workFlowVersion);
        List<TTransaction> GetAvailableTransactions(int workFlowVersion, TEnum statusesEnum);
    }
}