using MongoDB.Driver;
using System.Collections.Generic;
using LifeApi.Models;

namespace LifeApi{
    public class DatabaseConnection{

        public IMongoDatabase database;

        public Dictionary<string, string> collectionNames;

        public DatabaseConnection(ILifeDatabaseSettings settings){
            MongoClient client = new MongoClient(settings.ConnectionString);
            database = client.GetDatabase(settings.DatabaseName);

            collectionNames = settings.CollectionNames;
        }
    }
}