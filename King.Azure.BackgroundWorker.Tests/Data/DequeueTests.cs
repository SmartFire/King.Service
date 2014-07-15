﻿namespace King.Azure.BackgroundWorker.Tests.Data
{
    using King.Azure.BackgroundWorker.Data;
    using NSubstitute;
    using NUnit.Framework;
    using System;
    using System.Threading.Tasks;

    [TestFixture]
    public class DequeueTests
    {
        [Test]
        public void Constructor()
        {
            var poller = Substitute.For<IPoller<object>>();
            var processor = Substitute.For<IProcessor<object>>();
            new Dequeue<object>(poller, processor);
        }

        [Test]
        public void IsIBackoffRuns()
        {
            var poller = Substitute.For<IPoller<object>>();
            var processor = Substitute.For<IProcessor<object>>();
            Assert.IsNotNull(new Dequeue<object>(poller, processor) as IBackoffRuns);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorPollerNull()
        {
            var processor = Substitute.For<IProcessor<object>>();
            new Dequeue<object>(null, processor);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorProcessorNull()
        {
            var poller = Substitute.For<IPoller<object>>();
            new Dequeue<object>(poller, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorMinMaxZero()
        {
            var random = new Random();
            var poller = Substitute.For<IPoller<object>>();
            var processor = Substitute.For<IProcessor<object>>();
            new Dequeue<object>(poller, processor, random.Next(0));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorMinMaxSwitched()
        {
            var random = new Random();
            var value = random.Next(0, 1024);
            var poller = Substitute.For<IPoller<object>>();
            var processor = Substitute.For<IProcessor<object>>();
            new Dequeue<object>(poller, processor, value + 1, value - 1);
        }

        [Test]
        public void MinimumPeriodInSeconds()
        {
            var poller = Substitute.For<IPoller<object>>();
            var processor = Substitute.For<IProcessor<object>>();
            var d = new Dequeue<object>(poller, processor);
            Assert.AreEqual(15, d.MinimumPeriodInSeconds);
        }

        [Test]
        public void MaximumPeriodInSeconds()
        {
            var poller = Substitute.For<IPoller<object>>();
            var processor = Substitute.For<IProcessor<object>>();
            var d = new Dequeue<object>(poller, processor);
            Assert.AreEqual(300, d.MaximumPeriodInSeconds);
        }

        [Test]
        public async Task Run()
        {
            var data = new object();

            var message = Substitute.For<IQueued<object>>();
            message.Data().Returns(Task.FromResult(data));
            message.Delete();

            var poller = Substitute.For<IPoller<object>>();
            poller.Poll().Returns(Task.FromResult(message));

            var processor = Substitute.For<IProcessor<object>>();
            processor.Process(data).Returns(Task.FromResult(true));

            var d = new Dequeue<object>(poller, processor);

            var result = await d.Run();
            Assert.IsTrue(result);

            message.Received().Data();
            message.Received().Delete();
            poller.Received().Poll();
            processor.Received().Process(data);
        }

        [Test]
        public async Task RunPollNull()
        {
            var poller = Substitute.For<IPoller<object>>();
            poller.Poll().Returns(Task.FromResult<IQueued<object>>(null));

            var processor = Substitute.For<IProcessor<object>>();

            var d = new Dequeue<object>(poller, processor);

            var result = await d.Run();
            Assert.IsFalse(result);

            poller.Received().Poll();
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public async Task RunPollThrows()
        {
            var poller = Substitute.For<IPoller<object>>();
            poller.Poll().Returns(x => { throw new ApplicationException(); });

            var processor = Substitute.For<IProcessor<object>>();

            var d = new Dequeue<object>(poller, processor);
            await d.Run();
        }

        [Test]
        public async Task RunDataNull()
        {
            var message = Substitute.For<IQueued<object>>();
            message.Data().Returns(Task.FromResult<object>(null));

            var poller = Substitute.For<IPoller<object>>();
            poller.Poll().Returns(Task.FromResult(message));

            var processor = Substitute.For<IProcessor<object>>();

            var d = new Dequeue<object>(poller, processor);

            var result = await d.Run();
            Assert.IsTrue(result);

            poller.Received().Poll();
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public async Task RunDataThrows()
        {
            var message = Substitute.For<IQueued<object>>();
            message.Data().Returns(x => { throw new ApplicationException(); });
            message.Delete();

            var poller = Substitute.For<IPoller<object>>();
            poller.Poll().Returns(Task.FromResult(message));

            var processor = Substitute.For<IProcessor<object>>();

            var d = new Dequeue<object>(poller, processor);

            var result = await d.Run();
            Assert.IsTrue(result);
        }
    }
}