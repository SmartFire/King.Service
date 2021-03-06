﻿namespace King.Service.CloudService.Role.Scalable
{
    using King.Azure.Data;
    using King.Service;
    using King.Service.CloudService.Role.Queue;
    using King.Service.Data;
    using System.Collections.Generic;

    public class ScalableQueue : QueueAutoScaler<Configuration>
    {
        public ScalableQueue(IQueueCount count, Configuration config)
            : base(count, 1, config, 1, 15, 1)
        {
        }

        public override IEnumerable<IScalable> ScaleUnit(Configuration config)
        {
            yield return new BackoffRunner(new CompanyDequeuer(config.FastQueueName, config.ConnectionString));
        }
    }
}