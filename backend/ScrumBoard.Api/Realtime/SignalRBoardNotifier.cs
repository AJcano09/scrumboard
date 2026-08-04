using Microsoft.AspNetCore.SignalR;
using ScrumBoard.Application.Ports;

namespace ScrumBoard.Api.Realtime;

public class SignalRBoardNotifier(IHubContext<BoardHub> hubContext) : IRealtimeNotifier
{
    public async Task NotifyBoardChangedAsync(Guid projectId, string eventType, object payload)
        => await hubContext.Clients.Group($"board-{projectId}").SendAsync(eventType, payload);
}