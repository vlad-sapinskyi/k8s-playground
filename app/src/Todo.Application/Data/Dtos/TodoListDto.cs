using AutoMapper;
using Todo.Application.Data.Entities;

namespace Todo.Application.Data.Dtos;

public class TodoListDto
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Colour { get; init; } = string.Empty;

    public IReadOnlyCollection<TodoItemDto> Items { get; init; } = [];

    private class Mapping : Profile
    {
        public Mapping() => CreateMap<TodoListEntity, TodoListDto>()
            .ForMember(d => d.Colour, opt => opt
                .MapFrom(s => s.Colour.ToString()));
    }
}
