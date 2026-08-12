using AutoMapper;
using Todo.Domain.Entities;

namespace Todo.Application.Dtos;

public class TodoListDto
{
    public TodoListDto() => Items = [];

    public int Id { get; init; }
    public string? Title { get; init; }
    public string? Colour { get; init; }
    public IReadOnlyCollection<TodoItemDto> Items { get; init; }

    private class Mapping : Profile
    {
        public Mapping() => CreateMap<TodoList, TodoListDto>();
    }
}
