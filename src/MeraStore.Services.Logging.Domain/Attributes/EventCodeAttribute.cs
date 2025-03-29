using System.Diagnostics.CodeAnalysis;

namespace MeraStore.Services.Logging.Domain.Attributes;

[ExcludeFromCodeCoverage]
public class EventCodeAttribute(string code): Attribute
{
    public string EventCode { get; } = code;
}
