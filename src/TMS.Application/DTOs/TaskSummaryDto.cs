using TMS.Domain.Enums;
using TaskStatus = TMS.Domain.Enums.TaskStatus;

namespace TMS.Application.DTOs;

public class TaskSummaryDto
{
    public TaskStatus Status { get; set; }
    public Priority Priority { get; set; }
    public int Count { get; set; }
}
