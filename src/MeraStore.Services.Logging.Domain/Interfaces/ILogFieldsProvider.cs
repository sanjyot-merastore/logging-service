using MeraStore.Services.Logging.Domain.Models;

namespace MeraStore.Services.Logging.Domain.Interfaces;

public interface ILogFieldsProvider
{
  Task<LogFields> GetFieldsAsync();
}