using AutoMapper;
using Todo.Application.Data.Entities;

namespace Todo.Application.Data.Dtos;

public class TodoListDto
{
    public TodoListDto() => Items = [];

    public int Id { get; init; }
    public string? Title { get; init; }
    public string? Colour { get; init; }
    public IReadOnlyCollection<TodoItemDto> Items { get; init; }

    private class Mapping : Profile
    {
        public Mapping() => CreateMap<TodoListEntity, TodoListDto>()
            .ForMember(d => d.Colour, opt => opt
                .MapFrom(s => s.Colour.ToString()));
    }
}
