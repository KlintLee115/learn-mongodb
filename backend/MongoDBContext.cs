using backend.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace backend
{
    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;

        public MongoDBContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);

            RegisterClassMaps();
        }

        public IMongoCollection<Form> Forms => _database.GetCollection<Form>("forms");
        public IMongoCollection<FormResponse> Responses => _database.GetCollection<FormResponse>("responses");

        private static void RegisterClassMaps()
        {
            BsonClassMap.RegisterClassMap<QuestionBase>(cm =>
            {
                cm.AutoMap();
                cm.AddKnownType(typeof(QuestionChoice));
            });
        }
    }


    public class MongoDBSettings
    {
        public required string ConnectionString { get; set; }
        public required string DatabaseName { get; set; }
    }
}