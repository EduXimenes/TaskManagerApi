using System.ComponentModel;

namespace TaskManager.Domain.Enums
{
    public enum TaskStatusEnum
    {
        [Description("Tarefa pendente de início")]
        Pending = 1,

        [Description("Tarefa em andamento")]
        InProgress = 2,

        [Description("Tarefa concluída")]
        Completed = 3,

        [Description("Tarefa cancelada")]
        Cancelled = 4
    }
}
