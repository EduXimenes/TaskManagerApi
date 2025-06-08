using FluentValidation;
using TaskManager.Application.InputModels;

namespace TaskManager.Application.Validators
{
    public class CreateProjectInputModelValidator : AbstractValidator<CreateProjectInputModel>
    {
        public CreateProjectInputModelValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("O nome do projeto é obrigatório.");
        }
    }
}
