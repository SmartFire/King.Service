﻿namespace King.Service.CloudService.Role.Queue
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using King.Azure.Data;

    /// <summary>
    /// Company Processor, post dequeue action
    /// </summary>
    public class CompanyProcessor : IProcessor<CompanyModel>
    {
        public Task<bool> Process(CompanyModel data)
        {
            Trace.TraceInformation("Save company to data store: '{0}/{1}'", data.Name, data.Id);

            return Task.FromResult<bool>(true);
        }
    }
}