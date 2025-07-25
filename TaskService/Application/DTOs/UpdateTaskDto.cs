namespace TaskService.Application.DTOs
{
    public record UpdateTaskDto(int Id, string Title, string Description, DateTime? DueDate, bool IsCompleted);

}
