namespace ScrumBoard.Application.Common;

public static class BoardHubEvents
{
    public const string TaskCreated = "taskCreated";
    public const string TaskUpdated = "taskUpdated";
    public const string TaskDeleted = "taskDeleted";
    public const string TaskMoved = "taskMoved";
    
    public const string ColumnCreated = "columnCreated";
    public const string ColumnUpdated = "columnUpdated";
    public const string ColumnDeleted = "columnDeleted";
    public const string ColumnMoved = "columnMoved";
}