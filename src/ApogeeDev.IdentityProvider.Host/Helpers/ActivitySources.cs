using System.Diagnostics;

namespace ApogeeDev.IdentityProvider.Host.Helpers;

internal static class ActivitySources
{
    public static ActivitySource RequestHandlers { get; } = new ActivitySource(nameof(RequestHandlers));
    public static ActivitySource ClaimProcessors { get; } = new ActivitySource(nameof(ClaimProcessors));
}
