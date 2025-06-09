using System.ComponentModel;

namespace TaskManager.Domain.Enums
{
    public enum TaskPriority
    {
        [Description("Prioridade baixa")]
        Low = 1,

        [Description("Prioridade média")]
        Medium = 2,

        [Description("Prioridade alta")]
        High = 3,

        [Description("Prioridade urgente")]
        Urgent = 4
    }
}
