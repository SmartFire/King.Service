﻿namespace King.Service.CloudService.Role.Scalable
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using King.Service;

    public class ScalableTask : IDynamicRuns
    {
        public int MaximumPeriodInSeconds
        {
            get
            {
                return 30;
            }
        }

        public int MinimumPeriodInSeconds
        {
            get
            {
                return 20;
            }
        }

        public Task<bool> Run()
        {
            var random = new Random();
            var workWasDone = (random.Next() % 2) == 0;

            Trace.TraceInformation("Work was done: {0}", workWasDone);

            return Task.FromResult(workWasDone);
        }
    }
}