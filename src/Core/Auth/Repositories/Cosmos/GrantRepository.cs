﻿using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using Bit.Core.Auth.Models.Data;
using Bit.Core.Settings;
using Microsoft.Azure.Cosmos;

namespace Bit.Core.Auth.Repositories.Cosmos;

public class GrantRepository : IGrantRepository
{
    private readonly CosmosClient _client;
    private readonly Database _database;
    private readonly Container _container;

    public GrantRepository(GlobalSettings globalSettings)
        : this(globalSettings.IdentityServer.CosmosConnectionString)
    { }

    public GrantRepository(string cosmosConnectionString)
    {
        var options = new CosmosClientOptions
        {
            Serializer = new SystemTextJsonCosmosSerializer(new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = false,
            })
        };
        _client = new CosmosClient(cosmosConnectionString, options);
        _database = _client.GetDatabase("identity");
        _container = _database.GetContainer("grant");
    }

    public async Task<IGrant> GetByKeyAsync(string key)
    {
        var id = Base64IdStringConverter.ToId(key);
        try
        {
            var response = await _container.ReadItemAsync<GrantItem>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException e)
        {
            if (e.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            throw;
        }
    }

    public Task<ICollection<IGrant>> GetManyAsync(string subjectId, string sessionId, string clientId, string type)
        => throw new NotImplementedException();

    public async Task SaveAsync(IGrant obj)
    {
        if (obj is not GrantItem item)
        {
            item = new GrantItem(obj);
        }
        item.SetTtl();
        var id = Base64IdStringConverter.ToId(item.Key);
        await _container.UpsertItemAsync(item, new PartitionKey(id));
    }

    public async Task DeleteByKeyAsync(string key)
    {
        var id = Base64IdStringConverter.ToId(key);
        await _container.DeleteItemAsync<IGrant>(id, new PartitionKey(id));
    }

    public Task DeleteManyAsync(string subjectId, string sessionId, string clientId, string type)
        => throw new NotImplementedException();
}
