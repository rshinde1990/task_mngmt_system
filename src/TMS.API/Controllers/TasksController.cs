using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMS.Application.DTOs;
using TMS.Application.Interfaces;
using TMS.Domain.Enums;
using TMS.Shared;
using TaskStatus = TMS.Domain.Enums.TaskStatus;

namespace TMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<TaskItemDto>>>> GetAll(
        [FromQuery] TaskStatus? status,
        [FromQuery] Priority? priority,
        CancellationToken ct)
    {
        var result = await _taskService.GetAllAsync(status, priority, ct);
        return Ok(ApiResponse<IEnumerable<TaskItemDto>>.Ok(result));
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TaskSummaryDto>>>> GetSummary(CancellationToken ct)
    {
        var result = await _taskService.GetSummaryAsync(ct);
        return Ok(ApiResponse<IEnumerable<TaskSummaryDto>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<TaskItemDto>>> GetById(int id, CancellationToken ct)
    {
        var result = await _taskService.GetByIdAsync(id, ct);
        return Ok(ApiResponse<TaskItemDto>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TaskItemDto>>> Create([FromBody] CreateTaskDto dto, CancellationToken ct)
    {
        var result = await _taskService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<TaskItemDto>.Ok(result));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<TaskItemDto>>> Update(int id, [FromBody] UpdateTaskDto dto, CancellationToken ct)
    {
        var result = await _taskService.UpdateAsync(id, dto, ct);
        return Ok(ApiResponse<TaskItemDto>.Ok(result));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> SoftDelete(int id, CancellationToken ct)
    {
        await _taskService.SoftDeleteAsync(id, ct);
        return Ok(ApiResponse<object?>.Ok(null, "Task deleted."));
    }
}
