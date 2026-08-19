using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Application.Data.Entities;

namespace Todo.Application.Data.Configurations;

public class TodoListConfiguration : IEntityTypeConfiguration<TodoListEntity>
{
    public void Configure(EntityTypeBuilder<TodoListEntity> builder)
    {
        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();
    }
}
