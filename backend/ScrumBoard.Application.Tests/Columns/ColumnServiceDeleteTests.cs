using Moq;
using ScrumBoard.Application.Common;
using ScrumBoard.Application.Ports;
using ScrumBoard.Application.Columns;
using ScrumBoard.Domain.Exceptions;
using Column = ScrumBoard.Domain.Entities.Column;

namespace ScrumBoard.Application.Tests.Columns;

public class ColumnServiceDeleteTests
{
    private readonly Mock<IColumnRepository> _columnRepositoryMock = new();
    private readonly Mock<IProjectRepository> _projectRepositoryMock = new();
    private readonly Mock<IRealtimeNotifier> _notifierMock = new();
    private readonly ColumnService _sut;

    public ColumnServiceDeleteTests()
    {
        _sut = new ColumnService(
            _columnRepositoryMock.Object,
            _projectRepositoryMock.Object,
            _notifierMock.Object);
    }

    [Fact]
    public async Task DeleteAsync_ColumnConTareas_LanzaColumnValidationException()
    {
        var columnId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var column = new Column { Id = columnId, ProjectId = projectId, Name = "To Do" };

        _columnRepositoryMock.Setup(x => x.GetByIdAsync(columnId))
            .ReturnsAsync(column);
        _columnRepositoryMock.Setup(x => x.CountTasksByColumnIdAsync(columnId))
            .ReturnsAsync(3);

        var ex = await Assert.ThrowsAsync<ColumnValidationException>(
            () => _sut.DeleteAsync(projectId, columnId));

        Assert.Contains("3 tarea(s)", ex.Message);
    }

    [Fact]
    public async Task DeleteAsync_ColumnVacia_BorraYNotifica()
    {
        var columnId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var column = new Column { Id = columnId, ProjectId = projectId, Name = "To Do" };

        _columnRepositoryMock.Setup(x => x.GetByIdAsync(columnId))
            .ReturnsAsync(column);
        _columnRepositoryMock.Setup(x => x.CountTasksByColumnIdAsync(columnId))
            .ReturnsAsync(0);

        var result = await _sut.DeleteAsync(projectId, columnId);

        Assert.True(result);

        _columnRepositoryMock.Verify(x => x.DeleteAsync(columnId), Times.Once());
        _notifierMock.Verify(
            x => x.NotifyBoardChangedAsync(projectId, BoardHubEvents.ColumnDeleted, It.IsAny<object>()),
            Times.Once());
    }

    [Fact]
    public async Task DeleteAsync_ColumnDeOtroProyecto_RetornaFalse_NoBorra()
    {
        var columnId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var otherProjectId = Guid.NewGuid();
        var column = new Column { Id = columnId, ProjectId = otherProjectId, Name = "To Do" };

        _columnRepositoryMock.Setup(x => x.GetByIdAsync(columnId))
            .ReturnsAsync(column);

        var result = await _sut.DeleteAsync(projectId, columnId);

        Assert.False(result);

        _columnRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Never());
        _notifierMock.Verify(
            x => x.NotifyBoardChangedAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<object>()),
            Times.Never());
    }
}
