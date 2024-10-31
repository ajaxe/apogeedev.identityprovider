using System.Security.Claims;
using ApogeeDev.IdentityProvider.Host.Models.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;

namespace ApogeeDev.IdentityProvider.Host.Models.DatabaseModels;

[Collection("app_users")]
public class AppUser
{
    public ObjectId Id { get; set; }
    [BsonElement("username")]
    public string? UserName { get; set; }
    [BsonElement("subject")]
    public string? Subject { get; set; }
    [BsonElement("profile_picture")]
    public string? ProfilePicture { get; set; }
    [BsonElement("name")]
    public string? Name { get; set; }
    [BsonElement("email")]
    public string? Email { get; set; }
    [BsonElement("identity_provider")]
    public string? IdentityProvider { get; set; }

    public static AppUser Create(string subject, string idProvider)
    {
        return new AppUser
        {
            Subject = subject,
            IdentityProvider = idProvider,
        };
    }
}