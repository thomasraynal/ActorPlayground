using ActorPlayground.Orleans.Basics.EventStore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics
{
    [TestFixture]
    public class TestsEventStore
    {
        private EmbeddedEventStoreFixture _embeddedEventStore;

        [SetUp]
        public async Task SetupFixture()
        {
            _embeddedEventStore = new EmbeddedEventStoreFixture();
            await _embeddedEventStore.Initialize();
        }

        [TearDown]
        public async Task TearDown()
        {
            await _embeddedEventStore.Dispose();
        }

        [Test]
        public async Task ShouldCreateAndResumeConnection()
        {

            var repository = EventStoreRepository.Create(new EventStoreRepositoryConfiguration());

            await repository.Connect(TimeSpan.FromSeconds(10));

            //wait for EventStore to setup user accounts
            await Task.Delay(500);

            var counter = 0;

            var subscription = repository.Observe("EUR/USD")
                                       .Subscribe(ev =>
                                        {
                                            counter++;
                                        });

            await repository.SavePendingEvents("EUR/USD", -1, new[] { new CcyPairChanged("Harmony", "EUR/USD", true, 1.32, 1.34) });

            await Task.Delay(200);

            await _embeddedEventStore.Dispose();

            _embeddedEventStore = new EmbeddedEventStoreFixture();
            await _embeddedEventStore.Initialize();

            await Wait.Until(() => repository.IsConnected, TimeSpan.FromSeconds(5));

            //wait for EventStore to setup user accounts
            await Task.Delay(500);

            await repository.SavePendingEvents("EUR/USD", -1, new[] { new CcyPairChanged("Harmony", "EUR/USD", true, 1.32, 1.34) });

            await Task.Delay(200);

            Assert.AreEqual(2, counter);

            subscription.Dispose();
            repository.Dispose();

        }

        [Test]
        public async Task ShouldSubscribeFromVersion()
        {
            var repository = EventStoreRepository.Create(new EventStoreRepositoryConfiguration());

            await repository.Connect(TimeSpan.FromSeconds(10));

            //wait for EventStore to setup user accounts
            await Task.Delay(500);

            await repository.SavePendingEvents("EUR/USD", -1, new[] { new CcyPairChanged("Harmony", "EUR/USD", true, 1.32, 1.34) });
            await Task.Delay(200);

            await repository.SavePendingEvents("EUR/USD", 0, new[] { new CcyPairChanged("Harmony", "EUR/USD", true, 1.32, 1.34) });
            await Task.Delay(200);

            repository.Dispose();

            repository = EventStoreRepository.Create(new EventStoreRepositoryConfiguration());
            await repository.Connect(TimeSpan.FromSeconds(10));

            //wait for EventStore to setup user accounts
            await Task.Delay(500);

            var counter = 0;
            var subscription = repository.Observe("EUR/USD", 1)
                           .Subscribe(ev =>
                           {
                               counter++;
                           });

            await Task.Delay(200);

            Assert.AreEqual(1, counter);

            subscription.Dispose();
            repository.Dispose();
        }

        [Test]
        public async Task ShouldCatchUpStream()
        {
            var repository = EventStoreRepository.Create(new EventStoreRepositoryConfiguration());

            await repository.Connect(TimeSpan.FromSeconds(10));

            //wait for EventStore to setup user accounts
            await Task.Delay(500);

            await repository.SavePendingEvents("EUR/USD", -1, new[] { new CcyPairChanged("Harmony", "EUR/USD", true, 1.32, 1.34) });
            await Task.Delay(200);

            await repository.SavePendingEvents("EUR/USD", 0, new[] { new CcyPairChanged("Harmony", "EUR/USD", true, 1.32, 1.34) });
            await Task.Delay(200);

            repository.Dispose();

            repository = EventStoreRepository.Create(new EventStoreRepositoryConfiguration());
            await repository.Connect(TimeSpan.FromSeconds(10));

            //wait for EventStore to setup user accounts
            await Task.Delay(500);

            var counter = 0;
            var subscription = repository.Observe("EUR/USD",rewindAfterDisconnection : true)
                           .Subscribe(ev =>
                           {
                               counter++;
                           });

            await Task.Delay(200);

            Assert.AreEqual(2, counter);

            subscription.Dispose();
            repository.Dispose();
        }

    }
}
