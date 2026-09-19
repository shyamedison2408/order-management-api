using StackExchange.Redis;

namespace OrderManagementApi.Services;

public class RedisService
{
    private readonly IDatabase _database;

    public RedisService(IConnectionMultiplexer connection)
    {
        _database = connection.GetDatabase();
    }

    public async Task SetAsync(string key, string value, TimeSpan expiry)
    {
        await _database.StringSetAsync(key, value, expiry);
    }

    public async Task<string?> GetAsync(string key)
    {
        var value = await _database.StringGetAsync(key);

        return value.HasValue ? value.ToString() : null;
    }

    public async Task DeleteAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }
}