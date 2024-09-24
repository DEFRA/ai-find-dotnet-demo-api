using AiFindDotnetDemoApi.Example.Models;

namespace AiFindDotnetDemoApi.Example.Interfaces;

public interface IExampleService
{
    public Task<bool> CreateAsync(ExampleModel example);

    public Task<ExampleModel?> GetByExampleName(string name);

    public Task<IEnumerable<ExampleModel>> GetAllAsync();

    public Task<IEnumerable<ExampleModel>> SearchByValueAsync(string searchTerm);

    public Task<bool> UpdateAsync(ExampleModel example);

    public Task<bool> DeleteAsync(string name);
}
