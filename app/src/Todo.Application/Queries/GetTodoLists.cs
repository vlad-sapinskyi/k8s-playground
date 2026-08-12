using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo.Application.Common;
using Todo.Application.Dtos;

namespace Todo.Application.Queries;

public record GetTodoListsQuery : IRequest<IReadOnlyCollection<TodoListDto>>;

public class GetTodoListsQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetTodoListsQuery, IReadOnlyCollection<TodoListDto>>
{
    public async Task<IReadOnlyCollection<TodoListDto>> Handle(GetTodoListsQuery request, CancellationToken cancellationToken) => 
        await context.TodoLists
            .AsNoTracking()
            .ProjectTo<TodoListDto>(mapper.ConfigurationProvider)
            .OrderBy(t => t.Title)
            .ToListAsync(cancellationToken);
}
