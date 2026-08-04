using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ScrumBoard.Api.Realtime;
[Authorize]
public class BoardHub : Hub
{
    public async Task SubscribeToBoard(Guid projectId)
        => await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(projectId));

    public async Task UnsubscribeFromBoard(Guid projectId)
        => await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(projectId));

    private static string GroupName(Guid projectId) => $"board-{projectId}";
}