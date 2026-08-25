using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Core.Data.Entities;

namespace Todo.Core.Data.Configurations;

public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItemEntity>
{
    public void Configure(EntityTypeBuilder<TodoItemEntity> builder)
    {
        builder.Property(item => item.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(item => item.Note)
            .HasMaxLength(1000);

        builder.Property(item => item.Priority)
            .HasConversion<int>()
            .IsRequired();
    }
}
