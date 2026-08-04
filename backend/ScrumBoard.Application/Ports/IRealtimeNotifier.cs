namespace ScrumBoard.Application.Ports;

public interface IRealtimeNotifier
{
    Task NotifyBoardChangedAsync(Guid projectId, string eventType, object payload);
}