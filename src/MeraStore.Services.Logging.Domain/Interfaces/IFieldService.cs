using MeraStore.Services.Logging.Domain.Models;

namespace MeraStore.Services.Logging.Domain.Interfaces;

public interface IFieldService
{
  Task<LogFields> GetFieldsAsync();
}