using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TMS.Application.DTOs;
using TMS.Application.Interfaces;
using TMS.Application.Services;
using TMS.Domain.Entities;
using TMS.Domain.Enums;
using TMS.Domain.Exceptions;
using TMS.Infrastructure.Persistence;
using TaskStatus = TMS.Domain.Enums.TaskStatus;

namespace TMS.Tests;

public class TaskServiceTests : IDisposable
{
    private readonly TmsDbContext _db;
    private readonly Mock<ITaskSummaryRepository> _summaryRepoMock;
    private readonly TaskService _sut;

    public TaskServiceTests()
    {
        var options = new DbContextOptionsBuilder<TmsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new TmsDbContext(options);
        _summaryRepoMock = new Mock<ITaskSummaryRepository>();
        _sut = new TaskService(_db, _summaryRepoMock.Object);

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        _db.Tasks.AddRange(
            new TaskItem { Id = 1, Title = "Alpha",   Status = TaskStatus.ToDo,       Priority = Priority.Critical, AssignedTo = "alice", IsDeleted = false },
            new TaskItem { Id = 2, Title = "Beta",    Status = TaskStatus.InProgress, Priority = Priority.High,     AssignedTo = "bob",   IsDeleted = false },
            new TaskItem { Id = 3, Title = "Gamma",   Status = TaskStatus.Done,       Priority = Priority.Medium,   AssignedTo = "carol", IsDeleted = false },
            new TaskItem { Id = 4, Title = "Delta",   Status = TaskStatus.ToDo,       Priority = Priority.Low,      AssignedTo = "dave",  IsDeleted = false },
            new TaskItem { Id = 5, Title = "Epsilon", Status = TaskStatus.InProgress, Priority = Priority.High,     AssignedTo = "eve",   IsDeleted = false },
            new TaskItem { Id = 6, Title = "Deleted1",Status = TaskStatus.ToDo,       Priority = Priority.Low,      AssignedTo = null,    IsDeleted = true  },
            new TaskItem { Id = 7, Title = "Deleted2",Status = TaskStatus.Done,       Priority = Priority.Critical, AssignedTo = null,    IsDeleted = true  }
        );
        _db.SaveChanges();
    }

    public void Dispose() => _db.Dispose();

    // ── GetAllAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyNonDeleted()
    {
        var result = await _sut.GetAllAsync(null, null);

        result.Should().HaveCount(5);
        result.Should().NotContain(t => t.Title == "Deleted1" || t.Title == "Deleted2");
    }

    [Fact]
    public async Task GetAllAsync_FiltersByStatus()
    {
        var result = await _sut.GetAllAsync(TaskStatus.ToDo, null);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(t => t.Status == TaskStatus.ToDo);
    }

    [Fact]
    public async Task GetAllAsync_FiltersByPriority()
    {
        var result = await _sut.GetAllAsync(null, Priority.High);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(t => t.Priority == Priority.High);
    }

    [Fact]
    public async Task GetAllAsync_SortsByPriorityDescending()
    {
        var result = (await _sut.GetAllAsync(null, null)).ToList();

        result.Should().BeInDescendingOrder(t => t.Priority);
    }

    // ── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_ReturnsTask_WhenFound()
    {
        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Title.Should().Be("Alpha");
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenNotFound()
    {
        var act = async () => await _sut.GetByIdAsync(999);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*999*");
    }

    // ── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_PersistsAndReturnsNewTask()
    {
        var dto = new CreateTaskDto
        {
            Title      = "New Task",
            Status     = TaskStatus.ToDo,
            Priority   = Priority.Medium,
            AssignedTo = "frank"
        };

        var result = await _sut.CreateAsync(dto);

        result.Id.Should().BeGreaterThan(0);
        result.Title.Should().Be("New Task");
        result.Status.Should().Be(TaskStatus.ToDo);
        result.Priority.Should().Be(Priority.Medium);
        result.AssignedTo.Should().Be("frank");

        _db.Tasks.Should().ContainSingle(t => t.Title == "New Task");
    }

    [Fact]
    public async Task CreateAsync_VerifiesSaveChangesCalledOnce()
    {
        var dbOptions = new DbContextOptionsBuilder<TmsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var mockDb = new Mock<ITmsDbContext>();
        var tasks = new List<TaskItem>();
        var mockSet = CreateMockDbSet(tasks);
        mockDb.Setup(d => d.Tasks).Returns(mockSet.Object);
        mockDb.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var svc = new TaskService(mockDb.Object, _summaryRepoMock.Object);
        var dto = new CreateTaskDto { Title = "T", Status = TaskStatus.ToDo, Priority = Priority.Low };

        await svc.CreateAsync(dto);

        mockDb.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_AppliesPartialUpdate_OnlyNonNullFields()
    {
        var original = await _db.Tasks.FindAsync(2);
        var originalTitle      = original!.Title;
        var originalPriority   = original.Priority;
        var originalAssignedTo = original.AssignedTo;

        var dto = new UpdateTaskDto { Status = TaskStatus.Done };

        var result = await _sut.UpdateAsync(2, dto);

        result.Status.Should().Be(TaskStatus.Done);
        result.Title.Should().Be(originalTitle);
        result.Priority.Should().Be(originalPriority);
        result.AssignedTo.Should().Be(originalAssignedTo);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsNotFoundException_WhenNotFound()
    {
        var act = async () => await _sut.UpdateAsync(999, new UpdateTaskDto { Title = "X" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    // ── SoftDeleteAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task SoftDeleteAsync_SetsIsDeletedTrue()
    {
        await _sut.SoftDeleteAsync(3);

        var task = await _db.Tasks.FindAsync(3);
        task!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task SoftDeleteAsync_ThrowsNotFoundException_WhenNotFound()
    {
        var act = async () => await _sut.SoftDeleteAsync(999);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    // ── Helper ───────────────────────────────────────────────────────────────

    private static Mock<DbSet<TaskItem>> CreateMockDbSet(List<TaskItem> data)
    {
        var queryable = data.AsQueryable();
        var mockSet = new Mock<DbSet<TaskItem>>();
        mockSet.As<IQueryable<TaskItem>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockSet.As<IQueryable<TaskItem>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<TaskItem>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<TaskItem>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
        mockSet.Setup(m => m.Add(It.IsAny<TaskItem>())).Callback<TaskItem>(data.Add);
        return mockSet;
    }
}
