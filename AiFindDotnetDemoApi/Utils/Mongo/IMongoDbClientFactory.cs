using MongoDB.Driver;

namespace AiFindDotnetDemoApi.Utils.Mongo;

public interface IMongoDbClientFactory
{
    IMongoClient GetClient();

    IMongoCollection<T> GetCollection<T>(string collection);
}