using FluentValidation;
using TaskManager.Domain.InputModels;

namespace TaskManager.Application.Validators
{
    public class UpdateTaskInputModelValidator : AbstractValidator<UpdateTaskInputModel>
    {
        public UpdateTaskInputModelValidator()
        {
            RuleFor(t => t.Title)
                .NotEmpty().WithMessage("O título da tarefa é obrigatório.");

            RuleFor(t => t.Description)
                .NotEmpty().WithMessage("A descrição da tarefa é obrigatória.");

            RuleFor(t => t.DueDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("A data de entrega deve ser futura.");
        }
    }
}
