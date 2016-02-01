﻿namespace King.Service.Data
{
    using King.Azure.Data;
    using King.Service.Data.Model;
    using King.Service.Scalability;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Dequeue Factory
    /// </summary>
    public class DequeueFactory : IDequeueFactory
    {
        #region Members
        /// <summary>
        /// Connection String
        /// </summary>
        protected readonly string connectionString = null;

        /// <summary>
        /// Throughput
        /// </summary>
        protected readonly IQueueThroughput throughput = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="connectionString">Connection String</param>
        public DequeueFactory(string connectionString)
            : this(connectionString, new QueueThroughput())
        {
        }

        /// <summary>
        /// Mockable Constructor
        /// </summary>
        /// <param name="connectionString">Connection String</param>
        /// <param name="throughput">Queue Throughput</param>
        public DequeueFactory(string connectionString, IQueueThroughput throughput = null)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("connectionString");
            }

            this.connectionString = connectionString;
            this.throughput = throughput ?? new QueueThroughput();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the Queues, and Dequeuers
        /// </summary>
        /// <typeparam name="T">Model</typeparam>
        /// <param name="queueName">Queue Name</param>
        /// <param name="processor">Processor</param>
        /// <param name="priority">Priority</param>
        /// <returns>Tasks</returns>
        public virtual IEnumerable<IRunnable> Tasks<T>(string queueName, Func<IProcessor<T>> processor, QueuePriority priority = QueuePriority.Low)
        {
            if (string.IsNullOrWhiteSpace(queueName))
            {
                throw new ArgumentException("queueName");
            }
            if (null == processor)
            {
                throw new ArgumentNullException("processor");
            }

            var setup = new QueueSetup<T>()
            {
                Processor = processor,
                Priority = priority,
                Name = queueName,
            };

            return this.Tasks<T>(setup);
        }

        /// <summary>
        /// Initializes the Queues, and Dequeuers
        /// </summary>
        /// <typeparam name="T">Model</typeparam>
        /// <param name="setups">Setups</param>
        /// <returns>Tasks</returns>
        public virtual IEnumerable<IRunnable> Tasks<T>(IEnumerable<IQueueSetup<T>> setups)
        {
            if (null == setups)
            {
                throw new ArgumentNullException("setups");
            }

            foreach (var setup in setups)
            {
                foreach (var t in this.Tasks(setup))
                {
                    yield return t;
                }
            }
        }

        /// <summary>
        /// Initializes the Queue, and Dequeuer
        /// </summary>
        /// <typeparam name="T">Model</typeparam>
        /// <param name="setup">Setup</param>
        /// <returns>Tasks</returns>
        public virtual IEnumerable<IRunnable> Tasks<T>(IQueueSetup<T> setup)
        {
            if (null == setup)
            {
                throw new ArgumentNullException("setup");
            }

            yield return new InitializeStorageTask(new StorageQueue(setup.Name, this.connectionString));
            yield return this.Dequeue<T>(setup);
        }

        /// <summary>
        /// Dequeue Task (Storage Queue Auto Scaler)
        /// </summary>
        /// <typeparam name="T">Model</typeparam>
        /// <param name="setup">Setup</param>
        /// <returns>Storage Queue Auto Scaler</returns>
        public virtual IRunnable Dequeue<T>(IQueueSetup<T> setup)
        {
            if (null == setup)
            {
                throw new ArgumentNullException("setup");
            }

            var messagesPerScaleUnit = this.throughput.MessagesPerScaleUnit(setup.Priority);
            var scale = this.throughput.Scale(setup.Priority);
            var checkScaleInMinutes = this.throughput.CheckScaleEvery(setup.Priority);
            var connection = new QueueConnection<T>()
            {
                ConnectionString = this.connectionString,
                Setup = setup,
            };
            var queue = new StorageQueue(connection.Setup.Name, connection.ConnectionString);

            return new StorageQueueAutoScaler<T>(queue, connection, this.throughput, messagesPerScaleUnit, scale.Minimum, scale.Maximum, checkScaleInMinutes);
        }

        /// <summary>
        /// Dequeue Task Set
        /// </summary>
        /// <typeparam name="Y">Processor</typeparam>
        /// <typeparam name="T">Model</typeparam>
        /// <param name="name">Name</param>
        /// <param name="priority">Priority</param>
        /// <param name="shardCount">Shard Count</param>
        /// <returns>Tasks</returns>
        public virtual IEnumerable<IRunnable> Dequeue<T, Y>(string name, QueuePriority priority = QueuePriority.Low, int shardCount = 0)
            where T : IProcessor<Y>, new()
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name");
            }

            var queues = new List<IRunnable>();

            if (shardCount <= 0)
            {
                var setup = new QueueSetupProcessor<T, Y>()
                {
                    Priority = priority,
                    Name = name,
                };

                queues.AddRange(this.Tasks<Y>(setup));
            }
            else
            {
                for (var i = 0; i < shardCount; i++)
                {
                    var setup = new QueueSetupProcessor<T, Y>()
                    {
                        Priority = priority,
                        Name = string.Format("{0}{1}", name, i),
                    };

                    queues.AddRange(this.Tasks<Y>(setup));
                }
            }

            return queues;
        }
        #endregion
    }
}