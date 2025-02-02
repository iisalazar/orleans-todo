using GrainInterfaces;
using GrainInterfaces.Models;
using Microsoft.Extensions.Logging;

namespace Grains;

public class TodoGrain: Grain, ITodoGrain
{
  private readonly ILogger<TodoGrain> _logger;
  private readonly IPersistentState<TodoState> _todoState;

  public TodoGrain(ILogger<TodoGrain> logger,
    [PersistentState("todo", "todoStore")]
    IPersistentState<TodoState> todoState)
  {
    _logger = logger;
    _todoState = todoState;
  }

  public async Task AddTitle(string title)
  {
    var key = this.GetPrimaryKey();
    _logger.LogInformation($"[{key}] Adding new title: {title}");
    _todoState.State.Id = key;
    _todoState.State.Title = title;
    await _todoState.WriteStateAsync();
  }

  public Task<TodoDto> GetState()
  {
    var dto = new TodoDto()
    {
      Id = _todoState.State.Id,
      Title = _todoState.State.Title
    };
    return Task.FromResult(dto);
  }

  public Task UpdateTitle(string title)
  {
    _todoState.State.Title = title;
    return Task.CompletedTask;
  }
}