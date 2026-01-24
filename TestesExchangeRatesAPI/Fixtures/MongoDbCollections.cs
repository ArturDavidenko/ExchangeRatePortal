using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestesExchangeRatesAPI.Common;

namespace TestesExchangeRatesAPI.Fixtures
{
    [CollectionDefinition("MongoDb collection")]
    public class MongoDbCollections : ICollectionFixture<MongoDbFixture>
    {

    }
}
