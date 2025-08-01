using Application.Interfaces;
using Domain.Entities;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Literals.Enums;
using StackExchange.Redis;

namespace Infrastructure.Redis;

public class RedisSessionsService : ISessions
{
    private readonly IConnectionMultiplexer _muxer;
    private const string IndexName = "idx:sessions";
    private const string KeyPrefix = "session:";

    // The DI container injects the SINGLETON instance here.
    public RedisSessionsService(IConnectionMultiplexer muxer)
    {
        _muxer = muxer;
    }

    // This method should be called once at application startup.
    public static void CreateIndex(IConnectionMultiplexer muxer)
    {
        var db = muxer.GetDatabase();
        try
        {
            var info = db.FT().Info(IndexName);
            Console.WriteLine($"Index '{IndexName}' already exists. Skipping creation.");
        }
        catch (RedisServerException ex) when (ex.Message.Contains("no such index"))
        {
            // This is the expected exception when the index is missing.
            // Define the schema for a HASH, not JSON.
            var schema = new Schema()
                .AddNumericField("UserId"); // Simple field name for HASH

            // Create the index for HASHes with the specified prefix.
            db.FT().Create(
                IndexName,
                new FTCreateParams().Prefix(KeyPrefix),
                schema
            );
            Console.WriteLine($"Index '{IndexName}' created successfully.");
        }
    }

    public async Task<string> SetSession(Sessions session, string? id = null)
    {
        IDatabase db = _muxer.GetDatabase();

        if (string.IsNullOrEmpty(id))
            id = Guid.NewGuid().ToString();

        await db.HashSetAsync($"{KeyPrefix}{id}", session.ToHashEntries())
            .ConfigureAwait(false);

        return id;
    }

    public async Task<Sessions?> GetSession(string id)
    {
        IDatabase db = _muxer.GetDatabase();
        var sessionHash = await db.HashGetAllAsync($"{KeyPrefix}{id}");
        if (sessionHash.Length == 0)
            return null;
        var session = sessionHash.ConvertFromHash<Sessions>();
        return session;
    }

    public async Task<bool> RemoveSession(string id)
    {
        IDatabase db = _muxer.GetDatabase();
        return await db.KeyDeleteAsync($"{KeyPrefix}{id}");
    }

    public async Task<bool> RemoveSessionByUserId(string userId)
    {
        IDatabase db = _muxer.GetDatabase();

        // Corrected query syntax for a numeric field in a HASH index.
        // It's a good practice to escape the user id if it can contain special characters,
        // but since it's numeric, we are safe here.
        var query = new Query($"@UserId:[{userId} {userId}]")
        {
            NoContent = true // We only need the keys, not the content.
        };

        SearchResult sessionsResult = await db.FT().SearchAsync(IndexName, query);

        if (sessionsResult.TotalResults == 0)
            return false;

        // Use a transaction for bulk deletion for better performance.
        var transaction = db.CreateTransaction();
        var deletionTasks = new List<Task>();
        foreach (var doc in sessionsResult.Documents)
        {
            // The document ID includes the prefix, so no need to add it again.
            deletionTasks.Add(transaction.KeyDeleteAsync(doc.Id));
        }

        bool committed = await transaction.ExecuteAsync();
        return committed;
    }
}