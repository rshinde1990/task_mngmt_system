using Microsoft.EntityFrameworkCore;
using TMS.Application.DTOs;
using TMS.Application.Interfaces;
using TMS.Domain.Entities;
using TMS.Domain.Enums;
using TMS.Domain.Exceptions;
using TaskStatus = TMS.Domain.Enums.TaskStatus;

namespace TMS.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITmsDbContext _db;
    private readonly ITaskSummaryRepository _summaryRepo;

    public TaskService(ITmsDbContext db, ITaskSummaryRepository summaryRepo)
    {
        _db = db;
        _summaryRepo = summaryRepo;
    }

    public async Task<IEnumerable<TaskItemDto>> GetAllAsync(TaskStatus? status, Priority? priority, CancellationToken ct = default)
    {
        var query = _db.Tasks
            .Where(t => !t.IsDeleted);

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (priority.HasValue)
            query = query.Where(t => t.Priority == priority.Value);

        var items = await query
            .OrderByDescending(t => t.Priority)
            .ToListAsync(ct);

        return items.Select(MapToDto);
    }

    public async Task<TaskItemDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var task = await _db.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, ct);

        if (task is null)
            throw new NotFoundException(nameof(TaskItem), id);

        return MapToDto(task);
    }

    public async Task<TaskItemDto> CreateAsync(CreateTaskDto dto, CancellationToken ct = default)
    {
        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status,
            Priority = dto.Priority,
            AssignedTo = dto.AssignedTo
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync(ct);

        return MapToDto(task);
    }

    public async Task<TaskItemDto> UpdateAsync(int id, UpdateTaskDto dto, CancellationToken ct = default)
    {
        var task = await _db.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, ct);

        if (task is null)
            throw new NotFoundException(nameof(TaskItem), id);

        if (dto.Title != null)       task.Title       = dto.Title;
        if (dto.Description != null) task.Description = dto.Description;
        if (dto.Status.HasValue)     task.Status      = dto.Status.Value;
        if (dto.Priority.HasValue)   task.Priority    = dto.Priority.Value;
        if (dto.AssignedTo != null)  task.AssignedTo  = dto.AssignedTo;

        await _db.SaveChangesAsync(ct);

        return MapToDto(task);
    }

    public async Task SoftDeleteAsync(int id, CancellationToken ct = default)
    {
        var task = await _db.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, ct);

        if (task is null)
            throw new NotFoundException(nameof(TaskItem), id);

        task.IsDeleted = true;
        await _db.SaveChangesAsync(ct);
    }

    public Task<IEnumerable<TaskSummaryDto>> GetSummaryAsync(CancellationToken ct = default)
        => _summaryRepo.GetSummaryAsync(ct);

    private static TaskItemDto MapToDto(TaskItem t) => new()
    {
        Id           = t.Id,
        Title        = t.Title,
        Description  = t.Description,
        Status       = t.Status,
        Priority     = t.Priority,
        AssignedTo   = t.AssignedTo,
        CreatedDate  = t.CreatedDate,
        ModifiedDate = t.ModifiedDate
    };
}
