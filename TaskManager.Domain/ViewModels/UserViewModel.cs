using TaskManager.Domain.Enums;

namespace TaskManager.Domain.ViewModels
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; } 
    }
}
