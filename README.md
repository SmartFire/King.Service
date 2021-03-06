### Task scheduling for Azure on Windows
+ Plugs into Azure compute services
+ Auto-Scale workloads within your server
+ Tasks to initialize your environment
 + Create Azure Storage: Queues, Tables and Containers
 + Load WCF services
+ Create Tasks that Occur:
 + Every X seconds per server instance
 + Every X seconds; and lessens frequency to Y when there is no work
 + That determines the needed rate via frequency of processing tasks
 + Once, even with multiple servers
 + Runs at a specified time (resolution to the hour, or the minute) on one server
+ Dequeue from Azure Storage Queues
 + Batches of messages
 + Shards for high throughput
 + Variable timing for cost saving
+ [Cloud Service](https://github.com/jefking/King.Service/tree/master/Demos/King.Service.CloudService.Role)
+ [Service Bus](https://github.com/jefking/King.Service.ServiceBus)
+ [WebJob](https://github.com/jefking/King.Service/tree/master/Demos/King.Service.WebJob)

### [NuGet](https://www.nuget.org/packages/King.Service)
```
PM> Install-Package King.Service
```

### [Wiki](https://github.com/jefking/King.Service/wiki)
View the wiki to learn how to use this.