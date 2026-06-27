using Microsoft.EntityFrameworkCore;
using TMS.Domain.Entities;

namespace TMS.Application.Interfaces;

public interface ITmsDbContext
{
    DbSet<TaskItem> Tasks { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
