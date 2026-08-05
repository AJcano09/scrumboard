using Moq;
using ScrumBoard.Application.Ports;
using ScrumBoard.Application.Tasks;
using ScrumBoard.Domain.Exceptions;

namespace ScrumBoard.Application.Tests;

public class TaskServiceValidationTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock = new();
    private readonly Mock<IColumnRepository> _columnRepositoryMock = new();
    private readonly Mock<IRealtimeNotifier> _notifierMock = new();
    private readonly TaskService _sut;

    public TaskServiceValidationTests()
    {
        _sut = new TaskService(
            _taskRepositoryMock.Object,
            _columnRepositoryMock.Object,
            _notifierMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ConPrioridadInvalida_LanzaTaskValidationException()
    {
        var request = new CreateTaskRequest
        {
            Title = "Tarea válida",
            Priority = "AltaX",
            ColumnId = Guid.NewGuid()
        };

        var ex = await Assert.ThrowsAsync<TaskValidationException>(
            () => _sut.CreateAsync(request));

        Assert.Contains("prioridad", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_ConTituloVacio_LanzaTaskValidationException()
    {
        var request = new CreateTaskRequest
        {
            Title = string.Empty,
            Priority = "Alta",
            ColumnId = Guid.NewGuid()
        };

        var ex = await Assert.ThrowsAsync<TaskValidationException>(
            () => _sut.CreateAsync(request));

        Assert.Contains("título", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_ConPrioridadInvalida_NoLlamaRepositorio()
    {
        var request = new CreateTaskRequest
        {
            Title = "Tarea válida",
            Priority = "AltaX",
            ColumnId = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<TaskValidationException>(
            () => _sut.CreateAsync(request));

        _taskRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<ScrumBoard.Domain.Entities.Task>()),
            Times.Never());
    }
}
