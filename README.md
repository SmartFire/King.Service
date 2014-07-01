King.Azure.BackgroundWorker
============

Task scheduling for Azure; use with worker roles for running background processing tasks.
- Create a task to initialize your environment
- Create a task that runs every X amount of time per instance
- Create a task that runs every X amount of time across any number of instances

## NuGet
[Add via NuGet](https://www.nuget.org/packages/King.Service)
```
PM> Install-Package King.Service
```
## Examples
### [Initialization Task](https://github.com/jefking/King.Azure.BackgroundWorker/blob/master/Worker/InitTask.cs)
```
class MyTask : InitializeTask
{
	public virtual void Run()
	{
		//Initialize one time resources
	}
}
```
### [Repetitive Task](https://github.com/jefking/King.Azure.BackgroundWorker/blob/master/Worker/Task.cs)
```
class MyTask : TaskManager
{
	public virtual void Run()
	{
		//Process background work here.
	}
}
```
### [Coordinated Task](https://github.com/jefking/King.Azure.BackgroundWorker/blob/master/Worker/Coordinated.cs)
```
class MyTask : CoordinatedTask
{
	public MyTask()
		: base("Storage Account for Coordination between Instances", 60)
	{
	}
	public virtual void Run()
	{
		//Process background work here.
	}
}
```
### [Initialize Task](https://github.com/jefking/King.Azure.BackgroundWorker/blob/master/Worker/Factory.cs)
```
class Factory : TaskFactory
{
    public override IEnumerable<IRunnable> Tasks(object passthrough)
    {
		var tasks = new List<IRunnable>();
		// Initialization Task(s)
		tasks.Add(new InitTask());

		//Task(s)
		tasks.Add(new Task());

		//Cordinated Tasks between Instances

		var task = new Coordinated();
		// Add once to ensure that Table is created for Instances to communicate with
		tasks.Add(task.InitializeTask());

		// Add your coordinated task(s)
		tasks.Add(task);
            
		return tasks;
    }
}
```
### [Worker Role](https://github.com/jefking/King.Azure.BackgroundWorker/blob/master/Worker/WorkerRole.cs)
```
public class WorkerRole : RoleEntryPoint
{
    /// <summary>
    /// Role Service Manager
    /// </summary>
    private readonly RoleServiceManager manager = new RoleServiceManager(new Factory());

    /// <summary>
    /// Overloaded Role Entry Point Method
    /// </summary>
    public override void Run()
    {
        this.manager.Run();

        base.Run();
    }

    /// <summary>
    /// Overloaded Role Entry Point Method
    /// </summary>
    public override bool OnStart()
    {
        return this.manager.OnStart();
    }

    /// <summary>
    /// Overloaded Role Entry Point Method
    /// </summary>
    public override void OnStop()
    {
        this.manager.OnStop();

        base.OnStop();
    }
}
```
## Demo Project
[Azure Project](https://github.com/jefking/King.Azure.BackgroundWorker/tree/master/Azure.Demo)

[Worker Role](https://github.com/jefking/King.Azure.BackgroundWorker/tree/master/Worker)

## Contributing

Contributions are always welcome. If you have find any issues, please report them to the [Github Issues Tracker](https://github.com/jefking/King.Azure.BackgroundWorker/issues?sort=created&direction=desc&state=open).

## Apache 2.0 License

Copyright 2014 Jef King

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

[http://www.apache.org/licenses/LICENSE-2.0](http://www.apache.org/licenses/LICENSE-2.0)

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.