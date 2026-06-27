using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMS.Domain.Entities;
using TMS.Domain.Enums;
using TaskStatus = TMS.Domain.Enums.TaskStatus;

namespace TMS.Infrastructure.Persistence.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.Property(t => t.AssignedTo)
            .HasMaxLength(100);

        builder.Property(t => t.Status)
            .HasConversion<string>();

        builder.Property(t => t.Priority)
            .HasConversion<string>();

        builder.Property(t => t.IsDeleted)
            .HasDefaultValue(false);
    }
}
