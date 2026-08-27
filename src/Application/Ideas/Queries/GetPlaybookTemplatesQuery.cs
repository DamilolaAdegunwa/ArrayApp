namespace ArrayApp.Application.Ideas.Queries;

public record GetPlaybookTemplatesQuery(string? FormatId = null) : IRequest<List<WorkshopPlaybookDto>>;

public class GetPlaybookTemplatesQueryHandler : IRequestHandler<GetPlaybookTemplatesQuery, List<WorkshopPlaybookDto>>
{
    private readonly ISessionPlaybookService _playbookService;

    public GetPlaybookTemplatesQueryHandler(ISessionPlaybookService playbookService)
    {
        _playbookService = playbookService;
    }

    public async Task<List<WorkshopPlaybookDto>> Handle(GetPlaybookTemplatesQuery request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.FormatId))
        {
            var single = await _playbookService.GetPlaybookTemplateAsync(request.FormatId, cancellationToken);
            return new List<WorkshopPlaybookDto> { single };
        }

        return await _playbookService.GetAllPlaybooksAsync(cancellationToken);
    }
}
