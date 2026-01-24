using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MongoDb;

namespace TestesExchangeRatesAPI.Common
{
    public sealed class MongoDbFixture : IAsyncLifetime
    {
        private readonly MongoDbContainer _container;

        public IMongoDatabase Database { get; private set; } = default!;
        public string ConnectionString { get; private set; } = default!;

        public MongoDbFixture()
        {
            _container = new MongoDbBuilder()
                .WithImage("mongo:6.0")
                .WithCleanUp(true)
                .Build();
        }

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }

        public async Task InitializeAsync()
        {
            await _container.StartAsync();

            ConnectionString = _container.GetConnectionString();

            var client = new MongoClient(ConnectionString);

            Database = client.GetDatabase($"test-db-{Guid.NewGuid()}");
        }
    }
}
