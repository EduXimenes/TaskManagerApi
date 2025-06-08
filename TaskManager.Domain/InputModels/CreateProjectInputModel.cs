namespace TaskManager.Domain.InputModels
{
    public class CreateProjectInputModel
    {
        public string Name { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}
