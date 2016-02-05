namespace King.Service.CloudService.Role.Queue
{
    using King.Azure.Data;
    using King.Service;
    using System;

    public class CompanyShardQueuer : RecurringTask
    {
        private int id = 0;
        private readonly StorageQueueShards queue = null;

        public CompanyShardQueuer(string name, string connection)
            :base(10)
        {
            this.queue = new StorageQueueShards(name, connection);
        }

        public override void Run()
        {
            var company = new CompanyModel()
            {
                Id = Guid.NewGuid(),
                Name = string.Format("company-{0}", id),
            };

            var task = this.queue.Save(company);
            task.Wait();

            id++;
        }
    }
}