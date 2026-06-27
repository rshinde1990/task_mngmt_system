using System.Data;
using Microsoft.EntityFrameworkCore;
using TMS.Application.DTOs;
using TMS.Application.Interfaces;
using TMS.Domain.Enums;
using TaskStatus = TMS.Domain.Enums.TaskStatus;

namespace TMS.Infrastructure.Persistence;

public class TaskSummaryRepository : ITaskSummaryRepository
{
    private readonly TmsDbContext _context;

    public TaskSummaryRepository(TmsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskSummaryDto>> GetSummaryAsync(CancellationToken ct = default)
    {
        const string sql = """
            SELECT Status, Priority, COUNT(*) AS Count
            FROM Tasks
            WHERE IsDeleted = 0
            GROUP BY Status, Priority
            ORDER BY Status, Priority
            """;

        var results = new List<TaskSummaryDto>();

        var connection = _context.Database.GetDbConnection();
        var wasOpen = connection.State == ConnectionState.Open;

        try
        {
            if (!wasOpen)
                await connection.OpenAsync(ct);

            using var command = connection.CreateCommand();
            command.CommandText = sql;

            using var reader = await command.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                results.Add(new TaskSummaryDto
                {
                    Status   = Enum.Parse<TaskStatus>(reader.GetString(0)),
                    Priority = Enum.Parse<Priority>(reader.GetString(1)),
                    Count    = reader.GetInt32(2)
                });
            }
        }
        finally
        {
            if (!wasOpen)
                await connection.CloseAsync();
        }

        return results;
    }
}
