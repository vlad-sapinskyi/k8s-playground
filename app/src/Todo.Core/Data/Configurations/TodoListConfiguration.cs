using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Core.Data.Entities;

namespace Todo.Core.Data.Configurations;

public class TodoListConfiguration : IEntityTypeConfiguration<TodoListEntity>
{
    public void Configure(EntityTypeBuilder<TodoListEntity> builder)
    {
        builder.Property(list => list.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(list => list.Colour)
            .HasConversion<int>()
            .IsRequired();

        builder.HasMany(list => list.Items)
            .WithOne(item => item.List)
            .HasForeignKey(item => item.ListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
