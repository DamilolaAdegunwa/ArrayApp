using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Interfaces;
using ArrayApp.Domain.Entities;
using ArrayApp.Domain.Entities.CategoryAggregate;
using ArrayApp.Domain.Entities.IdeaAggregate;
using AutoMapper;
using MediatR;
using Serilog;
namespace ArrayApp.Application.Ideas.Commands;
public record CreateIdeaCommand : IRequest<int> //: IRequest<(bool status, string message, int response)>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public IdeaStatus Status { get; set; }
    public double Rating { get; set; }
    public int CategoryId { get; set; }
}

public class CreateIdeaCommandHandler : IRequestHandler<CreateIdeaCommand,int> //: IRequestHandler<CreateIdeaCommand, (bool status, string message, int response)>
{
    private readonly IApplicationDbContext _context;
    //private readonly IMapper _mapper;
    public CreateIdeaCommandHandler(IApplicationDbContext context
        //,IMapper mapper
        )
    {
        _context = context;
       // _mapper = mapper;
    }

    //public async Task<(bool status, string message, int response)> Handle(CreateIdeaCommand request, CancellationToken cancellationToken)
    public async Task<int> Handle(CreateIdeaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request == null)
            {
                //return (false, "request cannot be null", 0);
                return 0;
            }
            Idea idea = new Idea
            {
                Title = request.Title,
                Description = request.Description,
                Content = request.Content,
                Status = request.Status,
                Rating = request.Rating,
                Category = new Category
                {
                    Id = request.CategoryId
                }
            };
            if (idea == null)
            {
                //return (false, "idea request cannot be null", 0);
                return 0;
            }
            _context.Ideas.Add(idea);
            await _context.SaveChangesAsync(cancellationToken);

            //return (true, "saved!", idea.Id);
            return idea.Id;
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            //return (false, ex.Message, 0);
            return 0;
        }
        
    }
}