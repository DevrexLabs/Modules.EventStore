using System;
using System.Net;
using NUnit.Framework;
using OrigoDB.Core;
using OrigoDB.Core.Proxy;
using OrigoDB.Core.Test;

namespace OrigoDB.Modules.EventStore.Test
{
    [TestFixture]
    public class ESCommandStoreTests
    {

        [Test]
        public void SmokeTest2()
        {
            var config = new EngineConfiguration().ForIsolatedTest();
            var endPoint = new IPEndPoint(IPAddress.Loopback, 1113);
            var streamName = "origodb.SmokeTest2---" + Guid.NewGuid();
            config.SetCommandStoreFactory(cfg => new ESCommandStore(cfg, endPoint, streamName));
            var engine = Engine.Create<TestModel>(config);
            engine.Execute(new TestCommand());
            engine.Execute(new TestCommand());
            engine.Close();

            engine = Engine.Load<TestModel>(config);
            var db = engine.GetProxy();
            Assert.AreEqual(db.GetState(),2);
            var state = engine.Execute(m => m.State);
            Assert.AreEqual(2, state);
            engine.Close();
        }


        [Test]
        public void SmokeTest()
        {
            var config = new EngineConfiguration().ForIsolatedTest();
            var endPoint = new IPEndPoint(IPAddress.Loopback, 1113);
            var store = new ESCommandStore(config, endPoint, "origodb.SmokeTest-" + Guid.NewGuid().ToString());
            store.Initialize();
            var timeStamp = new DateTime(2000,1,1);

            var appender = JournalAppender.Create(1, store);

            //write 100 entries
            for (ulong i = 0; i < 100; i++)
            {
                appender.Append(new ProxyCommand<TestModel>("fish", null){Timestamp = timeStamp.AddMinutes(i)});
            }

            //read from beginning
            int numRead = 0;
            foreach (var journalEntry in store.GetJournalEntriesFrom(0))
            {
                Assert.IsInstanceOf<JournalEntry<Command>>(journalEntry);
                Assert.AreEqual(journalEntry.Created, timeStamp.AddMinutes(numRead));
                numRead++;
                Assert.AreEqual(journalEntry.Id, numRead);
                Console.WriteLine(journalEntry.Id);
            }
            Assert.AreEqual(100, numRead);
        }
    }
}