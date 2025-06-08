using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;

namespace MeraStore.Services.Logging.Application;

public class IndexTemplateManager(ElasticsearchClient client, Dictionary<string, string> fieldMappings, 
    string templateName = "app-logs-template", string indexPattern = "app-logs-*", string[]? indexPatterns = null, bool reindexOnPush = true)
{
    private readonly string[] _indexPatterns = indexPatterns ?? [indexPattern];

    public async Task PushAsync()
    {
        var exists = await client.Indices.ExistsIndexTemplateAsync(templateName);
        if (!exists.Exists)
        {
            var props = BuildDynamicProperties(fieldMappings);

            var request = new PutIndexTemplateRequest(templateName)
            {
                IndexPatterns = new[] { indexPattern },
                Template = new IndexTemplateMapping()
                {
                    Mappings = new TypeMapping
                    {
                        Dynamic = DynamicMapping.True,
                        Properties = props
                    }
                }
            };

            var response = await client.Indices.PutIndexTemplateAsync(request);

            if (!response.IsValidResponse)
                throw new Exception($"Failed to create index template '{templateName}': {response.ElasticsearchServerError}");
        }

        if (reindexOnPush)
            await ReindexMatchingIndicesAsync();
    }

    private Properties BuildDynamicProperties(Dictionary<string, string> fieldMap)
    {
        var props = new Properties();

        foreach (var (name, type) in fieldMap)
        {
            IProperty property = type.ToLower() switch
            {
                "keyword" => new KeywordProperty(),
                "text" => new TextProperty(),
                "text.keyword" => new TextProperty
                {
                    Fields = new Properties()
                    {
                        { "keyword", new KeywordProperty() }
                    }
                },
                "integer" => new IntegerNumberProperty(),
                "long" => new LongNumberProperty(),
                "double" => new DoubleNumberProperty(),
                "boolean" => new BooleanProperty(),
                "date" => new DateProperty(),
                "ip" => new IpProperty(),
                "float" => new FloatNumberProperty(),
                "short" => new ShortNumberProperty(),
                "byte" => new ByteNumberProperty(),
                "scaled_float" => new ScaledFloatNumberProperty { ScalingFactor = 100 },
                "geo_point" => new GeoPointProperty(),
                "nested" => new NestedProperty(),
                _ => new ObjectProperty()
            };

            props[name] = property;
        }

        return props;
    }

    private async Task ReindexMatchingIndicesAsync()
    {
        var getIndexResponse = await client.Indices.GetAsync("*");
        if (!getIndexResponse.IsValidResponse)
            throw new Exception("Failed to retrieve index list.");

        var allIndices = getIndexResponse.Indices.Keys.Select(k => k.ToString());

        foreach (var pattern in _indexPatterns)
        {
            var matchingIndices = allIndices
                .Where(index => index.StartsWith(pattern.TrimEnd('*'), StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var sourceIndex in matchingIndices)
            {
                var tempIndex = $"{sourceIndex}-reindexed-{DateTime.UtcNow:yyyyMMddHHmmss}";

                Console.WriteLine($"🔁 Reindexing: {sourceIndex} → {tempIndex}");

                // 1. Create temp index (applies template automatically)
                var createResp = await client.Indices.CreateAsync(tempIndex);
                if (!createResp.IsValidResponse)
                    throw new Exception($"Failed to create temp index '{tempIndex}': {createResp.ElasticsearchServerError}");

                // 2. Reindex source to temp
                var reindexResponse = await client.ReindexAsync(r => r
                    .Source(s => s.Indices(sourceIndex))
                    .Dest(d => d.Index(tempIndex))
                    .WaitForCompletion(true)
                );

                if (!reindexResponse.IsValidResponse)
                    throw new Exception($"Reindexing failed for index '{sourceIndex}': {reindexResponse.ElasticsearchServerError}");

                Console.WriteLine($"✅ Successfully reindexed {sourceIndex} to {tempIndex}");
            }
        }
    }

}