using GrainInterfaces.Models;

namespace GrainInterfaces;

public interface ITodoGrain: IGrainWithGuidKey
{
  Task AddTitle(string title);
  Task<TodoDto> GetState();
  Task UpdateTitle(string title);
}