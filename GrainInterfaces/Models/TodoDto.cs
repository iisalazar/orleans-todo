namespace GrainInterfaces.Models;

[GenerateSerializer]
public class TodoDto
{
  [Id(0)]
 public Guid Id { get; set; }
  [Id(1)]
 public string Title { get; set; }
}