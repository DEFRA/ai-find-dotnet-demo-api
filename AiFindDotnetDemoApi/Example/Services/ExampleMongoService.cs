using AiFindDotnetDemoApi.Example.Models;
using AiFindDotnetDemoApi.Utils.Mongo;
using MongoDB.Driver;

namespace AiFindDotnetDemoApi.Example.Interfaces;

public class ExampleMongoService : MongoService<ExampleModel>, IExampleService
{
    public ExampleMongoService(IMongoDbClientFactory connectionFactory, ILoggerFactory loggerFactory)
        : base(connectionFactory, "example", loggerFactory)
    {
    }

    public async Task<bool> CreateAsync(ExampleModel example)
    {
        try
        {
            await Collection.InsertOneAsync(example);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return false;
        }
    }

    public async Task<ExampleModel?> GetByExampleName(string name)
    {
        var result = await Collection.Find(b => b.Name == name).FirstOrDefaultAsync();
        _logger.LogInformation("Searching for {name}, found {result}", name, result);
        return result;
    }

    public async Task<IEnumerable<ExampleModel>> GetAllAsync()
    {
        return await Collection.Find(_ => true).ToListAsync();
    }

    public async Task<IEnumerable<ExampleModel>> SearchByValueAsync(string searchTerm)
    {
        var searchOptions = new TextSearchOptions { CaseSensitive = false, DiacriticSensitive = false };
        var filter = Builders<ExampleModel>.Filter.Text(searchTerm, searchOptions);
        var result = await Collection.Find(filter).ToListAsync();
        return result;
    }

    public async Task<bool> UpdateAsync(ExampleModel example)
    {
        var filter = Builders<ExampleModel>.Filter.Eq(e => e.Name, example.Name);
        var update = Builders<ExampleModel>.Update
            .Inc(e => e.Counter, 1)
            .Set(e => e.Value, example.Value);

        var result = await Collection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string name)
    {
        var result = await Collection.DeleteOneAsync(e => e.Name == name);
        return result.DeletedCount > 0;
    }

    protected override List<CreateIndexModel<ExampleModel>> DefineIndexes(IndexKeysDefinitionBuilder<ExampleModel> builder)
    {
        var options = new CreateIndexOptions { Unique = true };
        var nameIndex = new CreateIndexModel<ExampleModel>(builder.Ascending(e => e.Name), options);
        return [nameIndex];
    }
}
