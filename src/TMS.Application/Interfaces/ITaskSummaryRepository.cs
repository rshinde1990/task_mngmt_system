using TMS.Application.DTOs;

namespace TMS.Application.Interfaces;

public interface ITaskSummaryRepository
{
    Task<IEnumerable<TaskSummaryDto>> GetSummaryAsync(CancellationToken ct = default);
}
