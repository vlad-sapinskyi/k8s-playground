using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Application.Data.Entities;

namespace Todo.Application.Data.Configurations;

public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItemEntity>
{
    public void Configure(EntityTypeBuilder<TodoItemEntity> builder)
    {
        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();
    }
}
