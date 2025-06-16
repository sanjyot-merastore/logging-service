using FastEndpoints;
using MeraStore.Services.Logging.Domain.Interfaces;

namespace MeraStore.Services.Logging.Api.Endpoints;

public class SetupLogIndexTemplatesEndpoint(ILogIndexTemplateService indexTemplateService) : EndpointWithoutRequest<string>
{
    public override void Configure()
    {
        Post("index");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Pushes all Elasticsearch log index templates";
            s.Description = "Used to configure log index mappings and templates in Elasticsearch.";
            s.Response(200, "Success");
            s.Response(500, "Internal Server Error");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await indexTemplateService.SetupTemplatesAsync();
        await SendAsync("Ok", cancellation: ct);
    }
}