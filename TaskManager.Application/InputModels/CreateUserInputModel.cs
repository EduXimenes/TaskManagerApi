using TaskManager.Domain.Enums;

namespace TaskManager.Application.InputModels
{
    public class CreateUserInputModel
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }
}
