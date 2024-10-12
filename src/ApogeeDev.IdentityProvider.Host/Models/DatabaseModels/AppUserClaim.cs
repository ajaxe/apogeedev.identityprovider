using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;

namespace ApogeeDev.IdentityProvider.Host.Models.DatabaseModels;

[Collection("app_user_claims")]
public class AppUserClaim
{
    public ObjectId Id { get; set; }

    [BsonElement("app_user_id")]
    public ObjectId AppUserId { get; set; }
    [BsonElement("claim_type")]
    public string ClaimType { get; set; } = default!;
    [BsonElement("claim_value")]
    public string ClaimValue { get; set; } = default!;
}

public class AppUserClaimExistsEqualityComparer : GenericEqualityComparer<AppUserClaim>
{
    private static Func<AppUserClaim, AppUserClaim, bool> compareFn
        => (x, y) => x?.AppUserId == y?.AppUserId
            && x?.ClaimType == y?.ClaimType;
    public AppUserClaimExistsEqualityComparer() : base(compareFn)
    {
    }
}