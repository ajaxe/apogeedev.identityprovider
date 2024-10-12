using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;

namespace ApogeeDev.IdentityProvider.Host.Models.DatabaseModels;

[Collection(CollectionName)]

public class AppDbSettings
{
    public const string CollectionName = "app_db_settings";

    public ObjectId Id { get; set; }

    [BsonElement("setting_type")]
    public DbSettingType SettingType { get; set; } = DbSettingType.Global;

    [BsonElement("performance_index_created")]
    public bool? PerformanceIndexCreated { get; set; }
}

public enum DbSettingType
{
    Global = 0,
}