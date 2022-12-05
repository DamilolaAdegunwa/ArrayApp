using ArrayApp.Application.Common.Interfaces;
using ArrayApp.Domain.Entities;
using ArrayApp.Domain.Events;
using MediatR;

namespace ArrayApp.Application.TodoItems.Commands.CreateTodoItem;

public record CreateTodoItemCommand : IRequest<int>
{
    public int ListId { get; init; }

    public string? Title { get; init; }
}

public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateTodoItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoItem
        {
            //Id = request.ListId,
            //ListId = request.ListId,
            Title = request.Title,
            Done = false
        };

        var list = _context.TodoLists.FirstOrDefault(l => l.Id == request.ListId);
        if(list == null || list.Id <= 0)
        {
            throw new Exception("cant find the list!");
        }

        entity.List = list;

        entity.AddDomainEvent(new TodoItemCreatedEvent(entity));

        _context.TodoItems.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
