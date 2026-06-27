using TMS.Domain.Entities;
using TMS.Domain.Enums;
using TaskStatus = TMS.Domain.Enums.TaskStatus;

namespace TMS.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(TmsDbContext context)
    {
        if (Environment.GetEnvironmentVariable("SEED_DATA") != "true")
            return;

        if (context.Tasks.Any())
            return;

        var tasks = new List<TaskItem>
        {
            new() { Title = "Set up CI/CD pipeline",         Description = "Configure GitHub Actions for build and deploy.",           Status = TaskStatus.ToDo,       Priority = Priority.Critical, AssignedTo = "alice" },
            new() { Title = "Design database schema",        Description = "ERD and table definitions for the core domain.",           Status = TaskStatus.Done,       Priority = Priority.High,     AssignedTo = "bob"   },
            new() { Title = "Implement authentication",      Description = "JWT-based login and token refresh flow.",                  Status = TaskStatus.InProgress, Priority = Priority.Critical, AssignedTo = "alice" },
            new() { Title = "Write unit tests for services", Description = "Cover all service methods with xUnit tests.",              Status = TaskStatus.ToDo,       Priority = Priority.High,     AssignedTo = "carol" },
            new() { Title = "Build task list UI",            Description = "React table component with filters and inline editing.",   Status = TaskStatus.InProgress, Priority = Priority.High,     AssignedTo = "dave"  },
            new() { Title = "Add FluentValidation rules",    Description = "Validators for CreateTaskDto and UpdateTaskDto.",          Status = TaskStatus.Done,       Priority = Priority.Medium,   AssignedTo = "bob"   },
            new() { Title = "Configure Serilog logging",     Description = "Structured logging to console and rolling file sink.",    Status = TaskStatus.ToDo,       Priority = Priority.Medium,   AssignedTo = "carol" },
            new() { Title = "Create summary dashboard",      Description = "Panel showing task counts grouped by status and priority.",Status = TaskStatus.InProgress, Priority = Priority.Medium,   AssignedTo = "dave"  },
            new() { Title = "Write API integration tests",   Description = "End-to-end tests against the running API.",               Status = TaskStatus.ToDo,       Priority = Priority.Low,      AssignedTo = "alice" },
            new() { Title = "Update README documentation",   Description = "Setup instructions, env vars, and architecture notes.",   Status = TaskStatus.Done,       Priority = Priority.Low,      AssignedTo = "bob"   },
            new() { Title = "Performance-test endpoints",    Description = "k6 load tests for task list and summary endpoints.",      Status = TaskStatus.ToDo,       Priority = Priority.Low,      AssignedTo = "carol" },
            new() { Title = "Implement soft-delete cleanup", Description = "Background job to purge records deleted >90 days ago.",   Status = TaskStatus.Done,       Priority = Priority.Critical, AssignedTo = "dave"  },
        };

        await context.Tasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
    }
}
