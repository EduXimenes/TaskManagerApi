namespace TaskManager.Application.InputModels
{
    public class CreateProjectInputModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}
