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

        [OneTimeSetUp]
        public async Task SetupFixture()
        {
            _embeddedEventStore = new EmbeddedEventStoreFixture();
            await _embeddedEventStore.Initialize();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await _embeddedEventStore.Dispose();
        }

        [Test]
        public async Task ShouldCreateAndResumeConnection()
        {

            EventStoreRepository repository = null ;

            repository = EventStoreRepository.Create(new EventStoreRepositoryConfiguration());
            await repository.Connect(TimeSpan.FromSeconds(10));

            var counter = 0;

            var disposable = repository.Observe("EUR/USD")
                                       .Subscribe(ev =>
                                        {
                                            counter++;
                                        });

            await repository.Save("EUR/USD", -1, new[] { new CcyPairChanged("Harmony", "EUR/USD", true, 1.32, 1.34) });

            await Task.Delay(200);

            await _embeddedEventStore.Dispose();

            _embeddedEventStore = new EmbeddedEventStoreFixture();
            await _embeddedEventStore.Initialize();

            await Wait.Until(() => repository.IsConnected, TimeSpan.FromSeconds(5));

            await repository.Save("EUR/USD", -1, new[] { new CcyPairChanged("Harmony", "EUR/USD", true, 1.32, 1.34) });

            await Task.Delay(200);

            Assert.AreEqual(2, counter);

        }

    }
}
