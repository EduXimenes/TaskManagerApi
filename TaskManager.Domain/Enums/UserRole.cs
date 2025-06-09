using System.ComponentModel;

namespace TaskManager.Domain.Enums
{
    public enum UserRole
    {
        [Description("Usuário padrão")]
        Default = 1,

        [Description("Gerente")]
        Manager = 2
    }
}
