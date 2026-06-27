using TMS.Application.DTOs;
using TMS.Domain.Enums;
using TaskStatus = TMS.Domain.Enums.TaskStatus;

namespace TMS.Application.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskItemDto>> GetAllAsync(TaskStatus? status, Priority? priority, CancellationToken ct = default);
    Task<TaskItemDto> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TaskItemDto> CreateAsync(CreateTaskDto dto, CancellationToken ct = default);
    Task<TaskItemDto> UpdateAsync(int id, UpdateTaskDto dto, CancellationToken ct = default);
    Task SoftDeleteAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<TaskSummaryDto>> GetSummaryAsync(CancellationToken ct = default);
}
